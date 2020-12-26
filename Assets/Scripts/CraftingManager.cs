using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    public Transform sectionHolder;
    private Section[] sections;
    private Section selectedSection;
    public GameObject[] RecipeMaterialSlots;
    public static CraftingManager _instance;
    List<RecipeSO> UnlockedRecipe = new List<RecipeSO>();

    private void Awake() {
        if (_instance == null) {
            _instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {

        Init();
    }

    private void Init() {
        //AddRecipeToList();
        ImportSlots();
        UpdateInformation();
        SelectSection("Blocks");
    }

    private void ImportSlots() {
        sections = new Section[sectionHolder.childCount];

        for (int i = 0; i < sectionHolder.childCount; i++) {
            Transform sectionTransform = sectionHolder.GetChild(i);
            sections[i] = new Section(sectionTransform.name, sectionTransform.gameObject);
            Transform[] slotsTransform = new Transform[sectionTransform.childCount];
            for (int j = 0; j < slotsTransform.Length; j++) {

                slotsTransform[j] = sectionTransform.GetChild(j);

            }
            Array.Sort(slotsTransform, (slot1, slot2) => slot1.name.CompareTo(slot2.name));
            sections[i].sectionSlots = new Image[slotsTransform.Length];
            for (int j = 0; j < slotsTransform.Length; j++) {
                sections[i].sectionSlots[j] = slotsTransform[j].GetComponent<Image>();
            }
        }
        Array.Sort(sections, (section1, section2) => section1.name.CompareTo(section2.name));

    }

    private void UpdateInformation() {
        foreach (Section section in sections) {
            section.UpdateInformation();
        }
    }

    //void AddRecipeToList() {
    //    recipeList.Add(Recipe.Flower);
    //    recipeList.Add(Recipe.CraftingTable);
    //    recipeList.Add(Recipe.WoodenSword);
    //    recipeList.Add(Recipe.woodenHoe);
    //    recipeList.Add(Recipe.Apple);
    //    recipeList.Add(Recipe.WoodWall);
    //    recipeList.Add(Recipe.StoneWall);

    //    for (int i = 0; i < recipeList.Count; i++) {
    //        Player.AddUnlockedRecipes(recipeList[i]);
    //    }
    //    OnClickSelectRecipe(0);
    //}






    //UI
    public void OnClickSelectRecipe(int recipe) {
        selectedSection.SelectSlot(recipe);
    }

    public void SelectSection(string sectionName) {
        foreach (Section section in sections) {
            if (section.name == sectionName) {
                if (selectedSection != null) {
                    if (selectedSection.name != sectionName) {
                        selectedSection.GetSetIsSelected = false;
                        section.GetSetIsSelected = true;
                        selectedSection = section;
                    }
                }
                else {
                    section.GetSetIsSelected = true;
                    selectedSection = section;
                }
                break;
            }
        }
    }
    public void ShowRecipe(RecipeSO recipe) {
        int matsAmount = recipe.ResourcesCost.Length;
        for (int i = 0; i < matsAmount; i++) {
            if (i < matsAmount) {
                RecipeMaterialSlots[i].gameObject.SetActive(true);
                RecipeMaterialSlots[i].GetComponentInChildren<Text>().text = recipe.ResourcesCost[i].resource.resourceEnum.ToString();
            }
            else {
                RecipeMaterialSlots[i].gameObject.SetActive(false);
            }

        }
    }

    // public ResourceStruct CraftResource(Recipe recipe) { return resourceStruct; }
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

    public Section(string name, GameObject section) {
        this.name = name;
        this.section = section;
        recipeList = new List<RecipeSO>();
    }

    public bool GetSetIsSelected {
        get => isSelected;
        set {
            ChangeSectionSelection(value);
            isSelected = value;
        }
    }

    private void ChangeSectionSelection(bool state) {
        if (state == true) {
            SelectSlot(0);
        }
        section.SetActive(state);
    }

    public void SelectSlot(int slotNum) {
        sectionSlots[selectedSlot].color = Color.white;
        selectedSlot = slotNum;
        sectionSlots[selectedSlot].color = Color.yellow;
        if (recipeList.Count > 0) {
            CraftingManager._instance.ShowRecipe(recipeList[slotNum]);
        }
    }
    public void UpdateInformation() {

        for (int i = 0; i < recipeList.Count; i++) {
            sectionSlots[i].GetComponentInChildren<Text>().text = recipeList[i].Resource.resource.resourceEnum.ToString();
        }
    }
}

