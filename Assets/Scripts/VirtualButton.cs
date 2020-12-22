using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualButton : MonoBehaviour ,IPointerDownHandler,IPointerUpHandler
{
    internal bool IsPressed;
    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
       
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
       
    }

 
}
