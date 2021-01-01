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

    public PlantState(PlantTileSO tile) {
        currentStage = 0;
        this.tile = tile;
    }
    public TileBase GetMainTileBase => tile.getStages[currentStage];

    public InteractionType GetInteractionType => tile.GetInteractionType;

    public TileType GetTileType => throw new System.NotImplementedException();

    public bool GetIsSolid => throw new System.NotImplementedException();

    public void GatherInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) {
        Debug.Log("Tried gathering");
        GridManager._instance.SetTile(null, gridPosition, buildingLayer, true);
        Inventory inventory = Inventory.GetInstance;
        foreach (ItemSlot reward in tile.getRewards) {
            inventory.AddToInventory(0, reward);
        }
        inventory.PrintInventory(0);
    }

    public void Remove(Vector2Int gridPosition, TileMapLayer tilemapLayer) {
    }

    public void SpecialInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) {
        throw new System.NotImplementedException();
    }
}

