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

    // Canvases
    public GameObject[] _canvases;

    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
           
        }
        else {
         
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        _inputManager = InputManager._instance;

        for(int i =0; i<_canvases.Length; i++)
		{
            if(i != 1)
			{
                _canvases[i].SetActive(false);
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
