using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private static Inventory _instance;
    //Inventory IInventory.GetInstance => GetInstance;

    InventoryUIManager inventoryUI;
    public static Inventory GetInstance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Inventory();
                var x = EquipManager.GetInstance;
            }

            return _instance;
        }
    }
    int maxCapacityOfItemsInList;
    bool checkForItem = false;
    int counter = 0;
    int itemAmountCount;
    int amountOfIDChests; // 0 is the player's inventory

    ItemSlot[] inventoryList;
    //private int nextAddOnAmountForInventory = 5;



    public ItemSlot[] GetInventory { get => inventoryList; }
    private ItemSlot[] inventoryCache;


    Dictionary<int, ItemSlot[]> inventoryDict;
    // 0 = > player's inventory
    //1  = > Hot Keys
    //2+ = > Equips
    //3? = > tools
    //3+ = > local inventory chests




    private Inventory()
    {
        ResetInventoryClass();
    }





    public void MakeInventoryBigger(int _newSize, int chestID)
    {
        if (_newSize < maxCapacityOfItemsInList || inventoryDict.ContainsKey(chestID))
            return;
        inventoryCache = null;
        maxCapacityOfItemsInList += _newSize;

        ItemSlot[] newInventoryList = new ItemSlot[maxCapacityOfItemsInList];

        inventoryCache = GetInventoryFromDictionary(chestID);
        if (inventoryCache != null)
        {
            Array.Copy(inventoryCache, newInventoryList, inventoryList.Length);
            inventoryCache = newInventoryList;
        }
        else
            Debug.Log("cannot expand inventory");


    }

    public bool CheckIfEnoughSpaceInInventory(int chestID, ItemSlot item)
    {
        if (item == null || item.item == null)
            return false;

        inventoryCache = GetInventoryFromDictionary(chestID);
        if (inventoryCache == null)
            return false;





        checkForItem = false;

        counter = 0;
        if (item.amount <= item.item.getmaxStackSize)
        {
            counter = 1;
        }
        else
        {


            counter = item.amount / item.item.getmaxStackSize;

            if (item.amount % item.item.getmaxStackSize > 0 && item.amount > item.item.getmaxStackSize)
            {
                counter += 1;
            }
        }




        if (counter <= GetAmountOfItem(chestID, null))
        {
            checkForItem = true;
        }



        counter = 0;

        if (!checkForItem)
        {
            for (int i = 0; i < inventoryCache.Length; i++)
            {
                if (item.item.itemID == inventoryCache[i].item.itemID)
                {
                    counter += inventoryCache[i].item.getmaxStackSize;
                }
            }

            if (GetAmountOfItem(chestID, item) + item.amount <= counter)
            {
                checkForItem = true;
            }
        }

        return checkForItem;
    }

    void AddAmountOfItem(int chestID, ItemSlot item)
    {


        if (item == null || item.amount <= 0)
            return;


        inventoryCache = GetInventoryFromDictionary(chestID);
        if (inventoryCache == null)
            return;

        if (item.item.getmaxStackSize == 1)
        {
            if (item.amount <= GetAmountOfItem(chestID, null))
            {
                counter = 0;
                for (int i = 0; i < item.amount; i++)
                {
                    if (inventoryCache[i] == null)
                    {
                        counter++;

                        inventoryCache[i] = new ItemSlot(item.item, item.amount);
                        inventoryCache[i].amount = 1;
                    }


                    if (counter >= item.amount)
                    {
                        return;
                    }

                }

            }
        }



        int test = item.amount - itemAmountCount;

        for (int i = 0; i < inventoryCache.Length; i++)
        {
            if (inventoryCache[i] == null)
                continue;

            if (inventoryCache[i].item.itemID == item.item.itemID)
            {
                if (inventoryCache[i].amount == inventoryCache[i].item.getmaxStackSize)
                    continue;

                if (test + inventoryCache[i].amount > inventoryCache[i].item.getmaxStackSize)
                {

                    test = Mathf.Abs(inventoryCache[i].item.getmaxStackSize - (inventoryCache[i].amount + test));
                    inventoryCache[i].amount = inventoryCache[i].item.getmaxStackSize;
                    AddAmountOfItem(chestID, item);
                    break;
                }
                else if (test + inventoryCache[i].amount == inventoryCache[i].item.getmaxStackSize)
                {
                    inventoryCache[i].amount = inventoryCache[i].item.getmaxStackSize;
                    return;
                }
                else if (test + inventoryCache[i].amount < inventoryCache[i].item.getmaxStackSize)
                {
                    inventoryCache[i].amount += test;
                    return;
                }
            }
        }

        inventoryCache[GetItemIndexInArray(chestID, null)] = new ItemSlot(item.item, test); ;

    }

    public void ResetInventoryClass() {
        maxCapacityOfItemsInList = 18;
        checkForItem = false;
        itemAmountCount = 0;
        amountOfIDChests = 0;
        inventoryList = new ItemSlot[maxCapacityOfItemsInList];
        inventoryDict = new Dictionary<int, ItemSlot[]>();
        inventoryDict.Add(amountOfIDChests, inventoryList);
        GetNewIDForChest(5);
        GetNewIDForChest(5);
        GetNewIDForChest(5);
        inventoryUI = InventoryUIManager._instance;
    }
    public bool AddToInventory(int chestID, ItemSlot item)
    {
        if (item == null)
            return false;
        // if i dont have it in the inventory
        if (CheckIfEnoughSpaceInInventory(chestID, item))
        {
            itemAmountCount = 0;
            AddAmountOfItem(chestID, item);
            inventoryUI.UpdateInventoryToUI();
            return true;
        }
        Debug.Log("Cant Add The Item");
        return false;
    }

    private bool RemoveObjectFromInventory(int chestID, ItemSlot item)
    {
        if (item.amount < 0)
            item.amount *= -1;



        if (item == null || item.amount <= 0)
            return false;

        inventoryCache = GetInventoryFromDictionary(chestID);
        if (inventoryCache == null)
            return false;

        // if item is not stackable
        if (item.item.getmaxStackSize == 1)
        {
            for (int x = 0; x < item.amount; x++)
            {
                for (int i = inventoryList.Length - 1; i >= 0; i--)
                {
                    if (inventoryCache[i] != null && inventoryCache[i].item.itemID == item.item.itemID)
                    {
                        inventoryCache[i] = null;
                        break;
                    }
                }
            }

            return false;
        }


        int amountIHave = GetAmountOfItem(chestID, item);
        if (itemAmountCount > amountIHave)
        {
            Debug.Log("You are trying To Remove : " + item.amount + " and I Have Only This Amount : " + amountIHave);
            return false;
        }


        // if item is istackable

        counter = 0;
        for (int i = inventoryCache.Length - 1; i >= 0; i--)
        {
            if (inventoryCache[i] == null)
                continue;

            if (inventoryCache[i].item.itemID == item.item.itemID)
            {

                if (itemAmountCount - inventoryCache[i].amount > 0)
                {
                    itemAmountCount = itemAmountCount - inventoryCache[i].amount;
                    inventoryCache[i] = null;
                    RemoveObjectFromInventory(chestID, item);
                    return true ;
                }
                else if (itemAmountCount - inventoryCache[i].amount == 0)
                {
                    inventoryList[i] = null;
                    break;
                }
                else
                {
                    inventoryCache[i].amount -= itemAmountCount;
                    break;
                }


            }
        }
        return true;
    }

    public bool RemoveItemFromInventory(int chestID, ItemSlot item)
    {
        itemAmountCount = item.amount;
       
        if (RemoveObjectFromInventory(chestID, item))
        {
        inventoryUI.UpdateInventoryToUI();
            return true;
        }


        return false;
    }

    public bool CheckInventoryForItem(int chestID, ItemSlot item)
    {
        inventoryCache = GetInventoryFromDictionary(chestID);
        if (inventoryCache == null)
            return false;


        checkForItem = false;
        for (int i = 0; i < inventoryCache.Length; i++)
        {
            if (item == null && inventoryCache[i] == null)
            {

                checkForItem = true;
                break;

            }
            else if (item.item.itemID == inventoryCache[i].item.itemID)
            {
                checkForItem = true;
                break;
            }
        }

        return checkForItem;
    }

    public bool CheckEnoughItemsForRecipe(RecipeSO recipe, int amount = 1) //, TileSlot workBench
    {
        bool haveAllIngridients = true;


        foreach (var item in recipe.getitemCostArr)
        {
            haveAllIngridients &= HaveEnoughOfItemFromInventory(0, new ItemSlot(item.item, item.amount * amount));
            if (!haveAllIngridients)
                return false;
        }


        return haveAllIngridients;
    }

    public bool RemoveItemsByRecipe(RecipeSO recipe, int Amount = 1)
    {
        if (CheckEnoughItemsForRecipe(recipe, Amount))
        {
            for (int i = 0; i < recipe.getitemCostArr.Length; i++)
            {
                RemoveItemFromInventory(0, new ItemSlot(recipe.getitemCostArr[i].item, recipe.getitemCostArr[i].amount * Amount));
            }
        }

        return false;

    }

    public void RemoveItemFromButton(int buttonID, int chestID) {


        inventoryCache = GetInventoryFromDictionary(chestID);
        if (inventoryCache == null)
            return;

        if (buttonID<0 || buttonID >= inventoryCache.Length)
            return;
        

        inventoryCache[buttonID] = null;
    }

    bool HaveEnoughOfItemFromInventory(int chestID, ItemSlot item)
    {
        inventoryCache = GetInventoryFromDictionary(chestID);
        if (inventoryCache == null)
            return false;

        counter = 0;
        for (int i = 0; i < inventoryCache.Length; i++)
        {
            if (item == null && inventoryCache[i] == null)
            {
                counter += 1;
                continue;
            }
            if (inventoryCache[i] == null)
            {
                continue;
            }
            if (inventoryCache[i].item.itemID == item.item.itemID)
            {
                counter += inventoryList[i].amount;
                if (counter >= item.amount)
                {
                    return true;
                }
            }
        }
        //if ((float)counter >= (float)(item.amount) / (float)(item.item.getmaxStackSize))
        //{
        //    return true;
        //}

        return false;
    }

    public int GetAmountOfItem(int chestID, ItemSlot item)
    {
        inventoryCache = GetInventoryFromDictionary(chestID);
        if (inventoryCache == null)
        {
            Debug.Log("Couldnt find the inventory");
            return 0;
        }




        int counterCache = 0;
        for (int i = 0; i < inventoryCache.Length; i++)
        {
            if (item == null && inventoryCache[i] == null)
            {
                counterCache++;
                continue;
            }
            else if (item != null && inventoryCache[i] != null)
            {
                if (item.item.itemID == inventoryCache[i].item.itemID)
                {
                    counterCache += inventoryCache[i].amount;
                }
            }
        }
        return counterCache;

    }
    public int GetItemIndexInArray(int chestID, ItemSlot item)
    {

        inventoryCache = GetInventoryFromDictionary(chestID);
        if (inventoryCache == null)
            return 0;


        for (int i = 0; i < inventoryCache.Length; i++)
        {
            if (item == null)
            {
                if (inventoryCache[i] == null)
                    return i;
            }
            else
            if (inventoryCache[i] != null && item.item.itemID == inventoryCache[i].item.itemID)
            {
                return i;
            }
        }

        Debug.Log("You Dont Have This Item In Your Inventory");
        return 0;
    }
    public int GetNewIDForChest(int amountOfCapacity)
    {
        amountOfIDChests++;
        CreateNewInventory(amountOfIDChests , amountOfCapacity);
        return amountOfIDChests;
    }
    public ItemSlot[] GetInventoryFromDictionary(int id)
    {
        ItemSlot[] Cache = null;

        inventoryDict.TryGetValue(id, out Cache);

        return Cache;
    }

    public void CreateNewInventory(int chestId, int amountOfCapacity) => inventoryDict.Add(chestId, new ItemSlot[amountOfCapacity]);

    public void ChangeBetweenItems(int firstChestID, int secondChestID, int drag, int drop)
    {
     
        inventoryCache = GetInventoryFromDictionary(firstChestID);
        if (inventoryCache == null )
            return;
        if (drag < 0 || drag >= inventoryCache.Length)
            return;


        if (firstChestID != secondChestID)
        {
            var inventoryCacheTwo = GetInventoryFromDictionary(secondChestID);
            if (inventoryCacheTwo == null)
                return;
            if (drop < 0 || drop >= inventoryCacheTwo.Length)
                return;

            if (inventoryCache[drag] == null && inventoryCacheTwo[drop] == null)
            {
                return;
            }
            else if (inventoryCache[drag] != null && inventoryCacheTwo[drop] != null)
            {
                ItemSlot temporaryCache = inventoryCacheTwo[drop];
                inventoryCacheTwo[drop] = inventoryCache[drag];
                inventoryCache[drag] = temporaryCache;
            }
            else if (inventoryCache[drag] == null && inventoryCacheTwo[drop] != null)
            {
                inventoryCache[drag] = inventoryCacheTwo[drop];
                inventoryCacheTwo[drop] = null;
            }
            else if (inventoryCache[drag] != null && inventoryCacheTwo[drop] == null)
            {
                inventoryCacheTwo[drop] = inventoryCache[drag];
                inventoryCache[drag] = null;
            }

        }
        else
        {
            if (inventoryCache[drag] == null && inventoryCache[drop] == null)
            {
                return;
            }
            else if (inventoryCache[drag] != null && inventoryCache[drop] != null)
            {
                ItemSlot temporaryCache = inventoryCache[drop];
                inventoryCache[drop] = inventoryCache[drag];
                inventoryCache[drag] = temporaryCache;
            }
            else if (inventoryCache[drag] == null && inventoryCache[drop] != null)
            {
                inventoryCache[drag] = inventoryCache[drop];
                inventoryCache[drop] = null;
            }
            else if (inventoryCache[drag] != null && inventoryCache[drop] == null)
            {
                inventoryCache[drop] = inventoryCache[drag];
                inventoryCache[drag] = null;
            }

        }


    }

    public ItemSlot GetItemFromInventoryButton(int chestId, int buttonId)
    {
        inventoryCache = GetInventoryFromDictionary(chestId);
        if (inventoryCache == null)
            return null;

        if (buttonId < 0 || buttonId >= inventoryCache.Length)
            return null;

        return inventoryCache[buttonId];
    }

    public void PrintInventory(int chestID)
    {
        inventoryCache = GetInventoryFromDictionary(chestID);
        if (inventoryCache == null)
            return;

        for (int i = 0; i < inventoryCache.Length; i++)
        {
            if (inventoryCache[i] == null)
            {
                Debug.Log("Inventory list in spot " + i + "is Null");
            }
            else
                Debug.Log("Inventory list in spot " + i + " with the amount : " + inventoryCache[i].amount + " of type: " + inventoryCache[i].item.getItemName);
        }

    }
}


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
[Serializable]
public class ItemSlot
{
    public ItemSO item;
    public int amount;
    public int? durability;
    public ItemSlot(ItemSO item, int amount, int? durability = null)
    {
        this.item = item;
        this.amount = amount;
        this.durability = durability;
    }
}
