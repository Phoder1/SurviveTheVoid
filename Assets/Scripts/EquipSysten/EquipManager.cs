using UnityEngine.UIElements.Experimental;

public class EquipManager
{
    Inventory inventory;
    PlayerStats playerStats;
    PlayerStats GetPlayerStats {
        get {
            if (!playerStats) {
                playerStats = PlayerStats._instance;
            }
            return playerStats;
        }
    }
    static EquipManager _instance;
    ItemSlot equipSlotCache;
    GearItemSO gearCache; // itemSO
    ToolItemSO toolCache; // itemSO
    ItemSlot[] gearSlots;

    /// <summary>
    /// 0 helmet
    /// 1 Chest
    /// 2 Legging
    /// 3 Gloves
    /// 4 Shoes
    /// </summary>


    bool[] toolsActive;
    ItemSlot[] toolSlots;
    /// <summary>
    /// 0 Axe 
    /// 1 Pickaxe
    /// 2 Hoe 
    /// 3 Shovel
    /// 4 Hammer
    /// </summary>

    public static EquipManager GetInstance {
        get {
            if (_instance == null)
                _instance = new EquipManager();
            return _instance;
        }
    }
    EquipManager() {
        ResetEquip();
    }

    // do unequip function to the outside will get itemslot and REMOVE THE FUNCTION



    #region Equips
    public void ResetEquip() {
        if (gearSlots != null && toolSlots != null) {
            for (int i = 0; i < gearSlots.Length; i++) {
                if (gearSlots[i] != null)
                    UnEquipItem(gearSlots[i]);
               
                toolSlots[i] = null;
            }
        }


        inventory = Inventory.GetInstance;
        gearSlots = inventory.GetInventoryFromDictionary(2);
        toolSlots = inventory.GetInventoryFromDictionary(3);


        if (toolsActive== null)
            toolsActive = new bool[toolSlots.Length];

        for (int i = 0; i < toolsActive.Length; i++)
            toolsActive[i] = true;


        equipSlotCache = null;
        gearCache = null;
        toolCache = null;
    }
    public void ReEquipStats() {

        if (gearSlots == null || toolSlots == null)
            return;


        for (int i = 0; i < gearSlots.Length; i++) {
            if (gearSlots[i] != null)
                ApplyStats(gearSlots[i].item);

            if (toolSlots[i] != null)
                ApplyStats((toolSlots[i].item));
        }


    }

    /// <summary>
    /// 
    /// 
    /// 
    /// 
    /// </summary>

