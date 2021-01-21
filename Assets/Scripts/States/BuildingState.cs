using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class BuildingState : StateBase
{
    Vector2 touchPosition;
    TileHit currentTileHit;
    TileSlot tileSlotCache;
    bool isBuildingAttached, currentlyPlacedOnFloor, wasFloorLayer;

    Vector2Int[] Position = new Vector2Int[3];
    GridManager gridManager;
    int amountOfTheSameBuilding;
    public BuildingState() { gridManager = GridManager._instance; amountOfTheSameBuilding = 0; }

    public override void ButtonA()
    {
        Debug.Log("BuildingState");
        PressedConfirmBuildingButton();
    }


    public override void ButtonB()
    {
        PlayerStateMachine.GetInstance.SwitchState(InputState.RemovalState);
    }
    public override void StateOnTouch(Touch touch)
    {

        switch (touch.phase)
        {
            case TouchPhase.Began:
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (tileSlotCache == null || EventSystem.current.IsPointerOverGameObject() && currentTileHit.tile != null)
                    return;




                touchPosition = Camera.main.ScreenToWorldPoint(touch.position);


                CheckPosition();


                break;
        }
    }

    private void PlaceDummyBlock(bool isCurrentOnFloor)
    {
        if (Position[1] == Position[0])
            return;

        RemovePreviousTile();
        Position[1] = new Vector2Int(Position[0].x, Position[0].y);
        if (isCurrentOnFloor)
        {
            gridManager.SetDummyTile(tileSlotCache, Position[1], TileMapLayer.Floor);
            wasFloorLayer = true;
        }

        else
        {
            gridManager.SetDummyTile(tileSlotCache, Position[1], TileMapLayer.Buildings);
            wasFloorLayer = false;
        }

        Position[2] = new Vector2Int(Position[1].x, Position[1].y);
    }

    private void CheckPosition()
    {

        if (Position[0] == gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Floor).gridPosition)
            return;

        currentTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Floor);

        Position[0] = new Vector2Int(currentTileHit.gridPosition.x, currentTileHit.gridPosition.y);




        // there is a block on the floor
        // check if there is no a block above it 
        if (currentTileHit.tile != null && gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Buildings).tile == null)
        {

            RemovePreviousTile();
            PlaceDummyBlock(false);

        }
        else if (currentlyPlacedOnFloor)
        {
            if (currentTileHit.tile == null && gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Buildings).tile == null)
            {


                RemovePreviousTile();
                PlaceDummyBlock(true);

            }
        }




        return;
    }

    private void RemovePreviousTile()
    {
        if (Position[2] == null || Position[2] != Position[1])
            return;


        if (wasFloorLayer)
        {
            gridManager.SetDummyTile(null, Position[2], TileMapLayer.Floor);

        }
        else
        {
            gridManager.SetDummyTile(null, Position[2], TileMapLayer.Buildings);
        }

        Position[2] = Position[0];
    }

    public void PressedConfirmBuildingButton()
    {
        if (!isBuildingAttached || tileSlotCache == null)
            return;

        if (wasFloorLayer)
        {
            gridManager.SetTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Floor, true);
        }
        else
        {
            gridManager.SetTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Buildings, true);
        }
        var itemSlotCache = new ItemSlot(tileSlotCache.GetTileAbst, 1);
        Inventory.GetInstance.RemoveItemFromInventory(0, itemSlotCache);
        amountOfTheSameBuilding--;
        if (amountOfTheSameBuilding >= 1)
        {
            SetBuildingTile(itemSlotCache.item as TileAbstSO);
        }
        else
        {
            PlayerStateMachine.GetInstance.SwitchState(InputState.DefaultState);
            UIManager._instance.ButtonCancel();
            tileSlotCache = null;
        }

        Debug.Log("Placed");




    }
    public void SetBuildingTile(TileAbstSO Item)
    {
        tileSlotCache = null;
        if (Item == null)
            return;
        tileSlotCache = new TileSlot(Item);


        currentlyPlacedOnFloor = tileSlotCache.GetTileType == TileType.Block;
        if (amountOfTheSameBuilding == 0)
            amountOfTheSameBuilding = Inventory.GetInstance.GetAmountOfItem(0, new ItemSlot(Item, 1));

        isBuildingAttached = true;
        currentTileHit = null;
    }
    public Vector2 GetTouchPosition => touchPosition;
    public TileHit GetCurrentTileHit => currentTileHit;

    public bool GetIsBuildingAttached => isBuildingAttached;
    public void ResetBeforeChangeStates()
    {

        RemovePreviousTile();
    }
}
