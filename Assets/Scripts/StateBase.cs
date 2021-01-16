
using UnityEngine;

public abstract class StateBase
{
    public virtual void ButtonA()
    {
        Debug.Log("Implement ButtonA");
        Inventory.GetInstance.AddToInventory(0, new ItemSlot(CraftingManager._instance.items.getitemsArr[11], 1));
        PlayerManager._instance.ImplementSpecialInteraction();
    }
    public abstract void ButtonB();
    public virtual void StateOnTouch(Touch touch) {
    
    
    
    }
}

