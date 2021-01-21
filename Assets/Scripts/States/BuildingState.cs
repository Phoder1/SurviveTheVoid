using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class BuildingState : StateBase
{
    Vector2 touchPosition ;
    TileHit currentTileHit , lastTileHit;
    TileSlot tileSlotCache;
    bool isBuildingAttached, currentlyPlacedOnFloor , wasFloorLayer;
    List<Vector2Int> FloorLayerTileList = new List<Vector2Int>();
    List<Vector2Int>BuildingLayerTileList = new List<Vector2Int>();
    Vector2Int[] Position = new Vector2Int[3];
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





                CheckPosition();
          



                {

                    //if (tileSlotCache == null || EventSystem.current.IsPointerOverGameObject())
                    //    return;


                    //touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                    //if (Vector3.Distance(touchPosition, PlayerManager.GetPlayerTransform.position) > 7f) {
                    //   // tileSlotCache = null;

                    //    if (FloorLayerTileList.Count >= 1)
                    //    {
                    //        for (int i = 0; i < FloorLayerTileList.Count; i++)
                    //        {
                    //            gridManager.SetDummyTile(null, FloorLayerTileList[i], TileMapLayer.Floor);
                    //        }
                    //        FloorLayerTileList.Clear();
                    //    }


                    //    if (BuildingLayerTileList.Count >= 1)
                    //    {
                    //        for (int i = 0; i < BuildingLayerTileList.Count; i++)
                    //        {
                    //            gridManager.SetDummyTile(null, BuildingLayerTileList[i], TileMapLayer.Buildings);
                    //        }
                    //        BuildingLayerTileList.Clear();
                    //    }
                    //    return;
                    //}



                    //currentTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Floor);


                    //if (tileSlotCache.GetTileType != TileType.Block) { 

                    //    isFloorLayer = false;
                    //    canBeOnFloor = false;
                    //}
                    //else { 

                    //    isFloorLayer = true;
                    //    canBeOnFloor = true;
                    //}

                    //if (currentTileHit == null)
                    //    break;

                    //if (canBeOnFloor && currentTileHit.tile != null && gridManager.GetTileFromGrid(currentTileHit.gridPosition, TileMapLayer.Buildings) == null)
                    //{
                    //    isFloorLayer = false;
                    //}

                    //isBuildingAttached = true;












                    //if (lastTileHit != null && lastTileHit.gridPosition == currentTileHit.gridPosition)
                    //{

                    //    if (isFloorLayer)
                    //    {

                    //        if (canBeOnFloor)
                    //        {

                    //            if (currentTileHit.tile == null)
                    //            {
                    //                if (!FloorLayerTileList.Contains(currentTileHit.gridPosition))
                    //                    FloorLayerTileList.Add(currentTileHit.gridPosition);

                    //                gridManager.SetDummyTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Floor);
                    //                toPutOnFloor = true;

                    //            }
                    //        }



                    //    }
                    //    else
                    //    {

                    //        if (currentTileHit.tile!= null && !isFloorLayer && !FloorLayerTileList.Contains(currentTileHit.gridPosition))
                    //        {
                    //        if (!BuildingLayerTileList.Contains(currentTileHit.gridPosition))
                    //            BuildingLayerTileList.Add(currentTileHit.gridPosition);

                    //            gridManager.SetDummyTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Buildings);
                    //            toPutOnFloor = false;

                    //        }



                    //    }

                    //    return;

                    //}

                    //    if (FloorLayerTileList.Count >= 1)
                    //    {
                    //        for (int i = 0; i < FloorLayerTileList.Count; i++)
                    //        {
                    //            gridManager.SetDummyTile(null, FloorLayerTileList[i], TileMapLayer.Floor);
                    //        }
                    //        FloorLayerTileList.Clear();
                    //    }


                    //    if (BuildingLayerTileList.Count >= 1)
                    //    {
                    //        for (int i = 0; i < BuildingLayerTileList.Count; i++)
                    //        {
                    //            gridManager.SetDummyTile(null, BuildingLayerTileList[i], TileMapLayer.Buildings);
                    //        }
                    //        BuildingLayerTileList.Clear();
                    //    }


                    ////newTileHit = currentTileHit;


                    //lastTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Floor);





                    //if (lastTileHit == null)
                    //    break;

                }




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

        else { 
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
        //if (isCurrentOnFloor)
        //{
        //             gridManager.SetTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Floor, true);

        //}
        //else
        //{
        //          gridManager.SetTile(tileSlotCache, currentTileHit.gridPosition, TileMapLayer.Buildings, true);

        //}
        //lastTileHit = null;

        //currentTileHit = null;

        //var itemSlotCache = new ItemSlot(tileSlotCache.GetTileAbst, 1);
        //Inventory.GetInstance.RemoveItemFromInventory(0, itemSlotCache);
        //if (Inventory.GetInstance.GetAmountOfItem(0, itemSlotCache) >= 1)
        //{
        //    SetBuildingTile(itemSlotCache.item as TileAbstSO);
        //}
        //else
        //{
        //    PlayerStateMachine.GetInstance.SwitchState(InputState.DefaultState);
        //    UIManager._instance.ButtonCancel();
        //    tileSlotCache = null;
        //}

        //FloorLayerTileList.Clear();
        //BuildingLayerTileList.Clear();
        //Debug.Log("Placed");


    }
    public void SetBuildingTile(TileAbstSO Item)
    {
        tileSlotCache = null;
        if (Item == null)
            return;
        tileSlotCache = new TileSlot(Item);


        currentlyPlacedOnFloor = tileSlotCache.GetTileType == TileType.Block;
        isBuildingAttached = true;
        currentTileHit = null;
    }
    public Vector2 GetTouchPosition => touchPosition;
    public TileHit GetCurrentTileHit => currentTileHit;
    public TileHit GetNewTileHit => lastTileHit;
    public bool GetIsBuildingAttached => isBuildingAttached;
        
}
