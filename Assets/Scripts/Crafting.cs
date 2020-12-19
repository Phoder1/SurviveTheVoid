using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Crafting
{
    Dictionary<ResourceStruct, Recipe> RecipeDict;
    Crafting() {

        RecipeDict = new Dictionary<ResourceStruct, Recipe>();
    }
    public ResourceStruct CraftResource(Recipe) { return ResourceStruct; }
    public bool CanCraft(Recipe) { return true; }

}
