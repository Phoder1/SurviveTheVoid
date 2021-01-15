using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoSingleton<UIManager>
{
    InputManager inputManager;
    CraftingManager craftingManager;
    InventoryUIManager inventoryManager;
    // UI elements
    [SerializeField]
    private GameObject
        vjMove,
        viFight,
        bInteract,
        bGather,
        bHide,
        bTool1,
        bTool2,
        bTool3,
        bTool4,
        bTool5,
        bMainWeapon,
        bSwap,
        bQuickAccess1,
        bQuickAccess2,
        bQuickAccess3,
        bQuickAccess4,
        bQuickAccess5,
        bInventory,
        bSettings,
        InventoryUI,
        CraftingUI,
        PauseMenuUI,
        ChestUI,
        bCancel,
        bRotate;


    public override void Init()
    {
        craftingManager = CraftingManager._instance;
        inventoryManager = InventoryUIManager._instance;
        inputManager = InputManager._instance;
    }


    private void Update()
    {
        ButtonControls();
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




    public void ToggleCraftingUI(ProcessorType _type)
    {
        CraftingUI.SetActive(!CraftingUI.activeInHierarchy);
        craftingManager.GetSetProcessor = _type;
    }

    #endregion


    #region ButtonsFunctions

    bool isHoldingButton = false, stopHoldingButton = false, isButtonA;
    bool isShown = true;
    bool isQuickAccessSwapped = true;
    bool isInventoryOpen = false;
    bool isFightModeOn = false;

    

    void ButtonControls()
    {
        if (isHoldingButton)
        {
     
            ReleaseButton();
        }
    }
    public void ButtonPressedDown(bool _isButtonA)
    {
        Debug.Log(_isButtonA);
        this.isButtonA = _isButtonA;
        stopHoldingButton = false;
        isHoldingButton = true;
        inputManager.SinglePressedButton(_isButtonA);
    
    }
    public void ButtonPressedUp()
    {
     

        isHoldingButton = false;
        stopHoldingButton = true;
    }
    void ReleaseButton()
    {
    
        isHoldingButton = false;
        if (!stopHoldingButton)
        {
            PressButton();
        }
    }
    void PressButton()
    {
        isHoldingButton = true;
     
        inputManager.HoldingButton(isButtonA);
    }



    public void ButtonHide()
    {
       
        if (isShown == true)
        {
            bHide.GetComponentInChildren<TextMeshProUGUI>().SetText("SHOW");

            if (isQuickAccessSwapped == true)
            {
                bTool1.SetActive(false);
                bTool2.SetActive(false);
                bTool3.SetActive(false);
                bTool4.SetActive(false);
                bTool5.SetActive(false);
                bMainWeapon.SetActive(false);
                bSwap.SetActive(false);
            }
            else
            {
                bQuickAccess1.SetActive(false);
                bQuickAccess2.SetActive(false);
                bQuickAccess3.SetActive(false);
                bQuickAccess4.SetActive(false);
                bQuickAccess5.SetActive(false);
                bMainWeapon.SetActive(false);
                bSwap.SetActive(false);
            }

            isShown = false;
        }
        else
        {
            bHide.GetComponentInChildren<TextMeshProUGUI>().SetText("HIDE");

            if (isQuickAccessSwapped == true)
            {
                bTool1.SetActive(true);
                bTool2.SetActive(true);
                bTool3.SetActive(true);
                bTool4.SetActive(true);
                bTool5.SetActive(true);
                bMainWeapon.SetActive(true);
                bSwap.SetActive(true);
            }
            else
            {
                bQuickAccess1.SetActive(true);
                bQuickAccess2.SetActive(true);
                bQuickAccess3.SetActive(true);
                bQuickAccess4.SetActive(true);
                bQuickAccess5.SetActive(true);
                bMainWeapon.SetActive(true);
                bSwap.SetActive(true);
            }

            isShown = true;
        }
    }

    public void ButtonSwap()
    {
        if (isQuickAccessSwapped == true)
        {
            bTool1.SetActive(false);
            bTool2.SetActive(false);
            bTool3.SetActive(false);
            bTool4.SetActive(false);
            bTool5.SetActive(false);

            bQuickAccess1.SetActive(true);
            bQuickAccess2.SetActive(true);
            bQuickAccess3.SetActive(true);
            bQuickAccess4.SetActive(true);
            bQuickAccess5.SetActive(true);

            isQuickAccessSwapped = false;
        }
        else
        {
            bTool1.SetActive(true);
            bTool2.SetActive(true);
            bTool3.SetActive(true);
            bTool4.SetActive(true);
            bTool5.SetActive(true);

            bQuickAccess1.SetActive(false);
            bQuickAccess2.SetActive(false);
            bQuickAccess3.SetActive(false);
            bQuickAccess4.SetActive(false);
            bQuickAccess5.SetActive(false);

            isQuickAccessSwapped = true;
        }
    }

    public void ButtonInventory()
    {
        if (InventoryUI.activeSelf == true)
        {
            InventoryUI.SetActive(false);

            viFight.SetActive(true);
            bInteract.SetActive(true);
            bGather.SetActive(true);

            isInventoryOpen = false;
        }
        else
        {
            InventoryUI.SetActive(true);

            viFight.SetActive(false);
            bInteract.SetActive(false);
            bGather.SetActive(false);

            inventoryManager.UpdateInventoryToUI();
			isInventoryOpen = true;
        }
    }

    public void ButtonFightTransition()
    {
        viFight.transform.GetChild(0).gameObject.SetActive(false);
        viFight.transform.GetChild(1).gameObject.SetActive(true);

        bGather.transform.GetChild(0).gameObject.SetActive(false);
        bGather.transform.GetChild(1).gameObject.SetActive(true);

        isFightModeOn = true;
    }

    public void BottunGatherTransition()
    {
        viFight.transform.GetChild(0).gameObject.SetActive(true);
        viFight.transform.GetChild(1).gameObject.SetActive(false);

        bGather.transform.GetChild(0).gameObject.SetActive(true);
        bGather.transform.GetChild(1).gameObject.SetActive(false);

        isFightModeOn = false;
    }

    public void ButtonSettings()
    {
        if (PauseMenuUI.activeSelf == false)
        {
            PauseMenuUI.SetActive(true);

            vjMove.SetActive(false);
            viFight.SetActive(false);
            bInteract.SetActive(false);
            bGather.SetActive(false);
            bHide.SetActive(false);
            bTool1.SetActive(false);
            bTool2.SetActive(false);
            bTool3.SetActive(false);
            bTool4.SetActive(false);
            bTool5.SetActive(false);
            bMainWeapon.SetActive(false);
            bSwap.SetActive(false);
            bQuickAccess1.SetActive(false);
            bQuickAccess2.SetActive(false);
            bQuickAccess3.SetActive(false);
            bQuickAccess4.SetActive(false);
            bQuickAccess5.SetActive(false);
            bInventory.SetActive(false);
            InventoryUI.SetActive(false);
            CraftingUI.SetActive(false);
            ChestUI.SetActive(false);

            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;

            PauseMenuUI.SetActive(false);

            vjMove.SetActive(true);
            viFight.SetActive(true);
            bInteract.SetActive(true);
            bGather.SetActive(true);
            bHide.SetActive(true);
            bTool1.SetActive(true);
            bTool2.SetActive(true);
            bTool3.SetActive(true);
            bTool4.SetActive(true);
            bTool5.SetActive(true);
            bMainWeapon.SetActive(true);
            bSwap.SetActive(true);
            bInventory.SetActive(true);

            if (isInventoryOpen == true)
            {
                InventoryUI.SetActive(true);

                viFight.SetActive(false);
                bInteract.SetActive(false);
                bGather.SetActive(false);
            }

            if(isShown == false)
			{
                if (isQuickAccessSwapped == true)
                {
                    bTool1.SetActive(false);
                    bTool2.SetActive(false);
                    bTool3.SetActive(false);
                    bTool4.SetActive(false);
                    bTool5.SetActive(false);
                    bMainWeapon.SetActive(false);
                    bSwap.SetActive(false);
                }
                else
                {
                    bQuickAccess1.SetActive(false);
                    bQuickAccess2.SetActive(false);
                    bQuickAccess3.SetActive(false);
                    bQuickAccess4.SetActive(false);
                    bQuickAccess5.SetActive(false);
                    bMainWeapon.SetActive(false);
                    bSwap.SetActive(false);
                }
            }

            if (isQuickAccessSwapped == false)
            {
                bTool1.SetActive(false);
                bTool2.SetActive(false);
                bTool3.SetActive(false);
                bTool4.SetActive(false);
                bTool5.SetActive(false);

                bQuickAccess1.SetActive(true);
                bQuickAccess2.SetActive(true);
                bQuickAccess3.SetActive(true);
                bQuickAccess4.SetActive(true);
                bQuickAccess5.SetActive(true);
            }
            else
            {
                bTool1.SetActive(true);
                bTool2.SetActive(true);
                bTool3.SetActive(true);
                bTool4.SetActive(true);
                bTool5.SetActive(true);

                bQuickAccess1.SetActive(false);
                bQuickAccess2.SetActive(false);
                bQuickAccess3.SetActive(false);
                bQuickAccess4.SetActive(false);
                bQuickAccess5.SetActive(false);
            }

        }
    }

    public void BuildModeUI()
	{
        if (isQuickAccessSwapped == true)
        {
            bTool1.SetActive(false);
            bTool2.SetActive(false);
            bTool3.SetActive(false);
            bTool4.SetActive(false);
            bTool5.SetActive(false);
            bMainWeapon.SetActive(false);
            bSwap.SetActive(false);
        }
        else
        {
            bQuickAccess1.SetActive(false);
            bQuickAccess2.SetActive(false);
            bQuickAccess3.SetActive(false);
            bQuickAccess4.SetActive(false);
            bQuickAccess5.SetActive(false);
            bMainWeapon.SetActive(false);
            bSwap.SetActive(false);
        }
        bHide.SetActive(false);
        viFight.SetActive(false);
        InventoryUI.SetActive(false);

        bCancel.SetActive(true);
        bRotate.SetActive(true);

        bInteract.transform.GetChild(0).gameObject.SetActive(false);
        bInteract.transform.GetChild(1).gameObject.SetActive(true);

        if (isFightModeOn == false)
        {
            bGather.transform.GetChild(0).gameObject.SetActive(false);
            bGather.transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            bGather.transform.GetChild(1).gameObject.SetActive(false);
            bGather.transform.GetChild(2).gameObject.SetActive(true);
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
