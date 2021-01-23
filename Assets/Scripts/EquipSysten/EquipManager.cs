public class EquipManager
{
    Inventory inventory;
    PlayerStats playerStats;
    static EquipManager _instance;
    ItemSlot equipSlotCache;

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
        ResetEquip();
    }
    public void ResetEquip() {
        if (equipSlots!= null)
        {
            for (int i = 0; i < equipSlots.Length; i++)
            {
                if (equipSlots[i] != null)
                    UnEquipItem(i);
                
            }
        }


    inventory = Inventory.GetInstance;
        equipSlots = inventory.GetInventoryFromDictionary(2);
        equipSlotCache = null;

        playerStats = PlayerStats._instance;
    }
    public void ReEquipStats() {

        if (equipSlots == null)
            return;
        
        for (int i = 0; i < equipSlots.Length; i++)
        {
            if (equipSlots[i] == null)
                continue;

            ApplyStats((equipSlots[i].item as GearItemSO));
        }  
    }
    public bool CheckEquip(int firstButtonID, int chestID, int? secondButtonID = null, int secondChestID = 2)
    {
        if (chestID == 2 && secondChestID == 2)
        {
            UnEquipItem(firstButtonID);
            return true;
        }
        else if (chestID != 2 && secondChestID != 2)
            return false;

        equipSlotCache = null;

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
    private bool SwapItemsInEquip(int firstButtonID, int chestID, int secondButtonID, int secondChestID)
    {


        if (chestID == 2)
        {

            if (equipSlots[firstButtonID]!= null && equipSlots[firstButtonID] != null)
            { 
                if (!CheckIndexToEquip(firstButtonID, equipSlots[firstButtonID].item as GearItemSO))  return false;
              
                    RemoveStats(equipSlots[firstButtonID].item as GearItemSO);
            }


            ApplyStats(inventory.GetItemFromInventoryButton(secondChestID, secondButtonID).item as GearItemSO);
        }
        else if (secondChestID == 2)
        {




            RemoveStats(inventory.GetItemFromInventoryButton(chestID, firstButtonID).item as GearItemSO);
            if (equipSlots[secondChestID] != null && equipSlots[secondButtonID] != null) 
            {
                if (!CheckIndexToEquip(secondButtonID, equipSlots[secondButtonID].item as GearItemSO)) return false;

                ApplyStats(equipSlots[secondChestID].item as GearItemSO);
            }
        }


        return true;
    }
    private void EquipItem(int chestID, int buttonID)
    {




        int index = GetEquipSlotIndex((equipSlotCache.item as GearItemSO).GetEquipType);
        if (equipSlots[index] != null)
            UnEquipItem(index);
        else
            inventory.GetInventoryFromDictionary(chestID)[buttonID] = null;



        equipSlots[index] = equipSlotCache;



        ApplyStats((equipSlots[index].item as GearItemSO));


        InventoryUIManager._instance.UpdateInventoryToUI();


        equipSlotCache = null;
    }
    private void UnEquipItem(int buttonID)
    {
        if (equipSlots[buttonID] == null)
            return;


        //inventory.AddToInventory(0, equipSlots[buttonID]);

        RemoveStats((equipSlots[buttonID].item as GearItemSO));

        equipSlots[buttonID] = null;
        InventoryUIManager._instance.UpdateInventoryToUI();
    }
    public void LowerDurabilityOfEquipItem(EquipType equipType, int amount)
    {

        if (amount == 0)
            return;


        equipSlotCache = equipSlots[GetEquipSlotIndex(equipType)];

        if (equipSlotCache == null || equipSlotCache.durability == null)
            return;

        if (amount < 0)
            amount *= -1;


        if (equipSlotCache.durability - amount <= 0)
            UnEquipItem(GetEquipSlotIndex(equipType));


        equipSlotCache.durability -= amount;

    }
    public int? GetEquipDurability(EquipType equipType)
    {

        equipSlotCache = equipSlots[GetEquipSlotIndex(equipType)];

        if (equipSlotCache == null)
            return null;

        return equipSlotCache.durability;

    }
    private bool CheckIndexToEquip(int index, GearItemSO gear)
    {

        if (index == GetEquipSlotIndex(gear.GetEquipType))
            return true;

        return false;

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
    void ApplyStats(GearItemSO equip)
    {
        if (equip== null)
           return;
        

        if (equip.equipstats.Length <= 0)
            return;

        for (int i = 0; i < equip.equipstats.Length; i++)
        {
            if (equip.equipstats[i].isPercentage)
            {
                playerStats.AddToStatValue(equip.equipstats[i].statType, playerStats.GetStatValue(equip.equipstats[i].statType) * equip.equipstats[i].amount / 100);
                continue;
            }

            playerStats.AddToStatValue(equip.equipstats[i].statType, equip.equipstats[i].amount);
        }


    }
}
