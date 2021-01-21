
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnLongClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public enum ClickType
    {
        Crafting,
        Inventory
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

    public int DraggedItem;




    public void OnPointerDown(PointerEventData evenData)
    {
        pointerDown = true;
    }

    public void OnPointerUp(PointerEventData evendata)
    {

        if (pointerDownTimer < requiredHoldTime && pointerDown && onShortClick != null)
        {
            onShortClick.Invoke();
        }
        if(clickType == ClickType.Inventory)
        {
            if (InventoryUIManager._instance.DraggedItem != -1 && InventoryUIManager._instance.DroppedItem != -1)
            {
                InventoryUIManager._instance.SwapItems();
            }else if (InventoryUIManager._instance.IsDragginToTrash)
            {
                InventoryUIManager._instance.DeleteItem(InventoryUIManager._instance.DraggedItem);
            }
            else
            {
                InventoryUIManager._instance.ResetSwap();
            }





            DraggedItem = -1;
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
            }else if (pointerDownTimer >= requiredHoldTime && clickType == ClickType.Inventory)
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
    }

    public void OnPointerExit(PointerEventData eventData)
    {


        if(clickType == ClickType.Inventory)
        {

            InventoryUIManager._instance.CancelDropHighLight(InventoryUIManager._instance.DroppedItem);

            DraggedItem = -1;
        }


    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        DraggedItem = InventoryUIManager._instance.DraggedItem;
            if (InventoryUIManager._instance.IsHoldingItem && InventoryUIManager._instance.DraggedItem != InventoryUIManager._instance.DroppedItem)
            {
                onDropItem.Invoke();
            }
        
    }
}

