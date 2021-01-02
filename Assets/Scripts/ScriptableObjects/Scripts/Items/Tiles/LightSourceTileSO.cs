using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Light Source Tile", menuName = "SO/" + "Tiles/" + "Light Source", order = 3)]

public class LightSourceTileSO : TileAbstSO
{
}
public class LightSourceTileState : ITileState
{
    public LightSourceTileSO tile;

    public LightSourceTileState(LightSourceTileSO tile) {
        this.tile = tile;
    }

    public TileBase GetMainTileBase => throw new System.NotImplementedException();

    public InteractionType GetInteractionType => throw new System.NotImplementedException();

    public TileType GetTileType => throw new System.NotImplementedException();

    public bool GetIsSolid => throw new System.NotImplementedException();

    public void GatherInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) {
        throw new System.NotImplementedException();
    }

    public void CancelEvent(Vector2Int gridPosition, TileMapLayer tilemapLayer) {
        throw new System.NotImplementedException();
    }

    public void SpecialInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) {
        throw new System.NotImplementedException();
    }

    public void Init(Vector2Int gridPosition, TileMapLayer tilemapLayer) { }
}
