using System.Collections.Generic;
using UnityEngine;

public class BuildingState : StateBase
{
    Vector2 touchPosition;
    TileHit currentTileHit , newTileHit;
    TileSlot tileSlotCache;
    bool isBuildingAttached;
    List<Vector2Int> TileList = new List<Vector2Int>();
    GridManager gridManager;

    public BuildingState() { gridManager = GridManager._instance; }

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


                touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                currentTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Floor);
                if (tileSlotCache == null || currentTileHit == null || currentTileHit.tile == null || gridManager.GetTileFromGrid(currentTileHit.gridPosition, TileMapLayer.Buildings) != null)
                    return;


                isBuildingAttached = true;

                if (newTileHit != null && newTileHit.gridPosition == currentTileHit.gridPosition)
                {

                    gridManager.SetTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Buildings, false);
                    if (!TileList.Contains(currentTileHit.gridPosition))
                    {
                        TileList.Add(currentTileHit.gridPosition);
                    }
                }
                else
                {
                    if (TileList.Count > 0)
                    {
                        foreach (var tile in TileList)
                        {
                            if (tile == null)
                                continue;

                            gridManager.SetTile(null, tile, TileMapLayer.Buildings, false);
                        }
                        TileList.Clear();
                    }
                    newTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Floor);



                    if (newTileHit == null || gridManager.GetTileFromGrid(newTileHit.gridPosition, TileMapLayer.Buildings) != null)
                        gridManager.SetTile(null, currentTileHit.gridPosition, TileMapLayer.Buildings, false);

                    currentTileHit = newTileHit;
                }

                break;
        }
    }

    public void PressedConfirmBuildingButton()
    {
        if (!isBuildingAttached || tileSlotCache == null || (currentTileHit == null && TileList.Count < 1))
            return;

        if (TileList.Count >= 1)
        {
            for (int i = 0; i < TileList.Count; i++)
            {
                gridManager.SetTile(null, TileList[i], TileMapLayer.Buildings, true);
            }
        }


        if (currentTileHit == null || currentTileHit.gridPosition == null)
            gridManager.SetTile(tileSlotCache, TileList[0], TileMapLayer.Buildings, true);
        else
            gridManager.SetTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Buildings, true);



        newTileHit = null;

        currentTileHit = null;

        var itemSlotCache = new ItemSlot(tileSlotCache.GetTileAbst, 1);
        Inventory.GetInstance.RemoveItemFromInventory(0, itemSlotCache);
        if (Inventory.GetInstance.GetAmountOfItem(0, itemSlotCache) >= 1)
        {
            SetBuildingTile(itemSlotCache.item as TileAbstSO);
        }
        else
        {
            PlayerStateMachine.GetInstance.SwitchState(InputState.DefaultState);
            tileSlotCache = null;
        }

        TileList.Clear();
        Debug.Log("Placed");


    }
    public void SetBuildingTile(TileAbstSO Item)
    {
        tileSlotCache = null;
        if (Item == null)
            return;

        newTileHit = null;
        currentTileHit = null;
        tileSlotCache = new TileSlot(Item);
    }
    public Vector2 GetTouchPosition => touchPosition;
    public TileHit GetCurrentTileHit => currentTileHit;
    public TileHit GetNewTileHit => newTileHit;
    public bool GetIsBuildingAttached => isBuildingAttached;
        
}
