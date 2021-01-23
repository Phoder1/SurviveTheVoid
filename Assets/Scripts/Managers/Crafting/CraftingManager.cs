using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum ButtonState
{
    Openining,
    CanCraft,
    Collect,
    Crafting
}

public class CraftingManager : MonoSingleton<CraftingManager>, ICraftingManager
{
    public ButtonState buttonState;
    public ItemPackSO items;
    public RecipePackSO recipes;
    public Transform sectionHolder;
    public Transform ScrollsHolder;
    private Section[] sections;
    private Section selectedSection;
    public GameObject[] recipeMaterialSlots;
    List<RecipeSO> unlockedRecipes = new List<RecipeSO>();
    [SerializeField]
    private GameObject ItemSlotPrefab;
    private ProcessorType processor;
    public ProcessorType GetSetProcessor
    {
        get => processor;
        set
        {
            processor = value;

            SelectSection("Blocks");
            UpdateInformation();

        }
    }
    public ProcessingTableTileState CurrentProcessTile;



    [FormerlySerializedAs("SelectedRecipe")]
    [HideInInspector]
    public RecipeSO selectedRecipe;

    private Inventory inventory;
    InventoryUIManager inventoryUI;

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Y))
        {
            
        }




        if (Input.GetKeyDown(KeyCode.Z))
        {

            inventory.AddToInventory(0, new ItemSlot(items.getitemsArr[15], 1));

            ShowRecipe(selectedRecipe);
            inventoryUI.UpdateInventoryToUI();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            inventory.AddToInventory(0, new ItemSlot(items.getitemsArr[9], 1));
            inventoryUI.UpdateInventoryToUI();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            inventory.RemoveItemFromInventory(0, new ItemSlot(items.getitemsArr[6], 1));
            inventoryUI.UpdateInventoryToUI();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            inventory.RemoveItemFromInventory(0, new ItemSlot(items.getitemsArr[9], 1));
            inventoryUI.UpdateInventoryToUI();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            inventory.AddToInventory(0, new ItemSlot(items.getitemsArr[0], 1));
            inventoryUI.UpdateInventoryToUI();
            ShowRecipe(selectedRecipe);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.PrintInventory(0);
        }



    }




    public override void Init()
    {


        inventory = Inventory.GetInstance;
        inventoryUI = InventoryUIManager._instance;
        buttonState = ButtonState.CanCraft;
        // UIManager._instance.SetButtonToState(buttonState);
        ImportSlots();
        AddRecipeToList();
        InstantiateItemSlots();
        //SelectSection("Blocks");
        DeleteOutCome();

    }

    private void ImportSlots()
    {
        sections = new Section[sectionHolder.childCount];

        //Debug.Log(sectionHolder.childCount.ToString());
        for (int i = 0; i < sectionHolder.childCount; i++)
        {
            Transform sectionTransform = sectionHolder.GetChild(i);
            sections[i] = new Section(sectionTransform.name, sectionTransform.gameObject);
            Transform[] slotsTransform = new Transform[sectionTransform.GetChild(0).childCount];
            ;
            sections[i].scrollBar = ScrollsHolder.GetChild(i).gameObject;




            for (int j = 0; j < slotsTransform.Length; j++)
            {
                slotsTransform[j] = sectionTransform.GetChild(j);
            }
            Array.Sort(slotsTransform, (slot1, slot2) =>
            {
                string numberString = slot1.name;
                numberString = numberString.Replace("Slot", "");
                int number = int.Parse(numberString);

                string numberStringTwo = slot2.name;
                numberStringTwo = numberStringTwo.Replace("Slot", "");
                int numberTwo = int.Parse(numberStringTwo);

                return number - numberTwo;

            });



            sections[i].sectionSlotsList = new List<Image>();
            for (int j = 0; j < slotsTransform.Length; j++)
            {
                sections[i].sectionSlotsList[j] = slotsTransform[j].GetComponent<Image>();

            }
        }

        Array.Sort(sections, (section1, section2) => String.Compare(section1.name, section2.name, StringComparison.Ordinal));

    }

    public void UpdateInformation()
    {
        foreach (Section section in sections)
        {
            section.UpdateInformation(GetSetProcessor);

            //Debug.Log("Updating Information");
        }


    }

    void InstantiateItemSlots()
    {
        foreach (Section section in sections)
        {
            for (int i = 0; i < sections.Length; i++)
            {

                if (section.name == sectionHolder.GetChild(i).gameObject.name)
                {
                    for (int j = 0; j < section.recipeList.Count; j++)
                    {

                        GameObject instiatedSlot = Instantiate(ItemSlotPrefab, sectionHolder.GetChild(i).gameObject.transform.GetChild(0).transform);
                        instiatedSlot.transform.GetChild(0).GetComponent<Image>().sprite = section.recipeList[j].getoutcomeItem.item.getsprite;
                        instiatedSlot.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = section.recipeList[j].getoutcomeItem.item.getItemName;

                        // add function to the buttons
                        int Index = j;
                        instiatedSlot.GetComponent<Button>().onClick.AddListener(delegate { SelectRecipe(Index); });

                        //Debug.Log(instiatedSlot.gameObject.name);

                        section.GetSectionSlots(instiatedSlot.GetComponent<Image>());
                        section.CheckIflockedRecipe(section.recipeList[j]);




                    }

                }
            }

            UpdateInformation();
        }



    }


    void AddRecipeToList()
    {
        foreach (RecipeSO recipe in recipes.getrecipesArr)
        {
            GetSection(recipe.getSection).UpdateRecipeList(recipe);
        }
    }

    private Section GetSection(SectionEnum _section)
    {

        for (int j = 0; j < sections.Length; j++)
        {
            if (_section.ToString() == sections[j].name)
            {

                return sections[j];
            }
        }
        throw new NotImplementedException();

    }





    public void SelectRecipe(int recipe)
    {

        selectedSection.SelectSlot(recipe);

        if (selectedRecipe != null)
            UIManager._instance.ResetMultiple();
    }

    public void SelectSection(string sectionName)
    {
        if (selectedRecipe != null)
            UIManager._instance.ResetMultiple();

        foreach (Section section in sections)
        {
            if (section.name == sectionName)
            {

                if (selectedSection != null)
                {
                    if (selectedSection.name != sectionName)
                    {
                        selectedSection.getSetIsSelected = false;
                        section.getSetIsSelected = true;
                        selectedSection = section;
                        selectedSection.ChangeSectionSelection(true);
                    }
                }
                else
                {
                    section.getSetIsSelected = true;
                    selectedSection = section;
                    selectedSection.ChangeSectionSelection(true);
                }

                break;
            }
        }
        selectedSection.ChangeSectionSelection(true);
    
    }
    public void ShowRecipe(RecipeSO recipe)
    {
        UpdateMatsAmount();
        if (selectedRecipe != null)
        {
            int matsAmount = recipe.getitemCostArr.Length;
            for (int i = 0; i < recipeMaterialSlots.Length; i++)
            {
                if (i < matsAmount)
                {
                    if (!recipeMaterialSlots[i].gameObject.activeInHierarchy)
                    {
                        recipeMaterialSlots[i].gameObject.SetActive(true);
                    }
                    TextMeshProUGUI materialNameText = recipeMaterialSlots[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    materialNameText.text = Costitemso[i].getItemName;
                    TextMeshProUGUI materialCostText = recipeMaterialSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                    materialCostText.text = inventory.GetAmountOfItem(0, TempArr[i]) + " / " + (TempArr[i].amount * UIManager._instance.getCraftingAmount);
                    recipeMaterialSlots[i].GetComponent<Image>().sprite = recipe.getitemCostArr[i].item.getsprite;
                }
                else
                {
                    recipeMaterialSlots[i].gameObject.SetActive(false);
                }

            }


            UIManager._instance.craftingTimer.text = (selectedRecipe.GetCraftingTime * UIManager._instance.getCraftingAmount).ToString();
        }

    }
    public void clearRecipeMat()
    {
        for (int i = 0; i < recipeMaterialSlots.Length; i++)
        {
            recipeMaterialSlots[i].gameObject.SetActive(false);
        }
    }

    // public ResourceStruct CraftResource(Recipe recipe) { return resourceStruct; }
    bool IsCrafting;

    //updating for multiple crafting
    ItemSO OutComeitemso;
    int OutComeAmount;
    ItemSlot TempSlot;

    ItemSO[] Costitemso;
    int[] CostAmount;
    ItemSlot[] TempArr;

    public void UpdateMatsAmount()
    {
        if (selectedRecipe != null)
        {

            //seperating scriptable object 
            OutComeitemso = selectedRecipe.getoutcomeItem.item;
            OutComeAmount = selectedRecipe.getoutcomeItem.amount;
            TempSlot = new ItemSlot(OutComeitemso, OutComeAmount);

            Costitemso = new ItemSO[selectedRecipe.getitemCostArr.Length];
            CostAmount = new int[selectedRecipe.getitemCostArr.Length];



            for (int i = 0; i < selectedRecipe.getitemCostArr.Length; i++)
            {
                Costitemso[i] = selectedRecipe.getitemCostArr[i].item;
                CostAmount[i] = selectedRecipe.getitemCostArr[i].amount;
            }

            TempArr = new ItemSlot[selectedRecipe.getitemCostArr.Length];

            for (int i = 0; i < TempArr.Length; i++)
            {
                TempArr[i] = new ItemSlot(Costitemso[i], CostAmount[i]);
            }


            //AmountHolder.UpdateAmountHolder(selectedRecipe.getitemCostArr, selectedRecipe.getoutcomeItem, selectedRecipe.GetCraftingTime);
            //AmountHolder.DoubleAmountOutCome(UIManager._instance.getCraftingAmount);
            //ShowRecipe(selectedRecipe);
        }
        else
        {
            Debug.Log("Selected recipe is null");
        }
        ShowOutCome();
    }


    public void AttemptToCraft()
    {
        if (buttonState == ButtonState.CanCraft)
        {
            if (selectedRecipe != null)
            {
                if (!inventory.CheckEnoughItemsForRecipe(selectedRecipe, UIManager._instance.getCraftingAmount))
                {
                    Debug.Log("Not Enough Materials");
                }
                else
                {
                    

                    //if(inventory.RemoveItemFromInventory(0,new ItemSlot()))


                    inventory.RemoveItemsByRecipe(selectedRecipe, UIManager._instance.getCraftingAmount);

                        CurrentProcessTile.StartCrafting(selectedRecipe, (UIManager._instance.getCraftingAmount));
                    
                    
                    ShowRecipe(selectedRecipe);
                    buttonState = ButtonState.Crafting;
                   UIManager._instance.SetButtonToState(buttonState,CurrentProcessTile.ItemsCrafted,CurrentProcessTile.amount,CurrentProcessTile.CraftingTimeRemaining);


                    UIManager._instance.OnChangeGetCraftingAmount();
                }
            }
        }
        else if (buttonState == ButtonState.Collect)
        {
            //collect items
          
            for (int i = CurrentProcessTile.ItemsCrafted; i > 0; i--)
            {
                
                if (inventory.AddToInventory(0,new ItemSlot(CurrentProcessTile.craftingRecipe.getoutcomeItem.item, i * CurrentProcessTile.craftingRecipe.getoutcomeItem.amount)))
                {
                    CurrentProcessTile.CollectItems(i);
                    break;
                }

            }
            
           

            if (!CurrentProcessTile.IsCrafting)
            {
                buttonState = ButtonState.CanCraft;
                UIManager._instance.SetButtonToState(buttonState, 0, 0, 0);
            }


            UIManager._instance.OnChangeGetCraftingAmount();
            Debug.Log("Collect your item");
        }
        else if (buttonState == ButtonState.Crafting)
        {


            //you craft but there is no items to collect
            Debug.Log("You are still crafting");
        }

    }

    int CraftIndex;
    public void AttemptToCraftMax()
    {
        CraftIndex = 20;
        CheckIfYouCanCraft();


    }

    void CheckIfYouCanCraft()
    {
        if((CraftIndex + CurrentProcessTile.amount <= 20))
        {
            if (inventory.CheckEnoughItemsForRecipe(selectedRecipe, CraftIndex))
            {

                Debug.Log("Crafted: " + selectedRecipe.getoutcomeItem.item.getItemName + " Amount of: " + CraftIndex);

                inventory.RemoveItemsByRecipe(selectedRecipe,  CraftIndex);
                if (CurrentProcessTile.IsCrafting)
                {
                    CurrentProcessTile.AddToQueue(selectedRecipe.getoutcomeItem.amount * CraftIndex);
                }
                else
                {
                    CurrentProcessTile.StartCrafting(selectedRecipe, CraftIndex);
                }
                ShowRecipe(selectedRecipe);
                buttonState = ButtonState.Crafting;
                UIManager._instance.SetButtonToState(buttonState, CurrentProcessTile.ItemsCrafted, CurrentProcessTile.amount, CurrentProcessTile.CraftingTimeRemaining);


                UIManager._instance.OnChangeGetCraftingAmount();
            }
            else
            {

                Debug.Log("Cant craft amount of: " + CraftIndex + " Trying to check if you can craft: " + (CraftIndex - 1).ToString());
                CraftIndex--;
                if (CraftIndex >= 1)
                    CheckIfYouCanCraft();
            }

        }
        else
        {
            Debug.Log("Cant craft amount of: " + CraftIndex + " Trying to check if you can craft: " + (CraftIndex - 1).ToString());
            CraftIndex--;
            if (CraftIndex >= 1)
                CheckIfYouCanCraft();




        }


    }


    public void AddToCraft()
    {
        if (inventory.CheckEnoughItemsForRecipe(selectedRecipe, UIManager._instance.getCraftingAmount))
        {
            inventory.RemoveItemsByRecipe(selectedRecipe, UIManager._instance.getCraftingAmount);
            CurrentProcessTile.AddToQueue(UIManager._instance.getCraftingAmount);
        }
    }


    [SerializeField] float Timer;
    bool startcount;
    [SerializeField]
    Image OutComeImage;
    [SerializeField]
    TextMeshProUGUI OutComeText;
    public void ShowOutCome()
    {
        if(selectedRecipe != null)
        {
            OutComeImage.gameObject.SetActive(true);
            OutComeImage.sprite = selectedRecipe.getoutcomeItem.item.getsprite;
            OutComeText.text = (selectedRecipe.getoutcomeItem.amount * UIManager._instance.getCraftingAmount).ToString();
        }
    }
    public void DeleteOutCome()
    {
        OutComeImage.gameObject.SetActive(false);
        OutComeImage.sprite = null;
        OutComeText.text = "";
    }


    public void UnlockRecipe(RecipeSO _recipe) => GetSection(_recipe.getSection).UnlockRecipe(_recipe);




    public bool CanCraft(RecipeSO craftRecipe) { return true; }

}

