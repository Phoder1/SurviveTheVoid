
using NUnit.Framework.Internal.Commands;
using System;
using System.Collections.Generic;

public class EquipManager
{
    Inventory inventory;
    PlayerStats playerStats;
    static EquipManager _instance;
    ItemSlot equipSlotCache, toEquip;
 
    ItemSlot[] equipSlots;
    // 0 helmet
    // 1 Chest
    // 2 Legging
    // 3 Gloves
    // 4 Shoes


    public static EquipManager GetInstance
    {
        get
        {

            if (_instance == null)
                _instance = new EquipManager();

            return _instance;
        }
    }
    EquipManager()
    {
        equipSlots = inventory.GetInventoryFromDictionary(2); 
        equipSlotCache = null;
    
        playerStats = PlayerStats._instance;
        inventory = Inventory.GetInstance;
    }



    public bool CheckEquip(int firstButtonID,int chestID,int? secondButtonID = null,int secondChestID = 2)
    {
        if (chestID == 2  && secondChestID == 2) { 
          UnEquipItem(firstButtonID);
            return true;
        }else if (chestID != 2 && secondChestID != 2)
            return false; 
        
     

        if (secondButtonID == null)
        {
               toEquip = inventory.GetItemFromInventoryButton(chestID, firstButtonID);
            if (toEquip == null || toEquip.item == null)
                return false;


            if (!(toEquip.item is GearItemSO) || firstButtonID < 0 || firstButtonID >= equipSlots.Length)
                return false;

            EquipItem(firstButtonID, toEquip);
            return true;
        }

   



        SwapItemsInEquip(firstButtonID, chestID, secondButtonID.GetValueOrDefault(), secondChestID);



        return true;


    }

    private void SwapItemsInEquip(int firstButtonID, int chestID, int secondButtonID, int secondChestID)
    {

        if (chestID == 2)
        {
            RemoveStats(equipSlots[firstButtonID].item as GearItemSO);
            ApplyStats(inventory.GetItemFromInventoryButton(secondChestID, secondButtonID).item as GearItemSO);
        }
        else if (secondChestID == 2)
        {

            RemoveStats(inventory.GetItemFromInventoryButton(chestID, firstButtonID).item as GearItemSO);
            ApplyStats(equipSlots[secondChestID].item as GearItemSO);
        }

        inventory.ChangeBetweenItems(firstButtonID, chestID, secondButtonID, secondChestID);

    }

    public void EquipItem(int index, ItemSlot item)
    {

        equipSlotCache = item;// might need to do new itemSlot()
        if (equipSlots[index] != null)
        {
            UnEquipItem(index);
        }


        equipSlots[index] = equipSlotCache;


        
      ApplyStats((equipSlots[index].item as GearItemSO));

       
        InventoryUIManager._instance.UpdateInventoryToUI();


        equipSlotCache = null;
    }

  
    public void UnEquipItem(int buttonID)
    {
        if (equipSlots[buttonID] == null)
            return;


     inventory.AddToInventory(0, equipSlots[buttonID]);

        RemoveStats((equipSlots[buttonID].item as GearItemSO));

        equipSlots[buttonID] = null;
        InventoryUIManager._instance.UpdateInventoryToUI();
    }
    public void LowerDurabilityOfEquipItem(EquipType equipType, int amount)
    {

        if ( amount == 0)
            return;


        equipSlotCache = equipSlots[GetEquipSlotIndex(equipType)];

        if (equipSlotCache == null || equipSlotCache.durability == null)
            return;

        if (amount < 0)
            amount *= -1;
      

        if (equipSlotCache.durability - amount <=0)
            UnEquipItem(GetEquipSlotIndex(equipType));


        equipSlotCache.durability -= amount;

    }

    public int? GetEquipDurability(EquipType equipType) {
    
        equipSlotCache = equipSlots[GetEquipSlotIndex(equipType)];

        if (equipSlotCache == null)
            return null;

        return equipSlotCache.durability;

    }
    private int GetEquipSlotIndex(EquipType equipType)
    {
        switch (equipType)
        {
            case EquipType.Helmet:
                return 0;

            case EquipType.Chest:
                return 1;

            case EquipType.Legging:
                return 2;

            case EquipType.Gloves:
                return 3;

            case EquipType.Shoes:
                return 4;
        }
        return 0;
    }
   void RemoveStats(GearItemSO equip)
    {
        if (equip.equipstats.Length <= 0)
            return;

        for (int i = 0; i < equip.equipstats.Length; i++)
        {
            if (equip.equipstats[i].isPercentage)
            {
                playerStats.AddToStatValue(equip.equipstats[i].statType, -playerStats.GetStatValue(equip.equipstats[i].statType) * equip.equipstats[i].amount / 100);
                continue;
             }


            playerStats.AddToStatValue(equip.equipstats[i].statType, -equip.equipstats[i].amount);
        }



    }
    void ApplyStats(GearItemSO equip) {


        if (equip.equipstats.Length <= 0)
            return;
        
        for (int i = 0; i < equip.equipstats.Length; i++)
        {
            if (equip.equipstats[i].isPercentage) {
                playerStats.AddToStatValue(equip.equipstats[i].statType, playerStats.GetStatValue(equip.equipstats[i].statType) * equip.equipstats[i].amount / 100);
                continue;
            }
            
            playerStats.AddToStatValue(equip.equipstats[i].statType, equip.equipstats[i].amount);
        }
        

    }
}
