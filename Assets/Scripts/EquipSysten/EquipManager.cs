using System.Collections.Generic;
using System.Diagnostics;

public class EquipManager
{
    Dictionary<int, EquipAbst> equipDict;
    static EquipManager _instance;


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
        equipDict = new Dictionary<int, EquipAbst>();
        equipDict[0] = new HelmetEquip();
        equipDict[1] = new TopEquip();
        equipDict[2] = new BottomEquip();
        equipDict[3] = new GlovesEquip();
        equipDict[4] = new BootsEquip();
    }





    public bool EquipItem(int buttonID, ItemSlot item)
    {
        if (!equipDict.TryGetValue(buttonID, out var equip))
            return false;

        return equip.CheckedEquip(item);
    }
    public void UnEquipItem(int buttonID)
    {
        if (!equipDict.TryGetValue(buttonID, out var item))
            return;

        item.UnEquipItem();
    }
}


public abstract  class EquipAbst 
{
    EquipType type;

    ItemSlot equipedItem;
    public bool CheckEnoughDurability(int amountToRemove)
    {

        if (equipedItem == null)
            return false;

        if (equipedItem.durability - amountToRemove > 0)
            return true;


        UnEquipItem();
        return false;

    }

    public bool CheckedEquip(ItemSlot _equipedItem)
    {
        if (_equipedItem == null || _equipedItem.item == null|| _equipedItem.item.GetEquipType != type)
            return false;

        EquipItem(_equipedItem);
        return true;
    }

    void EquipItem(ItemSlot _equipedItem) {


        if (equipedItem == null)
        {
            SetEquipedItem = _equipedItem;
            return;
        }


        //Inventory.GetInstance.AddToInventory(0, GetEquipedItem);

        SetEquipedItem = _equipedItem;
        // apply effect
        // updateUI

    }

    public void UnEquipItem()
    {
        if (equipedItem == null)
            return;

       

        if (equipedItem.durability > 0)
            Inventory.GetInstance.AddToInventory(0, equipedItem);



        equipedItem = null;
        // Cancel Effect
        // updateUI


    }

    public int? GetItemDurability => equipedItem.durability;
    public EquipType SetEquipType { set { type = value; } }
    public ItemSlot GetEquipedItem => equipedItem;
    public ItemSlot SetEquipedItem { set { equipedItem = value; } }
}


public class HelmetEquip : EquipAbst
{

    public HelmetEquip()
    {
         this.SetEquipType = EquipType.Helmet;
    }
}


public class TopEquip : EquipAbst
{

    public TopEquip()
    {
        this.SetEquipType = EquipType.Top;
    }
}
public class BottomEquip : EquipAbst
{

    public BottomEquip()
    {
        this.SetEquipType = EquipType.Bottom; 
    }
}
public class GlovesEquip: EquipAbst
{

    public GlovesEquip()
    {
        this.SetEquipType = EquipType.Gloves;
    }
}
public class BootsEquip : EquipAbst
{

    public BootsEquip()
    {
        this.SetEquipType = EquipType.Boots;
    }
}