using System;
using UnityEngine;

public class Inventory : IInventory
{
    private static Inventory _instance;
    int maxCapacityOfItemsInList = 25;
    bool checkForItem;
    int counter;

    ItemSlot[] inventoryList;
    int nextAddOnAmountForInventory = 5;


    public ItemSlot[] GetInventory { get => inventoryList; }





    Inventory()
    {
        if (_instance == null)
        {
            _instance = new Inventory();
            inventoryList = new ItemSlot[maxCapacityOfItemsInList];
        }
    }

    public Inventory GetInstance
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



    public void MakeInventoryBigger(int _newSize)
    {
        if (_newSize < maxCapacityOfItemsInList)
            return;
        maxCapacityOfItemsInList += _newSize;
        ItemSlot[] newInventoryList = new ItemSlot[maxCapacityOfItemsInList];

        Array.Copy(inventoryList, newInventoryList, inventoryList.Length);
        inventoryList = newInventoryList;
    }

    public bool CheckIfEnoughSpaceInInventory(ItemSlot item) {
        if (item == null)
            return false;

        checkForItem = false;

        counter = 0;
        if (item.amount <= item.item.maxStackSize)
        {
            counter = 1;
        }
        else
        {


            counter = item.amount / item.item.maxStackSize;

            if (item.amount % item.item.maxStackSize > 0 && item.amount > item.item.maxStackSize)
            {
                counter += 1;
            }
        }




        if (counter <= GetAmountOfItem(null))
        {
            checkForItem = true;
        }



        counter = 0;

        if (!checkForItem)
        {
            for (int i = 0; i < inventoryList.Length; i++)
            {
                if (item.item.itemEnum == inventoryList[i].item.itemEnum)
                {
                    counter += inventoryList[i].item.maxStackSize;
                }
            }

            if (GetAmountOfItem(item) + item.amount <= counter)
            {
                checkForItem = true;
            }
        }

        return checkForItem;
    }


    void AddAmountOfItem(ItemSlot item)
    {


        if (item == null || item.amount <= 0)
            return;

        if (item.item.maxStackSize == 1)
        {
            if (item.amount <= GetAmountOfItem(null))
            {
                counter = 0;
                for (int i = 0; i < item.amount; i++)
                {
                    if (inventoryList[i] == null)
                    {
                        counter++;
                        inventoryList[i] = item;
                        inventoryList[i].amount = 1;
                    }


                    if (counter >= item.amount)
                    {
                        return;
                    }

                }

            }
        }





        for (int i = 0; i < inventoryList.Length; i++)
        {
            if (inventoryList[i] == null)
                continue;

            if (inventoryList[i].item.itemEnum == item.item.itemEnum)
            {
                if (inventoryList[i].amount == inventoryList[i].item.maxStackSize)
                    continue;

                if (item.amount + inventoryList[i].amount > inventoryList[i].item.maxStackSize)
                {
                    item.amount = Mathf.Abs(inventoryList[i].item.maxStackSize - (inventoryList[i].amount + item.amount));
                    inventoryList[i].amount = inventoryList[i].item.maxStackSize;
                    AddAmountOfItem(item);
                    break;
                }
                else if (item.amount + inventoryList[i].amount == inventoryList[i].item.maxStackSize)
                {
                    inventoryList[i].amount = inventoryList[i].item.maxStackSize;
                    return;
                }
                else if (item.amount + inventoryList[i].amount < inventoryList[i].item.maxStackSize)
                {
                    inventoryList[i].amount += item.amount;
                    return;
                }
            }
        }

        inventoryList[GetItemIndexInArray(null)] = item;

    }

    public void AddToInventory(ItemSlot item)
    {
        if (item == null)
            return;
        // if i dont have it in the inventory
        if (CheckIfEnoughSpaceInInventory(item))
        {
            AddAmountOfItem(item);
            return;
        }
        Debug.Log("Cant Add The Item");
    }
    public void RemoveObjectFromInventory(ItemSlot item)
    {
        if (item.amount < 0)
            item.amount *= -1;



        if (item == null || item.amount <= 0)
            return;

        // if item is not stackable
        if (item.item.maxStackSize == 1)
        {
            for (int x = 0; x < item.amount; x++)
            {
                for (int i = inventoryList.Length - 1; i >= 0; i--)
                {
                    if (inventoryList[i] != null && inventoryList[i].item.itemEnum == item.item.itemEnum)
                    {
                        inventoryList[i] = null;
                        break;
                    }
                }
            }

            return;
        }


        int amountIHave = GetAmountOfItem(item);
        if (item.amount > amountIHave)
        {
            Debug.Log("You are trying To Remove : " + item.amount + " and I Have Only This Amount : " + amountIHave);
            return;
        }


        // if item is istackable

        counter = 0;
        for (int i = inventoryList.Length - 1; i >= 0; i--)
        {
            if (inventoryList[i] == null)
                continue;

            if (inventoryList[i].item.itemEnum == item.item.itemEnum)
            {

                if (item.amount - inventoryList[i].amount > 0)
                {
                    item.amount = item.amount - inventoryList[i].amount;
                    inventoryList[i] = null;
                    RemoveObjectFromInventory(item);
                    return;
                }
                else if (item.amount - inventoryList[i].amount == 0)
                {
                    inventoryList[i] = null;
                    break;
                }
                else
                {
                    inventoryList[i].amount -= item.amount;
                    break;
                }


            }
        }
    }

