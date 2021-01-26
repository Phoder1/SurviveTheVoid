using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Image joystickKnob;
    private RectTransform rectTransform;
    [SerializeField] GameObject knobOutline;
    [SerializeField] float joystickOffset;
    [Range(0,1)]
    [SerializeField] float minimumLength;
    public Vector2 JoystickVector { set; get; }
    private Vector2 JoystickPosition => transform.position;
    private Vector2 JoystickSize => rectTransform.sizeDelta * rectTransform.localScale * mainCanvas.scaleFactor;
    [SerializeField] Canvas mainCanvas;
    private void Start()
    {
        joystickKnob = transform.GetChild(0).GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        



    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 size = JoystickSize;
        Vector2 tempVector = eventData.position - JoystickPosition;
        
        tempVector = Vector2.ClampMagnitude(tempVector, size.x / 2);
        if (tempVector.magnitude <= minimumLength * (size.x / 2))
            tempVector = Vector2.zero;
        joystickKnob.rectTransform.position = JoystickPosition + tempVector;
        tempVector = tempVector / (size.x / 2);
        JoystickVector = tempVector;

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
        knobOutline.SetActive(true);
        OnDrag(eventData);

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        knobOutline.SetActive(false);
        JoystickVector = Vector2.zero;
        joystickKnob.rectTransform.anchoredPosition = Vector2.zero;
    }


}
