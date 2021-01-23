﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public int ChestId;
    public int SlotKey;
    //public ItemSlot ItemHolder;
    public bool IsDraggingThis;


    public void OnPointerDown(PointerEventData eventData)
    {
        ItemSlot[] Test = Inventory.GetInstance.GetInventoryFromDictionary(ChestId);

        if (InventoryUIManager._instance.DraggedIntoBar <= -1 && Test[SlotKey] != null)
        {

            IsDraggingThis = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InventoryUIManager._instance.DraggedItem >= 0)
        {
            InventoryUIManager._instance.DraggedIntoBar = SlotKey;

            InventoryUIManager._instance.WhatInventory(ChestId);
        }
        else if (InventoryUIManager._instance.HotKeyDragged >= 0 && InventoryUIManager._instance.HotKeyDragged != SlotKey)
        {
            InventoryUIManager._instance.HotKeyDraggedInto = SlotKey;

            InventoryUIManager._instance.WhatInventory(ChestId);
        }
        else if (InventoryUIManager._instance.EquipDragged >= 0)
        {
            InventoryUIManager._instance.HotKeyDraggedInto = SlotKey;

            InventoryUIManager._instance.WhatInventory(ChestId);
        }
        else if (InventoryUIManager._instance.HotKeyDragged == SlotKey)
        {
            IsDraggingThis = false;
        }




    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsDraggingThis)
        {
            InventoryUIManager._instance.HotKeyDragged = SlotKey;
            InventoryUIManager._instance.TakingFrom(ChestId);
        }
        else
        {
            InventoryUIManager._instance.WhatInventory(-1);
            InventoryUIManager._instance.DraggedIntoBar = -1;
            InventoryUIManager._instance.HotKeyDraggedInto = -1;
        }
        //IsDraggingThis = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        if (InventoryUIManager._instance.HotKeyDragged >= 0 && InventoryUIManager._instance.DroppedItem >= 0)
        {
            InventoryUIManager._instance.SwitchKeyInventory(InventoryUIManager._instance.HotKeyDragged, InventoryUIManager._instance.DroppedItem);
        }
        else if (InventoryUIManager._instance.HotKeyDragged >= 0 && InventoryUIManager._instance.HotKeyDraggedInto >= 0)
        {
            InventoryUIManager._instance.SwitchKeyInventory(InventoryUIManager._instance.HotKeyDragged, InventoryUIManager._instance.HotKeyDraggedInto);
        }
        else
        {
            InventoryUIManager._instance.HotKeyDragged = -1;
            InventoryUIManager._instance.TakingFrom(-1);
            InventoryUIManager._instance.ResetSwap();
        }
        if (InventoryUIManager._instance.HotKeyDraggedInto >= 0 && InventoryUIManager._instance.HotKeyDragged >= 0)
        {
            //InventoryUIManager._instance.SwitchKeyInventory(InventoryUIManager._instance.HotKeyDragged, InventoryUIManager._instance.HotKeyDraggedInto);
        }

        if (InventoryUIManager._instance.HotKeyDragged <= -1 && !IsDraggingThis)
        {
            ConsumeHotBar();
        }

        IsDraggingThis = false;


    }

    void ConsumeHotBar()
    {
        var checkIfSlotIsItem = Inventory.GetInstance.GetItemFromInventoryButton(ChestId, SlotKey);


        if (checkIfSlotIsItem == null || checkIfSlotIsItem.item == null)
            return;


        ItemSlot itemCache = new ItemSlot(checkIfSlotIsItem.item, 1);

        if (itemCache.item.GetItemType == ItemType.Consumable)
        {
            if (ConsumeablesHandler._instance.GetEffectCoolDown(itemCache.item as ConsumableItemSO))
            {
                if (Inventory.GetInstance.RemoveItemFromInventory(ChestId, new ItemSlot(itemCache.item, 1)))
                {
                    Debug.Log("Consumed: " + itemCache.item.getItemName);
                    (itemCache.item as ConsumableItemSO).ApplyEffect();
                }
            }

        }
        InventoryUIManager._instance.UpdateHotKeysToUI();
    }




 

   
}
