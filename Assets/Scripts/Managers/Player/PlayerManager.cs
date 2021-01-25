using Assets.Scan;
using System.Collections;
using UnityEngine;

public partial class PlayerManager : MonoSingleton<PlayerManager>
{
    private PlayerStats playerStats;

    private InputManager inputManager;
    private GridManager gridManager;
    private Scanner scanner;
    private EquipManager equipManager;

    [SerializeField] float baseSpeed;

    [SerializeField] PlayerGFX _playerGFX;
    [SerializeField] int interactionLookRange = 5, airLookRange;
    [SerializeField] float InterractionDistance;

    TileHit closestTile;

    public bool SpecialInterracted;

    private bool gatherButton;
    private bool specialButton;

    private bool gatherWasPressed;
    private bool specialWasPressed;
    private Stat moveSpeed;
    private Stat gatheringSpeed;
    Coroutine gatherCoroutine = null;
    private Vector2Int lastCheckPosition = new Vector2Int(int.MaxValue, int.MaxValue);
    private float lastTreeCheckTime = 0;
 
    private const float treeCheckInterval = 0.5f;
    private Vector2Int currentPosOnGrid;
    private Vector3 startPositionOfPlayer;
    public Vector2Int GetCurrentPosOnGrid => currentPosOnGrid;
    private EffectController airRegenCont;
    private EffectData airRegenData;
    private GatherableTileSO tileBeingGathered;
    private DirectionEnum gridMovementDirection;
    public PlayerGFX GetPlayerGFX {

        get
        {
            if (_playerGFX == null)
            {
                _playerGFX = GetComponent<PlayerGFX>();
            }
            return _playerGFX;
        }
    
    }

    public DirectionEnum GetMovementDirection => gridMovementDirection;
    bool playerIsDead = false;

    public override void Init() {
        equipManager = EquipManager.GetInstance;
        _playerGFX = GetComponent<PlayerGFX>();
        _playerGFX._anim = GetComponent<Animator>();
        cameraController = CameraController._instance;
        inputManager = InputManager._instance;
        gridManager = GridManager._instance;
        playerStats = PlayerStats._instance;
        moveSpeed = playerStats.GetStat(StatType.MoveSpeed);
        gatheringSpeed = playerStats.GetStat(StatType.GatheringSpeed);
        scanner = new Scanner();
        airRegenCont = new EffectController(playerStats.GetStat(StatType.Air), 2);
        airRegenData = new EffectData(StatType.Air, EffectType.OverTime, 10f, Mathf.Infinity, 0.5f, false, false);
        startPositionOfPlayer = base.transform.position;
        GameManager.DeathEvent += DeathReset;
        GameManager.RespawnEvent += RespawnReset;
    }
    private void Update() {
        if (!playerIsDead) {
            currentPosOnGrid = gridManager.WorldToGridPosition((Vector2)base.transform.position, TileMapLayer.Floor);
            UpdateGridDirection();
            CheckForTrees();
        }
    }

    private void FixedUpdate() {
        if (!playerIsDead) {
            Vector2 movementVector = inputManager.VJAxis * Time.deltaTime * baseSpeed * moveSpeed.GetSetValue;
            movementVector.y *= 0.5f;
            if (movementVector != Vector2.zero) {
                Move(movementVector);

                _playerGFX.Walk(true, totalSpeed);

            }
            else {
                _playerGFX.Walk(false, null);
            }
        }

    }


    private void LateUpdate() {
        if (!playerIsDead) {
            if (specialWasPressed != specialButton) {
                specialButton = specialWasPressed;
                if (!specialButton)
                    closestTile = null;
            }
            if (gatherWasPressed != gatherButton) {
                gatherButton = gatherWasPressed;
                if (!gatherButton) {
                    if (gatherCoroutine != null) {
                        CancelGathering();
                    }
                    closestTile = null;
                }
            }
            gatherWasPressed = false;
            specialWasPressed = false;
        }
    }

    private void CancelGathering() {
        StopCoroutine(gatherCoroutine);
        gatherCoroutine = null;
        SoundManager._instance.DisableLooping(tileBeingGathered.getGatheringSound);
        UIManager._instance.CancelProgressBar();
        tileBeingGathered = null;
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
        return scanner.Scan(currentPosOnGrid, gridMovementDirection, interactionLookRange, TileMapLayer.Buildings, checkType);
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
            Vector3 destination = gridManager.GridToWorldPosition(closestTile.gridPosition, TileMapLayer.Buildings, true);
            destination.z = base.transform.position.z;
            float distance = Vector2.Distance(base.transform.position, destination);
            if (distance > InterractionDistance) {
                Vector3 moveVector = Vector3.ClampMagnitude((destination - transform.position).normalized * Time.deltaTime * baseSpeed * moveSpeed.GetSetValue, distance);
                _playerGFX.Walk(true, moveVector);
                Move(moveVector);
            }
            else {
                if (SpecialInteract) {
                    if (!SpecialInterracted) {
                        closestTile.tile.SpecialInteraction(closestTile.gridPosition, TileMapLayer.Buildings);
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
            UIManager._instance.CancelProgressBar();
            tileBeingGathered = gatherable;
            float gatheringTime = gatherable.GetGatheringTime / (gatheringSpeed.GetSetValue * equipManager.GetGatheringSpeedFromTool(gatherable.GetToolType));
            Vector2 tileWorldPos = gridManager.GridToWorldPosition(tileHit.gridPosition, TileMapLayer.Buildings, true);
            Camera currentCamera = CameraController._instance.GetCurrentActiveCamera;
            Vector2 tileScreenPos = currentCamera.WorldToScreenPoint(tileWorldPos);
            UIManager._instance.StartProgressBar(tileScreenPos, gatheringTime);
            SoundManager._instance.PlaySoundLooped(gatherable.getGatheringSound);


            yield return new WaitForSeconds(gatheringTime);

            tileHit.tile.GatherInteraction(tileHit.gridPosition, TileMapLayer.Buildings);
       
            if (( equipManager.GetToolDurability(gatherable.GetToolType)) != null )
                equipManager.LowerAmountOfToolDurability(gatherable.GetToolType, gatherable.GetGatherDurabilityCost);
            SoundManager._instance.DisableLooping(gatherable.getGatheringSound);
            Debug.Log("TileHarvested");
            tileBeingGathered = null;
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



    private void DeathReset() {
        playerIsDead = true;
        airRegenCont?.Stop();
        _playerGFX.Death();
    }
  
    private void RespawnReset() {
        _playerGFX.Reborn();
        //  Debug.Log("Player Reborn");
        transform.position = startPositionOfPlayer;
        playerIsDead = false;
    }

    public class GatheringScanChecker : IChecker
    {
        public bool CheckTile(TileSlot tile) {
            EquipManager equipManager = EquipManager.GetInstance;
            if (tile.GetTileAbst is GatherableTileSO gatherable) {
                return (equipManager.GetToolActive(gatherable.GetToolType) && gatherable.GetSourceTier <= equipManager.GetTierByEnum(gatherable.GetToolType));
            }
            return false;
        }
    }
    public class SpecialInterractionScanChecker : IChecker { public bool CheckTile(TileSlot tile) => tile.isSpecialInteraction; }
    public class AirSourcesScanChecker : IChecker { public bool CheckTile(TileSlot tile) => tile.GetIsAirSource; }

    [System.Serializable]
    public class PlayerMovementHandler
    {

    }
}
