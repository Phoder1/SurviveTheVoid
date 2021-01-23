using Assets.Scan;
using System.Collections;
using UnityEngine;

public partial class PlayerManager : MonoSingleton<PlayerManager>
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
        cameraController = CameraController._instance;
        inputManager = InputManager._instance;
        gridManager = GridManager._instance;
        playerStats = PlayerStats._instance;
        scanner = new Scanner();
        playerTransfrom = transform;
        buildingLayer = TileMapLayer.Buildings;
        DeathReset();
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
            Move(movementVector);
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
                Move(Vector3.ClampMagnitude((destination - transform.position).normalized * Time.deltaTime * baseSpeed * moveSpeed.GetSetValue, distance));
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

 

    public void DeathReset()
    {
        transform.position = startPositionOfPlayer;
        moveSpeed = playerStats.GetStat(StatType.MoveSpeed);
        gatheringSpeed = playerStats.GetStat(StatType.GatheringSpeed);
        if (airRegenCont != null)
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

    [System.Serializable]
    public class PlayerMovementHandler
    {

    }
}