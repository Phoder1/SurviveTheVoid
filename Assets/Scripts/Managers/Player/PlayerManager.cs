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
    private Vector2Int lastCheckPosition = new Vector2Int(int.MaxValue,int.MaxValue);
    private float lastTreeCheckTime = 0;
    private const float treeCheckInterval = 1f;
    private Vector2Int lastPosition;
    private Vector2Int currentPosOnGrid;
    private EffectController airRegenCont;
    private EffectData airRegenData;
    bool playerMoved;

    private DirectionEnum movementDirection;
    public static Transform GetPlayerTransform => playerTransfrom;

    public override void Init() {

        buildingLayer = TileMapLayer.Buildings;
        scanner = new Scanner();
        inputManager = InputManager._instance;
        gridManager = GridManager._instance;
        playerStats = PlayerStats._instance;
        playerController = PlayerMovementHandler._instance;
        moveSpeed = playerStats.GetStat(StatType.MoveSpeed);
        gatheringSpeed = playerStats.GetStat(StatType.GatheringSpeed);
        playerTransfrom = transform;
        airRegenCont = new EffectController(playerStats.GetStat(StatType.Air), 2);
        airRegenData = new EffectData(StatType.Air, EffectType.OverTime, 10f, Mathf.Infinity, 0.03f, false, false);
    }
    private void Update() {
        lastPosition = currentPosOnGrid;
        currentPosOnGrid = gridManager.WorldToGridPosition((Vector2)transform.position, TileMapLayer.Floor);
        UpdateDirection();
        playerMoved = lastPosition != currentPosOnGrid;
        CheckForTrees();
    }



    private void LateUpdate() {
        if (!anyInteracted) {
            Vector2 movementVector = inputManager.VJAxis * Time.deltaTime * baseSpeed * moveSpeed.GetSetValue;
            if (movementVector != Vector2.zero) {
                playerController.Move(movementVector);
            }
        }
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
                TileHit hit = scanner.Scan(currentPosOnGrid, movementDirection, airLookRange, TileMapLayer.Buildings, new AirSourcesScanChecker());
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
        return scanner.Scan(currentPosOnGrid, movementDirection, interactionLookRange, buildingLayer, checkType);
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
            anyInteracted = true;
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
    private void UpdateDirection() {
        float angle = Vector2.SignedAngle(inputManager.VJAxis, Vector2.up);
        int direction = Mathf.RoundToInt(angle / 90);
        switch (direction) {
            case 0:
                movementDirection = DirectionEnum.Up;
                break;
            case 1:
                movementDirection = DirectionEnum.Right;
                break;
            case -1:
                movementDirection = DirectionEnum.Left;
                break;
            case 2:
                movementDirection = DirectionEnum.Down;
                break;
            default:
                movementDirection = DirectionEnum.Down;
                break;
        }
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


}