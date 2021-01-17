
using UnityEngine;

public abstract class StateBase
{
    public virtual void ButtonA()
    {

        // Inventory.GetInstance.AddToInventory(0, new ItemSlot(CraftingManager._instance.items.getitemsArr[11], 1));

        PlayerManager._instance.ImplementInteraction(true);
    }
    public abstract void ButtonB();
    public virtual void StateOnTouch(Touch touch) {
    
    
    
    }
}

