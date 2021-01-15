using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class BuildingState : StateBase
{
    Vector2 touchPosition;
    TileHit currentTileHit , newTileHit;
    TileSlot tileSlotCache;
    bool isBuildingAttached, buildOnFloor;
    List<Vector2Int> FloorTileList = new List<Vector2Int>();
    List<Vector2Int>BuildingTileList = new List<Vector2Int>();
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

                if (tileSlotCache == null || EventSystem.current.IsPointerOverGameObject()) 
                    return;


                touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                buildOnFloor = false;

                if (tileSlotCache.GetTileType != TileType.Block)
                {
                     currentTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Floor);
                    if ( currentTileHit == null || currentTileHit.tile == null||gridManager.GetTileFromGrid(currentTileHit.gridPosition, TileMapLayer.Buildings) != null )
                        return;
                }
                else
                {
                    currentTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Floor);
                    
                    if (currentTileHit == null && gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Buildings) == null) //floor is empty & building  is empty good
                    {
                        // check only floor layer
                        buildOnFloor = true;
                    }
                    else if (currentTileHit == null && gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Buildings) != null) //floor is empty & building  is not empty - bad
                    {
                      return;
                        
                    }
                    else
                    if (currentTileHit != null && gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Buildings) == null)//floor is not empty & building and building is empty bad
                    { 
                        // check only building layer 
                      buildOnFloor = false;
                    }

                }

                
                    

               


                isBuildingAttached = true;

                if (newTileHit != null && newTileHit.gridPosition == currentTileHit.gridPosition)
                {
                    if (buildOnFloor)
                    {
                        if (!FloorTileList.Contains(currentTileHit.gridPosition))
                        {
                            FloorTileList.Add(currentTileHit.gridPosition);
                        }
                        gridManager.SetTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Floor, false);
                    }
                    else
                    {
                        gridManager.SetTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Buildings, false);
                        if (!BuildingTileList.Contains(currentTileHit.gridPosition))
                        {
                            BuildingTileList.Add(currentTileHit.gridPosition);
                        }
                    }
                }
                else
                {
                    if (FloorTileList.Count > 0)
                    {
                        foreach (var tile in FloorTileList)
                        {
                            if (tile == null)
                                continue;

                            gridManager.SetTile(null, tile, TileMapLayer.Floor, false);
                        }
                        FloorTileList.Clear();
                    }
                    if (BuildingTileList.Count > 0)
                    {
                        foreach (var tile in BuildingTileList)
                        {
                            if (tile == null)
                                continue;

                            gridManager.SetTile(null, tile, TileMapLayer.Buildings, false);
                        }
                        BuildingTileList.Clear();
                    }



                        newTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Floor);



                    //if (newTileHit == null || gridManager.GetTileFromGrid(newTileHit.gridPosition, TileMapLayer.Buildings) != null || gridManager.GetTileFromGrid(newTileHit.gridPosition, TileMapLayer.Floor) != null)
                    //{

                    //    if (buildOnFloor)
                    //        gridManager.SetTile(null, currentTileHit.gridPosition, TileMapLayer.Floor, false);
                    //    else
                    //        gridManager.SetTile(null, currentTileHit.gridPosition, TileMapLayer.Buildings, false);

                    //}
                    currentTileHit = newTileHit;
                }

                break;
        }
    }

    public void PressedConfirmBuildingButton()
    {
        if (!isBuildingAttached || tileSlotCache == null || (currentTileHit == null && FloorTileList.Count < 1))
            return;


        if (buildOnFloor)
        {
            if (FloorTileList.Count >= 1)
            {
                for (int i = 0; i < FloorTileList.Count; i++)
                {
                    gridManager.SetTile(null, FloorTileList[i], TileMapLayer.Floor, true);
                }
            }  
            if (currentTileHit == null || currentTileHit.gridPosition == null)
            gridManager.SetTile(tileSlotCache, FloorTileList[0], TileMapLayer.Floor, true);
        else
            gridManager.SetTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Floor, true);

          
        }
        else
        {

            if (BuildingTileList.Count >= 1)
            {
                for (int i = 0; i < BuildingTileList.Count; i++)
                {
                    gridManager.SetTile(null, BuildingTileList[i], TileMapLayer.Buildings, true);
                }
            }

            if (currentTileHit == null || currentTileHit.gridPosition == null)
                gridManager.SetTile(tileSlotCache, BuildingTileList[0], TileMapLayer.Buildings, true);
            else
                gridManager.SetTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Buildings, true);


        }





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

        FloorTileList.Clear();
        BuildingTileList.Clear();
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
