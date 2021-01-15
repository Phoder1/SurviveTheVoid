using Assets.TimeEvents;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Crafting Tile", menuName = "SO/" + "Tiles/" + "Processing Table", order = 2)]
public class ProcessingTableTileSO : TileAbstSO
{
    [SerializeField] private ProcessorType processorType;
    [SerializeField] private TileBase whenActiveTile;
    [SerializeField] private float speed;
    public ProcessorType GetProcessorType => processorType; 
    public TileBase GetWhenActiveTile => whenActiveTile;
    public float GetSpeed => speed;
}
public class ProcessingTableTileState : ITileState
{
    public ProcessingTableTileSO tile;
    public TimeEvent eventInstance;
    public float craftingTimeEnd;
    private bool isCrafting;
    public bool IsCrafting { get => isCrafting; 
        set => isCrafting = value;
    }
    public ProcessingTableTileState(ProcessingTableTileSO tile) {
        this.tile = tile;
    }

    public TileBase GetMainTileBase {
        get {
            if (IsCrafting) {
                return tile.GetWhenActiveTile;
            }
            else {
                return tile.GetMainTileBase;
            }
        }
    }
    public TileAbstSO GetTileAbst => tile;

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

    }

    public void Init(Vector2Int gridPosition, TileMapLayer tilemapLayer) { }
}

