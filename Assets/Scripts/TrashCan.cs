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
            InventoryUIManager._instance.HighLightTrashCan();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUIManager._instance.IsDragginToTrash = false;
        InventoryUIManager._instance.RemoveTrashHighLight();
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
        InventoryUIManager._instance.RemoveTrashHighLight();
    }



}
