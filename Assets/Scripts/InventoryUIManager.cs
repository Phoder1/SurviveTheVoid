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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            UpdateInventory();
    }



    #region Inventory Slots

    //public boolean = setactive of all inventory,  only update inventory after this boolean is true, if false dont update



    public void UpdateInventory()
    {
        if (IsInventoryOn)
        {
            UpdateInventoryToUI();
        }
    }

    public void OnPressedInventoryButton(int buttonId)
    {
        var checkIfSlotIsItem = inventory.GetItemFromInventoryButton(0, buttonId);


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
            if (EffectHandler._instance.GetEffectCoolDown(itemCache.item as ConsumableItemSO))
            {
                if (inventory.RemoveItemFromInventory(0, new ItemSlot(itemCache.item, 1)))
                {
                    Debug.Log("Consumed: " + itemCache.item.getItemName);
                    (itemCache.item as ConsumableItemSO).ApplyEffect();
                }
            }

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
    // 0 = head, 1 = Chest, 2 = Legging, 3 = Gloves, 4 = Shoes
    public Image[] EquipSlots;
    public int EquipIndex;
    // public GearItemSO[] EquippedSlots;

    public void UpdateEquipUI(GearItemSO Gear, Sprite GearSprite)
    {
        switch (Gear.GetGearTpye)
        {
            case GearItemSO.GearType.None:
                break;
            case GearItemSO.GearType.Helmet:
                EquipSlots[0].sprite = GearSprite;
                break;
            case GearItemSO.GearType.Chest:
                EquipSlots[1].sprite = GearSprite;
                break;
            case GearItemSO.GearType.Gloves:
                EquipSlots[2].sprite = GearSprite;
                break;
            case GearItemSO.GearType.Legging:
                EquipSlots[3].sprite = GearSprite;
                break;
            case GearItemSO.GearType.Shoes:
                EquipSlots[4].sprite = GearSprite;
                break;
            default:
                break;
        }
    }

    public void Test(int EquipmentSlot)
    {

        if (DraggedItem >= 0)
        {
            var checkIfSlotIsItem = Inventory.GetInstance.GetItemFromInventoryButton(0, EquipmentSlot);

            if (EquipManager.GetInstance.EquipItem(EquipmentSlot, checkIfSlotIsItem))
            {
                Debug.Log("Equipping: " + checkIfSlotIsItem.item.getItemName);
            }
            else
            {
                Debug.Log("this is not the right spot to place it");
            }
        }

    }


    #endregion

}
