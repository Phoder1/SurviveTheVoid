﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
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
		bRotate,
		stateText;

	[Header("Survival bar's fill")]
	[SerializeField]
	private Image
		hpFill,
		foodFill,
		waterFill,
		airFill,
		sleepFill,
		xpFill;

	public  enum StatType { HP, Food, Water, Air, Sleep, XP }
	private Dictionary<StatType, Image> barsDictionary;
	

	bool CanCollect;

	public override void Init()
	{
		craftingManager = CraftingManager._instance;
		inventoryManager = InventoryUIManager._instance;
		inputManager = InputManager._instance;
		UpdateUiState(InputManager.inputState);

		barsDictionary = new Dictionary<StatType, Image>
		{
			{StatType.HP, hpFill}, {StatType.Food, foodFill}, {StatType.Water, waterFill}, {StatType.Air, airFill}, {StatType.Sleep, sleepFill}, {StatType.XP, xpFill}
		};
	
	}


	private void Update()
	{
		ButtonControls();
		if (Input.GetKeyDown(KeyCode.T))
		{
			ShowTimeAndCollectable(10, 3, 10);
		}
		if (Input.GetKeyDown(KeyCode.Y))
		{
			ShowTimeAndCollectable(10, 0, 10);
		}
		if (Input.GetKeyDown(KeyCode.U))
		{
			ShowTimeAndCollectable(0, 0, 0);
		}

		if (CraftingUI.activeInHierarchy && craftingManager.CurrentProcessTile != null && craftingManager.CurrentProcessTile.IsCrafting)
		{
			ShowTimeAndCollectable(craftingManager.CurrentProcessTile.ItemsCrafted, craftingManager.CurrentProcessTile.amount, craftingManager.CurrentProcessTile.CraftingTimeRemaining);
		}

	}


	#region CraftingUI
	[Header("Crafting UI")]
	[SerializeField] Sprite[] sectionBackGroundSprite;
	[SerializeField] Image[] SectionBackGroundImage;
	[SerializeField] Button CraftingButton;
	[Header("")]
	[SerializeField] GameObject matsHolder;
	[SerializeField] TextMeshProUGUI craftingTimer;
	[SerializeField] GameObject SliderBackGround;
	[SerializeField] TextMeshProUGUI MultipleButt;
	[SerializeField] Slider amountSlider;
	[SerializeField] TextMeshProUGUI amountText;
	[SerializeField] int craftingAmount;
	public int getCraftingAmount => craftingAmount;

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

	//button related




	public void SetButtonToState(ButtonState CraftState, int craftedItem, int AmountRemaining, float timeCraftingRemaining = 0)
	{
		switch (CraftState)
		{
			case ButtonState.CanCraft:
				CanCraftState();
				break;
			case ButtonState.Collect:
				CanCollectState( craftedItem, AmountRemaining,timeCraftingRemaining);
				break;
			case ButtonState.Crafting:
				CraftingState(craftedItem, AmountRemaining,timeCraftingRemaining);
				break;
			default:
				break;
		}
	}


	public void CanCraftState()
	{
		craftingManager.sectionHolder.gameObject.SetActive(true);
		SliderBackGround.SetActive(false);
		SliderBackGround.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Multiple";
		craftingManager.buttonState = ButtonState.CanCraft;
		CraftingButton.interactable = true;
		CraftingButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Craft";
		MultipleButt.text = "Multiple";
		matsHolder.SetActive(true);
		craftingTimer.text = "";
	}

	public void CraftingState( int craftedItem, int AmountRemaining,float timeCraftingRemaining)
	{
		craftingManager.sectionHolder.gameObject.SetActive(false);
		SliderBackGround.SetActive(true);
		MultipleButt.text = "Add";
		SliderBackGround.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Craft: " + craftingAmount;
		craftingManager.buttonState = ButtonState.Crafting;
		CraftingButton.interactable = false;
		matsHolder.SetActive(true);
		craftingTimer.gameObject.SetActive(true);
		CraftingButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = craftedItem + "/" + AmountRemaining;
		craftingTimer.text = Mathf.CeilToInt(timeCraftingRemaining).ToString();
	}
	public void CanCollectState(int craftedItem, int AmountRemaining,float timeCraftingRemaining)
	{
		craftingManager.sectionHolder.gameObject.SetActive(false);
		SliderBackGround.SetActive(true);
		SliderBackGround.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Craft: " + craftingAmount;
		MultipleButt.text = "Add";
		craftingManager.buttonState = ButtonState.Collect;
		CraftingButton.interactable = true;
		matsHolder.SetActive(true);
		craftingTimer.gameObject.SetActive(true);
		CraftingButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = craftedItem + "/" + AmountRemaining;
		craftingTimer.text = Mathf.CeilToInt(timeCraftingRemaining).ToString();
	}



	public void ShowTimeAndCollectable(int craftedItem, int AmountRemaining,float timeCraftingRemaining)
	{
		if (timeCraftingRemaining <= 0)// no time
		{
			if (AmountRemaining <= 0 && craftedItem <= 0)// has no item to pick or any amount to craft
			{
				SetButtonToState(ButtonState.CanCraft, craftedItem, AmountRemaining, timeCraftingRemaining);
			}
			else
			{
				if (craftedItem > 0)// if you still have something to pick up
				{
					SetButtonToState(ButtonState.Collect,  craftedItem, AmountRemaining, timeCraftingRemaining);
				}
			}
		}
		else // have time
		{
			if (craftedItem <= 0)// if you dont have to pick any item
			{
				SetButtonToState(ButtonState.Crafting,  craftedItem, AmountRemaining, timeCraftingRemaining);
			}
			else // if you have to pick any item
			{
				SetButtonToState(ButtonState.Collect, craftedItem, AmountRemaining, timeCraftingRemaining);
			}
		}
	}



	//Slider amount related
	public void OnChangeGetCraftingAmount()
	{
		if(craftingManager.CurrentProcessTile != null)
		{
			if (craftingManager.CurrentProcessTile.amount <= craftingManager.selectedRecipe.getoutcomeItem.item.getmaxStackSize)
			{
				SliderBackGround.SetActive(true);
				if (craftingManager.CurrentProcessTile.IsCrafting)
				{
					amountSlider.maxValue = craftingManager.selectedRecipe.getoutcomeItem.item.getmaxStackSize - craftingManager.CurrentProcessTile.amount;
				}
				else
				{
					amountSlider.maxValue = craftingManager.selectedRecipe.getoutcomeItem.item.getmaxStackSize;
				}
				if(craftingManager.CurrentProcessTile.amount == craftingManager.selectedRecipe.getoutcomeItem.item.getmaxStackSize)
				{
					amountSlider.minValue = 0;
				}
				else
				{
					amountSlider.minValue = 1;
				}
			}
			else
			{
				
				amountSlider.value = 0;
				craftingAmount = 0;
				amountSlider.maxValue = 0;
				amountSlider.minValue = 0;
				SliderBackGround.SetActive(false);
			}
		}
		

		craftingAmount = Mathf.RoundToInt(amountSlider.value);
		amountText.text = "Craft: " + craftingAmount.ToString();
		if (craftingManager.selectedRecipe != null)
		{
			craftingManager.ShowRecipe(CraftingManager._instance.selectedRecipe);
		}
		//CraftingManager._instance.UpdateMatsAmount();
	}



	
	public void SetCraftingUIState(bool IsActive, ProcessorType _type, ProcessingTableTileState tile)
	{
		CraftingUI.SetActive(IsActive);
		craftingManager.GetSetProcessor = _type;
		craftingManager.CurrentProcessTile = tile;
		if (craftingManager.CurrentProcessTile.IsCrafting)
			ShowTimeAndCollectable( tile.ItemsCrafted, tile.amount, tile.CraftingTimeRemaining);

	}
	bool IsCrafting;
	public void ToggleMultiple()
	{
		if(craftingManager.CurrentProcessTile != null)
		{
			if (craftingManager.CurrentProcessTile.IsCrafting)
			{
				

				craftingManager.AddToCraft();
				amountSlider.value = 1;
				OnChangeGetCraftingAmount();
			}
			else
			{
				SliderBackGround.SetActive(!SliderBackGround.activeInHierarchy);
				if (!SliderBackGround.activeInHierarchy)
				{
					amountSlider.value = 1;
					OnChangeGetCraftingAmount();
				}
			}
		}

		

	}

	public void ResetMultiple()
	{
		SliderBackGround.SetActive(false);
		amountSlider.value = 1;
		OnChangeGetCraftingAmount();
	}




	public void CloseCraftingUI()
	{
		ResetMultiple();
		CraftingUI.SetActive(false);
		PlayerManager._instance.Interracted = false;
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
	bool isCraftTableOpen = false;
	bool isChestOpen = false;



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

	void SetTools(bool turnOn)
	{

		bTool1.SetActive(turnOn);
		bTool2.SetActive(turnOn);
		bTool3.SetActive(turnOn);
		bTool4.SetActive(turnOn);
		bTool5.SetActive(turnOn);

	}

	void SetQuickAccessSlots(bool turnOn)
	{

		bQuickAccess1.SetActive(turnOn);
		bQuickAccess2.SetActive(turnOn);
		bQuickAccess3.SetActive(turnOn);
		bQuickAccess4.SetActive(turnOn);
		bQuickAccess5.SetActive(turnOn);
	}


	// Main HUD logic
	public void ButtonHide()
	{
		if (isBuildModeOn == false)
		{
			if (isShown == true)
			{
				bHide.GetComponentInChildren<TextMeshProUGUI>().SetText("SHOW");

				if (isQuickAccessSlotsSwapped == true)
				{
					SetTools(false);
					bMainWeapon.SetActive(false);
					bSwap.SetActive(false);
				}
				else
				{
					SetQuickAccessSlots(false);
					bMainWeapon.SetActive(false);
					bSwap.SetActive(false);
				}

				isShown = false;
			}
			else
			{
				bHide.GetComponentInChildren<TextMeshProUGUI>().SetText("HIDE");

				if (isQuickAccessSlotsSwapped == true)
				{
					SetTools(true);
					bMainWeapon.SetActive(true);
					bSwap.SetActive(true);
				}
				else
				{
					SetQuickAccessSlots(true);
					bMainWeapon.SetActive(true);
					bSwap.SetActive(true);
				}

				isShown = true;
			}
		}
		else
		{
			if (isShownBuildTools == true)
			{
				bHide.GetComponentInChildren<TextMeshProUGUI>().SetText("SHOW");

				bCancel.SetActive(false);
				bRotate.SetActive(false);

				isShownBuildTools = false;
			}
			else
			{
				bHide.GetComponentInChildren<TextMeshProUGUI>().SetText("HIDE");

				bCancel.SetActive(true);
				bRotate.SetActive(true);

				isShownBuildTools = true;
			}
		}
	}

	public void ButtonSwap()
	{
		if (isQuickAccessSlotsSwapped == true)
		{
			SetTools(false);

			SetQuickAccessSlots(true);

			isQuickAccessSlotsSwapped = false;
		}
		else
		{
			SetTools(true);

			SetQuickAccessSlots(false);

			isQuickAccessSlotsSwapped = true;
		}
	}

	public void ButtonInventory()
	{
		if (InventoryUI.activeSelf == true)
		{
			InventoryUI.SetActive(false);

			bInteract.SetActive(true);
			bGather.SetActive(true);

			if (isBuildModeOn == false)
			{
				viFight.SetActive(true);
			}
			else
			{
				viFight.SetActive(false);
			}

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
			SetTools(false);
			bMainWeapon.SetActive(false);
			bSwap.SetActive(false);
			SetQuickAccessSlots(false);
			bInventory.SetActive(false);

			if (isInventoryOpen == true)
			{
				InventoryUI.SetActive(false);
			}
			if (isCraftTableOpen == true)
			{
				CraftingUI.SetActive(false);
			}
			if (isChestOpen == true)
			{
				ChestUI.SetActive(false);
			}
			if (isBuildModeOn == true)
			{
				bCancel.SetActive(false);
				bRotate.SetActive(false);
				stateText.SetActive(false);
			}

			Time.timeScale = 0f;
		}
		else
		{
			Time.timeScale = 1f;

			PauseMenuUI.SetActive(false);

			vjMove.SetActive(true);
			bInteract.SetActive(true);
			bGather.SetActive(true);
			bHide.SetActive(true);
			bInventory.SetActive(true);

			// Check if we was on Items and the tool bar wasn't hidden, also if we wasn't in build mode
			if (isQuickAccessSlotsSwapped == true && isShown != false && isBuildModeOn != true)
			{
				SetTools(true);
				bMainWeapon.SetActive(true);
				bSwap.SetActive(true);
			}
			// Check if we was on QASlots and the tool bar wasn't hidden, also if we wasn't in build mode
			else if (isQuickAccessSlotsSwapped == false && isShown != false && isBuildModeOn != true)
			{
				SetQuickAccessSlots(true);
				bMainWeapon.SetActive(true);
				bSwap.SetActive(true);
			}

			// Check if inventory was open
			if (isInventoryOpen == true)
			{
				InventoryUI.SetActive(true);

				viFight.SetActive(false);
				bInteract.SetActive(false);
				bGather.SetActive(false);
			}

			// Check if we was in build mode
			if (isBuildModeOn == true)
			{
				bCancel.SetActive(true);
				bRotate.SetActive(true);
				stateText.SetActive(true);
			}

			// Check if tools for build mode was hidden
			if(isShownBuildTools == false)
			{
				bCancel.SetActive(false);
				bRotate.SetActive(false);
			}
			
			// Check if we wasn't in build mode and inventory closed to show Visual Icon for Fight mode
			if(isBuildModeOn == false && isInventoryOpen != true)
			{
				viFight.SetActive(true);
			}
		}
	}

	// Build mode logic
	public void BuildModeUI()
	{
		isBuildModeOn = true;
		bHide.GetComponentInChildren<TextMeshProUGUI>().SetText("HIDE");

		if (isQuickAccessSlotsSwapped == true)
		{
			SetTools(false);
		}
		else
		{
			SetQuickAccessSlots(false);
		}
		bMainWeapon.SetActive(false);
		bSwap.SetActive(false);
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

	public void ButtonCancel()
	{
		isBuildModeOn = false;

		if (isQuickAccessSlotsSwapped == true && isShown == true)
		{
			SetTools(true);
			bMainWeapon.SetActive(true);
			bSwap.SetActive(true);
		}
		else if(isQuickAccessSlotsSwapped == false && isShown == true)
		{
			SetQuickAccessSlots(true);
			bMainWeapon.SetActive(true);
			bSwap.SetActive(true);
		}
		viFight.SetActive(true);

		bCancel.SetActive(false);
		bRotate.SetActive(false);

		bInteract.transform.GetChild(0).gameObject.SetActive(true);
		bInteract.transform.GetChild(1).gameObject.SetActive(false);

		if (isFightModeOn == false)
		{
			bGather.transform.GetChild(0).gameObject.SetActive(true);
			bGather.transform.GetChild(2).gameObject.SetActive(false);
		}
		else
		{
			bGather.transform.GetChild(1).gameObject.SetActive(true);
			bGather.transform.GetChild(2).gameObject.SetActive(false);
		}

		if (InventoryUI.activeSelf == true)
		{
			InventoryUI.SetActive(false);
			bInteract.SetActive(true);
			bGather.SetActive(true);

			isInventoryOpen = false;
		}

		if(isShown == false)
		{
			bHide.GetComponentInChildren<TextMeshProUGUI>().SetText("SHOW");
		}

		stateText.SetActive(false);
	}

	public void ButtonRotate()
	{

	}

	// Monitors logic
	public void UpdateSurvivalBar(StatType type, float value)
	{
		barsDictionary[type].fillAmount = Mathf.Clamp(value / PlayerStats._instance.GetSetMaximumAmount, 0, 1);
	}

	#endregion


	#region States
	[Header("States related")]
	[SerializeField] TextMeshProUGUI StateText;

	public void UpdateUiState(InputState CurrentState)
	{
		switch (CurrentState)
		{
			case InputState.DefaultState:
				StateText.gameObject.SetActive(false);
				break;
			case InputState.BuildState:
				StateText.gameObject.SetActive(true);
				StateText.text = "Building State";
				break;
			case InputState.FightState:
				StateText.gameObject.SetActive(true);
				StateText.text = "Fighting State";
				break;
			case InputState.RemovalState:
				StateText.gameObject.SetActive(true);
				StateText.text = "Removal State";
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
