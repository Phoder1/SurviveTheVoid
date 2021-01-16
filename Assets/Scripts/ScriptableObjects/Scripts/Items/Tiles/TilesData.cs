using UnityEngine;
using UnityEngine.Tilemaps;


public enum InteractionType { None, Axe, Pickaxe, Hoe, Shovel, Hammer, AnyTool, Special, Any }
public enum TileType { Block, Gatherable, Chest, ProcessingTable, LightSource }
#region Tile hit
public class TileHit
{
    public readonly Vector2Int gridPosition;
    public readonly TileSlot tile;

    public TileHit(TileSlot tile, Vector2Int gridPosition) {
        this.tile = tile;
        this.gridPosition = gridPosition;
    }
}
#endregion
#region Main abstract classes
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
public abstract class TileAbstSO : ItemSO
{
    [SerializeField] private TileBase mainTileBase;
    [SerializeField] private InteractionType interactionType;
    [SerializeField] private bool isSolid;
    public TileBase GetMainTileBase => mainTileBase;
    public InteractionType GetInteractionType => interactionType;
    public TileType GetTileType {
        get {
            switch (this) {
                case GatherableTileSO plant:
                    return TileType.Gatherable;
                case BlockTileSO block:
                    return TileType.Block;
                case ProcessingTableTileSO table:
                    return TileType.ProcessingTable;
                case LightSourceTileSO lightSource:
                    return TileType.LightSource;
                default:
                    throw new System.NotImplementedException();
            }
        }

    }

    public bool GetIsSolid => isSolid;
}
public interface ITileState
{
    TileBase GetMainTileBase { get; }
    TileAbstSO GetTileAbst { get; }
    InteractionType GetInteractionType { get; }
    TileType GetTileType { get; }
    bool GetIsSolid { get; }
    void GatherInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer);
    void SpecialInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer);
    void CancelEvent(Vector2Int gridPosition, TileMapLayer tilemapLayer);
    void Init(Vector2Int gridPosition, TileMapLayer tilemapLayer);
}
public class TileSlot : ITileState
{
    public ITileState tileState;
    public TileSlot(TileAbstSO tile) {
        if (tile == null)
            throw new System.NullReferenceException();
        switch (tile) {
            case GatherableTileSO plant:
                tileState = new GatherableState(plant, this);
                break;
            case BlockTileSO block:
                tileState = new BlockState(block);
                break;
            case ProcessingTableTileSO table:
                tileState = new ProcessingTableTileState(table);
                break;
            case LightSourceTileSO lightSource:
                tileState = new LightSourceTileState(lightSource);
                break;
            default:
                throw new System.NotImplementedException();

        }
    }
    #region Passthrough
    public virtual TileBase GetMainTileBase => tileState.GetMainTileBase;
    public virtual TileAbstSO GetTileAbst => tileState.GetTileAbst;
    public virtual InteractionType GetInteractionType => tileState.GetInteractionType;
    public virtual TileType GetTileType => tileState.GetTileType;
    public virtual bool GetIsSolid => tileState.GetIsSolid;

    public virtual void GatherInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer)
        => tileState.GatherInteraction(gridPosition, buildingLayer);

    public virtual void SpecialInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer)
        => tileState.SpecialInteraction(gridPosition, buildingLayer);
    public virtual void CancelEvent(Vector2Int gridPosition, TileMapLayer tilemapLayer)
        => tileState.CancelEvent(gridPosition, tilemapLayer);

    public void Init(Vector2Int gridPosition, TileMapLayer tilemapLayer)
        => tileState.Init(gridPosition, tilemapLayer);
    #endregion
}


#endregion












