

using NUnit.Framework;
using TMPro.EditorUtilities;
using UnityEditor;

public class EquipManager
{
    Inventory inventory;
    PlayerStats playerStats;
    static EquipManager _instance;
    ItemSlot equipSlotCache;
    GearItemSO gearCache;
    ToolItemSO toolCache;
    ItemSlot[] gearSlots;
    // 0 helmet
    // 1 Chest
    // 2 Legging
    // 3 Gloves
    // 4 Shoes

    ItemSlot[] toolSlots;
    /// <summary>
    /// 0 Axe 
    /// 1 Pickaxe
    /// 2 Hoe 
    /// 3 Shovel
    /// 4 Hammer
    /// </summary>

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
        ResetEquip();
    }
    public void ResetEquip()
    {
        if (gearSlots != null)
        {
            for (int i = 0; i < gearSlots.Length; i++)
            {
                if (gearSlots[i] != null)
                    UnEquipItem(i);

            }
        }
        if (toolSlots != null)
        {
            for (int i = 0; i < toolSlots.Length; i++)
            {
                toolSlots[i] = null;

            }
        }


        inventory = Inventory.GetInstance;
        gearSlots = inventory.GetInventoryFromDictionary(2);
        toolSlots = inventory.GetInventoryFromDictionary(3);
        equipSlotCache = null;
        gearCache = null;
        toolCache = null;
        playerStats = PlayerStats._instance;
    }
    public void ReEquipStats()
    {

        if (gearSlots == null && toolSlots == null)
            return;

        if (gearSlots != null)
        {
            for (int i = 0; i < gearSlots.Length; i++)
            {
                if (gearSlots[i] == null)
                    continue;

                ApplyStats((gearSlots[i].item as GearItemSO));
            }
        }
        if (toolSlots != null)
        {
            if (gearSlots != null)
            {
                for (int i = 0; i < toolSlots.Length; i++)
                {
                    if (toolSlots[i] == null)
                        continue;

                    ApplyStats((toolSlots[i].item as ToolItemSO));
                }
            }
        }


    }
    public bool CheckEquip(ItemSlot item, int firstButtonID, int chestID, int? secondButtonID = null, int secondChestID = 2)
    {

        if (!(item.item is GearItemSO || item.item is ToolItemSO) && item != null)
            return false;

             equipSlotCache = null;

        if (item.item is GearItemSO)
        {

            if (chestID == 2 && secondChestID == 2)
            {
                inventory.AddToInventory(0, gearSlots[firstButtonID]);
                UnEquipItem(firstButtonID);

                return true;
            }
            else if (chestID != 2 && secondChestID != 2)
                return false;



            if (secondButtonID == null)
            {
                equipSlotCache = inventory.GetItemFromInventoryButton(chestID, firstButtonID);
                if (equipSlotCache == null || equipSlotCache.item == null)
                    return false;


                if (!(equipSlotCache.item is GearItemSO) || firstButtonID < 0)
                    return false;

                EquipItem(chestID, firstButtonID);
                
                return true;
            }
            return SwapItemsInEquip(firstButtonID, chestID, secondButtonID.GetValueOrDefault(), secondChestID);
        }




        // is tool


        if ((chestID == 3 && secondChestID == 2))
        {
            if (toolSlots[firstButtonID] != null) 
            {
                SetActiveStateTool(firstButtonID, !GetToolActive((toolSlots[firstButtonID].item as ToolItemSO).GetToolType));
            }
        }
        else if (chestID != 3 && secondChestID != 2)
            return false;



        if (secondButtonID == null)
        {
            equipSlotCache = inventory.GetItemFromInventoryButton(chestID, firstButtonID);
            if (equipSlotCache == null || equipSlotCache.item == null)
                return false;


            if (!(equipSlotCache.item is ToolItemSO) || firstButtonID < 0)
                return false;

        

            int index = GetToolSlotIndex((equipSlotCache.item as ToolItemSO).GetToolType);
            if (toolSlots[index] != null) {
                inventory.AddToInventory(0, toolSlots[index]);
            }
                Inventory.GetInstance.RemoveItemFromInventory(0, equipSlotCache);
                toolSlots[firstButtonID] = equipSlotCache;


                InventoryUIManager._instance.UpdatePlayerInventory();
            return true;
        }






        return SwapItemsInEquip(firstButtonID, chestID, secondButtonID.GetValueOrDefault(), secondChestID); 
    }
    private bool SwapItemsInEquip(int firstButtonID, int chestID, int secondButtonID, int secondChestID)
    {
        gearCache = null;

        if (chestID == 2)
        {
            gearCache = inventory.GetItemFromInventoryButton(secondChestID, secondButtonID).item as GearItemSO;

            if (gearCache == null || !CheckIndexToGear(firstButtonID, gearCache))
                return false;


            if (gearSlots[firstButtonID] != null)
                RemoveStats(gearSlots[firstButtonID].item as GearItemSO);


            ApplyStats(gearCache);
            return true;
        }
        else if (secondChestID == 2)
        {
            gearCache = inventory.GetItemFromInventoryButton(chestID, firstButtonID).item as GearItemSO;

            if (gearCache == null || !CheckIndexToGear(secondButtonID, gearCache))
                return false;


            if (gearSlots[secondButtonID] != null)
                RemoveStats(gearSlots[secondButtonID].item as GearItemSO);

            ApplyStats(gearCache);

            return true;

        }
        else if (chestID == 3)
        {
            toolCache = inventory.GetItemFromInventoryButton(secondChestID, secondButtonID).item as ToolItemSO;

            if (toolCache == null || !CheckIndexToTool(firstButtonID, toolCache))
                return false;

            return true;
        }
        else if (secondChestID == 3)
        {
            toolCache = inventory.GetItemFromInventoryButton(chestID, firstButtonID).item as ToolItemSO;

            if (toolCache == null || !CheckIndexToTool(secondButtonID, toolCache))
                return false;


            return true;
        }


        return false;
    }
    private void EquipItem(int chestID, int buttonID)
    {
      
        int index = GetGearSlotIndex((equipSlotCache.item as GearItemSO).GetEquipType);
        if (gearSlots[index] != null)
            UnEquipItem(index);
        else
            inventory.GetInventoryFromDictionary(chestID)[buttonID] = null;



        gearSlots[index] = equipSlotCache;



        ApplyStats((gearSlots[index].item as GearItemSO));


        InventoryUIManager._instance.UpdateInventoryToUI();


        equipSlotCache = null;
    }
    private void UnEquipItem(int buttonID)
    {
        if (gearSlots[buttonID] == null)
            return;


        RemoveStats((gearSlots[buttonID].item as GearItemSO));

        gearSlots[buttonID] = null;
        InventoryUIManager._instance.UpdateInventoryToUI();
    }
    public void LowerDurabilityOfEquipItem(GearType equipType, int amount)
    {

        if (amount == 0)
            return;


        equipSlotCache = gearSlots[GetGearSlotIndex(equipType)];

        if (equipSlotCache == null || equipSlotCache.durability == null)
            return;

        if (amount < 0)
            amount *= -1;


        if (equipSlotCache.durability - amount <= 0)
            UnEquipItem(GetGearSlotIndex(equipType));


        equipSlotCache.durability -= amount;

    }
    public int? GetGearDurability(GearType gearType)
    {
        equipSlotCache = null;


        equipSlotCache = gearSlots[GetGearSlotIndex(gearType)];

        if (equipSlotCache == null)
            return null;

        return equipSlotCache.durability;

    }
    public int? GetToolDurabilityByEnum(ToolType type) {
        if (toolSlots == null)
            return null;

    

        return GetToolDurabilityByIndex(GetToolSlotIndex(type));
    }
    public int GetToolDurabilityByIndex(int index) => (toolSlots[index].item as ToolItemSO).GetSetDurability;
    public void SetAmountOfToolDurability(ToolType type, int amount)
    {

        if (toolSlots[GetToolSlotIndex(type)] == null || amount == 0)
            return;

        toolCache = toolSlots[GetToolSlotIndex(type)].item as ToolItemSO;


        if (amount < 0)
            amount *= -1;

        if (toolCache.GetSetDurability - amount <= 0)
        {
            toolSlots[GetToolSlotIndex(type)] = null;
            return;
        }

        toolCache.GetSetDurability -= amount;

    }
    public float? GetGatheringSpeedFromTool(ToolType tool) {
        toolCache = null;

        if (toolSlots[GetToolSlotIndex(tool)] == null)
        {
            return null;
        }

        toolCache = toolSlots[GetToolSlotIndex(tool)].item as ToolItemSO;

        return  (toolCache.GetPrecentage * toolCache.GetTier)/100;


    }
    private int GetToolSlotIndex(ToolType toolType) {
        switch (toolType)
        {
            case ToolType.Axe:

                return 0;
              
            case ToolType.Pickaxe:
                return 1;
               
            case ToolType.Hoe:
                return 2;
            
            case ToolType.Shovel:
                return 3;
        
            case ToolType.Hammer:
                return 4;
          
        }
        return 0;
    }
    private int GetGearSlotIndex(GearType equipType)
    {
        switch (equipType)
        {
            case GearType.Helmet:
                return 0;

            case GearType.Chest:
                return 1;

            case GearType.Legging:
                return 2;

            case GearType.Gloves:
                return 3;

            case GearType.Shoes:
                return 4;
        }
        return 0;
    }
    private bool CheckIndexToGear(int index, GearItemSO gear)
    {

        if (index == GetGearSlotIndex(gear.GetEquipType))
            return true;

        return false;

    }
    private bool CheckIndexToTool(int index, ToolItemSO tool) {


        if (index == GetToolSlotIndex(tool.GetToolType))
            return true;

        return false;
    }
    void RemoveStats(ItemSO equip)
    {

        if (!(equip is GearItemSO || equip is ToolItemSO))
        {
            return;
        }

        gearCache = null;
        if (equip is GearItemSO)
        {
            gearCache = equip as GearItemSO;
            if (gearCache.equipstats.Length <= 0)
                return;

            for (int i = 0; i < gearCache.equipstats.Length; i++)
            {
                if (gearCache.equipstats[i].isPercentage)
                {
                    playerStats.AddToStatValue(gearCache.equipstats[i].statType, -playerStats.GetStatValue(gearCache.equipstats[i].statType) * gearCache.equipstats[i].amount / 100);
                    continue;
                }


                playerStats.AddToStatValue(gearCache.equipstats[i].statType, -gearCache.equipstats[i].amount);
            }
        }



    }
    void ApplyStats(ItemSO equip)
    {
        if (equip == null)
            return;

        if (!(equip is GearItemSO || equip is ToolItemSO))
            return;


        
        if (equip is GearItemSO)
        {
            gearCache = null;
            gearCache = equip as GearItemSO;

            if (gearCache.equipstats.Length <= 0)
                return;

            for (int i = 0; i < gearCache.equipstats.Length; i++)
            {
                if (gearCache.equipstats[i].isPercentage)
                {
                    playerStats.AddToStatValue(gearCache.equipstats[i].statType, playerStats.GetStatValue(gearCache.equipstats[i].statType) * gearCache.equipstats[i].amount / 100);
                    continue;
                }

                playerStats.AddToStatValue(gearCache.equipstats[i].statType, gearCache.equipstats[i].amount);
            }
        }
    }
    public ToolType GetToolTypeByIndex(int buttonID)
    {
        /// <summary>
        /// 0 Axe 
        /// 1 Pickaxe
        /// 2 Hoe 
        /// 3 Shovel
        /// 4 Hammer
        /// </summary>

        if (buttonID == 0)
            return ToolType.Axe;

        if (buttonID == 1)
            return ToolType.Pickaxe;


        if (buttonID == 2)
            return ToolType.Hoe;

        if (buttonID == 3)
            return ToolType.Shovel;


        return ToolType.Hammer;

    }
    public bool GetToolActive(ToolType type)
    {
        toolCache = null;
        toolCache = toolSlots[GetToolSlotIndex(type)].item as ToolItemSO;
        if (toolCache == null)
                    return false;
        
        return toolCache.SetGetIsActive;
    }

    public void SetActiveStateTool(int buttonID , bool state) {

        if (buttonID < 0 || buttonID > toolSlots.Length - 1 || toolSlots[buttonID] == null)
            return;

        (toolSlots[buttonID].item as ToolItemSO).SetGetIsActive = state;
    }
}
