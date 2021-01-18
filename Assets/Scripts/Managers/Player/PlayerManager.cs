using Assets.Scan;
using System;
using System.Collections;
using UnityEngine;

public class PlayerManager : MonoSingleton<PlayerManager>
{
    private PlayerStats playerStats;

    private InputManager _inputManager;
    private GridManager _GridManager;
    private Scanner _scanner;
    private PlayerMovementHandler playerController;
    private TileMapLayer buildingLayer;

    [SerializeField] float baseSpeed;
    [SerializeField] int lookRange = 5;

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
    private DirectionEnum MovementDir {
        get {
            float angle = Vector2.SignedAngle(_inputManager.VJAxis, Vector2.up);
            int direction = Mathf.RoundToInt(angle / 90);
            switch (direction) {
                case 0:
                    return DirectionEnum.Up;
                case 1:
                    return DirectionEnum.Right;
                case -1:
                    return DirectionEnum.Left;
                case 2:
                    return DirectionEnum.Down;
                default:
                    return DirectionEnum.Down;
            }
        }
    }


    public override void Init() {

        buildingLayer = TileMapLayer.Buildings;
        _scanner = new Scanner();
        _inputManager = InputManager._instance;
        _GridManager = GridManager._instance;
        playerStats = PlayerStats._instance;
        playerController = PlayerMovementHandler._instance;
        moveSpeed = playerStats.GetStat(PlayerStatType.MoveSpeed);
        gatheringSpeed = playerStats.GetStat(PlayerStatType.GatheringSpeed);

    }
    private void LateUpdate() {
        if (!anyInteracted) {
            Vector2 movementVector = _inputManager.VJAxis * Time.deltaTime * baseSpeed * moveSpeed.GetSetValue;
            if (movementVector != Vector2.zero) {
                playerController.Move(movementVector);
            }
        }
        if (specialWasPressed != specialButton) {
            specialButton = specialWasPressed;
        }
        if (gatherWasPressed != gatherButton) {
            gatherButton = gatherWasPressed;
            if (!gatherButton && gatherCoroutine != null) {
                StopCoroutine(gatherCoroutine);
                gatherCoroutine = null;
            }
        }
        gatherWasPressed = false;
        specialWasPressed = false;
        anyInteracted = false;
    }

    public TileHit Scan(IChecker checkType) {
        Vector2Int currentPosOnGrid = _GridManager.WorldToGridPosition(new Vector3(transform.position.x, transform.position.y, 0), TileMapLayer.Buildings);
        return _scanner.Scan(currentPosOnGrid, MovementDir, lookRange, buildingLayer, checkType);
    }
    public void ImplementInteraction(bool SpecialInteract) {
        if (closestTile == null) {
            if (SpecialInteract) {
                closestTile = Scan(new SpecialInterractionScanChecker());
                specialWasPressed = true;
            }
            else {
                closestTile = Scan(new GatheringScanChecker());
                gatherWasPressed = true;
            }
        }
        else {
            anyInteracted = true;
            Vector3 destination = _GridManager.GridToWorldPosition(closestTile.gridPosition, buildingLayer, true);
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

}