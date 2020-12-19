using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEngine;

public class Recipe
{
    public readonly ResourceSlot[] ResourceForRecipe;
    public readonly ResourceStruct outCome;
    public readonly int level;


    private Recipe(ResourceSlot[] resourceForRecipe, ResourceStruct ResourceToGetFromRecipe, int level) 
    {
        ResourceForRecipe = resourceForRecipe;
        this.level = level;
            outCome = ResourceToGetFromRecipe;
    }
    public static Recipe EXAMPLE {
        get {
            return new Recipe(
                new ResourceSlot[1] { new ResourceSlot(ResourceStruct.EXAMPLE , 3) }
                , ResourceStruct.EXAMPLE
                , 10
                );
        }
    }
}
