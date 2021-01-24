
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnLongClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    public int ChestId;
    public int Slot;
    [SerializeField]
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
    [SerializeField]
    bool IsDragged;
    [SerializeField]
    DragNDropVisual Vis;

    private void Awake()
    {
        //DragVisual = this.GetComponent<DragNDropVisual>();
    }

    public void OnPointerDown(PointerEventData evenData)
    {
        pointerDown = true;

        //InventoryUIManager._instance.DraggedItem = Slot;




    }

    public void OnPointerUp(PointerEventData evendata)
    {

        if (pointerDownTimer < requiredHoldTime && pointerDown && onShortClick != null && !IsDragged)
        {
            onShortClick.Invoke();
        }
       
        Reset();

    }


    void Update()
    {
        if (pointerDown)
        {
            pointerDownTimer += Time.deltaTime;
            if (pointerDownTimer >= requiredHoldTime)
            {
                if (onLongClick != null)
                    onLongClick.Invoke();

                Reset();
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
       
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       


    }
}

