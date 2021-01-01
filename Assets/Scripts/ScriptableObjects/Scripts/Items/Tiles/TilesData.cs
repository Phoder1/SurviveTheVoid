
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum InteractionType { None, Axe, Pickaxe, Hoe, Shovel, Hammer, AnyTool, Special, Any }
public enum TileType { Block, Plant, Chest, ProcessingTable, LightSource }
#region Tile hit
public class TileHit
{
    public readonly Vector2Int gridPosition;
    public readonly TileSlot tile;

    public TileHit(TileSlot tile, Vector2Int gridPosition) {
        this.tile = tile;
        this.gridPosition = gridPosition;
    }

    public static TileHit none => new TileHit(null, Vector2Int.zero);

    public static bool operator ==(TileHit lhs, TileHit rhs) {
        return lhs.Equals(rhs);
    }
    public static bool operator !=(TileHit lhs, TileHit rhs) {
        return !lhs.Equals(rhs);
    }

    public override bool Equals(object obj) {
        return (obj is TileHit hit &&
                EqualityComparer<Vector2Int?>.Default.Equals(gridPosition, hit.gridPosition) &&
                EqualityComparer<TileSlot>.Default.Equals(tile, hit.tile));
    }
    public bool Equals(TileHit other) {
        return tile == other.tile && gridPosition == other.gridPosition;
    }

    public override int GetHashCode() {
        int hashCode = 1814505039;
        hashCode = hashCode * -1521134295 + gridPosition.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<TileSlot>.Default.GetHashCode(tile);
        return hashCode;
    }
}
#endregion
#region Main abstract classes
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public abstract class TileAbstSO : ItemSO
{
    [SerializeField] private TileBase mainTileBase;
    [SerializeField] private InteractionType interactionType;
    [SerializeField] private TileType tileType;
    [SerializeField] private bool isSolid;
    public TileBase GetMainTileBase => mainTileBase;
    public InteractionType GetInteractionType => interactionType;
    public TileType GetTileType => tileType;

    public bool GetIsSolid => isSolid;
}
public interface ITileState
{
    TileBase GetMainTileBase { get; }
    InteractionType GetInteractionType { get; }
    TileType GetTileType { get; }
    bool GetIsSolid { get; }
    void GatherInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer);
    void SpecialInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer);
    void Remove(Vector2Int gridPosition, TileMapLayer tilemapLayer);
}
public class TileSlot : ITileState
{
    public ITileState tileState;
    public TileSlot(TileAbstSO tile, Vector2Int gridPosition, TileMapLayer tileMapLayer) {
        switch (tile) {
            case PlantTileSO plant:
                tileState = new PlantState(plant,  gridPosition,  tileMapLayer);
                break;
            case BlockTileSO block:
                tileState = new BlockState(block, gridPosition, tileMapLayer);
                break;
            case ProcessingTableTileSO table:
                tileState = new ProcessingTableTileState(table, gridPosition, tileMapLayer);
                break;
            case LightSourceTileSO lightSource:
                tileState = new LightSourceTileState(lightSource, gridPosition, tileMapLayer);
                break;
            default:
                throw new System.NotImplementedException();

        }
    }
    #region Passthrough
    public virtual TileBase GetMainTileBase => tileState.GetMainTileBase;
    public virtual InteractionType GetInteractionType => tileState.GetInteractionType;
    public virtual TileType GetTileType => tileState.GetTileType;
    public virtual bool GetIsSolid => tileState.GetIsSolid;

    public virtual void GatherInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer)
        => tileState.GatherInteraction(gridPosition, buildingLayer);

    public virtual void SpecialInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) 
        => tileState.SpecialInteraction(gridPosition, buildingLayer);
    public virtual void Remove(Vector2Int gridPosition, TileMapLayer tilemapLayer) 
        => tileState.Remove(gridPosition, tilemapLayer);
    #endregion
}


#endregion












