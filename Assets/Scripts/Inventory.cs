using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Inventory
{
    static List<ItemSlot> inventoryList = new List<ItemSlot>();
    static List<ItemSlot> itemFoundList = new List<ItemSlot>();
    const int maxCapacityOfItems = 25;
    static bool isItemInList;
    static int remainer;

    //private static Inventory _instance;
    //public static Inventory _GetInstance {
    //    get {
    //        if (_instance == null) {
    //            _instance = new Inventory();
    //        }
    //        return _instance;
    //    }
    //}
    //Inventory()
    //{
    //    if (inventoryList == null)
    //    {
    //        inventoryList ;
    //        itemFoundList ;
    //    }
    //}

    public static List<ItemSlot> GetInventory { get => inventoryList; }

    public static bool CheckIfENnughSpaceInSlot(ItemSlot item) {
        if (item == null)
            return false;
        remainer = 0;
        isItemInList = false;

        // check if the list is full and if there's items of the same kind that can be stocked
        if (inventoryList.Count >= maxCapacityOfItems) {
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
        return isItemInList;
    }

    public static void AddToInventory(ItemSlot item)
    {
        if (item == null)
            return;



     

        // if i dont have it in the inventory
        if (!CheckIfENnughSpaceInSlot(item))
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



            if (inventoryList[i].resource.resourceEnum != item.resource.resourceEnum)
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
    public static void RemoveObjectFromInventory(ItemSlot item)
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
            if (inventoryList[i].resource.resourceEnum == item.resource.resourceEnum) // found the item in the inventory
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
            itemFoundList.OrderByDescending(itemForSorting => itemForSorting.amount);


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


    public static void PrintInventory() {

        for (int i = 0; i < inventoryList.Count; i++) {
            Debug.Log("Inventory list in spot "+i+" with the amount : " + inventoryList[i].amount + " of type: " + inventoryList[i].resource.resourceEnum.ToString());
        }
    
    }
}
