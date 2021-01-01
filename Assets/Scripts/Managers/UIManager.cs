using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager _instance;
    InputManager _inputManager;
    CraftingManager craftingManager;
    InventoryManager inventoryManager;
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
        inventoryManager = InventoryManager._instance;
        _inputManager = InputManager._instance;
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

            if(isQuickAccessSwapped == true)
			{
                for (int i = 6; i <= 12; i++)
                {
                    _uiElements[i].SetActive(false);
                }
            }
			else
			{
                for (int i = 15; i <= 19; i++)
                {
                    _uiElements[i].SetActive(false);
                }
                for (int i = 11; i <= 12; i++)
                {
                    _uiElements[i].SetActive(false);
                }
            }

            isShown = false;
        }
		else
		{
            _uiElements[5].gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("HIDE");

            if (isQuickAccessSwapped == true)
            {
                for (int i = 6; i <= 12; i++)
                {
                    _uiElements[i].SetActive(true);
                }
            }
            else
            {
                for (int i = 15; i <= 19; i++)
                {
                    _uiElements[i].SetActive(true);
                }
                for (int i = 11; i <= 12; i++)
                {
                    _uiElements[i].SetActive(true);
                }
            }

            isShown = true;
        }    
	}

    bool isQuickAccessSwapped = true;
    public void ButtonSwap()
	{
        if (isQuickAccessSwapped == true)
		{
            for(int i = 6; i<=10; i++)
			{
                _uiElements[i].SetActive(false);
            }
            for(int i = 15; i<=19; i++)
			{
                _uiElements[i].SetActive(true);
            }

            isQuickAccessSwapped = false;
		}
		else
		{
            for (int i = 6; i<=10; i++)
            {
                _uiElements[i].SetActive(true);
            }
            for (int i = 15; i<=19; i++)
            {
                _uiElements[i].SetActive(false);
            }

            isQuickAccessSwapped = true;
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