    public bool CheckEquip(ItemSlot firstItem, SlotChestType targetChestType, int targetButtonID) {
        if (firstItem == null)
            return true;
        //if (!(firstItem.item is GearItemSO || firstItem.item is ToolItemSO) && !(secondItem.item is GearItemSO || secondItem.item is ToolItemSO))
        //    return false;

        switch (firstItem.item) {
            case GearItemSO gear:
                return CheckMatchingInventory(firstItem, targetChestType) && CheckIndexToGear(targetButtonID, gear);
            case ToolItemSO tool:
                return CheckMatchingInventory(firstItem, targetChestType) && CheckIndexToTool(targetButtonID, tool);
            default:
                return false;
        }
    }
    private bool CheckMatchingInventory(ItemSlot item, SlotChestType chestType) {
        switch (item.item) {
            case GearItemSO gear:
                return chestType == SlotChestType.Gear;
            case ToolItemSO tool:
                return chestType == SlotChestType.Tools;
            default:
                return true;

        }
    }
    public bool EquipItem(ItemSlot item) {
        if (item == null || !(item.item is GearItemSO || item.item is ToolItemSO))
            return false;


        if (item.item is GearItemSO) {
            gearCache = null;
            gearCache = (item.item as GearItemSO);

            if (gearSlots[GetGearSlotIndex(gearCache.GetEquipType)] != null)
                RemoveStats(gearSlots[GetGearSlotIndex(gearCache.GetEquipType)].item);

            ApplyStats(gearCache);
            return true;


        }
        else if (item.item is ToolItemSO) {
            return true;
        }


        return false;
        {
            //int index = GetGearSlotIndex((equipSlotCache.item as GearItemSO).GetEquipType);
            //if (gearSlots[index] != null)
            //    UnEquipItem(index);
            //else
            //    inventory.GetInventoryFromDictionary(chestID)[buttonID] = null;



            //gearSlots[index] = equipSlotCache;



            //ApplyStats((gearSlots[index].item as GearItemSO));


            //InventoryUIManager._instance.UpdatePlayerInventory();


            //equipSlotCache = null;
        }
    }
    public bool UnEquipItem(ItemSlot item) {
        if (item == null || !(item.item is GearItemSO || item.item is ToolItemSO))
            return false;


        if (item.item is GearItemSO) {
            gearCache = null;
            gearCache = (item.item as GearItemSO);

            if (gearSlots[GetGearSlotIndex(gearCache.GetEquipType)] == null)
                return false;


            RemoveStats(gearSlots[GetGearSlotIndex(gearCache.GetEquipType)].item);
            return true;
        }
        else if (item.item is ToolItemSO) {
            if (toolSlots[GetToolSlotIndex((item.item as ToolItemSO).GetToolType)] == null)
                return false;

            return true;
        }


        return false;

    }
    #endregion
    #region Durability Section
    public int? GetGearDurability(GearType gearType) {
        equipSlotCache = null;


        equipSlotCache = gearSlots[GetGearSlotIndex(gearType)];

        if (equipSlotCache == null)
            return null;

        return equipSlotCache.GetSetDurability;

    }
    public int? GetToolDurability(ToolType type) {
        if (toolSlots == null)
            return null;



        return GetToolDurabilityByIndex(GetToolSlotIndex(type));
    }
    public int GetToolDurabilityByIndex(int index) {
        if (index < 0 || index >= toolSlots.Length || toolSlots[index] == null)
            return 0;


        return toolSlots[index].GetSetDurability;

    }
    public void LowerAmountOfToolDurability(ToolType type, int amount) {
        equipSlotCache = null;

        if (toolSlots[GetToolSlotIndex(type)] == null || amount == 0)
            return;

        equipSlotCache = toolSlots[GetToolSlotIndex(type)];

        if (equipSlotCache == null)
            return;


        if (amount < 0)
            amount *= -1;

        if (equipSlotCache.GetSetDurability - amount <= 0) {
            toolSlots[GetToolSlotIndex(type)] = null;
            InventoryUIManager._instance.UpdateToolsToUI();
            return;
        }

        equipSlotCache.GetSetDurability -= amount;
        InventoryUIManager._instance.UpdateToolsToUI();
    }
    public void SetAmountOfGearDurability(GearType equipType, int amount) {

        if (amount == 0)
            return;


        equipSlotCache = gearSlots[GetGearSlotIndex(equipType)];

        if (equipSlotCache == null)
            return;

        if (amount < 0)
            amount *= -1;


        if (equipSlotCache.GetSetDurability - amount <= 0) {
            RemoveStats(equipSlotCache.item);
            gearSlots[GetGearSlotIndex(equipType)] = null;
            equipSlotCache = null;
            return;
        }




        equipSlotCache.GetSetDurability -= amount;

    }
    #endregion
    #region Tiers
    private int GetTierOfGearByIndex(int index) {
        if ((index < 0) || index >= gearSlots.Length || gearSlots[index] == null)
            return 0;
        return (gearSlots[index].item as GearItemSO).GetGearTier;

    }
    public int GetTierByEnum(GearType type) {
        return GetTierOfGearByIndex(GetGearSlotIndex(type));
    }
    public int GetTierByEnum(ToolType type) {
        return GetTierOfToolByIndex(GetToolSlotIndex(type));
    }
    private int GetTierOfToolByIndex(int index) {

        if ((index < 0) || index >= toolSlots.Length || toolSlots[index] == null)
            return 0;

        return (toolSlots[index].item as ToolItemSO).GetToolTier;
    }

