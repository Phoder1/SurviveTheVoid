using System;
using System.Collections.Generic;
using UnityEditor;
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
            }

            return _instance;
        }
    }
    int maxCapacityOfItemsInList = 18, maxCapacityOfItemsInChest = 12;
    bool checkForItem = false;
    int counter = 0;
    int itemAmountCount = 0;
    int amountOfIDChests = 0; // 0 is the player's inventory

    ItemSlot[] inventoryList;
    //private int nextAddOnAmountForInventory = 5;



    public ItemSlot[] GetInventory { get => inventoryList; }
    private ItemSlot[] inventoryCache;


    Dictionary<int, ItemSlot[]> inventoryDict;
    // 0 = > player's inventory
    //1+ = > local inventory chests




    private Inventory()
    {
        inventoryList = new ItemSlot[maxCapacityOfItemsInList];
        inventoryDict = new Dictionary<int, ItemSlot[]>();
        inventoryDict.Add(amountOfIDChests, inventoryList);
        inventoryUI = InventoryUIManager._instance;
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
        if (item == null)
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
                if (item.item.getItemEnum == inventoryCache[i].item.getItemEnum)
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

            if (inventoryCache[i].item.getItemEnum == item.item.getItemEnum)
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

    public void AddToInventory(int chestID, ItemSlot item)
    {
        if (item == null)
            return;
        // if i dont have it in the inventory
        if (CheckIfEnoughSpaceInInventory(chestID, item))
        {
            itemAmountCount = 0;
            AddAmountOfItem(chestID, item);
            inventoryUI.UpdateInventoryToUI();
            return;
        }
        Debug.Log("Cant Add The Item");
    }

    private void RemoveObjectFromInventory(int chestID, ItemSlot item)
    {
        if (item.amount < 0)
            item.amount *= -1;



        if (item == null || item.amount <= 0)
            return;

        inventoryCache = GetInventoryFromDictionary(chestID);
        if (inventoryCache == null)
            return;

        // if item is not stackable
        if (item.item.getmaxStackSize == 1)
        {
            for (int x = 0; x < item.amount; x++)
            {
                for (int i = inventoryList.Length - 1; i >= 0; i--)
                {
                    if (inventoryCache[i] != null && inventoryCache[i].item.getItemEnum == item.item.getItemEnum)
                    {
                        inventoryCache[i] = null;
                        break;
                    }
                }
            }

            return;
        }


        int amountIHave = GetAmountOfItem(chestID, item);
        if (item.amount > amountIHave)
        {
            Debug.Log("You are trying To Remove : " + item.amount + " and I Have Only This Amount : " + amountIHave);
            return;
        }


        // if item is istackable

        counter = 0;
        for (int i = inventoryCache.Length - 1; i >= 0; i--)
        {
            if (inventoryCache[i] == null)
                continue;

            if (inventoryCache[i].item.getItemEnum == item.item.getItemEnum)
            {

                if (itemAmountCount - inventoryCache[i].amount > 0)
                {
                    itemAmountCount = itemAmountCount - inventoryCache[i].amount;
                    inventoryCache[i] = null;
                    RemoveObjectFromInventory(chestID, item);
                    return;
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
    }

    public void RemoveItemFromInventory(int chestID, ItemSlot item)
    {
        itemAmountCount = item.amount;
        RemoveObjectFromInventory(chestID, item);
        inventoryUI.UpdateInventoryToUI();
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
            else if (item.item.getItemEnum == inventoryCache[i].item.getItemEnum)
            {
                checkForItem = true;
                break;
            }
        }

        return checkForItem;
    }

    public bool CheckEnoughItemsForRecipe(RecipeSO recipe, TileSlot workBench)
    {
        bool haveAllIngridients = true;
        

        foreach (var item in recipe.getitemCostArr)
        {
            haveAllIngridients = haveAllIngridients && HaveEnoughOfItemFromInventory(0, item);
            if (!haveAllIngridients)
            {
                haveAllIngridients = false;
                break;
            }
        }

        if (haveAllIngridients)
        {
            for (int i = 0; i < recipe.getitemCostArr.Length; i++)
            {
                RemoveItemFromInventory(0, recipe.getitemCostArr[i]);
            }
        
            // workBench.add(,recipe.getoutcomeItem);


            if (GetAmountOfItem(0,null) > 0 || GetAmountOfItem(0, recipe.getoutcomeItem) < recipe.getoutcomeItem.item.getmaxStackSize)
            {
                AddToInventory(0, recipe.getoutcomeItem);
            }
            else
            {
                for (int i = 0; i < recipe.getitemCostArr.Length; i++)
                {
                    AddToInventory(0, recipe.getitemCostArr[i]);
                }
            }

        }
        else
            Debug.Log("Cant Craft Not Enough resources");



        return haveAllIngridients;
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
            if (inventoryCache[i].item.getItemEnum == item.item.getItemEnum)
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
                if (item.item.getItemEnum == inventoryCache[i].item.getItemEnum)
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
            if (inventoryCache[i] != null && item.item.getItemEnum == inventoryCache[i].item.getItemEnum)
            {
                return i;
            }
        }

        Debug.Log("You Dont Have This Item In Your Inventory");
        return 0;
    }
    public int GetNewIDForChest()
    {
        amountOfIDChests++;
        CreateNewInventory(amountOfIDChests);
        return amountOfIDChests;
    }
    public ItemSlot[] GetInventoryFromDictionary(int id)
    {
        inventoryCache = null;

        inventoryDict.TryGetValue(id, out inventoryCache);

        return inventoryCache;
    }

    public void CreateNewInventory(int chestId) => inventoryDict.Add(chestId, new ItemSlot[maxCapacityOfItemsInChest]);

    public void ChangeBetweenButtons(int chestID, int drag, int drop)
    {

        inventoryCache = GetInventoryFromDictionary(chestID);
        if (inventoryCache == null)
            return;
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

    public ItemSlot GetItemFromInventoryButton(int chestId, int buttonId)
	{
        inventoryCache = GetInventoryFromDictionary(chestId);
        if (inventoryCache == null)
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
                Debug.Log("Inventory list in spot " + i + " with the amount : " + inventoryCache[i].amount + " of type: " + inventoryCache[i].item.getItemEnum);
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
    public ItemSlot(ItemSO item, int amount,int? durability = null) {
        this.item = item;
        this.amount = amount;
        this.durability = durability;
    }
}
