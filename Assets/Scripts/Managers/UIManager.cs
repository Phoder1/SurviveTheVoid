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

        for (int i = 0; i < _uiElements.Length; i++)
        {
            if (i != 1)
            {
                _uiElements[i].SetActive(false);
            }
        }
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





}


public static class Settings
{
    //Example settings
    public static bool controllerMode;
    public static float volume;
}
