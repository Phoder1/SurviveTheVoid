using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager _instance;
    InputManager _inputManager;
    public VirtualButton[] _buttons;
    public VirtualJoystick vJ;
    CraftingManager craftingManager;
    // UI elements
    public GameObject[] _uiElements;

    private void Awake()
    {
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
        craftingManager = CraftingManager._instance;
        _inputManager = InputManager._instance;
    }

    private void Update()
    {
        _inputManager.ButtonCheck(_buttons);
        _inputManager.OnClicked("sdas", vJ);
    }


    #region CraftingUI
    public void OnClickSelectedSections(string _section)
    {
        craftingManager.SelectSection(_section);
    }

    public void OnClickSelectedRecipe(int _recipe)
    {
        craftingManager.SelectRecipe(_recipe);
    }

    public void OnClickCraftButton()
    {
        craftingManager.AttemptToCraft();
    }

    #endregion


    #region ButtonsFunctions

    bool isShown = true;

    public void ButtonHide()
	{
        if(isShown == true)
		{
            _uiElements[5].gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("SHOW");

            for (int i = 6; i <= 12; i++)
            {
                _uiElements[i].SetActive(false);
            }

            isShown = false;
        }
		else
		{
            _uiElements[5].gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("HIDE");

            for (int i = 6; i <= 12; i++)
            {
                _uiElements[i].SetActive(true);
            }

            isShown = true;
        }    
	}




    #endregion

}


public static class Settings
{
    //Example settings
    public static bool controllerMode;
    public static float volume;
}
