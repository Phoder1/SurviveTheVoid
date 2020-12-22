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

        // check if i have the item in the inventory
        if (!isItemInList)
        {
            for (int i = 0; i < inventoryList.Count; i++)
            {
                if (item.resource.id == inventoryList[i].resource.id)
                {
                    isItemInList = true;
                    break;
                }
            }
        }

        // if i dont have it in the inventory
        if (!isItemInList)
        {
            inventoryList.Add(item);
            return;
        }


        // if its stackable
        if (item.resource.maxStackSize == 1) {
            inventoryList.Add(item);
            return;
        }

            remainer = 0;

        // add to all existing slots the items amount
        foreach (var itemIsList in inventoryList)
        {
            if (itemIsList.resource.id == item.resource.id)
            {
                if (itemIsList.amount == itemIsList.resource.maxStackSize)
                    continue;


                if (itemIsList.amount + item.amount > itemIsList.resource.maxStackSize)
                {
                    remainer = itemIsList.amount + item.amount - itemIsList.resource.maxStackSize;

                    itemIsList.amount += (item.amount - remainer);

                    item.amount = remainer;

                }
                else
                    itemIsList.amount += item.amount;

            }
        }

        // after adding to all the previous slots if there is more then add another slot to inventory
        if (remainer > 0)
        {
            inventoryList.Add(item);
        }


    }
    public static void RemoveObjectFromInventory(ResourceSlot item)
    {

        if (item == null)
        {
            Debug.Log("Cant remove item because item is null");
            return;
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

        if (!isItemInList)
        {
            Debug.Log("No Item was found");
            return;
        }



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

                {

                // inventoryList.Remove(item); check this logic <-
                //for (int i = 0; i < inventoryList.Count; i++)
                //{
                //    if (inventoryList[i] == item)
                //    {
                //        inventoryList.Remove(inventoryList[i]);
                //        return;
                //    }
                //}
                }
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

                totalAmount = remainer;
                
                if (totalAmount >= itemFoundList[i].amount)
                {
                    itemFoundList[i].amount = 0;
                }
                else //totalAmount < itemFoundList[i].amount
                {
                    itemFoundList[i].amount -= totalAmount;
                }
                
                remainer = totalAmount - itemFoundList[i].amount;

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
}
