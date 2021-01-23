using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoSingleton<UIManager>
{
    InputManager inputManager;
    CraftingManager craftingManager;
    InventoryUIManager inventoryManager;
    PlayerStats playerStats;

    // UI elements
    [SerializeField]
    private GameObject
        vjMove,
        viFight,
        bInteractA,
        bGatherB,
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
        bRotate,
        stateText,
        xpBar;
    [SerializeField] private TextMeshProUGUI levelNumber;

    [Header("Survival bar's fill")]
    [SerializeField]
    private Image
        hpFill,
        foodFill,
        waterFill,
        airFill,
        sleepFill,
        xpFill;
    private Dictionary<StatType, Image> barsDictionary;


    bool CanCollect;

    public override void Init() {
        playerStats = PlayerStats._instance;
        craftingManager = CraftingManager._instance;
        inventoryManager = InventoryUIManager._instance;
        inputManager = InputManager._instance;
        UpdateUiState(InputManager.inputState);

        barsDictionary = new Dictionary<StatType, Image>
        {
            {StatType.HP, hpFill}, {StatType.Food, foodFill}, {StatType.Water, waterFill}, {StatType.Air, airFill}, {StatType.Sleep, sleepFill}, {StatType.EXP, xpFill}
        };

    }


    private void Update() {
        ButtonControls();


        if (CraftingUI.activeInHierarchy && craftingManager.CurrentProcessTile != null && craftingManager.CurrentProcessTile.IsCrafting && craftingManager.CurrentProcessTile.amount > 0) {
            ShowCraftingTimer(craftingManager.CurrentProcessTile.ItemsCrafted, craftingManager.CurrentProcessTile.amount, craftingManager.CurrentProcessTile.CraftingTimeRemaining);
            //ShowTimeAndCollectable(craftingManager.CurrentProcessTile.ItemsCrafted, craftingManager.CurrentProcessTile.amount, craftingManager.CurrentProcessTile.CraftingTimeRemaining);
        }

    }


    #region CraftingUI
    [Header("Crafting UI")]
    [SerializeField] Sprite[] sectionBackGroundSprite;
    [SerializeField] Image[] SectionBackGroundImage;
    [SerializeField] TextMeshProUGUI CraftingButtonText;
    [Header("")]
    [SerializeField] GameObject matsHolder;
    [SerializeField] TextMeshProUGUI craftingTimer;
    [SerializeField] GameObject SliderBackGround;
    [SerializeField] TextMeshProUGUI MultipleButt;
    [SerializeField] Slider amountSlider;
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] int craftingAmount;
    [SerializeField] Image CurrentRecipeOutSprite;
    [SerializeField] TextMeshProUGUI TimeToCraftText;
    public int getCraftingAmount => craftingAmount;

    public void OnClickSelectedSections(string _section) {
        if (craftingManager.buttonState == ButtonState.CanCraft) {
            craftingManager.SelectSection(_section);
            HighLightSection(_section);
        }
    }

    public void OnClickSelectedRecipe(int _recipe) {
        craftingManager.SelectRecipe(_recipe);
    }

    public void OnClickCraftButton() {
        craftingManager.AttemptToCraft();
        Debug.Log("Short Click");
    }
    public void OnLongClickCraftButton() {
        craftingManager.AttemptToCraftMax();
        Debug.Log("Craft the max you can");
    }

    public void HighLightSection(string _section) {
        for (int i = 0; i < SectionBackGroundImage.Length; i++) {
            if (_section == "Blocks") {
                if (i != 0)
                    SectionBackGroundImage[i].sprite = sectionBackGroundSprite[0];


                SectionBackGroundImage[0].sprite = sectionBackGroundSprite[1];
            }
            else if (_section == "Furnitures") {
                if (i != 1)
                    SectionBackGroundImage[i].sprite = sectionBackGroundSprite[0];

                SectionBackGroundImage[1].sprite = sectionBackGroundSprite[1];
            }
            else if (_section == "Plants") {
                if (i != 2)
                    SectionBackGroundImage[i].sprite = sectionBackGroundSprite[0];

                SectionBackGroundImage[2].sprite = sectionBackGroundSprite[1];
            }
            else if (_section == "Weapons") {
                if (i != 3)
                    SectionBackGroundImage[i].sprite = sectionBackGroundSprite[0];

                SectionBackGroundImage[3].sprite = sectionBackGroundSprite[1];
            }
            else if (_section == "Tools") {
                if (i != 4)
                    SectionBackGroundImage[i].sprite = sectionBackGroundSprite[0];

                SectionBackGroundImage[4].sprite = sectionBackGroundSprite[1];
            }
            else if (_section == "Food") {
                if (i != 5)
                    SectionBackGroundImage[i].sprite = sectionBackGroundSprite[0];

                SectionBackGroundImage[5].sprite = sectionBackGroundSprite[1];
            }
        }

    }

    //button related




    public void SetButtonToState(ButtonState CraftState, int craftedItem, int AmountRemaining, float timeCraftingRemaining = 0) {
        switch (CraftState) {
            case ButtonState.CanCraft:
                craftingManager.buttonState = ButtonState.CanCraft;
                CanCraftState();
                break;
            case ButtonState.Collect:
                craftingManager.buttonState = ButtonState.Collect;
                CanCollectState(craftedItem, AmountRemaining, timeCraftingRemaining);
                break;
            case ButtonState.Crafting:
                craftingManager.buttonState = ButtonState.Crafting;
                CraftingState(craftedItem, AmountRemaining, timeCraftingRemaining);
                break;
            default:
                break;
        }
    }


    public void CanCraftState() {
        //update only when need
        CurrentRecipeOutSprite.gameObject.SetActive(false);
        craftingTimer.gameObject.SetActive(false);
        craftingManager.sectionHolder.gameObject.SetActive(true);
        SliderBackGround.SetActive(false);
        craftingManager.buttonState = ButtonState.CanCraft;
        //CraftingButton.interactable = true;
        CraftingButtonText.text = "Craft";
        MultipleButt.text = "Multiple";
        matsHolder.SetActive(true);
        craftingTimer.text = "";
        Debug.Log("Testing if state is on update can craft");
        CurrentRecipeOutSprite.sprite = null;
    }

    public void CraftingState(int craftedItem, int AmountRemaining, float timeCraftingRemaining) {
        //update when need
        Debug.Log("Testing if state is on update crafting");
        CurrentRecipeOutSprite.gameObject.SetActive(true);
        craftingTimer.gameObject.SetActive(true);
        craftingManager.selectedRecipe = craftingManager.CurrentProcessTile.craftingRecipe;
        craftingManager.sectionHolder.gameObject.SetActive(false);
        SliderBackGround.SetActive(true);
        MultipleButt.text = "Add";
        amountText.text = "Craft: " + craftingAmount;
        craftingManager.buttonState = ButtonState.Crafting;
        //CraftingButton.interactable = false;
        matsHolder.SetActive(true);
        CurrentRecipeOutSprite.sprite = craftingManager.selectedRecipe.getoutcomeItem.item.getsprite;


    }
    public void CanCollectState(int craftedItem, int AmountRemaining, float timeCraftingRemaining) {
        //update when need
        Debug.Log("Testing if state is on update collect");
        craftingTimer.gameObject.SetActive(true);
        craftingManager.selectedRecipe = craftingManager.CurrentProcessTile.craftingRecipe;
        craftingManager.sectionHolder.gameObject.SetActive(false);
        SliderBackGround.SetActive(true);
        amountText.text = "Craft: " + craftingAmount;
        MultipleButt.text = "Add";
        craftingManager.buttonState = ButtonState.Collect;
        //CraftingButton.interactable = true;
        matsHolder.SetActive(true);
        CurrentRecipeOutSprite.sprite = craftingManager.selectedRecipe.getoutcomeItem.item.getsprite;



    }

    void ShowCraftingTimer(int craftedItem, int AmountRemaining, float timeCraftingRemaining) {


        //always update
        CraftingButtonText.text = craftedItem + "/" + AmountRemaining;
        craftingTimer.text = Mathf.CeilToInt(timeCraftingRemaining).ToString();


        ShowTimeAndCollectable(craftedItem, AmountRemaining, timeCraftingRemaining);



    }

    public void ShowTimeAndCollectable(int craftedItem, int AmountRemaining, float timeCraftingRemaining) {
        if (timeCraftingRemaining <= 0)// no time
        {
            //SetButtonToState(ButtonState.CanCraft, craftedItem, AmountRemaining, timeCraftingRemaining);
            if (AmountRemaining <= 0 && craftedItem <= 0)// has no item to pick or any amount to craft
            {
                if (craftingManager.buttonState != ButtonState.CanCraft)
                    SetButtonToState(ButtonState.CanCraft, craftedItem, AmountRemaining, timeCraftingRemaining);
            }
            else {
                if (craftedItem > 0)// if you still have something to pick up
                {
                    if (craftingManager.buttonState != ButtonState.Collect)
                        SetButtonToState(ButtonState.Collect, craftedItem, AmountRemaining, timeCraftingRemaining);
                }
            }
        }
        else // have time
        {
            if (craftedItem <= 0)// if you dont have to pick any item
            {
                if (craftingManager.buttonState != ButtonState.Crafting)
                    SetButtonToState(ButtonState.Crafting, craftedItem, AmountRemaining, timeCraftingRemaining);
            }
            else // if you have to pick any item
            {
                if (craftingManager.buttonState != ButtonState.Collect)
                    SetButtonToState(ButtonState.Collect, craftedItem, AmountRemaining, timeCraftingRemaining);
            }
        }
    }



    //Slider amount related
    public void OnChangeGetCraftingAmount() {

        if (craftingManager.CurrentProcessTile != null && craftingManager.selectedRecipe != null) {
            if (craftingManager.CurrentProcessTile.amount <= craftingManager.selectedRecipe.getoutcomeItem.item.getmaxStackSize) {
                //SliderBackGround.SetActive(true);
                if (craftingManager.CurrentProcessTile.IsCrafting) {
                    amountSlider.maxValue = craftingManager.selectedRecipe.getoutcomeItem.item.getmaxStackSize - craftingManager.CurrentProcessTile.amount;
                }
                else {
                    amountSlider.maxValue = craftingManager.selectedRecipe.getoutcomeItem.item.getmaxStackSize;
                }
                if (craftingManager.CurrentProcessTile.amount == craftingManager.selectedRecipe.getoutcomeItem.item.getmaxStackSize) {
                    if (craftingManager.buttonState != ButtonState.CanCraft)
                        SliderBackGround.SetActive(false);
                    amountSlider.minValue = 0;
                }
                else {
                    amountSlider.minValue = 1;
                    if (craftingManager.buttonState != ButtonState.CanCraft)
                        SliderBackGround.SetActive(true);
                }
            }
            else {
                SliderBackGround.SetActive(false);
                amountSlider.value = 0;
                craftingAmount = 0;
                amountSlider.maxValue = 0;
                amountSlider.minValue = 0;

            }
        }


        craftingAmount = Mathf.RoundToInt(amountSlider.value);
        amountText.text = "Craft: " + craftingAmount.ToString();

        if (craftingManager.selectedRecipe != null) {
            TimeToCraftText.text = "Time to craft: " + craftingManager.selectedRecipe.GetCraftingTime * craftingAmount + " Seconds";
            craftingManager.ShowRecipe(CraftingManager._instance.selectedRecipe);
        }
        //CraftingManager._instance.UpdateMatsAmount();
    }




    public void SetCraftingUIState(bool IsActive, ProcessorType _type, ProcessingTableTileState tile) {
        CraftingScreenUI();
        craftingManager.buttonState = ButtonState.Openining;
        craftingManager.GetSetProcessor = _type;
        craftingManager.CurrentProcessTile = tile;



        ShowTimeAndCollectable(tile.ItemsCrafted, tile.amount, tile.CraftingTimeRemaining);
        if (craftingManager.CurrentProcessTile.IsCrafting) {

        }
        else {
            ResetMultiple();
        }

        if (tile.craftingRecipe != null) {
            craftingManager.SelectSection(tile.craftingRecipe.getSection.ToString());
            craftingManager.selectedRecipe = tile.craftingRecipe;
            craftingManager.ShowRecipe(craftingManager.selectedRecipe);
        }




    }
    bool IsCrafting;
    public void ToggleMultiple() {
        if (craftingManager.CurrentProcessTile != null) {
            if (craftingManager.CurrentProcessTile.IsCrafting) {


                craftingManager.AddToCraft();
                amountSlider.value = 1;
                OnChangeGetCraftingAmount();
            }
            else if (craftingManager.selectedRecipe != null) {
                SliderBackGround.SetActive(!SliderBackGround.activeInHierarchy);
                if (!SliderBackGround.activeInHierarchy) {
                    amountSlider.value = 1;
                    OnChangeGetCraftingAmount();
                }
            }
        }



    }

    public void ResetMultiple() {
        SliderBackGround.SetActive(false);
        amountSlider.value = 1;
        OnChangeGetCraftingAmount();
    }




    public void ButtonCloseCraftingUI() {
        ResetMultiple();
        CloseCraftScreen();
        craftingManager.selectedRecipe = null;
        PlayerManager._instance.MenuClosed();
    }

    #endregion


    #region ButtonsFunctionsAndMonitorsUpdate

    bool isHoldingButton = false, stopHoldingButton = false, isButtonA;
    bool isShown = true;
    bool isShownBuildTools = true;
    bool isQuickAccessSlotsSwapped = true;
    bool isInventoryOpen = false;
    bool isFightModeOn = false;
    bool isBuildModeOn = false;
    bool isRemoveModeOn = false;
    bool isChestOpen = false;



    void ButtonControls() {
        if (isHoldingButton && bInteractA.activeInHierarchy == true) {
            ReleaseButton();
        }
    }
    public void ButtonPressedDown(bool _isButtonA) {
        Debug.Log(_isButtonA);
        this.isButtonA = _isButtonA;
        stopHoldingButton = false;
        isHoldingButton = true;
        inputManager.SinglePressedButton(_isButtonA);

    }
    public void ButtonPressedUp() {


        isHoldingButton = false;
        stopHoldingButton = true;
    }
    void ReleaseButton() {

        isHoldingButton = false;
        if (!stopHoldingButton) {
            PressButton();
        }
    }
    void PressButton() {
        if (bInteractA.activeInHierarchy == true) {
            isHoldingButton = true;

            inputManager.HoldingButton(isButtonA);
        }
    }

    void SetTools(bool turnOn) {

        bTool1.SetActive(turnOn);
        bTool2.SetActive(turnOn);
        bTool3.SetActive(turnOn);
        bTool4.SetActive(turnOn);
        bTool5.SetActive(turnOn);

    }
    void SetQuickAccessSlots(bool turnOn) {

        bQuickAccess1.SetActive(turnOn);
        bQuickAccess2.SetActive(turnOn);
        bQuickAccess3.SetActive(turnOn);
        bQuickAccess4.SetActive(turnOn);
        bQuickAccess5.SetActive(turnOn);
    }


    // Main HUD logic
    public void ButtonHide() {
        if (isBuildModeOn == false && isRemoveModeOn == false) {
            if (isShown == true) {
                bHide.GetComponentInChildren<TextMeshProUGUI>().SetText("SHOW");

                if (isQuickAccessSlotsSwapped == true) {
                    SetTools(false);
                    bMainWeapon.SetActive(false);
                    bSwap.SetActive(false);
                }
                else {
                    SetQuickAccessSlots(false);
                    bMainWeapon.SetActive(false);
                    bSwap.SetActive(false);
                }

                isShown = false;
            }
            else {
                bHide.GetComponentInChildren<TextMeshProUGUI>().SetText("HIDE");

                if (isQuickAccessSlotsSwapped == true) {
                    SetTools(true);
                    bMainWeapon.SetActive(true);
                    bSwap.SetActive(true);
                }
                else {
                    SetQuickAccessSlots(true);
                    bMainWeapon.SetActive(true);
                    bSwap.SetActive(true);
                }

                isShown = true;
            }
        }
        else if (isBuildModeOn == true || isRemoveModeOn == true) {
            if (isShownBuildTools == true) {
                bHide.GetComponentInChildren<TextMeshProUGUI>().SetText("SHOW");

                bCancel.SetActive(false);
                bRotate.SetActive(false);

                isShownBuildTools = false;
            }
            else {
                bHide.GetComponentInChildren<TextMeshProUGUI>().SetText("HIDE");

                bCancel.SetActive(true);
                bRotate.SetActive(true);

                isShownBuildTools = true;
            }
        }
    }

    public void ButtonSwap() {
        if (isQuickAccessSlotsSwapped == true) {
            SetTools(false);

            SetQuickAccessSlots(true);

            isQuickAccessSlotsSwapped = false;
        }
        else {
            SetTools(true);

            SetQuickAccessSlots(false);

            isQuickAccessSlotsSwapped = true;
        }
    }

    public void ButtonInventory() {
        if (InventoryUI.activeSelf == true) {
            InventoryUI.SetActive(false);

            bInteractA.SetActive(true);
            bGatherB.SetActive(true);

            if (isBuildModeOn == false) {
                viFight.SetActive(true);
            }
            else {
                viFight.SetActive(false);
            }

            isInventoryOpen = false;
        }
        else {
            InventoryUI.SetActive(true);

            viFight.SetActive(false);
            bInteractA.SetActive(false);
            bGatherB.SetActive(false);

            inventoryManager.UpdateInventoryToUI();
            isInventoryOpen = true;
        }
    }

    public void ButtonFightTransition() {
        viFight.transform.GetChild(0).gameObject.SetActive(false);
        viFight.transform.GetChild(1).gameObject.SetActive(true);

        bGatherB.transform.GetChild(0).gameObject.SetActive(false);
        bGatherB.transform.GetChild(1).gameObject.SetActive(true);

        isFightModeOn = true;
    }

    public void BottunGatherTransition() {
        viFight.transform.GetChild(0).gameObject.SetActive(true);
        viFight.transform.GetChild(1).gameObject.SetActive(false);

        bGatherB.transform.GetChild(0).gameObject.SetActive(true);
        bGatherB.transform.GetChild(1).gameObject.SetActive(false);

        isFightModeOn = false;
    }

    public void ButtonSettings() {
        if (PauseMenuUI.activeSelf == false) {
            PauseMenuUI.SetActive(true);

            vjMove.SetActive(false);
            viFight.SetActive(false);
            bInteractA.SetActive(false);
            bGatherB.SetActive(false);
            bHide.SetActive(false);
            SetTools(false);
            bMainWeapon.SetActive(false);
            bSwap.SetActive(false);
            SetQuickAccessSlots(false);
            bInventory.SetActive(false);

            if (isInventoryOpen == true) {
                InventoryUI.SetActive(false);
            }
            if (isChestOpen == true) {
                ChestUI.SetActive(false);
            }
            if (isBuildModeOn == true || isRemoveModeOn == true) {
                bCancel.SetActive(false);
                bRotate.SetActive(false);
                stateText.SetActive(false);
            }

            Time.timeScale = 0f;
        }
        else {
            Time.timeScale = 1f;

            PauseMenuUI.SetActive(false);

            vjMove.SetActive(true);
            bGatherB.SetActive(true);
            bHide.SetActive(true);

            // Check if we was in remove mode
            if (isRemoveModeOn == false) {
                bInventory.SetActive(true);
                bInteractA.SetActive(true);
            }

            // Check if we was on Items and the tool bar wasn't hidden, also if we wasn't in build or remove mode
            if (isQuickAccessSlotsSwapped == true && isShown != false && isBuildModeOn != true && isRemoveModeOn != true) {
                SetTools(true);
                bMainWeapon.SetActive(true);
                bSwap.SetActive(true);
            }
            // Check if we was on QASlots and the tool bar wasn't hidden, also if we wasn't in build or remove mode
            else if (isQuickAccessSlotsSwapped == false && isShown != false && isBuildModeOn != true && isRemoveModeOn != true) {
                SetQuickAccessSlots(true);
                bMainWeapon.SetActive(true);
                bSwap.SetActive(true);
            }

            // Check if inventory was open
            if (isInventoryOpen == true) {
                InventoryUI.SetActive(true);

                viFight.SetActive(false);
                bInteractA.SetActive(false);
                bGatherB.SetActive(false);
            }

            // Check if we was in build or remove mode
            if (isBuildModeOn == true || isRemoveModeOn == true) {
                bCancel.SetActive(true);
                bRotate.SetActive(true);
                stateText.SetActive(true);
            }

            // Check if tools for build mode was hidden
            if (isShownBuildTools == false) {
                bCancel.SetActive(false);
                bRotate.SetActive(false);
            }

            // Check if we wasn't in build or remove mode and inventory closed to show Visual Icon for Fight mode
            if (isBuildModeOn == false && isRemoveModeOn == false && isInventoryOpen != true) {
                viFight.SetActive(true);
            }
        }
    }


    // Build mode logic
    public void BuildModeUI() {
        isBuildModeOn = true;
        bHide.GetComponentInChildren<TextMeshProUGUI>().SetText("HIDE");

        if (isQuickAccessSlotsSwapped == true) {
            SetTools(false);
        }
        else {
            SetQuickAccessSlots(false);
        }
        bMainWeapon.SetActive(false);
        bSwap.SetActive(false);
        viFight.SetActive(false);
        InventoryUI.SetActive(false);

        bCancel.SetActive(true);
        bRotate.SetActive(true);

        bInteractA.transform.GetChild(0).gameObject.SetActive(false);
        bInteractA.transform.GetChild(1).gameObject.SetActive(true);

        if (isFightModeOn == false) {
            bGatherB.transform.GetChild(0).gameObject.SetActive(false);
            bGatherB.transform.GetChild(2).gameObject.SetActive(true);
        }
        else {
            bGatherB.transform.GetChild(1).gameObject.SetActive(false);
            bGatherB.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    public void ButtonCancel() {
        if (isBuildModeOn == true) {
            PlayerStateMachine.GetInstance.SwitchState(InputState.DefaultState);
            isBuildModeOn = false;

            if (isQuickAccessSlotsSwapped == true && isShown == true) {
                SetTools(true);
                bMainWeapon.SetActive(true);
                bSwap.SetActive(true);
            }
            else if (isQuickAccessSlotsSwapped == false && isShown == true) {
                SetQuickAccessSlots(true);
                bMainWeapon.SetActive(true);
                bSwap.SetActive(true);
            }
            viFight.SetActive(true);

            bCancel.SetActive(false);
            bRotate.SetActive(false);

            bInteractA.transform.GetChild(0).gameObject.SetActive(true);
            bInteractA.transform.GetChild(1).gameObject.SetActive(false);

            if (isFightModeOn == false) {
                bGatherB.transform.GetChild(0).gameObject.SetActive(true);
                bGatherB.transform.GetChild(2).gameObject.SetActive(false);
            }
            else {
                bGatherB.transform.GetChild(1).gameObject.SetActive(true);
                bGatherB.transform.GetChild(2).gameObject.SetActive(false);
            }

            if (InventoryUI.activeSelf == true) {
                InventoryUI.SetActive(false);
                bInteractA.SetActive(true);
                bGatherB.SetActive(true);

                isInventoryOpen = false;
            }

            if (isShown == false) {
                bHide.GetComponentInChildren<TextMeshProUGUI>().SetText("SHOW");
            }

            stateText.SetActive(false);
        }
        else if (isRemoveModeOn == true) {
            isRemoveModeOn = false;
            isBuildModeOn = true;

            bInteractA.SetActive(true);
            bInventory.SetActive(true);


            PlayerStateMachine.GetInstance.SwitchState(InputState.BuildState);
        }
    }

    public void ButtonRotate() {

    }

    public void RemoveStateOn() {
        isBuildModeOn = false;
        isRemoveModeOn = true;

        bInteractA.SetActive(false);
        bInventory.SetActive(false);
    }


    // Crafting screen logic
    public void CraftingScreenUI() {
        CraftingUI.SetActive(true);

        if (isQuickAccessSlotsSwapped == true && isShown != false) {
            SetTools(false);
            bMainWeapon.SetActive(false);
            bSwap.SetActive(false);
        }
        else if (isQuickAccessSlotsSwapped == false && isShown != false) {
            SetQuickAccessSlots(false);
            bMainWeapon.SetActive(false);
            bSwap.SetActive(false);
        }

        bHide.SetActive(false);
        bInventory.SetActive(false);
        bSettings.SetActive(false);
        vjMove.SetActive(false);
        bInteractA.SetActive(false);
        ButtonPressedUp();
        bGatherB.SetActive(false);
        viFight.SetActive(false);
        xpBar.SetActive(false);
    }

    public void CloseCraftScreen() {
        CraftingUI.SetActive(false);

        if (isQuickAccessSlotsSwapped == true && isShown != false) {
            SetTools(true);
            bMainWeapon.SetActive(true);
            bSwap.SetActive(true);
        }
        else if (isQuickAccessSlotsSwapped == false && isShown != false) {
            SetQuickAccessSlots(true);
            bMainWeapon.SetActive(true);
            bSwap.SetActive(true);
        }

        bHide.SetActive(true);
        bInventory.SetActive(true);
        bSettings.SetActive(true);
        vjMove.SetActive(true);
        bInteractA.SetActive(true);
        bGatherB.SetActive(true);
        viFight.SetActive(true);
        xpBar.SetActive(true);
    }


    // Monitors logic
    public void UpdateSurvivalBar(Stat stat, float value) {
        if (barsDictionary.TryGetValue(stat.statType, out Image image) && stat.GetIsCapped)
            image.fillAmount = Mathf.Clamp(value / stat.maxStat.GetSetValue, 0, 1);
    }

    public void UpdateExpAndLvlBar() {
        xpFill.fillAmount = Mathf.Clamp(playerStats.GetStat(StatType.EXP).GetSetValue / playerStats.GetStat(StatType.EXPtoNextLevel).GetSetValue, 0, 1);
        levelNumber.SetText(Mathf.RoundToInt(playerStats.GetStat(StatType.Level).GetSetValue).ToString());
    }

    #endregion


    #region States
    [Header("States related")]
    [SerializeField] TextMeshProUGUI StateText;

    public void UpdateUiState(InputState CurrentState) {
        switch (CurrentState) {
            case InputState.DefaultState:
                StateText.gameObject.SetActive(false);
                break;
            case InputState.BuildState:
                StateText.gameObject.SetActive(true);
                StateText.text = "Building State";
                BuildModeUI();
                break;
            case InputState.FightState:
                StateText.gameObject.SetActive(true);
                StateText.text = "Fighting State";
                break;
            case InputState.RemovalState:
                StateText.gameObject.SetActive(true);
                StateText.text = "Removal State";
                RemoveStateOn();
                break;
            default:
                break;
        }
    }
    //IEnumerator BlinkTextState()
    //{
    //    StateText.color = Color.red;

    //    StateText.color = Color.white;
    //}


    #endregion
}


public static class Settings
{
    //Example settings
    public static bool controllerMode;
    public static float volume;
}
