using Assets.TimeEvents;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Plant Tile", menuName = "SO/" + "Tiles/" + "Plant", order = 1)]
public class PlantTileSO : TileAbstSO
{
    [SerializeField] private ItemSlot[] rewards;
    public ItemSlot[] getRewards => rewards;
    [SerializeField] private TileBase[] stages;
    public TileBase[] getStages => stages;
}
public class PlantState : ITileState
{
    public TimeEvent eventInstance;
    public PlantTileSO tile;
    public int currentStage = 0;

    public PlantState(PlantTileSO tile, Vector2Int gridPosition, TileMapLayer tileMapLayer) {
        currentStage = 0;
        this.tile = tile;
    }
    public TileBase GetMainTileBase => tile.getStages[currentStage];

    public InteractionType GetInteractionType => tile.GetInteractionType;

    public TileType GetTileType => tile.GetTileType;

    public bool GetIsSolid => tile.GetIsSolid;

    public void GatherInteraction(Vector2Int gridPosition, TileMapLayer tileMapLayer) {
        Debug.Log("Tried gathering");
        GridManager._instance.SetTile(null, gridPosition, tileMapLayer, true);
        Inventory inventory = Inventory.GetInstance;
        foreach (ItemSlot reward in tile.getRewards) {
            inventory.AddToInventory(0, reward);
        }
        inventory.PrintInventory(0);
    }

    public void Remove(Vector2Int gridPosition, TileMapLayer tilemapLayer) {
    }

    public void SpecialInteraction(Vector2Int gridPosition, TileMapLayer tileMapLayer) {
        if (GetInteractionType == InteractionType.Special) {
            throw new System.NotImplementedException();
        }
    }
    public void Grow(Vector2Int gridPosition, TileMapLayer tileMapLayer) {

    }
}
public class PlantGrowEvent : TimeEvent
{
    protected TileSlot triggeringTile;
    protected readonly Vector2Int eventPosition;
    protected readonly TileMapLayer tileMapLayer;
    public PlantGrowEvent(float triggerTime, TileSlot triggeringTile, Vector2Int eventPosition, TileMapLayer tileMapLayer) : base(triggerTime) {
        this.triggeringTile = triggeringTile;
        this.eventPosition = eventPosition;
        this.tileMapLayer = tileMapLayer;
    }

    public override void Trigger() {
        ((PlantState)triggeringTile.tileState).Grow(eventPosition, tileMapLayer);
    }
}

