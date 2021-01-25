using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoSingleton<InventoryUIManager>
{
    [Header("Inventory Related")]

    ItemSlot[] InventoryItem;
    public InventorySlot[] Slots;


    Inventory inventory;
    public Inventory GetInventory => inventory;
    EquipManager equipManager;


    [Header("Slots Related")]
    public Color SlotColor;
    public Image TrashCanBackGround;
    public bool IsDragging;
    private bool isUiOpen;
    public bool GetSetIsUiClosed {
        get => isUiOpen;
        set {
            isUiOpen = value;
            if (isUiOpen)
                UpdateInventoryToUI();
        }
    }
    public override void Init() {
        inventory = Inventory.GetInstance;
        InventoryItem = inventory.GetInventoryFromDictionary(0);
        HotKeysInventory = inventory.GetInventoryFromDictionary(1);
        EquipInventory = inventory.GetInventoryFromDictionary(2);
        toolsInventory = inventory.GetInventoryFromDictionary(3);
        equipManager = EquipManager.GetInstance;
    }




    // Update is called once per frame
    private void Update() {
        if (Input.GetKeyDown(KeyCode.H)) {
            //inventory.AddToInventory(1,new ItemSlot(CraftingManager._instance.items.getitemsArr[0],5));
            //UpdateHotKeysToUI();
        }
    }



    #region Inventory Slots

    //public boolean = setactive of all inventory,  only update inventory after this boolean is true, if false dont update
    public SlotChestType takingFrom;
    public int takingFromIndex;

    public SlotChestType droppingAt;
    public int droppingAtIndex;

    public void DroppingAt(SlotChestType type, int slot) {
        droppingAt = type;
        droppingAtIndex = slot;
    }
    public void TakingFrom(SlotChestType type, int slot) {
        takingFrom = type;
        takingFromIndex = slot;
    }


    public void OnPressedInventoryButton() {
        Debug.Log("Short press");
        var checkIfSlotIsItem = inventory.GetItemFromInventoryButton(GetInventoryID(takingFrom), takingFromIndex);


        if (checkIfSlotIsItem == null || checkIfSlotIsItem.item == null)
            return;


        ItemSlot itemCache = new ItemSlot(checkIfSlotIsItem.item, 1);


        switch (itemCache.item) {
            case ConsumableItemSO consumable:
                if (ConsumeablesHandler._instance.GetEffectCoolDown(consumable)) {
                    if (inventory.RemoveItemFromInventory(GetInventoryID(takingFrom), new ItemSlot(itemCache.item, 1))) {
                        Debug.Log("Consumed: " + itemCache.item.getItemName);
                        consumable.ApplyEffect();
                    }
                }
                UpdatePlayerInventory();
                break;
            case TileAbstSO tile:
                UIManager._instance.ButtonInventory();
                PlayerStateMachine.GetInstance.SwitchState(InputState.BuildState);
                (InputManager.GetCurrentState as BuildingState).SetBuildingTile(tile);
                break;
            case ToolItemSO tool:
            case GearItemSO gear:
                Equip(itemCache);
                break;
        }
    }

    void Equip(ItemSlot itemSlot) {
        ItemSO itemSO = itemSlot.item;
        if (itemSO.GetItemType != ItemType.Gear && itemSO.GetItemType != ItemType.Tool)
            return;
        bool isTool = itemSO.GetItemType == ItemType.Tool;
        int originInvID = GetInventoryID(takingFrom);
        int targetInvID = (takingFrom == SlotChestType.Inventory ? GetInventoryID(isTool ? SlotChestType.Tools : SlotChestType.Gear) : GetInventoryID(SlotChestType.Inventory));
        int targetIndex;

        if (itemSO is ToolItemSO tool)
            targetIndex = equipManager.GetIndexByToolType(tool.GetToolType);
        else if (itemSO is GearItemSO gear)
            targetIndex = equipManager.GetGearSlotIndex(gear.GetEquipType);
        else
            return;


        if (takingFrom == SlotChestType.Inventory) {
            equipManager.UnEquipItem(inventory.GetItemFromInventoryButton(targetInvID, targetIndex));
            equipManager.EquipItem(itemSlot);
            inventory.ChangeBetweenItems(originInvID, targetInvID, takingFromIndex, targetIndex);
            UpdatePlayerInventory();
        }
        else if (inventory.CheckIfEnoughSpaceInInventory(GetInventoryID(SlotChestType.Inventory), itemSlot)) {
            equipManager.UnEquipItem(itemSlot);
            inventory.RemoveItemAtPosition(originInvID, takingFromIndex);
            inventory.AddToInventory(GetInventoryID(SlotChestType.Inventory), itemSlot);
            UpdatePlayerInventory();
        }
    }




    public int GetInventoryID(SlotChestType chestType) {
        switch (chestType) {
            case SlotChestType.Inventory:
                return 0;
            case SlotChestType.HotKey:
                return 1;
            case SlotChestType.Gear:
                return 2;
            case SlotChestType.Tools:
                return 3;
            case SlotChestType.Chest:
                return 4;
            default:
                return -1;
        }
    }
    public void UpdateInventoryToUI() {
        for (int i = 0; i < InventoryItem.Length; i++) {
            if (InventoryItem[i] != null) {
                Slots[i].UpdateSlot(InventoryItem[i]);
            }
            else {
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



    public void OnLongInventoryPress(int buttonId) {
        if (inventory.GetItemFromInventoryButton(0, buttonId) != null && !IsHoldingItem) {
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


    public void HighLightTrashCan() {
        TrashCanBackGround.color = Color.red;
    }
    public void RemoveTrashHighLight() {
        TrashCanBackGround.color = SlotColor;
    }

    #endregion

    #region Equip
    [Header("Equip Related")]

    ItemSlot[] EquipInventory;
    public InventorySlot[] EquipSlots;

    public void UpdateEquipToUI() {
        //update Equip
        for (int i = 0; i < EquipInventory.Length; i++) {
            if (EquipInventory[i] != null) {
                //show the item sprite
                EquipSlots[i].UpdateSlot(EquipInventory[i]);
            }
            else {
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



    public void UpdateHotKeysToUI() {
        //update hotkeys
        for (int i = 0; i < HotKeysInventory.Length; i++) {
            if (HotKeysInventory[i] != null) {
                ConsumableHotKey[i].UpdateSlot(HotKeysInventory[i]);
            }
            else {
                ConsumableHotKey[i].EmptySlot();
            }
            ConsumableHotKey[i].DeHighLightSlot();
        }
    }




    public bool TrySwapItems() {

        ItemSlot draggedItem = inventory.GetItemFromInventoryButton(GetInventoryID(takingFrom), takingFromIndex);
        ItemSlot occupyingTile = inventory.GetItemFromInventoryButton(GetInventoryID(droppingAt), droppingAtIndex);

        bool swapable = CanPlaceOnSlot(draggedItem, droppingAt, droppingAtIndex) && CanPlaceOnSlot(occupyingTile, takingFrom, takingFromIndex);
        if (swapable) {
            if (takingFrom == SlotChestType.Gear || takingFrom == SlotChestType.Tools) {
                equipManager.UnEquipItem(draggedItem);
            if(occupyingTile != null)
                equipManager.EquipItem(occupyingTile);
            }
            if (droppingAt == SlotChestType.Gear || droppingAt == SlotChestType.Tools) {
                if (occupyingTile != null) 
                    equipManager.UnEquipItem(occupyingTile);
                equipManager.EquipItem(draggedItem);
            }
            inventory.ChangeBetweenItems(GetInventoryID(takingFrom), GetInventoryID(droppingAt), takingFromIndex, droppingAtIndex);

            UpdatePlayerInventory();
        }
        return swapable;
    }

    public void UpdatePlayerInventory() {
        UpdateHotKeysToUI();
        UpdateEquipToUI();
        UpdateInventoryToUI();
        UpdateToolsToUI();
    }

    bool CanPlaceOnSlot(ItemSlot dragged, SlotChestType toChest, int DropInvSlot) {
        if (dragged == null || dragged.item == null)
            return true;
        switch (toChest) {
            case SlotChestType.HotKey:
                return (dragged.item.GetItemType == ItemType.Consumable);
            case SlotChestType.Gear:
            case SlotChestType.Tools:
                return EquipManager.GetInstance.CheckEquip(dragged, toChest, DropInvSlot);
            default:
                return true;
        }

    }

    #endregion



    #region Tools related
    ItemSlot[] toolsInventory;
    public InventorySlot[] toolsSlots;

    public void UpdateToolsToUI() {
        for (int i = 0; i < toolsInventory.Length; i++) {
            if (toolsInventory[i] != null) {
                toolsSlots[i].UpdateSlot(toolsInventory[i]);
            }
            else {
                toolsSlots[i].EmptySlot();
            }
            toolsSlots[i].DeHighLightSlot();
        }
    }


    #endregion
}
