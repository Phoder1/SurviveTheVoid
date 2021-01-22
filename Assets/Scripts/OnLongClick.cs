
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnLongClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public enum ClickType
    {
        Crafting,
        Inventory,
        Equip
    }

    public ClickType clickType;

    private bool pointerDown;
    private float pointerDownTimer;
    private bool ShortClick;

    public float requiredHoldTime;

    private const float minimumHoldAmount = 5;

    public UnityEvent onLongClick;
    public UnityEvent onShortClick;
    public UnityEvent onDropItem;

    [SerializeField]
    private Image fillImage;
    public int InventorySlotIndex;



    public int DraggedItem;
    bool IsDragged;

    [SerializeField]
    int EquipmentSlot;
    [SerializeField]
    bool DraggedInto;

    public void OnPointerDown(PointerEventData evenData)
    {
        pointerDown = true;
    }

    public void OnPointerUp(PointerEventData evendata)
    {

        if (pointerDownTimer < requiredHoldTime && pointerDown && onShortClick != null && !IsDragged)
        {
            if(onShortClick != null)
            onShortClick.Invoke();
        }
        if (clickType == ClickType.Inventory)
        {
            if (InventoryUIManager._instance.DraggedItem != -1 && InventoryUIManager._instance.DroppedItem != -1 && !InventoryUIManager._instance.IsDragginToTrash)
            {
                InventoryUIManager._instance.SwapItems();


            }
            else if (InventoryUIManager._instance.IsDragginToTrash)
            {
                InventoryUIManager._instance.DeleteItem(InventoryUIManager._instance.DraggedItem);
            }
            //else if (InventoryUIManager._instance.EquipIndex >= 0)
            //{
            //    InventoryUIManager._instance.Test(EquipmentSlot);
            //}else if (InventoryUIManager._instance.DraggedIntoBar >= 0 && InventoryUIManager._instance.GetConsumableToHotKey(InventoryUIManager._instance.GetItem(InventoryUIManager._instance.DraggedItem), InventoryUIManager._instance.DraggedIntoBar))
            //{
               
            //}
            else
            {
                InventoryUIManager._instance.ResetSwap();
            }
           





            DraggedItem = -1;
        }
        if(clickType == ClickType.Equip)
        {
            //if(InventoryUIManager._instance.EquipIndex >= 0)
            //{
            //    InventoryUIManager._instance.Test(EquipmentSlot);
            //}
        }



        Reset();

    }


    void Update()
    {


        if (pointerDown)
        {
            pointerDownTimer += Time.deltaTime;
            if (pointerDownTimer >= requiredHoldTime && clickType == ClickType.Crafting)
            {
                if (onLongClick != null)
                    onLongClick.Invoke();

                Reset();
            }
            else if (pointerDownTimer >= requiredHoldTime && clickType == ClickType.Inventory && !IsDragged)
            {
                if (onLongClick != null)
                    onLongClick.Invoke();



                //Debug.Log(DraggedItem.item.getItemName);
            }

            if (fillImage != null)
            {


                if (pointerDownTimer >= (requiredHoldTime / minimumHoldAmount))
                    fillImage.fillAmount = (pointerDownTimer - (requiredHoldTime / minimumHoldAmount)) / (requiredHoldTime - (requiredHoldTime / minimumHoldAmount));
            }
        }

       



    }

    void Reset()
    {
        pointerDown = false;
        pointerDownTimer = 0;
        if (fillImage != null)
            fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
        IsDragged = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {


        if (clickType == ClickType.Inventory)
        {
            if (pointerDown)
            {
                IsDragged = true;
                onLongClick.Invoke();
                //Debug.Log("Test");
            }



            InventoryUIManager._instance.CancelDropHighLight(InventoryUIManager._instance.DroppedItem);

            DraggedItem = -1;
        }
        if (clickType == ClickType.Equip)
        {
            //InventoryUIManager._instance.EquipIndex = -1;
            DraggedInto = false;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (clickType == ClickType.Inventory)
        {


            DraggedItem = InventoryUIManager._instance.DraggedItem;
            if (InventoryUIManager._instance.DraggedItem != InventoryUIManager._instance.DroppedItem)
            {
                onDropItem.Invoke();
            }
            //else if(InventoryUIManager._instance.HotKeyDragged >= 0)
            //{
            //    InventoryUIManager._instance.DroppedItem = InventorySlotIndex;
            //}


        }
        if (clickType == ClickType.Equip)
        {
            if (InventoryUIManager._instance.DraggedItem >= 0)
            {
                //InventoryUIManager._instance.EquipIndex = EquipmentSlot;
                DraggedInto = true;
            }

        }

    }
}

