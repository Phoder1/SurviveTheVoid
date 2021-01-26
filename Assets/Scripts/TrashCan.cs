using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class TrashCan : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public bool IsDragginOnTrashCan;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InventoryUIManager._instance.takingFromIndex >= 0)
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


  


}
