using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UImanager : MonoBehaviour 
{
    public static UImanager _instance;
    InputManager _inputManager;
    public VirtualButton[] _buttons;

    //UI Canvases
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

        for(int i=1; i<_canvases.Length; i++)
		{
            _canvases[i].SetActive(false);
		}
    }
    private void Update()
    {
        _inputManager.ButtonCheck(_buttons);





    }
    


}


public static class Settings
{
    //Example settings
    public static bool controllerMode;
    public static float volume;
}
