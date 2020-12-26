using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public static class Inventory
{
  //  static List<ItemSlot> inventoryList = new List<ItemSlot>();
    static int maxCapacityOfItemsInList = 25;
    static bool checkForItem;
    static int counter;

    static ItemSlot[] inventoryList = new ItemSlot[maxCapacityOfItemsInList];
    static int nextAddOnAmountForInventory = 5;




    public static ItemSlot[] GetInventory { get => inventoryList; }
    public static void  MakeInventoryBigger(int _newSize) {
        if (_newSize< maxCapacityOfItemsInList)
            return;
        maxCapacityOfItemsInList += _newSize;
        ItemSlot[] newInventoryList = new ItemSlot[maxCapacityOfItemsInList];

        Array.Copy(inventoryList, newInventoryList, inventoryList.Length);
        inventoryList = newInventoryList;
    }

    public static bool CheckIfEnoughSpaceInInventory(ItemSlot item) {
        if (item == null)
            return false;

        checkForItem = false;

        counter = 0;
        if (item.amount <= item.resource.maxStackSize)
        {
            counter = 1;
        }
        else
        {


            counter = item.amount / item.resource.maxStackSize;

            if (item.amount % item.resource.maxStackSize > 0 && item.amount > item.resource.maxStackSize)
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
                if (item.resource.resourceEnum == inventoryList[i].resource.resourceEnum)
                {
                    counter += inventoryList[i].resource.maxStackSize;
                }
            }

            if (GetAmountOfItem(item) + item.amount <= counter)
            {
                checkForItem = true;
            }
        }

        return checkForItem;
    }


    static void AddAmountOfItem(ItemSlot item)
    {


        if (item == null || item.amount <= 0)
            return;

        if (item.resource.maxStackSize == 1)
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


                    if (counter>= item.amount)
                    {
                        return;
                    }
                 
                }

            }
        }





        for (int i = 0; i < inventoryList.Length; i++)
        {
            if (inventoryList[i]== null)
                continue;
            
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
                    return;
                }
                else if (item.amount + inventoryList[i].amount < inventoryList[i].resource.maxStackSize)
                {
                    inventoryList[i].amount += item.amount;
                    return;
                }
            }
        }

        inventoryList[GetItemIndexInArray(null)] = item;

    }

    public static void AddToInventory(ItemSlot item)
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

    
    public static void RemoveObjectFromInventory(ItemSlot item)
    {
        if (item.amount <0)
            item.amount *= -1;
        

        
        if (item == null || item.amount <= 0)
            return;

        // if item is not stackable
        if (item.resource.maxStackSize == 1)
        {
            for (int x = 0; x < item.amount; x++)
            {
                for (int i = inventoryList.Length-1; i >= 0 ; i--)
                {
                    if (inventoryList[i] != null && inventoryList[i].resource.resourceEnum == item.resource.resourceEnum)
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
        for (int i = inventoryList.Length-1; i >= 0; i--)
        {
            if ( inventoryList[i] == null)
                continue;
            
            if (inventoryList[i].resource.resourceEnum == item.resource.resourceEnum)
            {

                if (item.amount - inventoryList[i].amount > 0)
                {
                    item.amount =  item.amount - inventoryList[i].amount ;
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

    public static bool CheckInventoryForItem(ItemSlot item)
    {

        checkForItem = false;
        for (int i = 0; i < inventoryList.Length; i++)
        {
            if (item == null && inventoryList[i] == null)
            {

                checkForItem = true;
                break;

            }
            else if (item.resource.resourceEnum == inventoryList[i].resource.resourceEnum)
            {
                checkForItem = true;
                break;
            }
        }

        return checkForItem;
    }

    public static bool CheckEnoughItemsForRecipe(RecipeSO recipe)
    {
        bool haveAllIngridients = true;


        foreach (var item in recipe.ResourcesCost)
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
            for (int i = 0; i < recipe.ResourcesCost.Length; i++)
            {
                RemoveObjectFromInventory(recipe.ResourcesCost[i]);
            }
            AddToInventory(recipe.Resource);
        }
        else
            Debug.Log("Cant Craft Not Enough resources");
        


        return haveAllIngridients;
    }

    static bool HaveEnoughOfItemFromInventory(ItemSlot item) {
        counter = 0;
        for (int i = 0; i < inventoryList.Length; i++)
        {
            if (inventoryList[i].resource.resourceEnum == item.resource.resourceEnum)
            {
                counter += inventoryList[i].amount;
                if (counter>= item.amount)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static int GetAmountOfItem(ItemSlot item) {
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
                if (item.resource.resourceEnum == inventoryList[i].resource.resourceEnum)
                {
                    counter += inventoryList[i].amount;
                }
            }
        }
        return counter;
    
    }
    public static int GetItemIndexInArray(ItemSlot item)
    {

        for (int i = 0; i < inventoryList.Length; i++)
        {
            if (item == null)
            {
                if (inventoryList[i] == null)
                    return i;
            }
            else
            if (inventoryList[i] != null && item.resource.resourceEnum == inventoryList[i].resource.resourceEnum)
            {
                return i;
            }
        }

        Debug.Log("You Dont Have This Item In Your Inventory");
        return 0;
    }


    public static void PrintInventory() {

        for (int i = 0; i < inventoryList.Length; i++) {
            if (inventoryList[i] == null)
            {
                Debug.Log("Inventory list in spot " + i + "is Null");
            }
            else
            Debug.Log("Inventory list in spot "+i+" with the amount : " + inventoryList[i].amount + " of type: " + inventoryList[i].resource.resourceEnum);
        }
    
    }
}