public interface ICraftingManager
{
    void AttemptToCraft();
    bool CanCraft(RecipeSO craftRecipe);
    void SelectRecipe(int recipe);
    void SelectSection(string sectionName);
    void ShowRecipe(RecipeSO recipe);
    void UnlockRecipe(RecipeSO _recipe);
}


/*public interface ICrafting
{
    void ImportSlots();
    void AddRecipeToList();
    void UpdateInformation();

    Section GetSection(SectionEnum _section);

    void SelectRecipe(int recipe);

    public void SelectSection(string sectionName);

    void ShowRecipe(RecipeSO recipe);

    void AttemptToCraft();

    void UnlockRecipe(RecipeSO _recipe);

    bool CanCraft(RecipeSO craftRecipe);
}
*/

[Serializable]
public class Section
{
    public string name;
    public GameObject section;
    public GameObject scrollBar;
    public List<Image> sectionSlotsList = new List<Image>();
    //public Image[] sectionSlots;
    private int selectedSlot = 0;
    public List<RecipeSO> recipeList;
    private bool isSelected;
    public Section(string name, GameObject section)
    {
        this.name = name;
        this.section = section;
        recipeList = new List<RecipeSO>();
    }

    public bool getSetIsSelected
    {
        get => isSelected;
        set
        {
            ChangeSectionSelection(value);
            isSelected = value;

        }
    }

