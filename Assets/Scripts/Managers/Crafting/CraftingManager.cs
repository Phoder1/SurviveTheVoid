using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public ItemPackSO items;
    public RecipePackSO recipes;
    public Transform sectionHolder;
    private Section[] sections;
    private Section selectedSection;
    public GameObject[] recipeMaterialSlots;
    public static CraftingManager _instance;
    List<RecipeSO> unlockedRecipes = new List<RecipeSO>();

    [FormerlySerializedAs("SelectedRecipe")] [HideInInspector]
    public RecipeSO selectedRecipe;

    private Inventory inventory;



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
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            inventory.AddToInventory(new ItemSlot(items.getitemsArr[3], 1));
            ShowRecipe(selectedRecipe);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            inventory.AddToInventory(new ItemSlot(items.getitemsArr[4], 1));
            ShowRecipe(selectedRecipe);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            inventory.RemoveItemFromInventory(new ItemSlot(items.getitemsArr[3], 1));
            ShowRecipe(selectedRecipe);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            inventory.RemoveItemFromInventory(new ItemSlot(items.getitemsArr[4], 1));
            ShowRecipe(selectedRecipe);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventory.PrintInventory();
        }



    }




    private void Init()
    {
        inventory = Inventory.GetInstance;

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
                materialCostText.text = inventory.GetAmountOfItem(recipe.getitemCostArr[i]).ToString() + " / " + recipe.getitemCostArr[i].amount;
               
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

    void UnlockRecipe(RecipeSO _recipe) => GetSection(_recipe.getSection).UnlockRecipe(_recipe);
    



    public bool CanCraft(RecipeSO craftRecipe) { return true; }

}

public interface ICrafting
{
    CraftingManager GetInstance { get; }





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

    void CheckIflockedRecipe(RecipeSO _recipe)
    {
        if (!_recipe.getisUnlocked)
        {
            int Test = recipeList.IndexOf(_recipe);
            sectionSlots[Test].color = Color.black;

        }
    }

    public void UnlockRecipe(RecipeSO _recipe)
    {
        _recipe.UpdateIfRecipeUnlocked(true);

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
        if (recipeList[slotNum].getisUnlocked)
        {
            sectionSlots[selectedSlot].color = Color.white;
            selectedSlot = slotNum;
            sectionSlots[selectedSlot].color = Color.yellow;

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
    }
    public void UpdateInformation()
    {
        for (int i = 0; i < recipeList.Count; i++)
        {

            sectionSlots[i].GetComponentInChildren<Text>().text = recipeList[i].getoutcomeItem.item.getItemEnum.ToString();





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

