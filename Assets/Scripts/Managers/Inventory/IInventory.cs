public interface IInventory
{
    ItemSlot[] GetInventory { get; }

    void AddToInventory(int chestID, ItemSlot item);
    bool CheckEnoughItemsForRecipe(RecipeSO recipe);
    bool CheckIfEnoughSpaceInInventory(int chestID, ItemSlot item);
    bool CheckInventoryForItem(int chestID, ItemSlot item);
    void CreateNewInventory(int chestId);
    int GetAmountOfItem(int chestID, ItemSlot item);
    ItemSlot[] GetInventoryFromDictionary(int id);
    int GetItemIndexInArray(int chestID, ItemSlot item);
    int GetNewIDForChest();
    void MakeInventoryBigger(int _newSize, int chestID);
    void PrintInventory(int chestID);
    void RemoveItemFromInventory(int chestID, ItemSlot item);
}