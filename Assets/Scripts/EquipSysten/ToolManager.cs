
//public class ToolManager
//{

//    Inventory inventory;
//    PlayerStats playerStats;
//    static ToolManager _instance;
//    ItemSlot equipSlotCache;
//    GearItemSO gearCache;
//    ItemSlot[] equipSlots;
//    // 0 
//    // 1 
//    // 2 
//    // 3 
//    // 4 

//    public static ToolManager GetInstance
//    {
//        get
//        {
//            if (_instance == null)
//                _instance = new ToolManager();

//            return _instance;
//        }
//    }
//    ToolManager()
//    {
//        ResetEquip();
//    }
//    public void ResetEquip()
//    {
//        if (equipSlots != null)
//        {
//            for (int i = 0; i < equipSlots.Length; i++)
//            {
//                if (equipSlots[i] != null)
//                    UnEquipItem(i);

//            }
//        }


//        inventory = Inventory.GetInstance;
//        equipSlots = inventory.GetInventoryFromDictionary(2);
//        equipSlotCache = null;

//        playerStats = PlayerStats._instance;
//    }
//}
