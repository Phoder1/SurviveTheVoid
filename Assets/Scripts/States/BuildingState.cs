using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class BuildingState : StateBase
{
    Vector2 touchPosition ;
    TileHit currentTileHit , lastTileHit;
    TileSlot tileSlotCache;
    bool isBuildingAttached, canBeOnFloor , toPutOnFloor, isFloorLayer;
    List<Vector2Int> FloorLayerTileList = new List<Vector2Int>();
    List<Vector2Int>BuildingLayerTileList = new List<Vector2Int>();
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

                if (Vector3.Distance(touchPosition, PlayerManager.GetPlayerTransform.position) > 7f) {
                   // tileSlotCache = null;

                    if (FloorLayerTileList.Count >= 1)
                    {
                        for (int i = 0; i < FloorLayerTileList.Count; i++)
                        {
                            gridManager.SetDummyTile(null, FloorLayerTileList[i], TileMapLayer.Floor);
                        }
                        FloorLayerTileList.Clear();
                    }


                    if (BuildingLayerTileList.Count >= 1)
                    {
                        for (int i = 0; i < BuildingLayerTileList.Count; i++)
                        {
                            gridManager.SetDummyTile(null, BuildingLayerTileList[i], TileMapLayer.Buildings);
                        }
                        BuildingLayerTileList.Clear();
                    }
                    return;
                }

                

                currentTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Floor);


                if (tileSlotCache.GetTileType != TileType.Block) { 
                 
                    isFloorLayer = false;
                    canBeOnFloor = false;
                }
                else { 

                    isFloorLayer = true;
                    canBeOnFloor = true;
                }
                
                if (currentTileHit == null)
                    break;

                if (canBeOnFloor && currentTileHit.tile != null && gridManager.GetTileFromGrid(currentTileHit.gridPosition, TileMapLayer.Buildings) == null)
                {
                    isFloorLayer = false;
                }

                isBuildingAttached = true;












                if (lastTileHit != null && lastTileHit.gridPosition == currentTileHit.gridPosition)
                {

                    if (isFloorLayer)
                    {

                        if (canBeOnFloor)
                        {

                            if (currentTileHit.tile == null)
                            {
                                if (!FloorLayerTileList.Contains(currentTileHit.gridPosition))
                                    FloorLayerTileList.Add(currentTileHit.gridPosition);

                                gridManager.SetDummyTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Floor);
                                toPutOnFloor = true;

                            }
                        }



                    }
                    else
                    {

                        if (currentTileHit.tile!= null && !isFloorLayer && !FloorLayerTileList.Contains(currentTileHit.gridPosition))
                        {
                        if (!BuildingLayerTileList.Contains(currentTileHit.gridPosition))
                            BuildingLayerTileList.Add(currentTileHit.gridPosition);

                            gridManager.SetDummyTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Buildings);
                            toPutOnFloor = false;

                        }
                        


                    }

                    return;

                }

                    if (FloorLayerTileList.Count >= 1)
                    {
                        for (int i = 0; i < FloorLayerTileList.Count; i++)
                        {
                            gridManager.SetDummyTile(null, FloorLayerTileList[i], TileMapLayer.Floor);
                        }
                        FloorLayerTileList.Clear();
                    }


                    if (BuildingLayerTileList.Count >= 1)
                    {
                        for (int i = 0; i < BuildingLayerTileList.Count; i++)
                        {
                            gridManager.SetDummyTile(null, BuildingLayerTileList[i], TileMapLayer.Buildings);
                        }
                        BuildingLayerTileList.Clear();
                    }


                //newTileHit = currentTileHit;


                lastTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Floor);
                




                if (lastTileHit == null)
                    break;






                break;
        }
    }

    public void PressedConfirmBuildingButton()
    {
        if (!isBuildingAttached || tileSlotCache == null ||  currentTileHit.tile == null )
            return;


        if (toPutOnFloor)
        {
                     gridManager.SetTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Floor, true);

        }
        else
        {
                  gridManager.SetTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Buildings, true);

        }
        lastTileHit = null;

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
            UIManager._instance.ButtonCancel();
            tileSlotCache = null;
        }

        FloorLayerTileList.Clear();
        BuildingLayerTileList.Clear();
        Debug.Log("Placed");


    }
    public void SetBuildingTile(TileAbstSO Item)
    {
        tileSlotCache = null;
        if (Item == null)
            return;

        lastTileHit = null;
        currentTileHit = null;
        tileSlotCache = new TileSlot(Item);
    }
    public Vector2 GetTouchPosition => touchPosition;
    public TileHit GetCurrentTileHit => currentTileHit;
    public TileHit GetNewTileHit => lastTileHit;
    public bool GetIsBuildingAttached => isBuildingAttached;
        
}
