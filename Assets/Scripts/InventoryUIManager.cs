using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoSingleton<InventoryUIManager>
{

    [SerializeField] bool IsInventoryOn = true;

    [Header("Inventory Related")]

    public GameObject[] InventorySlots;
    [SerializeField]
    Image[] InventorySlotImage;

    [SerializeField]
    TextMeshProUGUI[] inventorySlotText;

    Inventory inventory;
    public Inventory GetInventory => inventory;


    [Header("Slots Related")]
    public Color SlotColor;
    public Image TrashCanBackGround;
    public bool IsDragging;
    public override void Init()
    {
        inventory = Inventory.GetInstance;
        HotKeysInventory = inventory.GetInventoryFromDictionary(1);
        EquipInventory = inventory.GetInventoryFromDictionary(2);
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
    [SerializeField]
    int LookingAtInventory;
    [SerializeField]
    int TakingFromInventory;

    public void WhatInventory(int id)
    {
        LookingAtInventory = id;
    }
    public void TakingFrom(int id)
    {
        TakingFromInventory = id;
    }


    public void OnPressedInventoryButton(int buttonId)
    {
        Debug.Log("Short press");
        var checkIfSlotIsItem = inventory.GetItemFromInventoryButton(LookingAtInventory, buttonId);


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
                if (inventory.RemoveItemFromInventory(LookingAtInventory, new ItemSlot(itemCache.item, 1)))
                {
                    Debug.Log("Consumed: " + itemCache.item.getItemName);
                    (itemCache.item as ConsumableItemSO).ApplyEffect();
                }
            }

        }
        if (itemCache.item.GetItemType == ItemType.Equipable)
        {
            EquipManager.GetInstance.CheckEquip(buttonId, 0);
            UpdatePlayerInventory();
        }


    }





    public void UpdateInventoryToUI()
    {
        for (int i = 0; i < inventory.GetInventory.Length; i++)
        {

            if (inventory.GetInventory[i] != null)
            {
                if (!InventorySlotImage[i].isActiveAndEnabled)
                {
                    SetInventorySlotActive(i, true);
                }

                InventorySlotImage[i].sprite = inventory.GetInventory[i].item.getsprite;

                UpdateInventorySlotAmountUI(i, inventory.GetInventory[i].amount, true);
            }
            else
            {
                if (InventorySlotImage[i].isActiveAndEnabled)
                {
                    SetInventorySlotActive(i, false);
                }
                InventorySlotImage[i].sprite = null;
                UpdateInventorySlotAmountUI(i, 0, false);


            }
        }



    }





    void SetInventorySlotActive(int Index, bool IsActive)
    {
        InventorySlotImage[Index].gameObject.SetActive(IsActive);
        inventorySlotText[Index].gameObject.SetActive(IsActive);
    }
    void UpdateInventorySlotAmountUI(int Index, int Amount, bool IsItemExist)
    {
        if (IsItemExist && Amount > 1)
        {
            inventorySlotText[Index].text = Amount.ToString();
        }
        else
        {
            inventorySlotText[Index].text = "";
        }

    }


    public int DraggedItem;
    public int DroppedItem;

    public void OnLongInventoryPress(int buttonId)
    {
        if (inventory.GetItemFromInventoryButton(0, buttonId) != null && !IsHoldingItem)
        {
            Debug.Log("holding Item");
            IsHoldingItem = true;
            DraggedItem = buttonId;
            InventorySlots[buttonId].GetComponent<Image>().color = Color.yellow;
        }


    }
    public void GetDroppedItem(int buttonId)
    {
        if (DraggedItem != buttonId)
        {
            
            DroppedItem = buttonId;
            HightLightDrop(buttonId);
        }


    }

    public void HightLightDrop(int buttonId)
    {
        InventorySlots[buttonId].GetComponent<Image>().color = Color.yellow;
    }


    public void CancelDropHighLight(int buttonId)
    {
        if (buttonId >= 0)
        {
            InventorySlots[DroppedItem].GetComponent<Image>().color = SlotColor;
            DroppedItem = -1;
            UpdateInventoryToUI();
            //ResetSwap();
        }
    }

    [HideInInspector]
    public bool IsHoldingItem;
    [HideInInspector]
    public bool IsDragginToTrash;

    public void ResetSwap()
    {
        IsHoldingItem = false;
        if (DraggedItem >= 0)
            InventorySlots[DraggedItem].GetComponent<Image>().color = SlotColor;
        if (DroppedItem >= 0)
            InventorySlots[DroppedItem].GetComponent<Image>().color = SlotColor;

        DraggedItem = -1;
        DroppedItem = -1;

        DraggedIntoBar = -1;
        HotKeyDragged = -1;

        LookingAtInventory = -1;
        TakingFromInventory = -1;

        EquipDragged = -1;
        EquipDraggedInto = -1;
        DraggedIntoEquip = -1;

    }
    public void SwapItems()
    {
        //new Color(190, 99, 22);

        inventory.ChangeBetweenItems(0, 0, DraggedItem, DroppedItem);
        UpdateInventoryToUI();
        ResetSwap();
    }
    public void DeleteItem(int buttonId)
    {
        inventory.RemoveItemFromButton(buttonId, 0);
        UpdateInventoryToUI();
        ResetSwap();
    }

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
    public int DraggedIntoEquip;
    public int EquipDragged;
    public int EquipDraggedInto;
    ItemSlot[] EquipInventory;
    public EquipSlot[] EquipSlots;

    public void UpdateEquipToUI()
    {
        //update Equip
        for (int i = 0; i < EquipInventory.Length; i++)
        {
            if (EquipInventory[i] != null)
            {
                //show the item sprite
                EquipSlots[i].ShowEquippedGear(EquipInventory[i]);
            }
            else
            {
                //show that you don't wear anything
                EquipSlots[i].NoGearEquipped();
            }
        }
    }




    #endregion

    #region HotKey
    [Header("HotKey Related")]
    public int DraggedIntoBar;
    public int HotKeyDragged;
    public int HotKeyDraggedInto;
    ItemSlot[] HotKeysInventory;
    public ConsumableHotBar[] ConsumableHotKey;



    public void UpdateHotKeysToUI()
    {
        //update hotkeys
        for (int i = 0; i < HotKeysInventory.Length; i++)
        {
            if (HotKeysInventory[i] != null)
            {
                ConsumableHotKey[i].ShowEquippedConsumable(HotKeysInventory[i]);
            }
            else
            {
                ConsumableHotKey[i].NoConsumableEquipped();
            }
        }
    }

    public void SwitchKeyInventory(int DraggedItem, int DroppedOn)
    {
        ItemSlot DraggedTemp = inventory.GetItemFromInventoryButton(TakingFromInventory, DraggedItem);
        ItemSlot DroppedTemp = inventory.GetItemFromInventoryButton(LookingAtInventory, DroppedOn);
        if (canEquipOnConsumable(TakingFromInventory, LookingAtInventory, DraggedTemp, DroppedTemp, DraggedItem, DroppedOn))
        {
            inventory.ChangeBetweenItems(TakingFromInventory, LookingAtInventory, DraggedItem, DroppedOn);

        }
        UpdatePlayerInventory();
        ResetSwap();


    }

    public void UpdatePlayerInventory()
    {
        UpdateHotKeysToUI();
        UpdateEquipToUI();
        UpdateInventoryToUI();
    }

    bool canEquipOnConsumable(int fromChest, int toChest, ItemSlot Dragged, ItemSlot Drop, int draggedSlot, int DropSlot)
    {
        if (toChest == 0)
        {
            return true;
        }
        else if (toChest == 1)
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
        else if (toChest == 2)
        {


            if (Dragged.item.GetItemType == ItemType.Equipable)
            {

                if (EquipManager.GetInstance.CheckEquip(draggedSlot, fromChest, DropSlot, toChest))
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
        else
        {
            return true;
        }

    }

    #endregion
}
