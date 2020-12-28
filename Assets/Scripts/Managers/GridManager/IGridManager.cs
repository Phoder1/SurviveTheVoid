using Assets.TilesData;
using UnityEngine;
using UnityEngine.Tilemaps;

public interface IGridManager
{
    Vector2Int WorldToGridPosition(Vector3 worldPosition, BuildingLayer buildingLayer);
    Vector3 GridToWorldPosition(Vector2Int gridPosition, BuildingLayer buildingLayer);
    GenericTile GetTileFromGrid(Vector2Int gridPosition, BuildingLayer buildingLayer);
    GenericTile GetTileFromWorld(Vector2 worldPosition, BuildingLayer buildingLayer);
    void SetTile(GenericTile tile, Vector2Int gridPosition, BuildingLayer buildingLayer, bool playerAction = true);
    TileHitStruct GetHitFromClickPosition(Vector2 clickPosition, BuildingLayer buildingLayer);
    Tilemap GetTilemap(BuildingLayer buildingLayer);
    bool IsTileWalkable(Vector2 worldPosition, Vector2 movementVector);
    void UpdateView(Rect view);  
}