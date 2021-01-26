using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]Canvas SettingMenu;

    public void playButton()
    { 
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void SettingButton()
    {
        SettingMenu.enabled = true;
    }
    public void CloseSettings()
    {
        SettingMenu.enabled = false;
    }
}
