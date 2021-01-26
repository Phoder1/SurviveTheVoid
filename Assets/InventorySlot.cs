using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotChestType
{
    none,
    Inventory,
    HotKey,
    Gear,
    Tools,
    Chest
}


public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public SlotChestType slotType;
    [Tooltip("Tools:\n" +
        "/// 0 Axe\n"+
        "/// 1 Pickaxe\n"+
        "/// 2 Hoe\n"+
        "/// 3 Shovel\n"+
        "/// 4 Hammer\n" +
        "Gear:\n" +
        "/// 0 Helmet\n" +
        "/// 1 Chest\n" +
        "/// 2 Leggin\n" +
        "/// 3 Gloves\n" +
        "/// 4 Shoes")]
    [Min(0)]
    public int slotPosition;
    [SerializeField] private Image highLightedSprite;
    [SerializeField] private Image itemSprite;
    [SerializeField] private Image defualtItemSprite;
    [SerializeField] private TextMeshProUGUI itemAmount;
    [SerializeField] private Color NormalColor;
    [SerializeField] private Color HighLightedColor;
    [SerializeField] private GameObject toggleOutline;
    [SerializeField] private GameObject highlightOutline;
    private EquipManager equipManager = EquipManager.GetInstance;
    private InventoryUIManager inventoryUI;
    private bool leftFrame;
    
    private DragNDropVisual Vis;
    private void Start() {
        inventoryUI = InventoryUIManager._instance;
        Vis = gameObject.GetComponent<DragNDropVisual>();
        SetToggleOutline(true);
    }


    public void OnPointerDown(PointerEventData eventData) {
        if (inventoryUI.takingFrom == SlotChestType.none) {
            leftFrame = false;
            var checkIfSlotIsItem = inventoryUI.GetInventory.GetItemFromInventoryButton(inventoryUI.GetInventoryID(slotType), slotPosition);
            if (checkIfSlotIsItem != null) {
                inventoryUI.TakingFrom(slotType, slotPosition);
                HighLightSlot();
            }

        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (inventoryUI.takingFrom != SlotChestType.none && (inventoryUI.takingFromIndex != slotPosition || inventoryUI.takingFrom != slotType)) {
            inventoryUI.DroppingAt(slotType, slotPosition);
            HighLightSlot();
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        //if taking anything
        if (inventoryUI.takingFrom != SlotChestType.none) {
            leftFrame = true;
            //to not by accident dehighlight the one you picked up

            //dropping at equip != inventory = true + taking from 0 = 0 = true = true both
            // dropping at Inventory != inventory = false + taking from 0 = 0 = false one true one = false;
            //


            if (inventoryUI.takingFromIndex != slotPosition || inventoryUI.takingFrom != slotType) // if 0 != 0 = true
            {
                //de highlight slot in correct inventory
                DeHighLightSlot();
            }


            // resets so you dont drop at anywhere
            inventoryUI.DroppingAt(SlotChestType.none, -1);



        }

    }

    public void OnPointerUp(PointerEventData eventData) {
        if (inventoryUI.takingFromIndex >= 0 && inventoryUI.droppingAtIndex >= 0) {
            inventoryUI.TrySwapItems();
        }
        else if (!leftFrame) {
            SlotAction();
        }else if (inventoryUI.IsDragginToTrash)
        {
            inventoryUI.RemoveItemViaTrashCan();
        }
        if (Vis != null) {
            Vis.ReturnToPos();
        }

        DeHighLightSlot();
        ResetDrag();

    }



    public void ResetDrag() {
        inventoryUI.DroppingAt(SlotChestType.none, -1);
        inventoryUI.TakingFrom(SlotChestType.none, -1);

    }


    public void UpdateSlot(ItemSlot item) {
        SetItemSprite(true, item);
    }
    public void EmptySlot() {
        SetItemSprite(false, null);
    }



    public void HighLightSlot() {
        highLightedSprite.color = HighLightedColor;
        if (highlightOutline != null)
            highlightOutline.SetActive(true);
    }

    public void DeHighLightSlot() {
        highLightedSprite.color = NormalColor;
        if (highlightOutline != null)
            highlightOutline.SetActive(false);
    }

    void SlotAction() {
        if (!inventoryUI.GetSetIsUiClosed && slotType == SlotChestType.Tools) {
            equipManager.SetActiveStateTool(equipManager.GetToolTypeByIndex(slotPosition), !equipManager.GetToolActive(equipManager.GetToolTypeByIndex(slotPosition)));
            SetToggleOutline(equipManager.GetToolActive(equipManager.GetToolTypeByIndex(slotPosition)));
        }
        else {
            inventoryUI.OnPressedInventoryButton();
        }
    }
    public void SetToggleOutline(bool state) {
        if (toggleOutline != null)
            toggleOutline.SetActive(state);
    }
    private void SetItemSprite(bool state, ItemSlot item) {
        itemSprite.gameObject.SetActive(state);
        if (defualtItemSprite)
            defualtItemSprite.gameObject.SetActive(!state);
        if (state) {
            itemSprite.sprite = item.item.getsprite;
            if (itemAmount != null)
                itemAmount.text = (item.amount <= 1 ? "" : item.amount.ToString());
        }
        else {
            itemSprite.sprite = null;
            if (itemAmount != null)
                itemAmount.text = "";
        }
    }
}
