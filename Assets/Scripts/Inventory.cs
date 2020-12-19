using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    static List<ResourcesStruct> inventoryList;
    const int maxCapacityPerSlot = 40;
    const int maxCapacityOfItems = 25;


    Inventory()
    {
        if (inventoryList == null)
        {
            inventoryList = new List<ResourcesStruct>();
        }
    }

    public static List<ResourcesStruct> GetInventory {get => inventoryList;}

    public static void AddToInventory(ResourceStruct item)
    {
    
    }
    public static void RemoveObjectFromInventtory(ResourceStruct item, int amount) { }

    public static bool HasObjectInInventory(ResourceStruct item, int amount) {
        return true;
    }
}
