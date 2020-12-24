using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrafting : MonoBehaviour
{
    List<Recipe> UnlockedRecipe = new List<Recipe>();

    


    public List<Recipe> BlocksList = new List<Recipe>();
    public List<Recipe> DecorationsList = new List<Recipe>();
    public List<Recipe> NatureList = new List<Recipe>();
    public List<Recipe> WeaponsList = new List<Recipe>();
    public List<Recipe> ToolsList = new List<Recipe>();
    public List<Recipe> FoodList = new List<Recipe>();


    public void AddUnlockedRecipes(Recipe Type)
    {
        switch (Type.sectionType)
        {
            case Recipe.sectionTypes.Blocks:
                BlocksList.Add(Type);
                break;
            case Recipe.sectionTypes.Decorations:
                DecorationsList.Add(Type);
                break;
            case Recipe.sectionTypes.Nature:
                NatureList.Add(Type);
                break;
            case Recipe.sectionTypes.Weapons:
                WeaponsList.Add(Type);
                break;
            case Recipe.sectionTypes.Tools:
                ToolsList.Add(Type);
                break;
            case Recipe.sectionTypes.Food:
                FoodList.Add(Type);
                break;
            default:
                break;
        }
        //Debug.Log("Adding to List: " + Type.outCome.ToString());
    }








}
