using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Image joystick;
    private Image joystickBG;
    [SerializeField] float joystickOffset;
    public Vector2 inpudDir { set; get; }
    private void Start()
    {
        joystickBG = GetComponent<Image>();
        joystick = transform.GetChild(0).GetComponent<Image>();
        

    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = Vector2.zero;
        float joystickBGx = joystickBG.rectTransform.sizeDelta.x;
        float joystickBGy = joystickBG.rectTransform.sizeDelta.y;
        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBG.rectTransform,eventData.position,eventData.pressEventCamera,out pos))
        {
            pos.x /= joystickBGx;
            pos.y /= joystickBGy;
            inpudDir = new Vector2(pos.x, pos.y);
            
            if (inpudDir.magnitude > 1)
            {
                inpudDir = inpudDir.normalized; 
            }

            joystick.rectTransform.anchoredPosition = new Vector2(inpudDir.x * joystickBGx/joystickOffset, inpudDir.y * joystickBGy/ joystickOffset);


        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inpudDir = Vector2.zero;
        joystick.rectTransform.anchoredPosition = Vector2.zero;
    }


}
