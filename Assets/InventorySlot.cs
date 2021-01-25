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
    [Min(0)]
    public int slotPosition;
    [SerializeField] private Image highLightedSprite;
    [SerializeField] private Image ItemSprite;
    [SerializeField] private TextMeshProUGUI ItemAmount;
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
            if (inventoryUI.TrySwapItems()) {
                ResetSlot();
            }

        }
        else if (!leftFrame) {
            SlotAction();
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


    public void UpdateSlot(ItemSlot Item) {
        ItemSprite.gameObject.SetActive(true);
        ItemSprite.sprite = Item.item.getsprite;
        if (ItemAmount != null)
            ItemAmount.text = (Item.amount <= 1 ? "" : Item.amount.ToString());
    }
    public void EmptySlot() {
        ItemSprite.gameObject.SetActive(false);
        ItemSprite.sprite = null;

        if (ItemAmount != null)
            ItemAmount.text = "";
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
            equipManager.SetActiveStateTool(slotPosition, !equipManager.GetToolActive(equipManager.GetToolTypeByIndex(slotPosition)));
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
    private void ResetSlot() {

    }
}
