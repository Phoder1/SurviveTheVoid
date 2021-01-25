using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Image joystickKnob;
    private Image joystickBG;
    private RectTransform rectTransform;
    [SerializeField] float joystickOffset;
    [Range(0,1)]
    [SerializeField] float minimumLength;
    public Vector2 joystickVector { set; get; }
    private Vector2 joystickPosition => transform.position;
    private Vector2 joystickSize => rectTransform.sizeDelta * rectTransform.localScale * mainCanvas.scaleFactor;
    [SerializeField] Canvas mainCanvas;
    private void Start()
    {
        joystickBG = GetComponent<Image>();
        joystickKnob = transform.GetChild(0).GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        



    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 size = joystickSize;
        Vector2 tempVector = eventData.position - joystickPosition;
        Debug.Log("JS size: " + size);
        
        tempVector = Vector2.ClampMagnitude(tempVector, size.x / 2);
        if (tempVector.magnitude <= minimumLength * (size.x / 2))
            tempVector = Vector2.zero;
        joystickKnob.rectTransform.position = joystickPosition + tempVector;
        tempVector = tempVector / (size.x / 2);
        Debug.Log("JS pos: " + joystickPosition + ", event pos: " + eventData.position + ", vector: " + tempVector);
        joystickVector = tempVector;

        //float joystickBGx = joystickBG.rectTransform.sizeDelta.x;
        //float joystickBGy = joystickBG.rectTransform.sizeDelta.y;

        //Vector2 pos;
        //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBG.rectTransform, eventData.position, eventData.pressEventCamera, out pos)) {
        //    //pos.x /= joystickBGx;
        //    //pos.y /= joystickBGy;
        //    inpudDir = new Vector2(pos.x, pos.y);

        //    if (inpudDir.magnitude > 1) {
        //        inpudDir = inpudDir.normalized;
        //    }

        //    joystickKnob.rectTransform.anchoredPosition = new Vector2(inpudDir.x * joystickBGx / joystickOffset, inpudDir.y * joystickBGy / joystickOffset);


        //}
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickVector = Vector2.zero;
        joystickKnob.rectTransform.anchoredPosition = Vector2.zero;
    }


}
