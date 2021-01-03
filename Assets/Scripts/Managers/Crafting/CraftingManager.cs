using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;



public class CraftingManager : MonoSingleton<CraftingManager>, ICraftingManager
{
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



    [FormerlySerializedAs("SelectedRecipe")]
    [HideInInspector]
    public RecipeSO selectedRecipe;

    private Inventory inventory;



    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            inventory.AddToInventory(0,new ItemSlot(items.getitemsArr[3], 1));
            ShowRecipe(selectedRecipe);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            inventory.AddToInventory(0, new ItemSlot(items.getitemsArr[4], 1));
            ShowRecipe(selectedRecipe);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            inventory.RemoveItemFromInventory(0, new ItemSlot(items.getitemsArr[3], 1));
            ShowRecipe(selectedRecipe);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            inventory.RemoveItemFromInventory(0, new ItemSlot(items.getitemsArr[4], 1));
            ShowRecipe(selectedRecipe);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.PrintInventory(0);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            UnlockRecipe(recipes.getrecipesArr[4]);
        }


    }




    public void Init()
    {
        inventory = Inventory.GetInstance;

        ImportSlots();
        AddRecipeToList();
        InstantiateItemSlots();
        SelectSection("Blocks");

    }

    private void ImportSlots()
    {
        sections = new Section[sectionHolder.childCount];
        
        Debug.Log(sectionHolder.childCount.ToString());
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

    private void UpdateInformation()
    {
        foreach (Section section in sections)
        {
            section.UpdateInformation();
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
    }

    public void SelectSection(string sectionName)
    {
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
                    }
                }
                else
                {
                    section.getSetIsSelected = true;
                    selectedSection = section;
                }
                break;
            }
        }

        //for (int i = 0; i < scrolls.Length; i++)
        //{
        //    if (scrolls[i].gameObject.name == sectionName + "ScrollBar")
        //    {
        //        if (!scrolls[i].activeInHierarchy)
        //        {
        //            scrolls[i].SetActive(true);
        //        }
        //    }
        //    else
        //    {
        //        if (scrolls[i].activeInHierarchy)
        //        {
        //            scrolls[i].SetActive(false);
        //        }
        //    }
        //}


    }
    public void ShowRecipe(RecipeSO recipe)
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
                Text materialNameText = recipeMaterialSlots[i].transform.GetChild(0).GetComponent<Text>();
                materialNameText.text = recipe.getitemCostArr[i].item.getItemEnum.ToString();
                Text materialCostText = recipeMaterialSlots[i].transform.GetChild(1).GetComponent<Text>();
                materialCostText.text = inventory.GetAmountOfItem(0,recipe.getitemCostArr[i]).ToString() + " / " + recipe.getitemCostArr[i].amount;

            }
            else
            {
                recipeMaterialSlots[i].gameObject.SetActive(false);
            }

        }
    }

    // public ResourceStruct CraftResource(Recipe recipe) { return resourceStruct; }

    public void AttemptToCraft()
    {
        if (!inventory.CheckEnoughItemsForRecipe(selectedRecipe))
        {
            Debug.Log("Not Enough Materials");
        }
        else
        {
            ShowRecipe(selectedRecipe);
        }
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



    private void ChangeSectionSelection(bool state)
    {
        if (state == true)
        {
            SelectSlot(0);
            
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

    public void UpdateInformation()
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

        //for (int i = 0; i < recipeList.Count; i++)
        //{
        //    CheckIflockedRecipe(recipeList[i]);
        //}


    }
}

