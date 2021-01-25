using Assets.TimeEvents;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Crafting Tile", menuName = "SO/" + "Tiles/" + "Processing Table", order = 2)]
public class ProcessingTableTileSO : TileAbstSO
{
    [SerializeField] private ProcessorType processorType;
    [SerializeField] private TileBase whenActiveTile;
    [SerializeField] private float speed;
    [SerializeField] Sound gatheringSound;
    public ProcessorType GetProcessorType => processorType;
    public TileBase GetWhenActiveTile => whenActiveTile;
    public float GetSpeed => speed;
}
public class ProcessingTableTileState : ITileState
{
    public ProcessingTableTileSO tile;
    public TimeEvent eventInstance;
    public Vector2Int gridPosition;
    public TileSlot tileSlot;
    public ProcessingTableTileState(ProcessingTableTileSO tile, TileSlot tileSlot) {
        this.tileSlot = tileSlot;
        this.tile = tile;
    }
    public RecipeSO craftingRecipe;
    private float craftingStartTime;
    public int amount;
    public int ItemsCrafted {
        get {
            if (!IsCrafting)
                return 0;
            return Mathf.Min(Mathf.FloorToInt((Time.time - craftingStartTime) / craftingRecipe.GetCraftingTime), amount);

        }
    }
    public float CraftingEndTime {
        get {
            if (!craftingRecipe)
                return Time.time;
            return craftingStartTime + craftingRecipe.GetCraftingTime * amount;
        }
    }
    public float CraftingTimeRemaining {
        get {
            if (!IsCrafting)
                return 0;
            return Mathf.Max(CraftingEndTime - Time.time, 0);
        }
    }
    public bool GetIsDestructible => !IsCrafting;
    private bool queueFinished = true;
    public bool QueueFinished {
        get => queueFinished;
        set {
            if (queueFinished != value) {
                queueFinished = value;
                GridManager._instance.SetTile(tileSlot, gridPosition, TileMapLayer.Buildings);
            }
        }
    }
    private bool isCrafting;
    public bool IsCrafting {
        get => isCrafting;
        set => isCrafting = value;
    }
    public void StartCrafting(RecipeSO recipe, int amount) {
        if (IsCrafting)
            throw new System.Exception();
        craftingRecipe = recipe;
        craftingStartTime = Time.time;
        IsCrafting = true;
        QueueFinished = false;
        this.amount = amount;
        eventInstance = new TileChangeTimeEvent(CraftingEndTime, this);
    }
    public void CollectItems(int numOfItems) {

        amount -= numOfItems;
        craftingStartTime += craftingRecipe.GetCraftingTime * numOfItems;
        if (amount == 0) {
            ResetCrafting();
        }
        if (amount < 0) {
            throw new System.NotImplementedException();
        }
    }
    public void AddToQueue(int numOfItems) {
        amount += numOfItems;
        eventInstance.UpdateTriggerTime(CraftingEndTime);
    }

    public void ResetCrafting() {
        IsCrafting = false;
        craftingRecipe = null;

    }
    public TileBase GetMainTileBase {
        get {
            if (IsCrafting && !QueueFinished) {
                return tile.GetWhenActiveTile;
            }
            else {
                return tile.GetMainTileBase;
            }
        }
    }
    public TileAbstSO GetTileAbst => tile;

    public ToolType GetInteractionType => throw new System.NotImplementedException();

    public TileType GetTileType => tile.GetTileType;

    public bool GetIsSolid => tile.GetIsSolid;
    public bool isSpecialInteraction => tile.isSpecialInteraction;


    public void GatherInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) {
        throw new System.NotImplementedException();
    }

    public void CancelEvent(Vector2Int gridPosition, TileMapLayer tilemapLayer) {
        if (eventInstance != null)
            eventInstance.Cancel();
    }

    public void SpecialInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) {
        UIManager._instance.SetCraftingUIState(true, tile.GetProcessorType, this);
    }

    public void Init(Vector2Int gridPosition, TileMapLayer tilemapLayer, bool playerAction = true) {
        this.gridPosition = gridPosition;
    }

    private class TileChangeTimeEvent : TimeEvent
    {
        private readonly ProcessingTableTileState triggeringTile;
        public TileChangeTimeEvent(float triggerTime, ProcessingTableTileState triggeringTile) : base(triggerTime) {
            this.triggeringTile = triggeringTile;
        }

        protected override void TriggerBehaviour() {
            triggeringTile.QueueFinished = true;
        }
    }
}

