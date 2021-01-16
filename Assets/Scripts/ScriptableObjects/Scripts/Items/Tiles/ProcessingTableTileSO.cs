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
    public RecipeSO craftingRecipe;
    public TimeEvent eventInstance;
    private float craftingStartTime;
    public int amount;
    public int ItemsCrafted => Mathf.FloorToInt((Time.time - craftingStartTime) / craftingRecipe.GetCraftingTime);
    public float CraftingTimeRemaining => (craftingStartTime  + craftingRecipe.GetCraftingTime * amount) - Time.time;
    private bool isCrafting;
    public bool IsCrafting {
        get => isCrafting;
        set => isCrafting = value;
    }
    public ProcessingTableTileState(ProcessingTableTileSO tile) {
        this.tile = tile;
    }
    public void StartCrafting(RecipeSO recipe, int amount) {
        if (isCrafting)
            throw new System.Exception();
        craftingRecipe = recipe;
        craftingStartTime = Time.time;
        this.amount = amount;
    }

    public void CollectItems(int numOfItems) {
        amount -= numOfItems;
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

    public ToolType GetInteractionType => throw new System.NotImplementedException();

    public TileType GetTileType => throw new System.NotImplementedException();

    public bool GetIsSolid => tile.GetIsSolid;
    public bool isSpecialInteraction => tile.isSpecialInteraction;



    public void GatherInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) {
        throw new System.NotImplementedException();
    }

    public void CancelEvent(Vector2Int gridPosition, TileMapLayer tilemapLayer) {
    }

    public void SpecialInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) {
        UIManager._instance.SetCraftingUIState(true,tile.GetProcessorType,this);
    }

    public void Init(Vector2Int gridPosition, TileMapLayer tilemapLayer, bool playerAction = true) { }
}

