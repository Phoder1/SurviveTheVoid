using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    static List<ResourceSO> inventoryList;
    const int maxCapacityPerSlot = 40;
    const int maxCapacityOfItems = 25;


    Inventory()
    {
        if (inventoryList == null)
        {
            inventoryList = new List<ResourceSO>();
        }
    }

    public static List<ResourceSO> GetInventory {get => inventoryList;}

    public static void AddToInventory(ResourceSO item)
    {
    
    }
    public static void RemoveObjectFromInventtory(ResourceSO item, int amount) { }

    public static bool HasObjectInInventory(ResourceSO item, int amount) {
        return true;
    }
}