    public void UpdateRecipeList(RecipeSO _recipe)
    {
        recipeList.Add(_recipe);

    }

    public void CheckIflockedRecipe(RecipeSO _recipe)
    {
        int recipeIndex = recipeList.IndexOf(_recipe);
        if (!_recipe.getisUnlocked)
        {

            sectionSlotsList[recipeIndex].color = Color.black;

        }
        else if (selectedSlot == recipeList.IndexOf(_recipe))
        {

            sectionSlotsList[recipeIndex].color = Color.black;
        }
        else
        {

            sectionSlotsList[recipeIndex].color = Color.white;
        }
    }

    public void UnlockRecipe(RecipeSO _recipe)
    {
        _recipe.UpdateIfRecipeUnlocked(true);

        foreach (RecipeSO recipe in recipeList)
        {
            if (recipe == _recipe)
            {
                CheckIflockedRecipe(recipe);
            }
        }

    }



    public void ChangeSectionSelection(bool state)
    {

        if (state == true)
        {
            int Index = 0;
            for (int i = 0; i < recipeList.Count; i++)
            {

                if (recipeList[i].GetProcessorType != CraftingManager._instance.GetSetProcessor)
                {
                    Index++;
                }
                else
                {
                    SelectSlot(i);
                    break;
                }
            }
            if (Index == recipeList.Count)
            {
                CraftingManager._instance.selectedRecipe = null;
                CraftingManager._instance.clearRecipeMat();
            }


        }
        scrollBar.SetActive(state);
        section.SetActive(state);
    }

