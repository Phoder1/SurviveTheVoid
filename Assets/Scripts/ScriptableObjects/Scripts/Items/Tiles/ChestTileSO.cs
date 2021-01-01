using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Block Tile", menuName = "SO/" + "Tiles/" + "Block", order = 0)]
public class ChestTileSO : TileAbstSO
{
    public ChestTileSlot GetNewSlot => new ChestTileSlot(this);
}
public class ChestTileSlot : ITileState
{
    public ChestTileSO tile;

    public ChestTileSlot(ChestTileSO tile) {
        this.tile = tile;
    }

    public TileBase GetMainTileBase => throw new System.NotImplementedException();

    public InteractionType GetInteractionType => throw new System.NotImplementedException();

    public TileType GetTileType => throw new System.NotImplementedException();

    public bool GetIsSolid => throw new System.NotImplementedException();

    public void GatherInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) {
        throw new System.NotImplementedException();
    }

    public void Remove(Vector2Int gridPosition, TileMapLayer tilemapLayer) {
        throw new System.NotImplementedException();
    }

    public void SpecialInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) {
        throw new System.NotImplementedException();
    }
}
