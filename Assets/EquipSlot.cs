using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public Image SlotImage;
    public int ChestId;
    public int EquipId;

    public bool IsDraggingThis;



    public void OnPointerDown(PointerEventData eventData)
    {
        ItemSlot[] Test = Inventory.GetInstance.GetInventoryFromDictionary(ChestId);
        if (InventoryUIManager._instance.DraggedIntoEquip <= -1 && Test[EquipId] != null)
        {
            HighLightEquipSlot();
            IsDraggingThis = true;
           
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       if(InventoryUIManager._instance.DraggedItem >= 0)
        {
            InventoryUIManager._instance.WhatInventory(ChestId);
            InventoryUIManager._instance.DraggedIntoEquip = EquipId;
            HighLightEquipSlot();
        }else if(InventoryUIManager._instance.EquipDragged >= 0 && InventoryUIManager._instance.EquipDragged != EquipId)
        {
            InventoryUIManager._instance.WhatInventory(ChestId);
            InventoryUIManager._instance.DraggedIntoEquip = EquipId;
            HighLightEquipSlot();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(!IsDraggingThis)
        DeHighLightEquipSlot();
        else
        {
            InventoryUIManager._instance.EquipDragged = EquipId;
            InventoryUIManager._instance.TakingFrom(ChestId);

        }
        InventoryUIManager._instance.WhatInventory(-1);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsDraggingThis)
        {
            if(InventoryUIManager._instance.EquipDragged >= 0 && InventoryUIManager._instance.DroppedItem >= 0)
            {
                InventoryUIManager._instance.SwitchKeyInventory(InventoryUIManager._instance.EquipDragged, InventoryUIManager._instance.DroppedItem);
            }
            else if(InventoryUIManager._instance.EquipDragged >= 0 && InventoryUIManager._instance.DraggedIntoEquip >= 0)
            {
                InventoryUIManager._instance.SwitchKeyInventory(InventoryUIManager._instance.EquipDragged, InventoryUIManager._instance.DraggedIntoEquip);
            }
            else if (InventoryUIManager._instance.EquipDragged <= -1)
            {
                var checkIfSlotIsItem = Inventory.GetInstance.GetItemFromInventoryButton(2, EquipId);


                if (checkIfSlotIsItem == null || checkIfSlotIsItem.item == null)
                    return;


                EquipManager.GetInstance.CheckEquip(new ItemSlot(checkIfSlotIsItem.item, 1), EquipId, 2);
                InventoryUIManager._instance.UpdatePlayerInventory();
            }
        }
        else
        {
            InventoryUIManager._instance.EquipDragged = -1;
            InventoryUIManager._instance.TakingFrom(-1);
            InventoryUIManager._instance.ResetSwap();
        }


        IsDraggingThis = false;
        DeHighLightEquipSlot();










    }

    public void ShowEquippedGear(ItemSlot Equip)
    {
        SlotImage.gameObject.SetActive(true);
        SlotImage.sprite = Equip.item.getsprite;
        DeHighLightEquipSlot();
    }

    public void NoGearEquipped()
    {
        SlotImage.gameObject.SetActive(false);
        SlotImage.sprite = null;
    }


    public void HighLightEquipSlot()
    {
        transform.GetComponent<Image>().color = Color.yellow;

    }

    public void DeHighLightEquipSlot()
    {
        transform.GetComponent<Image>().color = Color.white;

    }




}
