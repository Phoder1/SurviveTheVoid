using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe Pack", menuName = "Crafting/" + "Recipe Pack")]
public class RecipePackSO : ScriptableObject
{
    [SerializeField]
    private RecipeSO[] recipesArr;
    public RecipeSO[] getrecipesArr { get => recipesArr; }
}
