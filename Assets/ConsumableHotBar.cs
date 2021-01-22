using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConsumableHotBar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public int SlotKey;
    //public ItemSlot ItemHolder;
    public Image ItemBarImage;
    //public bool HasItem;
    public bool IsDraggingThis;


    public void OnPointerDown(PointerEventData eventData)
    {
        //if (InventoryUIManager._instance.DraggedIntoBar <= -1 && ItemHolder.item != null)
        //{
        //    IsDraggingThis = true;
        //    InventoryUIManager._instance.HotKeyDragged = SlotKey;
        //}

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InventoryUIManager._instance.DraggedItem >= 0)
        {
            InventoryUIManager._instance.DraggedIntoBar = SlotKey;
        }
        else if (InventoryUIManager._instance.HotKeyDragged >= 0)
        {
            InventoryUIManager._instance.HotKeyDraggedInto = SlotKey;
        }



    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUIManager._instance.DraggedIntoBar = -1;
        InventoryUIManager._instance.HotKeyDraggedInto = -1;
        IsDraggingThis = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    //    if (InventoryUIManager._instance.HotKeyDraggedInto >= 0 && InventoryUIManager._instance.HotKeyDragged >= 0)
    //    {
    //        InventoryUIManager._instance.SwapBetweenKeys(InventoryUIManager._instance.HotKeyDragged, InventoryUIManager._instance.HotKeyDraggedInto);
    //    }
    //    else if(InventoryUIManager._instance.HotKeyDragged >= 0 && InventoryUIManager._instance.DroppedItem >= 0)
    //    {
    //        InventoryUIManager._instance.SwapBetweenKeyToInventory(InventoryUIManager._instance.HotKeyDragged, InventoryUIManager._instance.DroppedItem);
    //    }
    }


    public void ShowEquippedConsumable()
    {
        
    }
    public void NoConsumableEquipped()
    {
        ItemBarImage.sprite = null;
    }



}
