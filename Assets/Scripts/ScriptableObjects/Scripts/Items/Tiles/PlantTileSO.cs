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


    [SerializeField] private float minGrowTime;
    [SerializeField] private float maxGrowTime;
    public float GetMinGrowTime { get => minGrowTime; }
    public float GetMaxGrowTime { get => maxGrowTime; }
}
public class PlantState : ITileState
{
    public TileSlot tileSlot;
    public TimeEvent eventInstance;
    public PlantTileSO tile;
    public int currentStage = 0;

    public bool reachedMaxStage => currentStage >= tile.getStages.Length - 1;

    public PlantState(PlantTileSO tile, TileSlot tileSlot) {
        currentStage = 0;
        this.tile = tile;
        this.tileSlot = tileSlot;
    }
    public TileBase GetMainTileBase => tile.getStages[currentStage];
    public TileAbstSO GetTileAbst => tile;

    public InteractionType GetInteractionType => tile.GetInteractionType;

    public TileType GetTileType => tile.GetTileType;

    public bool GetIsSolid => tile.GetIsSolid;

    public void GatherInteraction(Vector2Int gridPosition, TileMapLayer tileMapLayer) {
        if (reachedMaxStage) {
            Debug.Log("Tried gathering");
            GridManager._instance.SetTile(null, gridPosition, tileMapLayer, true);
            Inventory inventory = Inventory.GetInstance;
            foreach (ItemSlot reward in tile.getRewards) {
                inventory.AddToInventory(0, reward);
            }
            inventory.PrintInventory(0);
        }
    }

    public void CancelEvent(Vector2Int gridPosition, TileMapLayer tilemapLayer) {
        if (eventInstance != null)
            eventInstance.Cancel();
    }

    public void SpecialInteraction(Vector2Int gridPosition, TileMapLayer tileMapLayer) {
        if (GetInteractionType == InteractionType.Special) {
            throw new System.NotImplementedException();
        }
    }
    public void Grow(Vector2Int gridPosition, TileMapLayer tileMapLayer) {
        currentStage++;
        GridManager._instance.SetTile(tileSlot, gridPosition, tileMapLayer, false);
        if (!reachedMaxStage) {
            InitEvent(gridPosition, tileMapLayer);
        }
    }
    public void Init(Vector2Int gridPosition, TileMapLayer tilemapLayer) { if (eventInstance == null) InitEvent(gridPosition, tilemapLayer); }
    private void InitEvent(Vector2Int gridPosition, TileMapLayer tileMapLayer) {
        eventInstance = new PlantGrowEvent(Time.time + Random.Range(tile.GetMinGrowTime, tile.GetMaxGrowTime), tileSlot, gridPosition, tileMapLayer);
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
        eventTriggered = true;
        ((PlantState)triggeringTile.tileState).Grow(eventPosition, tileMapLayer);

    }
}

