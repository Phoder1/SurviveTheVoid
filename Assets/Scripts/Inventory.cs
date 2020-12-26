using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public static class Inventory
{
    static List<ItemSlot> inventoryList = new List<ItemSlot>();
    const int maxCapacityOfItemsInList = 25;
    static bool isItemInList;
    static int remainer;
    public static List<ItemSlot> GetInventory { get => inventoryList; }

    public static bool CheckIfEnoughSpaceInSlot(ItemSlot item) {
        if (item == null)
            return false;

        remainer = 0;
        isItemInList = false;

        // check if the list is full and if there's items of the same kind that can be stocked
        if (inventoryList.Count >= maxCapacityOfItemsInList) {
            for (int i = 0; i < inventoryList.Count; i++) {
                if (item.resource.resourceEnum == inventoryList[i].resource.resourceEnum) {
                    remainer += inventoryList[i].resource.maxStackSize - inventoryList[i].amount;
                    isItemInList = true;
                }
            }

            if (item.amount < remainer) {
                Debug.Log("Inventory is full!");
                isItemInList = false;
            }
        }
        else
        {
            isItemInList =  true;
        }




        return isItemInList;
    }


    static void AddAmountOfItem(ItemSlot item)
    {


        if (item == null ||item.amount <= 0 )
            return;
        


        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].resource.resourceEnum == item.resource.resourceEnum)
            {
                if (inventoryList[i].amount == inventoryList[i].resource.maxStackSize)
                    continue;

                if (item.amount + inventoryList[i].amount > inventoryList[i].resource.maxStackSize)
                {
                    item.amount = Mathf.Abs(inventoryList[i].resource.maxStackSize - (inventoryList[i].amount + item.amount));
                    inventoryList[i].amount = inventoryList[i].resource.maxStackSize;
                    AddAmountOfItem(item);
                    break;
                }
                else if (item.amount + inventoryList[i].amount == inventoryList[i].resource.maxStackSize)
                {
                    inventoryList[i].amount = inventoryList[i].resource.maxStackSize;
                    break;
                }
                else
                {
                    if (CheckIfEnoughSpaceInSlot(item))
                    {
                        inventoryList.Add(item);
                        break;
                    }
                }


            }
        }

    }

    public static void AddToInventory(ItemSlot item)
    {
        if (item == null)
            return;
        // if i dont have it in the inventory
        if (!CheckIfEnoughSpaceInSlot(item))
        {
            AddAmountOfItem(item);
            return;
        }
    }

    
    public static void RemoveObjectFromInventory(ItemSlot item)
    {
        if (item == null || item.amount <= 0)
            return;
        
        // if item is not stackable
        if (item.resource.maxStackSize == 1)
        {
            inventoryList.Remove(item);
            return;
        }


        int amountIHave = GetAmountOfItem(item);
        if (item.amount > amountIHave)
        {
            Debug.Log("You are trying To Remove : " + item.amount + " and I Have Only This Amount : " + amountIHave);
            return;
        }


        // if item is istackable

        remainer = 0;
        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].resource.resourceEnum == item.resource.resourceEnum)
            {

                if (item.amount - inventoryList[i].amount > 0)
                {
                    item.amount =  item.amount - inventoryList[i].amount ;
                    inventoryList.Remove(inventoryList[i]); 
                    RemoveObjectFromInventory(item);
                    return;
                }
                else if (item.amount + inventoryList[i].amount == inventoryList[i].resource.maxStackSize)
                {
                    inventoryList.Remove(inventoryList[i]);
                    break;
                }
                else
                {
                    inventoryList[i].amount -= item.amount;
                }


            }
        }
    }

    public static bool CheckInventoryForItem(ItemSlot item) {
        if (item == null)
        {
            Debug.Log("Cant remove item because item is null");
            return false;
        }



        isItemInList = false;
        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (item.resource.resourceEnum == inventoryList[i].resource.resourceEnum)
            {
                isItemInList = true;
                break;
            }
        }

        return isItemInList;
    }

    public static bool CheckEnoughItemsForRecipe(RecipeSO recipe)
    {
        bool haveAllIngridients = true;


        foreach (var item in recipe.ResourcesCost)
        {
            haveAllIngridients = haveAllIngridients && HaveEnoughOfItemFromInventory(item);
        }

        if (haveAllIngridients)
        {
            for (int i = 0; i < recipe.ResourcesCost.Length; i++)
            {
                RemoveObjectFromInventory(recipe.ResourcesCost[i]);
            }
        }
        else
            Debug.Log("Cant Craft Not Enough resources");
        


        return haveAllIngridients;
    }

    static bool HaveEnoughOfItemFromInventory(ItemSlot item) {
        remainer = 0;
        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].resource.resourceEnum == item.resource.resourceEnum)
            {
                remainer += inventoryList[i].amount;
                if (remainer>= item.amount)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static int GetAmountOfItem(ItemSlot item) {
        remainer = 0;
        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (item.resource.resourceEnum == inventoryList[i].resource.resourceEnum)
            {
                remainer += inventoryList[i].amount;
            }
        }
        return remainer;
    
    }


    public static void PrintInventory() {

        for (int i = 0; i < inventoryList.Count; i++) {
            Debug.Log("Inventory list in spot "+i+" with the amount : " + inventoryList[i].amount + " of type: " + inventoryList[i].resource.resourceEnum.ToString());
        }
    
    }
}
