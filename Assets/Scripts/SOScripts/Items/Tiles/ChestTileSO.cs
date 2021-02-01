using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Block Tile", menuName = "SO/" + "Tiles/" + "Chest", order = 0)]
public class ChestTileSO : TileAbstSO
{
    public ChestTileState GetNewSlot => new ChestTileState(this);
}
public class ChestTileState : ITileState
{
    public ChestTileSO tile;
    public int chestId;

    public ChestTileState(ChestTileSO tile) {
        this.tile = tile;
        chestId = Inventory.GetInstance.GetNewIDForChest(12);
        Debug.Log("Creating new Chest. The ID is: " + chestId);
    }
    public bool isSpecialInteraction => tile.isSpecialInteraction;
    public TileBase GetMainTileBase => tile.GetMainTileBase;

    public ToolType GetInteractionType => throw new System.NotImplementedException();

    public TileType GetTileType => tile.GetTileType;

    public bool GetIsSolid => tile.GetIsSolid;

    public TileAbstSO GetTileAbst => tile;

    public void GatherInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) {
        throw new System.NotImplementedException();
    }

    public void CancelEvent(Vector2Int gridPosition, TileMapLayer tilemapLayer) {
        
    }

    public void SpecialInteraction(Vector2Int gridPosition, TileMapLayer buildingLayer) {
        UIManager._instance.OpenChest(chestId);
    }

    public void Init(Vector2Int gridPosition, TileMapLayer tilemapLayer, bool playerAction = true) 
    {

    }
}
