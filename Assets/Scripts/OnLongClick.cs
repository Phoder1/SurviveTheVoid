using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnLongClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool pointerDown;
    private float pointerDownTimer;
    private bool ShortClick;

    public float requiredHoldTime;

    public UnityEvent onLongClick;
    public UnityEvent onShortClick;

    [SerializeField]
    private Image fillImage;

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


        Reset();

    }




    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
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

                if(pointerDownTimer >= (requiredHoldTime / 5))
                fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
            }


        


    }

    void Reset()
    {
        pointerDown = false;
        pointerDownTimer = 0;
        fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
    }

}
