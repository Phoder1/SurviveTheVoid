using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Crafting : MonoBehaviour
{
    Dictionary<ResourceStruct, Recipe> RecipeDict;


    public int SelectedSection;
    public GameObject[] sectionsObjects;
    public Text[] RecipeSlotText;
    List<Recipe> recipeList = new List<Recipe>();
    int selectedRecipe;
    //int CurTier;
    public PlayerCrafting Player;
    public SectionManager Sections;
    public GameObject[] RecipeMaterialSlots;

    [SerializeField]
    GameObject sectionHolder;


    Crafting()
    {
        RecipeDict = new Dictionary<ResourceStruct, Recipe>();
    }

    private void Start()
    {
        



        AddRecipeToDic();

    }

    void AddRecipeToDic()
    {
        recipeList.Add(Recipe.Flower);
        recipeList.Add(Recipe.CraftingTable);
        recipeList.Add(Recipe.WoodenSword);
        recipeList.Add(Recipe.woodenHoe);
        recipeList.Add(Recipe.Apple);
        recipeList.Add(Recipe.WoodWall);
        recipeList.Add(Recipe.StoneWall);
   

        Debug.Log("Adding Recipies to List");

        for (int i = 0; i < recipeList.Count; i++)
        {
            Player.AddUnlockedRecipes(recipeList[i]);
        }

        UpdateSlotInformation();
        OnClickSelectRecipe(0);
    }


    void CheckIfUnlocked()
    {
        //for (int i = 0; i < recipeList.Count; i++)
        //{
        //    if (recipeList[i].tier <= CurTier)
        //    {
        //        Debug.Log(recipeList[i].outCome.resource.resourceEnum.ToString() + " Testing If you can craft it:  True, Your Tier is: " + CurTier + " and this recipe needs atleast tier: " + recipeList[i].tier + " "+ i);
        //        RecipeSlot[i].GetComponent<Button>().interactable = true;
        //    }
        //    else
        //    {
        //        Debug.Log(recipeList[i].outCome.resource.resourceEnum.ToString() + " Testing If you can craft it:  false, Your Tier is: " + CurTier + " and this recipe needs atleast tier: " + recipeList[i].tier + " " + i);
        //        RecipeSlot[i].GetComponent<Button>().interactable = false;
        //        ;
        //    }

        //}
        
    }

    void UpdateSlotInformation()
    {
 
        for (int i = 0; i < Player.BlocksList.Count; i++)
        {
            Sections.SectionOneSlots[i].GetComponentInChildren<Text>().text = Player.BlocksList[i].outCome.resource.resourceEnum.ToString();
        }
        for (int i = 0; i < Player.DecorationsList.Count; i++)
        {
            Sections.SectionTwoSlots[i].GetComponentInChildren<Text>().text = Player.DecorationsList[i].outCome.resource.resourceEnum.ToString();
        }
        for (int i = 0; i < Player.NatureList.Count; i++)
        {
            Sections.SectionThreeSlots[i].GetComponentInChildren<Text>().text = Player.NatureList[i].outCome.resource.resourceEnum.ToString();
        }
        for (int i = 0; i < Player.WeaponsList.Count; i++)
        {
            Sections.SectionFourSlots[i].GetComponentInChildren<Text>().text = Player.WeaponsList[i].outCome.resource.resourceEnum.ToString();
        }
        for (int i = 0; i < Player.ToolsList.Count; i++)
        {
            Sections.SectionFiveSlots[i].GetComponentInChildren<Text>().text = Player.ToolsList[i].outCome.resource.resourceEnum.ToString();
        }
        for (int i = 0; i < Player.FoodList.Count; i++)
        {
            Sections.SectionSixSlots[i].GetComponentInChildren<Text>().text = Player.FoodList[i].outCome.resource.resourceEnum.ToString();
        }
    }




    //UI
    public void OnClickSelectRecipe(int Recipe)
    {
        selectedRecipe = Recipe;
        HighLightSelectedRecipe(Recipe);
        ShowRecipe(Recipe);
    }

    void HighLightSelectedRecipe(int Recipe)
    {
       
        switch (SelectedSection)
        {
            case 0:
                for (int i = 0; i < Sections.SectionOneSlots.Length; i++)
                {
                    if (i != Recipe)
                    {
                        Sections.SectionOneSlots[i].GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        Sections.SectionOneSlots[i].GetComponent<Image>().color = Color.yellow;
                    }
                }
                break;
            case 1:
                for (int i = 0; i < Sections.SectionTwoSlots.Length; i++)
                {
                    if (i != Recipe)
                    {
                        Sections.SectionTwoSlots[i].GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        Sections.SectionTwoSlots[i].GetComponent<Image>().color = Color.yellow;
                    }
                }
                break;
            case 2:
                for (int i = 0; i < Sections.SectionThreeSlots.Length; i++)
                {
                    if (i != Recipe)
                    {
                        Sections.SectionThreeSlots[i].GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        Sections.SectionThreeSlots[i].GetComponent<Image>().color = Color.yellow;
                    }
                }
                break;
            case 3:
                for (int i = 0; i < Sections.SectionFourSlots.Length; i++)
                {
                    if (i != Recipe)
                    {
                        Sections.SectionFourSlots[i].GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        Sections.SectionFourSlots[i].GetComponent<Image>().color = Color.yellow;
                    }
                }
                break;
            case 4:
                for (int i = 0; i < Sections.SectionFiveSlots.Length; i++)
                {
                    if (i != Recipe)
                    {
                        Sections.SectionFiveSlots[i].GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        Sections.SectionFiveSlots[i].GetComponent<Image>().color = Color.yellow;
                    }
                }
                break;
            case 5:
                for (int i = 0; i < Sections.SectionSixSlots.Length; i++)
                {
                    if (i != Recipe)
                    {
                        Sections.SectionSixSlots[i].GetComponent<Image>().color = Color.white;
                    }
                    else
                    {
                        Sections.SectionSixSlots[i].GetComponent<Image>().color = Color.yellow;
                    }
                }
                break;
            default:
                break;
        }

    }

    void ShowRecipe(int _recipe)
    {
        int matsAmount = 0;
        switch (SelectedSection)
        {
            case 0:
                matsAmount = Player.BlocksList[_recipe].ResourceForRecipe.Length;
                for (int i = 0; i < RecipeMaterialSlots.Length; i++)
                {
                    if(i < matsAmount)
                    {
                        RecipeMaterialSlots[i].gameObject.SetActive(true);
                        RecipeMaterialSlots[i].GetComponentInChildren<Text>().text = Player.BlocksList[_recipe].ResourceForRecipe[i].resource.resourceEnum.ToString();
                    }
                    else
                    {
                        RecipeMaterialSlots[i].gameObject.SetActive(false);
                    }
                   
                }
                break;
            case 1:
                matsAmount = Player.DecorationsList[_recipe].ResourceForRecipe.Length;
                for (int i = 0; i < RecipeMaterialSlots.Length; i++)
                {
                    if (i < matsAmount)
                    {
                        RecipeMaterialSlots[i].gameObject.SetActive(true);
                        RecipeMaterialSlots[i].GetComponentInChildren<Text>().text = Player.DecorationsList[_recipe].ResourceForRecipe[i].resource.resourceEnum.ToString();
                    }
                    else
                    {
                        RecipeMaterialSlots[i].gameObject.SetActive(false);
                    }

                }
                break;
            case 2:
                matsAmount = Player.NatureList[_recipe].ResourceForRecipe.Length;
                for (int i = 0; i < RecipeMaterialSlots.Length; i++)
                {
                    if (i < matsAmount)
                    {
                        RecipeMaterialSlots[i].gameObject.SetActive(true);
                        RecipeMaterialSlots[i].GetComponentInChildren<Text>().text = Player.NatureList[_recipe].ResourceForRecipe[i].resource.resourceEnum.ToString();
                    }
                    else
                    {
                        RecipeMaterialSlots[i].gameObject.SetActive(false);
                    }

                }
                break;
            case 3:
                matsAmount = Player.WeaponsList[_recipe].ResourceForRecipe.Length;
                for (int i = 0; i < RecipeMaterialSlots.Length; i++)
                {
                    if (i < matsAmount)
                    {
                        RecipeMaterialSlots[i].gameObject.SetActive(true);
                        RecipeMaterialSlots[i].GetComponentInChildren<Text>().text = Player.WeaponsList[_recipe].ResourceForRecipe[i].resource.resourceEnum.ToString();
                    }
                    else
                    {
                        RecipeMaterialSlots[i].gameObject.SetActive(false);
                    }

                }
                break;
            case 4:
                matsAmount = Player.ToolsList[_recipe].ResourceForRecipe.Length;
                for (int i = 0; i < RecipeMaterialSlots.Length; i++)
                {
                    if (i < matsAmount)
                    {
                        RecipeMaterialSlots[i].gameObject.SetActive(true);
                        RecipeMaterialSlots[i].GetComponentInChildren<Text>().text = Player.ToolsList[_recipe].ResourceForRecipe[i].resource.resourceEnum.ToString();
                    }
                    else
                    {
                        RecipeMaterialSlots[i].gameObject.SetActive(false);
                    }

                }
                break;
            case 5:
                matsAmount = Player.FoodList[_recipe].ResourceForRecipe.Length;
                for (int i = 0; i < RecipeMaterialSlots.Length; i++)
                {
                    if (i < matsAmount)
                    {
                        RecipeMaterialSlots[i].gameObject.SetActive(true);
                        RecipeMaterialSlots[i].GetComponentInChildren<Text>().text = Player.FoodList[_recipe].ResourceForRecipe[i].resource.resourceEnum.ToString();
                    }
                    else
                    {
                        RecipeMaterialSlots[i].gameObject.SetActive(false);
                    }

                }
                break;
            default:
                break;
        }

    }

    public void OnClickSelectedSection(int _section)
    {
        SelectedSection = _section;
        ShowSelectedSection(_section);
        OnClickSelectRecipe(0);
    }
    void ShowSelectedSection(int _section)
    {
        for (int i = 0; i < sectionsObjects.Length; i++)
        {
            if (i != _section)
            {
                sectionsObjects[i].SetActive(false);
            }
            else
            {
                sectionsObjects[i].SetActive(true);
            }
        }
    }

    // public ResourceStruct CraftResource(Recipe recipe) { return resourceStruct; }
    public bool CanCraft(Recipe CraftRecipe) { return true; }

}

[Serializable]
public class SectionManager
{

    public GameObject[] SectionOneSlots;
    public GameObject[] SectionTwoSlots;
    public GameObject[] SectionThreeSlots;
    public GameObject[] SectionFourSlots;
    public GameObject[] SectionFiveSlots;
    public GameObject[] SectionSixSlots;
}

