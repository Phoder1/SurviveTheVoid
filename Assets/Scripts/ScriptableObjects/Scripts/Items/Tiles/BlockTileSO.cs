using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Block Tile", menuName = "SO/" + "Tiles/" + "Block", order = 0)]
public class BlockTileSO : TileAbstSO
{
}
public class BlockState : ITileState
{
    public BlockTileSO tile;

    public BlockState(BlockTileSO tile) {
        this.tile = tile;
    }

    public TileBase GetMainTileBase => tile.GetMainTileBase;

    public InteractionType GetInteractionType => tile.GetInteractionType;

    public TileType GetTileType => tile.GetTileType;

    public bool GetIsSolid => tile.GetIsSolid;

    public void GatherInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) {
        Tilemap tilemap = GridManager.GetInstance.GetTilemap(buildingLayer);
        tilemap.RemoveTileFlags((Vector3Int)gridPosition, TileFlags.LockColor);
        tilemap.SetColor((Vector3Int)gridPosition, new Color(0.9f, 0.9f, 1f, 0.7f));
    }

    public void CancelEvent(Vector2Int gridPosition, TileMapLayer tilemapLayer) {
    }

    public void SpecialInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) {
        throw new System.NotImplementedException();
    }

    public void Init(Vector2Int gridPosition, TileMapLayer tilemapLayer) { }
}


