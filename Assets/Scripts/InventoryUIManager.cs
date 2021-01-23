using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoSingleton<InventoryUIManager>
{

    [SerializeField] bool IsInventoryOn = true;

    [Header("Inventory Related")]

    public GameObject[] InventorySlots;
    Image[] InventorySlotImage;
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
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            inventory.AddToInventory(1,new ItemSlot(CraftingManager._instance.items.getitemsArr[0],5));
            UpdateHotKeysToUI();
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
        }


    }





    public void UpdateInventoryToUI()
    {
        InventorySlotImage = new Image[inventory.GetInventory.Length];
        inventorySlotText = new TextMeshProUGUI[inventory.GetInventory.Length];
        for (int i = 0; i < inventory.GetInventory.Length; i++)
        {
            InventorySlotImage[i] = InventorySlots[i].transform.GetChild(0).GetComponent<Image>();

            inventorySlotText[i] = InventorySlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();

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

    public void UpdateEquipToUI()
    {
        //update Equip

        UpdateInventoryToUI();
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
            if(HotKeysInventory[i] != null)
            {
                ConsumableHotKey[i].ShowEquippedConsumable(HotKeysInventory[i]);
            }
            else
            {
                ConsumableHotKey[i].NoConsumableEquipped();
            }
        }
        UpdateInventoryToUI();
    }

    public void SwitchKeyInventory(int DraggedItem,int DroppedOn)
    {
        ItemSlot DraggedTemp = inventory.GetItemFromInventoryButton(TakingFromInventory,DraggedItem);
        ItemSlot DroppedTemp = inventory.GetItemFromInventoryButton(LookingAtInventory, DroppedOn);
        if (DroppedTemp == null && DraggedTemp.item.GetItemType == ItemType.Consumable)
        {
            inventory.ChangeBetweenItems(TakingFromInventory, LookingAtInventory, DraggedItem, DroppedOn);
        }
        else if (DraggedTemp.item.GetItemType == ItemType.Consumable && LookingAtInventory == 1)
        {
            inventory.ChangeBetweenItems(TakingFromInventory, LookingAtInventory, DraggedItem, DroppedOn);
        }
        else if (DraggedTemp.item.GetItemType != ItemType.Consumable && LookingAtInventory == 1 || DroppedTemp.item.GetItemType != ItemType.Consumable && TakingFromInventory == 1)
        {
            Debug.Log("Cant Drag Non Consumable items into hotkeys");
        }
        else
        {
            inventory.ChangeBetweenItems(TakingFromInventory, LookingAtInventory, DraggedItem, DroppedOn);
        }


        ResetSwap();
        UpdateHotKeysToUI();
    }



    #endregion
}
