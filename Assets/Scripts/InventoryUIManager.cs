using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoSingleton<InventoryUIManager>
{
    [Header("Inventory Related")]

    ItemSlot[] InventoryItem;
    public InventorySlot[] Slots;


    Inventory inventory;
    public Inventory GetInventory => inventory;


    [Header("Slots Related")]
    public Color SlotColor;
    public Image TrashCanBackGround;
    public bool IsDragging;
    public override void Init()
    {
        inventory = Inventory.GetInstance;
        InventoryItem = inventory.GetInventoryFromDictionary(0);
        HotKeysInventory = inventory.GetInventoryFromDictionary(1);
        EquipInventory = inventory.GetInventoryFromDictionary(2);
        toolsInventory = inventory.GetInventoryFromDictionary(3);
    }
    



    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            //inventory.AddToInventory(1,new ItemSlot(CraftingManager._instance.items.getitemsArr[0],5));
            //UpdateHotKeysToUI();
        }
    }



    #region Inventory Slots

    //public boolean = setactive of all inventory,  only update inventory after this boolean is true, if false dont update
    public SlotChestTypes takingFrom;
    public int takingFromIndex;

    public SlotChestTypes droppingAt;
    public int droppingAtIndex;

    public void DroppingAt(SlotChestTypes type,int slot)
    {
        droppingAt = type;
        droppingAtIndex = slot;
    }
    public void TakingFrom(SlotChestTypes type, int slot)
    {
        takingFrom = type;
        takingFromIndex = slot;
    }


    public void OnPressedInventoryButton()
    {
        Debug.Log("Short press");
        int ChestId = (int)takingFrom - 1;
        var checkIfSlotIsItem = inventory.GetItemFromInventoryButton(ChestId, takingFromIndex);


        if (checkIfSlotIsItem == null || checkIfSlotIsItem.item == null)
            return;
       

        ItemSlot itemCache = new ItemSlot(checkIfSlotIsItem.item, 1);



        if (itemCache.item.GetItemType == ItemType.Building)
        {
            UIManager._instance.ButtonInventory();
            PlayerStateMachine.GetInstance.SwitchState(InputState.BuildState);
            (InputManager.GetCurrentState as BuildingState).SetBuildingTile(itemCache.item as TileAbstSO);
        }


        if (itemCache.item.GetItemType == ItemType.Consumable)
        {
            if (ConsumeablesHandler._instance.GetEffectCoolDown(itemCache.item as ConsumableItemSO))
            {
                if (inventory.RemoveItemFromInventory(ChestId, new ItemSlot(itemCache.item, 1)))
                {
                    Debug.Log("Consumed: " + itemCache.item.getItemName);
                    (itemCache.item as ConsumableItemSO).ApplyEffect();
                }
            }
            UpdatePlayerInventory();
        }
        if (itemCache.item.GetItemType == ItemType.Equipable|| itemCache.item.GetItemType == ItemType.Tools)
        {


            EquipManager.GetInstance.CheckEquip(itemCache,  takingFromIndex ,ChestId);
            UpdatePlayerInventory();



        }


    }





    public void UpdateInventoryToUI()
    {
        for (int i = 0; i < InventoryItem.Length; i++)
        {
            if (InventoryItem[i] != null)
            {
                Slots[i].UpdateSlot(InventoryItem[i]);
            }
            else
            {
                Slots[i].EmptySlot();
            }
            Slots[i].DeHighLightSlot();

        }
    }





    //void SetInventorySlotActive(int Index, bool IsActive)
    //{
    //    InventorySlotImage[Index].gameObject.SetActive(IsActive);
    //    inventorySlotText[Index].gameObject.SetActive(IsActive);
    //}
    //void UpdateInventorySlotAmountUI(int Index, int Amount, bool IsItemExist)
    //{
    //    if (IsItemExist && Amount > 1)
    //    {
    //        inventorySlotText[Index].text = Amount.ToString();
    //    }
    //    else
    //    {
    //        inventorySlotText[Index].text = "";
    //    }

    //}



    public void OnLongInventoryPress(int buttonId)
    {
        if (inventory.GetItemFromInventoryButton(0, buttonId) != null && !IsHoldingItem)
        {
            Debug.Log("holding Item");
            IsHoldingItem = true;
            //DraggedItem = buttonId;
            //InventorySlots[buttonId].GetComponent<Image>().color = Color.yellow;
        }


    }





  
    [HideInInspector]
    public bool IsHoldingItem;
    [HideInInspector]
    public bool IsDragginToTrash;

   
    public void HighLightTrashCan()
    {
        TrashCanBackGround.color = Color.red;
    }
    public void RemoveTrashHighLight()
    {
        TrashCanBackGround.color = SlotColor;
    }

    #endregion

    #region Equip
    [Header("Equip Related")]

    ItemSlot[] EquipInventory;
    public InventorySlot[] EquipSlots;

    public void UpdateEquipToUI()
    {
        //update Equip
        for (int i = 0; i < EquipInventory.Length; i++)
        {
            if (EquipInventory[i] != null)
            {
                //show the item sprite
                EquipSlots[i].UpdateSlot(EquipInventory[i]);
            }
            else
            {
                //show that you don't wear anything
                EquipSlots[i].EmptySlot();
            }
            EquipSlots[i].DeHighLightSlot();
        }
    }




    #endregion

    #region HotKey
    [Header("HotKey Related")]
    ItemSlot[] HotKeysInventory;
    public InventorySlot[] ConsumableHotKey;



    public void UpdateHotKeysToUI()
    {
        //update hotkeys
        for (int i = 0; i < HotKeysInventory.Length; i++)
        {
            if (HotKeysInventory[i] != null)
            {
                ConsumableHotKey[i].UpdateSlot(HotKeysInventory[i]);
            }
            else
            {
                ConsumableHotKey[i].EmptySlot();
            }
            ConsumableHotKey[i].DeHighLightSlot();
        }
    }




    public void SwapItems()
    {
        int FirstChestID = ((int)takingFrom - 1);
        int SecondChestID = ((int)droppingAt - 1);

        ItemSlot DraggedTemp = inventory.GetItemFromInventoryButton(FirstChestID, takingFromIndex);
        ItemSlot DroppedTemp = inventory.GetItemFromInventoryButton(SecondChestID, droppingAtIndex);
        if (CanEquipOnCurrentInventory(takingFrom, droppingAt, DraggedTemp, DroppedTemp, takingFromIndex, droppingAtIndex))
        {
            inventory.ChangeBetweenItems(FirstChestID, SecondChestID, takingFromIndex, droppingAtIndex);

        }
        UpdatePlayerInventory();
    }

    public void UpdatePlayerInventory()
    {
        UpdateHotKeysToUI();
        UpdateEquipToUI();
        UpdateInventoryToUI();
        UpdateToolsToUI();
    }

    bool CanEquipOnCurrentInventory(SlotChestTypes fromChest, SlotChestTypes toChest, ItemSlot Dragged, ItemSlot Drop, int draggedSlot, int DropSlot)
    {
        if (toChest == SlotChestTypes.Inventory)
        {
            return true;
        }
        else if (toChest == SlotChestTypes.HotKey)
        {
            if (Dragged.item.GetItemType == ItemType.Consumable)
            {

                return true;
            }
            else
            {
                return false;
            }
        }
        else if (toChest == SlotChestTypes.Equip)
        {


            if (Dragged.item.GetItemType == ItemType.Equipable)
            {


                if (EquipManager.GetInstance.CheckEquip(Dragged, draggedSlot, (int)fromChest-1, DropSlot, (int)toChest-1))
                {
                    return true;
                }


                return false;
            }
            else
            {
                return false;
            }
        }
        else if(toChest == SlotChestTypes.Tools)
        {
            if (EquipManager.GetInstance.CheckEquip(Dragged, draggedSlot, (int)fromChest - 1, DropSlot, (int)toChest - 1))
            {
                return true;
            }
            return false;
        }
        else
        {
            return true;
        }

    }

    #endregion



    #region Tools related
    ItemSlot[] toolsInventory;
    public InventorySlot[] toolsSlots;

    public void UpdateToolsToUI()
    {
        for (int i = 0; i < toolsInventory.Length; i++)
        {
            if (toolsInventory[i] != null)
            {
                toolsSlots[i].UpdateSlot(toolsInventory[i]);
            }
            else
            {
                toolsSlots[i].EmptySlot();
            }
            toolsSlots[i].DeHighLightSlot();
        }
    }


	#endregion
}
