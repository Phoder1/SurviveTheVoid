using Assets.TilesData;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IGridManager
{
    Vector2Int WorldToGridPosition(Vector3 worldPosition, TileMapLayer buildingLayer);
    Vector3 GridToWorldPosition(Vector2Int gridPosition, TileMapLayer buildingLayer, bool getCenter);
    TileAbst GetTileFromGrid(Vector2Int gridPosition, TileMapLayer buildingLayer);
    TileAbst GetTileFromWorld(Vector2 worldPosition, TileMapLayer buildingLayer);
    void SetTile(TileAbst tile, Vector2Int gridPosition, TileMapLayer buildingLayer, bool playerAction = true);
    TileHitStruct GetHitFromClickPosition(Vector2 clickPosition, TileMapLayer buildingLayer);
    Tilemap GetTilemap(TileMapLayer buildingLayer);
    bool IsTileWalkable(Vector2 worldPosition, Vector2 movementVector);
    void UpdateView(Rect view);  
}