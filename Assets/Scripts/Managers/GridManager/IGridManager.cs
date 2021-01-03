using UnityEngine;
using UnityEngine.Tilemaps;

public interface IGridManager
{
    Vector2Int WorldToGridPosition(Vector3 worldPosition, TileMapLayer buildingLayer);
    Vector3 GridToWorldPosition(Vector2Int gridPosition, TileMapLayer buildingLayer, bool getCenter);
    TileSlot GetTileFromGrid(Vector2Int gridPosition, TileMapLayer buildingLayer);
    TileSlot GetTileFromWorld(Vector2 worldPosition, TileMapLayer buildingLayer);
    void SetTile(TileSlot tile, Vector2Int gridPosition, TileMapLayer buildingLayer, bool playerAction = true);

    TileHit GetHitFromWorldPosition(Vector2 clickPosition, TileMapLayer buildingLayer);
    Tilemap GetTilemap(TileMapLayer buildingLayer);
    bool IsTileWalkable(Vector2 worldPosition, Vector2 movementVector);
    void UpdateView(Rect view);
}