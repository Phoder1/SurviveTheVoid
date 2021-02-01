using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Image background;
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


    bool isVJActive = false;

    public bool GetVJActivity => isVJActive;
    private void Start()
    {
        background = GetComponent<Image>();
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
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isVJActive = true;
        knobOutline.SetActive(true);
        background.color = new Color(169f/255f, 255f/255f, 158f/255f, 70f/255f);
        OnDrag(eventData);

    }
    private void OnDisable() {
        Disable();
    }
    public void OnPointerUp(PointerEventData eventData) {
        Disable();
    }
    private void Disable() {
        isVJActive = false;
        knobOutline.SetActive(false);
        background.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 35f / 255f);
        JoystickVector = Vector2.zero;
        joystickKnob.rectTransform.anchoredPosition = Vector2.zero;
    }
}
