
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingState : StateBase
{
    public UIRaycastDetector GR;
    Vector2 touchPosition;
    TileHit currentTileHit;
    TileSlot tileSlotCache;
    bool isBuildingAttached, currentlyPlacedOnFloor, wasFloorLayer;
    Color blueprintColor = new Color(0.5f, 0.5f, 1, 0.55f);
    Vector2 localTouchPos;

    Vector2Int[] Position = new Vector2Int[3];
    GridManager gridManager;
    int amountOfCurrentItem;

    public BuildingState() {
        ResetParam();
           gridManager = GridManager._instance; 
    }

     void ResetParam() {
        currentTileHit = null;
        amountOfCurrentItem = 0;
        tileSlotCache = null;
    }
    public override void ButtonA() {
        Debug.Log("BuildingState");
        PressedConfirmBuildingButton();
    }


    public override void ButtonB() {
        PlayerStateMachine.GetInstance.SwitchState(InputState.RemovalState);
    }
    public override void StateOnTouch(Touch touch) {

        switch (touch.phase) {
            case TouchPhase.Began:
            case TouchPhase.Moved:
            case TouchPhase.Stationary:

                if (tileSlotCache == null || EventSystem.current.IsPointerOverGameObject() || UIRaycastDetector.GetInstance.RayCastCheck(touch))//|| (currentTileHit != null && currentTileHit.tile == null)
                    return;
              //  localTouchPos = CameraController._instance.GetCurrentActiveCamera.ScreenToWorldPoint(touch.position) - PlayerManager._instance.transform.position;
                touchPosition = CameraController._instance.GetCurrentActiveCamera.ScreenToWorldPoint(touch.position);

                CheckPosition(touchPosition);




                break;
        }
    }

    public void BuildWithVJ() {

        //    touchPosition= (Vector2)PlayerManager._instance.GetPlayerVector;
        Debug.Log(localTouchPos + (Vector2)PlayerManager._instance.transform.position);
        CheckPosition(localTouchPos + (Vector2)PlayerManager._instance.transform.position);
       
    }

 
    private void PlaceDummyBlock(bool isCurrentOnFloor) {
        if (Position[1] == Position[0])
            return;

        RemovePreviousTile();
        Position[1] = new Vector2Int(Position[0].x, Position[0].y);
        if (isCurrentOnFloor) {
            gridManager.SetDummyTile(tileSlotCache, Position[1], TileMapLayer.Floor, blueprintColor);
            wasFloorLayer = true;
        }

        else {
            gridManager.SetDummyTile(tileSlotCache, Position[1], TileMapLayer.Buildings, blueprintColor);
            wasFloorLayer = false;
        }

        Position[2] = new Vector2Int(Position[1].x, Position[1].y);
    }

    private void CheckPosition(Vector2 worldPos) {

        if (Position[0] == gridManager.GetHitFromWorldPosition(worldPos, TileMapLayer.Floor).gridPosition)
            return;





        currentTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Floor);

        if (currentTileHit == null)
            return;


        // there is a block on the floor
        // check if there is no a block above it 
        if (currentTileHit.tile != null && gridManager.GetTileFromGrid(currentTileHit.gridPosition, TileMapLayer.Buildings) == null) {



            Position[0] = new Vector2Int(currentTileHit.gridPosition.x, currentTileHit.gridPosition.y);


            PlaceDummyBlock(false);

        }
        else if (currentlyPlacedOnFloor) {
            if (currentTileHit.tile == null && gridManager.GetTileFromGrid(currentTileHit.gridPosition, TileMapLayer.Buildings) == null) {
                Position[0] = new Vector2Int(currentTileHit.gridPosition.x, currentTileHit.gridPosition.y);

                PlaceDummyBlock(true);

            }
        }




        return;
    }

    private void RemovePreviousTile() {
        if (Position[2] == null || Position[2] != Position[1])
            return;


        if (wasFloorLayer) {
            gridManager.SetDummyTile(null, Position[2], TileMapLayer.Floor, Color.white);

        }
        else {
            gridManager.SetDummyTile(null, Position[2], TileMapLayer.Buildings, Color.white);
        }

        Position[2] = Position[0];
    }

    public void PressedConfirmBuildingButton() {
        if (!isBuildingAttached || tileSlotCache == null || currentTileHit == null)
            return;
      


        if (wasFloorLayer) {
            gridManager.SetTile(tileSlotCache, Position[2], TileMapLayer.Floor, true);
        }
        else {
            gridManager.SetTile(tileSlotCache, Position[2], TileMapLayer.Buildings, true);
        }

        var itemSlotCache = new ItemSlot(tileSlotCache.GetTileAbst, 1);
        Inventory.GetInstance.RemoveItemFromInventory(0, itemSlotCache);
        amountOfCurrentItem--;
        if (amountOfCurrentItem >= 1) {
            SetBuildingTile(itemSlotCache.item as TileAbstSO);
        }
        else {
            PlayerStateMachine.GetInstance.SwitchState(InputState.DefaultState);
            UIManager._instance.ButtonCancel();
            tileSlotCache = null;
        }

        Debug.Log("Placed");

    }
    public void SetBuildingTile(TileAbstSO Item) {
        tileSlotCache = null;
        if (Item == null)
            return;
        tileSlotCache = new TileSlot(Item);
        if (amountOfCurrentItem <= 0)
            amountOfCurrentItem = Inventory.GetInstance.GetAmountOfItem(0, new ItemSlot(Item, 1));



        currentlyPlacedOnFloor = tileSlotCache.GetTileType == TileType.Block;
        if (amountOfCurrentItem == 0)
            amountOfCurrentItem = Inventory.GetInstance.GetAmountOfItem(0, new ItemSlot(Item, 1));

        isBuildingAttached = true;
        currentTileHit = null;
    }
    public Vector2 GetTouchPosition => touchPosition;
    public TileHit GetCurrentTileHit => currentTileHit;

    public bool GetIsBuildingAttached => isBuildingAttached;

    public override void MousePos() {
        CheckPosition(CameraController._instance.GetCurrentActiveCamera.ScreenToWorldPoint(Input.mousePosition));

        if (Input.GetMouseButton(0)) {
            PressedConfirmBuildingButton();
        }

    }



    public override void OnSwitchState()
    {
         ResetParam();
       RemovePreviousTile();
    }
}
