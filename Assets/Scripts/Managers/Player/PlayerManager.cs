using Assets.Scan;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    private PlayerStats playerStats;
    private static Transform playerTransfrom;

    private InputManager inputManager;
    private GridManager gridManager;
    private Scanner scanner;
    private PlayerMovementHandler playerController;
    private TileMapLayer buildingLayer;

    [SerializeField] float baseSpeed;
    [SerializeField] int interactionLookRange = 5, airLookRange;

    [SerializeField] float InterractionDistance;

    TileHit closestTile;

    public bool SpecialInterracted;

    private bool gatherButton;
    private bool specialButton;

    private bool gatherWasPressed;
    private bool specialWasPressed;
    private bool anyInteracted;
    private Stat moveSpeed;
    private Stat gatheringSpeed;
    Coroutine gatherCoroutine = null;
    private Vector2Int lastCheckPosition = new Vector2Int(int.MaxValue, int.MaxValue);
    private float lastTreeCheckTime = 0;
    private const float treeCheckInterval = 0.5f;
    private Vector2Int lastPosition;
    private Vector2Int currentPosOnGrid;
    private Vector3 startPositionOfPlayer= new Vector3(0, 0.25f, 3.81f);
    public Vector2Int GetCurrentPosOnGrid => currentPosOnGrid;
    private EffectController airRegenCont;
    private EffectData airRegenData;
    bool playerMoved;

    private DirectionEnum gridMovementDirection;
    public static Transform GetPlayerTransform => playerTransfrom;

    public DirectionEnum GetMovementDirection => gridMovementDirection;

    public override void Init()
    {
        inputManager = InputManager._instance;
        gridManager = GridManager._instance;
        playerStats = PlayerStats._instance;
        playerController = PlayerMovementHandler.GetInstance;
        scanner = new Scanner();
        playerTransfrom = transform;
        buildingLayer = TileMapLayer.Buildings;
        HardReset();
    }
    private void Update() {
        lastPosition = currentPosOnGrid;
        currentPosOnGrid = gridManager.WorldToGridPosition((Vector2)transform.position, TileMapLayer.Floor);
        UpdateGridDirection();
        playerMoved = lastPosition != currentPosOnGrid;
        CheckForTrees();
    }

    private void FixedUpdate() {
        Vector2 movementVector = inputManager.VJAxis * Time.deltaTime * baseSpeed * moveSpeed.GetSetValue;
        movementVector.y *= 0.5f;
        if (movementVector != Vector2.zero) {
            playerController.Move(movementVector);
        }
    }


    private void LateUpdate() {
        if (specialWasPressed != specialButton) {
            specialButton = specialWasPressed;
            if (!specialButton)
                closestTile = null;
        }
        if (gatherWasPressed != gatherButton) {
            gatherButton = gatherWasPressed;
            if (!gatherButton) {
                if (gatherCoroutine != null) {
                    StopCoroutine(gatherCoroutine);
                    gatherCoroutine = null;
                }
                closestTile = null;
            }
        }
        gatherWasPressed = false;
        specialWasPressed = false;
        anyInteracted = false;
    }

    private void CheckForTrees() {
        if (Time.time >= lastTreeCheckTime + treeCheckInterval) {
            lastTreeCheckTime = Time.time;
            if (currentPosOnGrid != lastCheckPosition) {
                lastCheckPosition = currentPosOnGrid;
                TileHit hit = scanner.Scan(currentPosOnGrid, gridMovementDirection, airLookRange, TileMapLayer.Buildings, new AirSourcesScanChecker());
                if (hit != null) {
                    airRegenCont.Begin(airRegenData);
                }
                else {
                    airRegenCont.Stop();
                }
            }
        }
    }

    public TileHit Scan(IChecker checkType) {
        return scanner.Scan(currentPosOnGrid, gridMovementDirection, interactionLookRange, buildingLayer, checkType);
    }
    public void ImplementInteraction(bool SpecialInteract) {
        gatherWasPressed = true;
        if (closestTile == null) {
            if (SpecialInteract) {
                closestTile = Scan(new SpecialInterractionScanChecker());
            }
            else {
                closestTile = Scan(new GatheringScanChecker());
            }
        }
        else {
            Vector3 destination = gridManager.GridToWorldPosition(closestTile.gridPosition, buildingLayer, true);
            destination.z = transform.position.z;
            float distance = Vector2.Distance(transform.position, destination);
            if (distance > InterractionDistance) {
                playerController.Move(Vector3.ClampMagnitude((destination - transform.position).normalized * Time.deltaTime * baseSpeed * moveSpeed.GetSetValue, distance));
            }
            else {
                if (SpecialInteract) {
                    if (!SpecialInterracted) {
                        closestTile.tile.SpecialInteraction(closestTile.gridPosition, buildingLayer);
                        SpecialInterracted = true;
                    }
                }
                else {
                    if (gatherCoroutine == null)
                        gatherCoroutine = StartCoroutine(HarvestTile(closestTile));
                }
            }
        }
    }
    IEnumerator HarvestTile(TileHit tileHit) {
        if (tileHit.tile.GetTileAbst is GatherableTileSO gatherable) {
            yield return new WaitForSeconds(gatherable.GetGatheringTime / gatheringSpeed.GetSetValue);
            tileHit.tile.GatherInteraction(tileHit.gridPosition, buildingLayer);
            Debug.Log("TileHarvested");
        }
        gatherCoroutine = null;
        closestTile = null;
    }
    internal void MenuClosed() {
        SpecialInterracted = false;
        closestTile = null;
    }
    private void UpdateGridDirection() {
        float angle = Vector2.SignedAngle(inputManager.VJAxis, new Vector2(-0.25f, 0.25f));
        int direction = Mathf.RoundToInt(angle / 90);
        switch (direction) {
            case 0:
                gridMovementDirection = DirectionEnum.Up;
                break;
            case 1:
                gridMovementDirection = DirectionEnum.Right;
                break;
            case -1:
                gridMovementDirection = DirectionEnum.Left;
                break;
            case 2:
            case -2:
                gridMovementDirection = DirectionEnum.Down;
                break;
            default:
                gridMovementDirection = DirectionEnum.Down;
                break;
        }
    }

    public override void HardReset()
    {
        DeathReset();
    }

    public override void DeathReset()
    {
        transform.position = startPositionOfPlayer;
        moveSpeed = playerStats.GetStat(StatType.MoveSpeed);
        gatheringSpeed = playerStats.GetStat(StatType.GatheringSpeed);
        airRegenCont.Stop();
        airRegenCont = new EffectController(playerStats.GetStat(StatType.Air), 2);
        airRegenData = new EffectData(StatType.Air, EffectType.OverTime, 10f, Mathf.Infinity, 0.5f, false, false);
    }

    public class GatheringScanChecker : IChecker
    {
        public bool CheckTile(TileSlot tile) {
            return tile.IsGatherable;
        }
    }
    public class SpecialInterractionScanChecker : IChecker
    {
        public bool CheckTile(TileSlot tile) {
            return tile.isSpecialInteraction;
        }
    }
    public class AirSourcesScanChecker : IChecker
    {
        public bool CheckTile(TileSlot tile) {
            return tile.GetIsAirSource;
        }
    }


    public class PlayerMovementHandler
    {
        [Range(0.1f, 1f)]
        [SerializeField] float playerColliderSize;
        CameraController cameraController;
        GridManager gridManager;
       static PlayerMovementHandler _instance;

        Vector2Int currentGridPos;
        Vector2 gridMoveVector;

        bool moved;
        public static PlayerMovementHandler GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlayerMovementHandler();
                    
                }
                return _instance;
            }
        }

        PlayerMovementHandler() { Init(); }
        public void Init()
        {
            cameraController = CameraController._instance;
            gridManager = GridManager._instance;
          
        }
        public void Move(Vector2 moveVector)
        {
            moved = false;
            gridMoveVector = UnityToGridVector(moveVector);
            currentGridPos = gridManager.WorldToGridPosition((Vector2)GetPlayerTransform.position, TileMapLayer.Floor);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moved = true;
            GetPlayerTransform.Translate(moveVector);
            }
            else
            {
                MoveOnY();
                MoveOnX();
            }
            if (moved)
                UpdateView();
        }

        private void MoveOnY()
        {
            Vector2 UnityVectorOnGridY = GridToUnityVector(new Vector2(0, gridMoveVector.y));
            if (UnityVectorOnGridY == Vector2.zero)
            {
                return;
            }
            if (gridMoveVector.y > 0)
            {
                if (CheckTilesOnPos(tileLeftCorner(GetPlayerTransform.position, playerColliderSize) + UnityVectorOnGridY) && CheckTilesOnPos(tileTopCorner(GetPlayerTransform.position, playerColliderSize) + UnityVectorOnGridY))
                {
                    ApplyMove(UnityVectorOnGridY);
                }
                //else {
                //    Vector2 nextTilePos = gridManager.GridToWorldPosition(currentGridPos + Vector2Int.up, TileMapLayer.Floor, true);
                //    float gridDistance = UnityToGridVector(nextTilePos).y - UnityToGridVector(tileLeftCorner(transform.position, playerColliderSize)).y;
                //    Debug.Log(gridDistance);
                //    Vector2 moveVector = GridToUnityVector(new Vector2(0, gridDistance - 0.5f));
                //    transform.position += (Vector3)moveVector;
                //}
            }
            else
            {
                if (CheckTilesOnPos(tileBottomCorner(GetPlayerTransform.position, playerColliderSize) + UnityVectorOnGridY) && CheckTilesOnPos(tileRightCorner(GetPlayerTransform.position, playerColliderSize) + UnityVectorOnGridY))
                {
                    ApplyMove(UnityVectorOnGridY);
                }
                //else {
                //    Vector2 nextGridPos = UnityToGridVector(transform.position + (Vector3)UnityVectorOnGridY);
                //    nextGridPos.y = Mathf.Ceil(nextGridPos.y);
                //    transform.position = (Vector3)GridToUnityVector(nextGridPos) + Vector3.forward * transform.position.z;
                //}
            }
        }
        private void MoveOnX()
        {
            Vector2 UnityVectorOnGridX = GridToUnityVector(new Vector2(gridMoveVector.x, 0));
            if (UnityVectorOnGridX == Vector2.zero)
            {
                return;
            }
            if (gridMoveVector.x > 0)
            {
                if (CheckTilesOnPos(tileRightCorner(GetPlayerTransform.position, playerColliderSize) + UnityVectorOnGridX) && CheckTilesOnPos(tileTopCorner(GetPlayerTransform.position, playerColliderSize) + UnityVectorOnGridX))
                {
                    ApplyMove(UnityVectorOnGridX);
                }
                //else {
                //    Vector2 nextGridPos = UnityToGridVector(transform.position + (Vector3)UnityVectorOnGridX);
                //    nextGridPos.x = Mathf.Floor(nextGridPos.x);
                //    transform.position = (Vector3)GridToUnityVector(nextGridPos) + Vector3.forward * transform.position.z;
                //    ;
                //}
            }
            else
            {
                if (CheckTilesOnPos(tileBottomCorner(GetPlayerTransform.position, playerColliderSize) + UnityVectorOnGridX) && CheckTilesOnPos(tileLeftCorner(GetPlayerTransform.position, playerColliderSize) + UnityVectorOnGridX))
                {
                    ApplyMove(UnityVectorOnGridX);
                }
                //else {
                //    Vector2 nextGridPos = UnityToGridVector(transform.position + (Vector3)UnityVectorOnGridX);
                //    nextGridPos.x = Mathf.Ceil(nextGridPos.x);
                //    transform.position = (Vector3)GridToUnityVector(nextGridPos) + Vector3.forward * transform.position.z;
                //}
            }
        }
        private void ApplyMove(Vector2 vector) => ApplyMove((Vector3)vector);
        private void ApplyMove(Vector3 vector)
        {
            GetPlayerTransform.position += vector;
            moved = true;
        }

        private Vector2 UnityToGridVector(Vector2 vector) => new Vector2(2 * vector.x + vector.y, -2 * vector.x + vector.y);
        private Vector2 GridToUnityVector(Vector2 vector) => new Vector2(0.125f * vector.x - 0.125f * vector.y, 0.25f * vector.x + 0.25f * vector.y);
        private bool CheckTilesOnPos(Vector2 pos)
        {
            Vector2Int gridPos = gridManager.WorldToGridPosition(pos, TileMapLayer.Floor);
            if (gridPos == currentGridPos)
                return true;
            TileSlot buildingTile = gridManager.GetTileFromGrid(gridPos, TileMapLayer.Buildings);
            TileSlot floorTile = gridManager.GetTileFromGrid(gridPos, TileMapLayer.Floor);
            return (buildingTile == null || !buildingTile.GetIsSolid) && floorTile != null;
        }
        private Vector2 tileTopCorner(Vector2 pos, float colliderSize) => pos + Vector2.up * 0.25f * colliderSize;
        private Vector2 tileBottomCorner(Vector2 pos, float colliderSize) => pos + Vector2.down * 0.25f * colliderSize;
        private Vector2 tileRightCorner(Vector2 pos, float colliderSize) => pos + Vector2.right * 0.5f * colliderSize;
        private Vector2 tileLeftCorner(Vector2 pos, float colliderSize) => pos + Vector2.left * 0.5f * colliderSize;
        private void UpdateView() => cameraController.UpdateView();
    }
}