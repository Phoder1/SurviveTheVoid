using System;
using UnityEngine;

public class UImanager : MonoBehaviour
{
    public UImanager _instance;
    private void Awake() {
        if (_instance == null) {
            Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
        else {
            _instance = this;
        }
    }


}

public static class Settings
{
    //Example settings
    public static bool controllerMode;
    public static float volume;
}
