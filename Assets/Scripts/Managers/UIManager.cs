using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIManager : MonoBehaviour 
{
    public static UIManager _instance;
    InputManager _inputManager;
    public VirtualButton[] _buttons;
    public VirtualJoystick vJ;

    // UI elements
    public GameObject[] _uiElements;

    private void Awake() {
        if (_instance != null) 
        {
            Destroy(gameObject);    
        }
        else 
        {
            _instance = this;
        }
    }
    void Start()
    {
        _inputManager = InputManager._instance;

        for(int i =0; i<_uiElements.Length; i++)
		{
            if(i != 1)
			{
                _uiElements[i].SetActive(false);
			}
		}
    }

    private void Update()
    {
        _inputManager.ButtonCheck(_buttons);
        _inputManager.OnClicked("sdas",vJ);
    }
    
    


}


public static class Settings
{
    //Example settings
    public static bool controllerMode;
    public static float volume;
}
