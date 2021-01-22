
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.AI;
using UnityEngine.UI;

public class EquipManager
{
    Inventory inventory;
    EffectHandler effectHandler;
    static EquipManager _instance;
    ItemSlot equipSlotCache;
    GearItemSO gearItemCache;
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
        gearItemCache = null;
        effectHandler = EffectHandler._instance;
        inventory = Inventory.GetInstance;
    }



    public bool CheckEquip(int buttonID, ItemSlot _equipedItem)
    {
        if (_equipedItem == null || _equipedItem.item == null)
            return false;


        if (!(_equipedItem.item is GearItemSO) || buttonID < 0 || buttonID >= equipSlots.Length)
            return false;

        EquipItem(buttonID, _equipedItem);
        return true;
    }


    public void EquipItem(int buttonID, ItemSlot item)
    {

        equipSlotCache = item;// might need to do new itemSlot()
        if (equipSlots[buttonID] != null)
        {
            inventory.RemoveItemFromInventory(0, item);
            UnEquipItem(buttonID);
        }


        equipSlots[buttonID] = equipSlotCache;


        
        gearItemCache= (equipSlots[buttonID].item as GearItemSO);

           // effectHandler.BeginAllEffects(gearItemCache.effectDatas , EffectHandler.GetStatControllers(gearItemCache.effectDatas));

        // apply ui
        InventoryUIManager._instance.UpdateInventoryToUI();


        equipSlotCache = null;
    }
    public void UnEquipItem(int buttonID)
    {
        if (equipSlots[buttonID] == null)
            return;


        inventory.AddToInventory(0, equipSlots[buttonID]);


        // disable effect
        gearItemCache = (equipSlots[buttonID].item as GearItemSO);

        //    effectHandler.StopAllEffects(EffectHandler.GetStatControllers(gearItemCache.effectDatas)); 


        equipSlots[buttonID] = null;
        InventoryUIManager._instance.UpdateInventoryToUI();
    }
    public void LowerDurabilityOfEquipItem(EquipType equipType, int amount)
    {

        if ( amount == 0)
            return;


        equipSlotCache = equipSlots[GetEquipSlot(equipType)];

        if (equipSlotCache == null)
            return;

        if (amount < 0)
            amount *= -1;


        if (equipSlotCache.durability - amount <=0)
            UnEquipItem(GetEquipSlot(equipType));


        equipSlotCache.durability -= amount;

    }

    public int? GetEquipDurability(EquipType equipType) {
    
        equipSlotCache = equipSlots[GetEquipSlot(equipType)];

        if (equipSlotCache == null)
            return null;

        return equipSlotCache.durability;

    }

    private int GetEquipSlot(EquipType equipType)
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
  
}
