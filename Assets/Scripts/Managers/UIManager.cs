using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoSingleton<UIManager>
{
    InputManager inputManager;
    CraftingManager craftingManager;
    InventoryUIManager inventoryManager;
    // UI elements
    public GameObject[] _uiElements;


    public override void Init()
    {
        craftingManager = CraftingManager._instance;
        inventoryManager = InventoryUIManager._instance;
        inputManager = InputManager._instance;
    }

    #region CraftingUI
    [Header("Crafting UI")]
    [SerializeField] Sprite[] sectionBackGroundSprite;
    [SerializeField] Image[] SectionBackGroundImage;
    public void OnClickSelectedSections(string _section)
    {
        craftingManager.SelectSection(_section);
        HighLightSection(_section);
    }

    public void OnClickSelectedRecipe(int _recipe)
    {
        craftingManager.SelectRecipe(_recipe);
    }

    public void OnClickCraftButton()
    {
        craftingManager.AttemptToCraft();
    }

    public void HighLightSection(string _section)
    {
        for (int i = 0; i < SectionBackGroundImage.Length; i++)
        {
            if (_section == "Blocks")
            {
                if (i != 0)
                    SectionBackGroundImage[i].sprite = sectionBackGroundSprite[0];


                SectionBackGroundImage[0].sprite = sectionBackGroundSprite[1];
            }
            else if (_section == "Furnitures")
            {
                if (i != 1)
                    SectionBackGroundImage[i].sprite = sectionBackGroundSprite[0];

                SectionBackGroundImage[1].sprite = sectionBackGroundSprite[1];
            }
            else if (_section == "Plants")
            {
                if (i != 2)
                    SectionBackGroundImage[i].sprite = sectionBackGroundSprite[0];

                SectionBackGroundImage[2].sprite = sectionBackGroundSprite[1];
            }
            else if (_section == "Weapons")
            {
                if (i != 3)
                    SectionBackGroundImage[i].sprite = sectionBackGroundSprite[0];

                SectionBackGroundImage[3].sprite = sectionBackGroundSprite[1];
            }
            else if (_section == "Tools")
            {
                if (i != 4)
                    SectionBackGroundImage[i].sprite = sectionBackGroundSprite[0];

                SectionBackGroundImage[4].sprite = sectionBackGroundSprite[1];
            }
            else if (_section == "Food")
            {
                if (i != 5)
                    SectionBackGroundImage[i].sprite = sectionBackGroundSprite[0];

                SectionBackGroundImage[5].sprite = sectionBackGroundSprite[1];
            }
        }

    }

    public void ToggleCraftingUI()
    {
        _uiElements[4].SetActive(!_uiElements[4].activeInHierarchy);
        if (_uiElements[4].activeInHierarchy)
        {
            //craftingManager.clearRecipeMat();
            craftingManager.UpdateInformation();
            craftingManager.SelectSection("Blocks");
            HighLightSection("Blocks");
        }
    }

    #endregion


    #region ButtonsFunctions

    bool isShown = true;
    bool isQuickAccessSwapped = true;
    bool isInventoryOpen = false;

    public void ButtonA()
    {
        inputManager.PressButtonA();
    }

    public void ButtonB()
    {
        inputManager.PressButtonB();
    }

    public void ButtonHide()
    {
        if (isShown == true)
        {
            _uiElements[5].gameObject.GetComponentInChildren<TextMeshProUGUI>().SetText("SHOW");

            if (isQuickAccessSwapped == true)
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

    public void ButtonSwap()
    {
        if (isQuickAccessSwapped == true)
        {
            for (int i = 6; i <= 10; i++)
            {
                _uiElements[i].SetActive(false);
            }
            for (int i = 15; i <= 19; i++)
            {
                _uiElements[i].SetActive(true);
            }

            isQuickAccessSwapped = false;
        }
        else
        {
            for (int i = 6; i <= 10; i++)
            {
                _uiElements[i].SetActive(true);
            }
            for (int i = 15; i <= 19; i++)
            {
                _uiElements[i].SetActive(false);
            }

            isQuickAccessSwapped = true;
        }
    }

    public void ButtonInventory()
    {
        if (_uiElements[20].activeSelf == true)
        {
            _uiElements[20].SetActive(false);

            for (int i = 1; i <= 3; i++)
            {
                _uiElements[i].SetActive(true);
            }

            isInventoryOpen = false;
        }
        else
        {
            _uiElements[20].SetActive(true);

            for (int i = 1; i <= 3; i++)
            {
                _uiElements[i].SetActive(false);
            }

            isInventoryOpen = true;
        }
    }

    public void ButtonFightTransition()
    {
        _uiElements[1].transform.GetChild(0).gameObject.SetActive(false);
        _uiElements[1].transform.GetChild(1).gameObject.SetActive(true);

        _uiElements[3].transform.GetChild(0).gameObject.SetActive(false);
        _uiElements[3].transform.GetChild(1).gameObject.SetActive(true);
    }

    public void BottunGatherTransition()
    {
        _uiElements[1].transform.GetChild(0).gameObject.SetActive(true);
        _uiElements[1].transform.GetChild(1).gameObject.SetActive(false);

        _uiElements[3].transform.GetChild(0).gameObject.SetActive(true);
        _uiElements[3].transform.GetChild(1).gameObject.SetActive(false);
    }

    public void ButtonSettings()
    {
        if (_uiElements[21].activeSelf == false)
        {
            _uiElements[21].SetActive(true);

            for (int i = 0; i <= 20; i++)
            {
                if (i == 13)
                {
                    _uiElements[i].SetActive(true);
                }
                else
                {
                    _uiElements[i].SetActive(false);
                }
            }

            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;

            for (int i = 0; i <= 20; i++)
            {
                if (i == 20 || i == 4 || i == 15 || i == 16 || i == 17 || i == 18 || i == 19)
                {
                    _uiElements[i].SetActive(false);
                }
                else
                {
                    _uiElements[i].SetActive(true);
                }
            }

            _uiElements[21].SetActive(false);

            if (isInventoryOpen == true)
            {
                _uiElements[20].SetActive(true);

                for (int i = 1; i <= 3; i++)
                {
                    _uiElements[i].SetActive(false);
                }
            }

            if (isQuickAccessSwapped == false)
            {
                for (int i = 6; i <= 10; i++)
                {
                    _uiElements[i].SetActive(false);
                }
                for (int i = 15; i <= 19; i++)
                {
                    _uiElements[i].SetActive(true);
                }
            }
            else
            {
                for (int i = 6; i <= 10; i++)
                {
                    _uiElements[i].SetActive(true);
                }
                for (int i = 15; i <= 19; i++)
                {
                    _uiElements[i].SetActive(false);
                }
            }
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