    #endregion
    #region Index of tool / gear 
    public ToolType GetToolTypeByIndex(int buttonID) {
        /// <summary>
        /// 0 Axe 
        /// 1 Pickaxe
        /// 2 Hoe 
        /// 3 Shovel
        /// 4 Hammer
        /// </summary>
        switch (buttonID) {
            case 0:
                return ToolType.Axe;
            case 1:
                return ToolType.Pickaxe;
            case 2:
                return ToolType.Hoe;
            case 3:
                return ToolType.Scissor;
            case 4:
                return ToolType.Hammer;
            default:
                throw new System.NotImplementedException();
        }
    }
    public int GetIndexByToolType(ToolType toolType) {
        /// <summary>
        /// 0 Axe 
        /// 1 Pickaxe
        /// 2 Hoe 
        /// 3 Shovel
        /// 4 Hammer
        /// </summary>

        switch (toolType) {
            case ToolType.Axe:
                return 0;
            case ToolType.Pickaxe:
                return 1;
            case ToolType.Hoe:
                return 2;
            case ToolType.Scissor:
                return 3;
            case ToolType.Hammer:
                return 4;
            default:
                throw new System.NotImplementedException();
        }

    }
    private int GetToolSlotIndex(ToolType toolType) {
        switch (toolType) {
            case ToolType.Axe:

                return 0;

            case ToolType.Pickaxe:
                return 1;

            case ToolType.Hoe:
                return 2;

            case ToolType.Scissor:
                return 3;

            case ToolType.Hammer:
                return 4;

        }
        return 0;
    }
    public int GetGearSlotIndex(GearType equipType) {
        /// <summary>
        /// 0 Helmet 
        /// 1 Chest
        /// 2 Legging 
        /// 3 Gloves
        /// 4 Shoes
        /// </summary>
        switch (equipType) {
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
    #endregion
    #region Index Check
    private bool CheckIndexToGear(int index, GearItemSO gear) {

        if (index == GetGearSlotIndex(gear.GetEquipType))
            return true;

        return false;

    }
    private bool CheckIndexToTool(int index, ToolItemSO tool) {


        if (index == GetToolSlotIndex(tool.GetToolType))
            return true;

        return false;
    }
    #endregion
    #region GearStats
    void RemoveStats(ItemSO equip) {

        if (!(equip is GearItemSO || equip is ToolItemSO))
            return;


        gearCache = null;
        if (equip is GearItemSO) {
            gearCache = equip as GearItemSO;
            if (gearCache.equipstats.Length <= 0)
                return;

            for (int i = 0; i < gearCache.equipstats.Length; i++) {
                if (gearCache.equipstats[i].isPercentage) {
                    GetPlayerStats.AddToStatValue(gearCache.equipstats[i].statType, -GetPlayerStats.GetStatValue(gearCache.equipstats[i].statType) * gearCache.equipstats[i].amount / 100);
                    continue;
                }


                GetPlayerStats.AddToStatValue(gearCache.equipstats[i].statType, -gearCache.equipstats[i].amount);
            }
        }



    }
    void ApplyStats(ItemSO equip) {
        if (equip == null)
            return;

        if (!(equip is GearItemSO || equip is ToolItemSO))
            return;



        if (equip is GearItemSO) {
            gearCache = null;
            gearCache = equip as GearItemSO;

            if (gearCache.equipstats.Length <= 0)
                return;

            for (int i = 0; i < gearCache.equipstats.Length; i++) {
                if (gearCache.equipstats[i].isPercentage) {
                    GetPlayerStats.AddToStatValue(gearCache.equipstats[i].statType, GetPlayerStats.GetStatValue(gearCache.equipstats[i].statType) * gearCache.equipstats[i].amount / 100);
                    continue;
                }

                GetPlayerStats.AddToStatValue(gearCache.equipstats[i].statType, gearCache.equipstats[i].amount);
            }
        }
    }

    #endregion
    #region Tool's Activity
    public bool GetToolActive(ToolType type) => toolsActive[GetToolSlotIndex(type)];
    
    public void SetActiveStateTool(ToolType type, bool state) {
        if (toolsActive[GetToolSlotIndex(type)] != state)
               toolsActive[GetToolSlotIndex(type)] = state;
    }
    public float GetGatheringSpeedFromTool(ToolType tool) {
        toolCache = null;

        if (toolSlots[GetToolSlotIndex(tool)] == null) {
            return 1;
        }

        toolCache = toolSlots[GetToolSlotIndex(tool)].item as ToolItemSO;

        return 1 + (toolCache.GetSpeedPrecentage) / 100;
        // GetTier 

    }
    #endregion
}