    public void SelectSlot(int slotNum)
    {
        if (recipeList[slotNum].getisUnlocked)
        {
            sectionSlotsList[selectedSlot].color = Color.white;
            selectedSlot = slotNum;
            sectionSlotsList[selectedSlot].color = Color.yellow;

            updateSelectedRecipe(slotNum);


        }


    }
    void updateSelectedRecipe(int slotNum)
    {
        if (recipeList.Count > 0)
        {
            CraftingManager._instance.selectedRecipe = recipeList[slotNum];
            CraftingManager._instance.ShowRecipe(recipeList[slotNum]);
            CraftingManager._instance.UpdateMatsAmount();
        }
        else
        {
            CraftingManager._instance.selectedRecipe = null;
        }
    }


    public void GetSectionSlots(Image _Slot)
    {
        sectionSlotsList.Add(_Slot);
    }

    public void UpdateInformation(ProcessorType _Type)
    {

        for (int i = 0; i < sectionSlotsList.Count; i++)
        {
            if (i < recipeList.Count)
            {
                sectionSlotsList[i].gameObject.SetActive(true);
            }
            else
            {
                sectionSlotsList[i].gameObject.SetActive(false);
            }

        }
        GetTableType(_Type);
        //for (int i = 0; i < recipeList.Count; i++)
        //{
        //    CheckIflockedRecipe(recipeList[i]);
        //}


    }


    public void GetTableType(ProcessorType _Type)
    {
        for (int i = 0; i < sectionSlotsList.Count; i++)
        {
            if (_Type != recipeList[i].GetProcessorType)
            {
                sectionSlotsList[i].gameObject.SetActive(false);
            }
        }
    }


}





