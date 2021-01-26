
using UnityEngine;

public class RemovalState : StateBase
{
    TileHit currentTileHit;
    TileSlot tileSlotCache;
    Vector2 touchPosition;
    GridManager gridManager;
    Vector2Int[] Position = new Vector2Int[3];
    Color blueprintColor = new Color(1f,0.5f, 0.5f, 0.55f);
    public RemovalState() { 
        gridManager = GridManager._instance;
    }

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

                touchPosition = CameraController._instance.GetCurrentActiveCamera.ScreenToWorldPoint(touch.position);

                CheckPosition(touchPosition);
                break;
        }
    }




    private void CheckPosition(Vector2 worldPos)
    {

        if (Position[0] == gridManager.GetHitFromWorldPosition(worldPos, TileMapLayer.Floor).gridPosition)
            return;

        currentTileHit = gridManager.GetHitFromWorldPosition(worldPos, TileMapLayer.Floor);

        Position[0] = new Vector2Int(currentTileHit.gridPosition.x, currentTileHit.gridPosition.y);

        // there is a block on the floor
        // check if there is no a block above it 
        if (currentTileHit.tile != null && gridManager.GetHitFromWorldPosition(worldPos, TileMapLayer.Buildings).tile != null)
        {
            ReturnPreviousTile();
            PlaceDummyBlock();
        }
    
        return;
    }
    private void PlaceDummyBlock()
    {
        if (Position[1] == Position[0])
            return;

        ReturnPreviousTile();

        Position[1] = new Vector2Int(Position[0].x, Position[0].y);
       

        gridManager.SetTileColor(Position[1], TileMapLayer.Buildings, blueprintColor);

        Position[2] = new Vector2Int(Position[1].x, Position[1].y);
    }

    private void ReturnPreviousTile()
    {
        if (Position[2] == null || Position[2] != Position[1])
            return;


        gridManager.SetTileColor(Position[2], TileMapLayer.Buildings, Color.white);

        Position[2] = Position[0];
    }

    public override void OnSwitchState()
    {
        for (int i = 0; i < Position.Length; i++)
        {
            gridManager.SetTileColor(Position[i], TileMapLayer.Buildings, Color.white);
        }
        currentTileHit = null;
    }




    public void ConfirmRemoval()
    {
        tileSlotCache = currentTileHit.tile;
        if (tileSlotCache == null)
            return;



        if (currentTileHit != null&& currentTileHit.tile.GetIsDestructible && Inventory.GetInstance.AddToInventory(0, new ItemSlot(currentTileHit.tile.GetTileAbst, 1)))
        {
            gridManager.SetTile(null, currentTileHit.gridPosition, TileMapLayer.Buildings, true);
            tileSlotCache = null;

        }
    }



    public override void MousePos()
    {
        touchPosition = CameraController._instance.GetCurrentActiveCamera.ScreenToWorldPoint(Input.mousePosition);

        currentTileHit = gridManager.GetHitFromWorldPosition(touchPosition, TileMapLayer.Buildings);

        if (currentTileHit == null || currentTileHit.tile == null || gridManager.GetTileFromGrid(currentTileHit.gridPosition, TileMapLayer.Buildings) == null)
            return;

        tileSlotCache = currentTileHit.tile;

        if (Input.GetMouseButtonDown(0))
            ConfirmRemoval();
        
    }
}



