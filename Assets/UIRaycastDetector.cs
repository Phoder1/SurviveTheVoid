using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRaycastDetector : MonoBehaviour
{
    static UIRaycastDetector _instance;
    public static UIRaycastDetector GetInstance => _instance;

   static GraphicRaycaster graphicRaycaster;
  [SerializeField] EventSystem eventSystem;
    static  PointerEventData pointerEventData;
    private void Start()
    {
        graphicRaycaster = GetComponent<GraphicRaycaster>();
     
    }
    private void Awake()
    {
        _instance = this;
    }
    public bool RayCastCheck(Touch touch) {

        //Set up the new Pointer Event
        pointerEventData = new PointerEventData(eventSystem);
        //Set the Pointer Event Position to that of the mouse position
        pointerEventData.position = touch.position;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        graphicRaycaster.Raycast(pointerEventData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray

        if (results.Count > 0)
            return true;
        


        return false;
    }
}
