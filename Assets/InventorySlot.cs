using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotChestTypes
{
    none,
    Inventory,
    HotKey,
    Equip,
    Tools,
    Chest
}


public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public SlotChestTypes slotType;
    public int slotPosition;
    InventoryUIManager inventoryUI;
    public bool IsDraggingThis;
    public Image highLightedSprite;
    public Image ItemSprite;
    public TextMeshProUGUI ItemAmount;
    public bool isInventoryUIClose = false;
    public Color NormalColor;
    public Color HighLightedColor;
    DragNDropVisual Vis;
    private void Start()
    {
        inventoryUI = InventoryUIManager._instance;
        Vis = gameObject.GetComponent<DragNDropVisual>();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (inventoryUI.takingFrom == SlotChestTypes.none)
        {
            int ChestId = (int)slotType - 1;
            var checkIfSlotIsItem = inventoryUI.GetInventory.GetItemFromInventoryButton(ChestId, slotPosition);
            if (checkIfSlotIsItem != null)
            {
                IsDraggingThis = true;
                inventoryUI.TakingFrom(slotType, slotPosition);
                HighLightSlot();
            }

        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (inventoryUI.takingFrom != SlotChestTypes.none)
        {
            inventoryUI.DroppingAt(slotType, slotPosition);
            HighLightSlot();
        }



    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //if taking anything
        if (inventoryUI.takingFrom != SlotChestTypes.none)
        {
            //to not by accident dehighlight the one you picked up

            //dropping at equip != inventory = true + taking from 0 = 0 = true = true both
            // dropping at Inventory != inventory = false + taking from 0 = 0 = false one true one = false;
            //


            if (inventoryUI.takingFromIndex != slotPosition) // if 0 != 0 = true
            {
                if (inventoryUI.droppingAt == slotType)// if equip == equip = true;
                {
                    //de highlight in correct inventory
                    DeHighLightSlot();
                }
                //de highlight slot in correct inventory
                DeHighLightSlot();
            }


            // resets so you dont drop at anywhere
            inventoryUI.DroppingAt(SlotChestTypes.none, -1);

          

        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (inventoryUI.takingFromIndex >= 0 && inventoryUI.droppingAtIndex >= 0)
        {
            inventoryUI.SwapItems();
        }
        else
        {
            SlotAction();
        }
        if(Vis != null)
        {
            Vis.ReturnToPos();
        }

        DeHighLightSlot();
        ResetDrag();
        IsDraggingThis = false;
    }



    public void ResetDrag()
    {
        inventoryUI.DroppingAt(SlotChestTypes.none, -1);
        inventoryUI.TakingFrom(SlotChestTypes.none, -1);

    }


    public void UpdateSlot(ItemSlot Item)
    {
        ItemSprite.gameObject.SetActive(true);
        ItemSprite.sprite = Item.item.getsprite;
        if (ItemAmount != null)
            ItemAmount.text = Item.amount.ToString();
    }
    public void EmptySlot()
    {
        ItemSprite.gameObject.SetActive(false);
        ItemSprite.sprite = null;

        if (ItemAmount != null)
            ItemAmount.text = "";
    }



    public void HighLightSlot()
    {
        highLightedSprite.color = HighLightedColor;
    }

    public void DeHighLightSlot()
    {
        highLightedSprite.color = NormalColor;
    }

    void SlotAction()
    {
        if (isInventoryUIClose)
        {
            EquipManager.GetInstance.SetActiveStateTool(slotPosition, !EquipManager.GetInstance.GetToolActive(EquipManager.GetInstance.GetToolTypeByIndex(slotPosition)));
         //  if true == EquipManager.GetInstance.GetToolActive(EquipManager.GetInstance.GetToolTypeByIndex(slotPosition))
         // visual effect
        }
        else
        {
        inventoryUI.OnPressedInventoryButton();
         }
    }









}
