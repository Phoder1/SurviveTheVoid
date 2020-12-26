using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory
{
    static List<ResourceSlot> inventoryList;
    static List<ResourceSlot> itemFoundList;
    const int maxCapacityOfItems = 25;
    static bool isItemInList;
    static int remainer;
    Inventory()
    {
        if (inventoryList == null)
        {
            inventoryList = new List<ResourceSlot>();
            itemFoundList = new List<ResourceSlot>();
        }
    }

    public static List<ResourceSlot> GetInventory { get => inventoryList; }

    public static void AddToInventory(ResourceSlot item)
    {
        if (item == null)
            return;

       remainer = 0;
       isItemInList = false;

        // check if the list is full and if there's items of the same kind that can be stocked
        if (inventoryList.Count >= maxCapacityOfItems)
        {
            for (int i = 0; i < inventoryList.Count; i++)
            {
                if (item.resource.id == inventoryList[i].resource.id)
                {
                    remainer += inventoryList[i].resource.maxStackSize - inventoryList[i].amount;
                    isItemInList = true;
                }
            }

            if (item.amount < remainer)
            {
                Debug.Log("Inventory is full!");
                return;
            }
        }

     

        // if i dont have it in the inventory
        if (!isItemInList)
        {
            inventoryList.Add(item);
            return;
        }


        // if its unstackable
        if (item.resource.maxStackSize == 1) {
            inventoryList.Add(item);
            return;
        }

            remainer = 0;

        // add to all existing slots the items amount
        for (int i = 0; i < inventoryList.Count; i++)
        {



            if (inventoryList[i].resource.id != item.resource.id)
                continue;


                if (inventoryList[i].amount == inventoryList[i].resource.maxStackSize)
                    continue;


                if (inventoryList[i].amount + item.amount > inventoryList[i].resource.maxStackSize)
                {
                    remainer = inventoryList[i].amount + item.amount - inventoryList[i].resource.maxStackSize;



                    inventoryList[i].amount += (item.amount - remainer);

                    item.amount = remainer;

                }
                else
                    inventoryList[i].amount += item.amount;

            
            if (remainer > 0)
            {
                inventoryList.Add(item);

            }


        }

        // after adding to all the previous slots if there is more then add another slot to inventory
        
    }
    public static void RemoveObjectFromInventory(ResourceSlot item)
    {
        // if item is not stackable
        if (item.resource.maxStackSize == 1)
        {
            inventoryList.Remove(item);
            return;
        }



        // if item is istackable

        itemFoundList.Clear();
        int totalAmount = 0; 

        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].resource.id == item.resource.id) // found the item in the inventory
            {
                itemFoundList.Add(inventoryList[i]);
                totalAmount += inventoryList[i].amount;
            }
        }

        // check if the plyaer has enough amount in the inventory
        if (totalAmount < item.amount)
        {
            Debug.Log("The player dont have enough amount for this item to be removed\n remove failed");
            return;
        }


        if (itemFoundList.Count == 1)
        {
            // there is only one item of this kind

            if (totalAmount > item.amount)
            {
                itemFoundList[0].amount -= item.amount;
            }
            else if (totalAmount == item.amount)
            {
                itemFoundList[0].amount = 0;
            }

        }
        else
        {
            // there is more of this item 
            itemFoundList.OrderByDescending(item => item.amount);


            remainer = item.amount;
            
            for (int i = 0; i < itemFoundList.Count; i++)
            {

                if (remainer <= 0)
                    break;
                

                if (itemFoundList[i].amount <= remainer)
                {
                    itemFoundList[i].amount = 0;
                    remainer = Mathf.Abs(remainer - itemFoundList[i].amount);
                    continue;
                }
                else {
                    itemFoundList[i].amount -= remainer;
                }




            }

        }



        // remove empty fields
        for (int  i = 0;  i < inventoryList.Count;  i++)
        {
            if (inventoryList[i].amount <= 0)
            {
                inventoryList.Remove(inventoryList[i]);
            }
        }

        
        Debug.Log("Finished Removing");

    }

    public static bool CheckInventoryForItem(ResourceSlot item) {
        if (item == null)
        {
            Debug.Log("Cant remove item because item is null");
            return false;
        }



        isItemInList = false;
        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (item.resource.id == inventoryList[i].resource.id)
            {
                isItemInList = true;
                break;
            }
        }

        return isItemInList;
    }


    public static void PrintInventory() {

        foreach (var item in inventoryList)
        {
            Debug.Log("Inventory list in spot "+i+" with the amount : " + item.amount);
        }
    
    }
}
