using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovalState : StateBase
{
    TileHit currentTileHit;
    TileSlot tileSlotCache;
    Vector2 touchPosition;
    GridManager gridManager;
    public RemovalState() { gridManager = GridManager._instance; }

    public override void ButtonB()
    {
        ConfirmRemoval();
    }
    public override void StateOnTouch(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
            case TouchPhase.Moved:
            case TouchPhase.Stationary:

                touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                currentTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Buildings);
                gridManager.SetDummyTile(null, currentTileHit.gridPosition, TileMapLayer.Buildings);
                if (currentTileHit == null || currentTileHit.tile == null || gridManager.GetTileFromGrid(currentTileHit.gridPosition, TileMapLayer.Buildings) == null)
                    return;
                Debug.Log("Found!");
                
                break;
        }
    }
    public void ConfirmRemoval()
    {
        tileSlotCache = currentTileHit.tile;
        if (tileSlotCache == null)
            return;



        if (currentTileHit != null && Inventory.GetInstance.AddToInventory(0, new ItemSlot(currentTileHit.tile.GetTileAbst, 1)))
        {

            gridManager.SetTile(null, currentTileHit.gridPosition, TileMapLayer.Buildings, true);
        }
    }



    public override void MousePos()
    {
        touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        currentTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Buildings);

        if (currentTileHit == null || currentTileHit.tile == null || gridManager.GetTileFromGrid(currentTileHit.gridPosition, TileMapLayer.Buildings) == null)
            return;

        tileSlotCache = currentTileHit.tile;

        if (Input.GetMouseButtonDown(0))
            ConfirmRemoval();

          
        
    }
}



