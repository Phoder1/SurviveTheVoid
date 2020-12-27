using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public ItemPackSO items;
    public RecipePackSO Recipes;
    public Transform sectionHolder;
    private Section[] sections;
    private Section selectedSection;
    public GameObject[] RecipeMaterialSlots;
    public static CraftingManager _instance;
    List<RecipeSO> UnlockedRecipe = new List<RecipeSO>();

    [HideInInspector]
    public RecipeSO SelectedRecipe;



    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        Init();

    }





    private void Init()
    {

        ImportSlots();
        AddRecipeToList();
        UpdateInformation();
        SelectSection("Blocks");

    }

    private void ImportSlots()
    {
        sections = new Section[sectionHolder.childCount];

        for (int i = 0; i < sectionHolder.childCount; i++)
        {
            Transform sectionTransform = sectionHolder.GetChild(i);
            sections[i] = new Section(sectionTransform.name, sectionTransform.gameObject);
            Transform[] slotsTransform = new Transform[sectionTransform.childCount];
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



            sections[i].sectionSlots = new Image[slotsTransform.Length];
            for (int j = 0; j < slotsTransform.Length; j++)
            {
                sections[i].sectionSlots[j] = slotsTransform[j].GetComponent<Image>();
            }
        }

        Array.Sort(sections, (section1, section2) => section1.name.CompareTo(section2.name));

    }

    private void UpdateInformation()
    {
        foreach (Section section in sections)
        {
            section.UpdateInformation();
            //Debug.Log("Updating Information");
        }
    }

    void AddRecipeToList()
    {
        foreach (RecipeSO recipe in Recipes.recipesArr)
        {
            GetSection(recipe.section).UpdateRecipeList(recipe);
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




    //UI
    public void OnClickSelectRecipe(int recipe)
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
                        selectedSection.GetSetIsSelected = false;
                        section.GetSetIsSelected = true;
                        selectedSection = section;
                    }
                }
                else
                {
                    section.GetSetIsSelected = true;
                    selectedSection = section;
                }
                break;
            }
        }
    }
    public void ShowRecipe(RecipeSO recipe)
    {
        int matsAmount = recipe.itemCostArr.Length;
        for (int i = 0; i < RecipeMaterialSlots.Length; i++)
        {
            if (i < matsAmount)
            {
                RecipeMaterialSlots[i].gameObject.SetActive(true);
                RecipeMaterialSlots[i].GetComponentInChildren<Text>().text = recipe.itemCostArr[i].item.itemEnum.ToString();
            }
            else
            {
                RecipeMaterialSlots[i].gameObject.SetActive(false);
            }

        }
    }

    // public ResourceStruct CraftResource(Recipe recipe) { return resourceStruct; }

    public void OnClickCraftButton()
    {
        if (SelectedRecipe != null)
        {
            for (int i = 0; i < SelectedRecipe.itemCostArr.Length; i++)
            {
                if (Inventory.CheckInventoryForItem(SelectedRecipe.itemCostArr[i]))
                {
                    Debug.Log("You have enough: " + SelectedRecipe.itemCostArr[i].item.itemEnum.ToString());
                }
                else
                {
                    Debug.Log("You don't have enough: " + SelectedRecipe.itemCostArr[i].item.itemEnum.ToString());
                }
            }
        }
        else
        {
            Debug.Log("No recipe is selected.");
        }

    }

    void UnlockRecipe(RecipeSO _recipe) => GetSection(_recipe.section).UnlockRecipe(_recipe);
    



    public bool CanCraft(RecipeSO CraftRecipe) { return true; }

}
[Serializable]
public class Section
{
    public string name;
    public GameObject section;
    public Image[] sectionSlots;
    private int selectedSlot = 0;
    public List<RecipeSO> recipeList;
    private bool isSelected;

    public Section(string name, GameObject section)
    {
        this.name = name;
        this.section = section;
        recipeList = new List<RecipeSO>();
    }

    public bool GetSetIsSelected
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

    void CheckIflockedRecipe(RecipeSO _recipe)
    {
        if (!_recipe.isUnlocked)
        {
            int Test = recipeList.IndexOf(_recipe);
            sectionSlots[Test].color = Color.black;

        }
    }

    public void UnlockRecipe(RecipeSO _recipe)
    {
        _recipe.isUnlocked = true;

        foreach (RecipeSO recipe in recipeList)
        {
            if (recipe == _recipe)
            {

            }
        }

    }



    private void ChangeSectionSelection(bool state)
    {
        if (state == true)
        {
            SelectSlot(0);
        }
        section.SetActive(state);
    }

    public void SelectSlot(int slotNum)
    {
        if (recipeList[slotNum].isUnlocked)
        {
            sectionSlots[selectedSlot].color = Color.white;
            selectedSlot = slotNum;
            sectionSlots[selectedSlot].color = Color.yellow;

            if (recipeList.Count > 0)
            {
                CraftingManager._instance.SelectedRecipe = recipeList[slotNum];
                CraftingManager._instance.ShowRecipe(recipeList[slotNum]);
            }
            else
            {
                CraftingManager._instance.SelectedRecipe = null;
            }
        }
    }
    public void UpdateInformation()
    {
        for (int i = 0; i < recipeList.Count; i++)
        {

            sectionSlots[i].GetComponentInChildren<Text>().text = recipeList[i].outcomeItem.item.itemEnum.ToString();
            CheckIflockedRecipe(recipeList[i]);
        }
        for (int i = 0; i < sectionSlots.Length; i++)
        {
            if (i < recipeList.Count)
            {
                sectionSlots[i].gameObject.SetActive(true);
            }
            else
            {
                sectionSlots[i].gameObject.SetActive(false);
            }
        }
    }
}

