using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

        for (int i = 0; i < _uiElements.Length; i++)
        {
            if (i != 1)
            {
                _uiElements[i].SetActive(false);
            }
        }
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

	#region PlayerInventoryUI


	#endregion

}


public static class Settings
{
    //Example settings
    public static bool controllerMode;
    public static float volume;
}
