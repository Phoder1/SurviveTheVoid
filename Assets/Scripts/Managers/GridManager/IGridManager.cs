using UnityEngine;
using UnityEngine.Tilemaps;

public interface IGridManager
{
    TileHit GetHitFromClickPosition(Vector2 clickPosition, BuildingLayer buildingLayer);
    TileAbst GetTileFromGrid(Vector2Int gridPosition, BuildingLayer buildingLayer);
    TileAbst GetTileFromWorld(Vector2 worldPosition, BuildingLayer buildingLayer);
    Tilemap GetTilemap(BuildingLayer buildingLayer);
    Vector3 GridToWorldPosition(Vector2Int gridPosition, BuildingLayer buildingLayer);
    bool IsTileWalkable(Vector2 worldPosition, Vector2 movementVector);
    void SetTile(TileAbst tile, Vector2Int gridPosition, BuildingLayer buildingLayer, bool playerAction = true);
    void UpdateView(Rect view);
    Vector2Int WorldToGridPosition(Vector3 worldPosition, BuildingLayer buildingLayer);
}