using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Tilemaps;
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
		bRotate,
		stateText;
	bool CanCollect;

	public override void Init()
	{
		craftingManager = CraftingManager._instance;
		inventoryManager = InventoryUIManager._instance;
		inputManager = InputManager._instance;
		UpdateUiState(InputManager.inputState);
	}


	private void Update()
	{
		ButtonControls();

		if (Input.GetKeyDown(KeyCode.T))
		{
			CraftingManager._instance.buttonState = ButtonState.Craft;
			CanCraftState();
		}
		if (Input.GetKeyDown(KeyCode.Y))
		{
			
			CraftingState();
			CanCollect = !CanCollect;
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

	

	public void SetButtonToState(ButtonState CraftState)
	{
		switch (CraftState)
		{
			case ButtonState.Craft:
				CanCraftState();
				break;
			case ButtonState.Collect:
				CanCollectState();
				break;
			case ButtonState.Crafting:
				CraftingState();
				break;
			default:
				break;
		}
	}


	public void CanCraftState()
	{
		CraftingButton.interactable = true;
		CraftingButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Craft";
		matsHolder.SetActive(true);
		craftingTimer.gameObject.SetActive(false);
	}

	public void CraftingState()
	{

		matsHolder.SetActive(false);
		craftingTimer.gameObject.SetActive(true);
		craftingTimer.text = "Time Remaining: 1:00 Crafted: 1/10";
		if (CanCollect)
		{
			CraftingManager._instance.buttonState = ButtonState.Collect;
			CraftingButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Collect: 1/10";
			CraftingButton.interactable = true;
		}
		else
		{
			CraftingManager._instance.buttonState = ButtonState.Crafting;
			CraftingButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Crafting";
			CraftingButton.interactable = false;
		}


	}
	public void CanCollectState()
	{
		matsHolder.SetActive(false);
		craftingTimer.gameObject.SetActive(true);
		craftingTimer.text = "Time Remaining: 0:00 Crafted: 10/10";

		CraftingManager._instance.buttonState = ButtonState.Collect;
		CraftingButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Collect: 10/10";
		CraftingButton.interactable = true;

	}
	//Slider amount related
	public void OnChangeGetCraftingAmount()
	{
		craftingAmount = Mathf.RoundToInt(amountSlider.value);
		amountText.text = "Craft: " + craftingAmount.ToString();
		CraftingManager._instance.UpdateMatsAmount();
	}




	public void SetCraftingUIState(bool IsActive,ProcessorType _type,ProcessingTableTileSO Tile)
	{
		CraftingUI.SetActive(IsActive);
		craftingManager.GetSetProcessor = _type;
	}

	#endregion


	#region ButtonsFunctions

	bool isHoldingButton = false, stopHoldingButton = false, isButtonA;
	bool isShown = true;
	bool isQuickAccessSlotsSwapped = true;
	bool isInventoryOpen = false;
	bool isFightModeOn = false;
	bool isBuildModeOn = false;



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



	public void ButtonHide()
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
			bInventory.SetActive(true);

			// Check if we was on Items and the tool bar wasn't hidden
			if (isQuickAccessSlotsSwapped == true && isShown != false)
			{
				SetTools(true);
				bMainWeapon.SetActive(true);
				bSwap.SetActive(true);
			}
			// Check if we was on QASlots and the tool bar wasn't hidden
			else if (isQuickAccessSlotsSwapped == false && isShown != false)
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

		}
	}

	public void BuildModeUI()
	{
		isBuildModeOn = true;

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

	public void ButtonCancel()
	{
		isBuildModeOn = false;

		if (isQuickAccessSlotsSwapped == true)
		{
			SetTools(true);
		}
		else
		{
			SetQuickAccessSlots(true);
		}
		bMainWeapon.SetActive(true);
		bSwap.SetActive(true);
		bHide.SetActive(true);
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
		}

		stateText.SetActive(false);
	}

	public void ButtonRotate()
	{

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