    public bool CheckInventoryForItem(ItemSlot item)
    {

        checkForItem = false;
        for (int i = 0; i < inventoryList.Length; i++)
        {
            if (item == null && inventoryList[i] == null)
            {

                checkForItem = true;
                break;

            }
            else if (item.item.itemEnum == inventoryList[i].item.itemEnum)
            {
                checkForItem = true;
                break;
            }
        }

        return checkForItem;
    }

    public bool CheckEnoughItemsForRecipe(RecipeSO recipe)
    {
        bool haveAllIngridients = true;


        foreach (var item in recipe.itemCostArr)
        {
            haveAllIngridients = haveAllIngridients && HaveEnoughOfItemFromInventory(item);
            if (!haveAllIngridients)
            {
                haveAllIngridients = false;
                break;
            }
        }

        if (haveAllIngridients)
        {
            for (int i = 0; i < recipe.itemCostArr.Length; i++)
            {
                RemoveObjectFromInventory(recipe.itemCostArr[i]);
            }
            AddToInventory(recipe.outcomeItem);
        }
        else
            Debug.Log("Cant Craft Not Enough resources");



        return haveAllIngridients;
    }

    bool HaveEnoughOfItemFromInventory(ItemSlot item)
    {
        counter = 0;
        for (int i = 0; i < inventoryList.Length; i++)
        {
            if (inventoryList[i].item.itemEnum == item.item.itemEnum)
            {
                counter += inventoryList[i].amount;
                if (counter >= item.amount)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public int GetAmountOfItem(ItemSlot item)
    {
        counter = 0;
        for (int i = 0; i < inventoryList.Length; i++)
        {
            if (item == null && inventoryList[i] == null)
            {
                counter++;
                continue;
            }
            else if (item != null && inventoryList[i] != null)
            {
                if (item.item.itemEnum == inventoryList[i].item.itemEnum)
                {
                    counter += inventoryList[i].amount;
                }
            }
        }
        return counter;

    }
    public int GetItemIndexInArray(ItemSlot item)
    {

        for (int i = 0; i < inventoryList.Length; i++)
        {
            if (item == null)
            {
                if (inventoryList[i] == null)
                    return i;
            }
            else
            if (inventoryList[i] != null && item.item.itemEnum == inventoryList[i].item.itemEnum)
            {
                return i;
            }
        }

        Debug.Log("You Dont Have This Item In Your Inventory");
        return 0;
    }


    public void PrintInventory()
    {

        for (int i = 0; i < inventoryList.Length; i++)
        {
            if (inventoryList[i] == null)
            {
                Debug.Log("Inventory list in spot " + i + "is Null");
            }
            else
                Debug.Log("Inventory list in spot " + i + " with the amount : " + inventoryList[i].amount + " of type: " + inventoryList[i].item.itemEnum);
        }

    }
}
public interface IInventory
{
    Inventory GetInstance { get; }
    ItemSlot[] GetInventory { get; }

    void AddToInventory(ItemSlot item);
    bool CheckEnoughItemsForRecipe(RecipeSO recipe);
    bool CheckIfEnoughSpaceInInventory(ItemSlot item);
    bool CheckInventoryForItem(ItemSlot item);
    int GetAmountOfItem(ItemSlot item);
    int GetItemIndexInArray(ItemSlot item);
    void MakeInventoryBigger(int _newSize);
    void PrintInventory();
    void RemoveObjectFromInventory(ItemSlot item);
}















[Serializable]
public class ItemSlot
{
    public ItemSO item;
    public int amount;

    public ItemSlot(ItemSO item, int amount) {
        this.item = item;
        this.amount = amount;
    }
}
