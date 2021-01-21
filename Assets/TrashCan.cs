using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class TrashCan : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler,IPointerUpHandler
{
    bool IsDragginOnTrashCan;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InventoryUIManager._instance.IsHoldingItem)
        {
            IsDragginOnTrashCan = true;
            InventoryUIManager._instance.IsDragginToTrash = true;
            Debug.Log("Holding item on trashcan");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUIManager._instance.IsDragginToTrash = false;
        IsDragginOnTrashCan = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsDragginOnTrashCan)
        {
            Debug.Log("pointer up");
            ThrowItem();
        }
    }

    void ThrowItem()
    {
        Debug.Log("Throwing Item");
        InventoryUIManager._instance.DeleteItem(InventoryUIManager._instance.DraggedItem);
    }



}
