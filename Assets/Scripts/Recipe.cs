
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe
{
    public readonly ResourceSlot[] ResourceForRecipe;
    public readonly ResourceSlot outCome;
    public readonly int tier;



    public enum sectionTypes
    {
        Blocks,
        Decorations,
        Nature,
        Weapons,
        Tools,
        Food
    }
    public sectionTypes sectionType;




    private Recipe(ResourceSlot[] resourceForRecipe, ResourceSlot ResourceToGetFromRecipe, int _tier, sectionTypes _section)
    {
        ResourceForRecipe = resourceForRecipe;
        this.tier = _tier;
        outCome = ResourceToGetFromRecipe;
        sectionType = _section;
    }



    //Blocks

    public static Recipe WoodWall => new Recipe(new ResourceSlot[2] { new ResourceSlot(ResourceStruct.OakLog, 2), new ResourceSlot(ResourceStruct.WoodenStick, 1) }, new ResourceSlot(ResourceStruct.WoodWall, 1), 1, sectionTypes.Blocks);
    public static Recipe StoneWall => new Recipe(new ResourceSlot[2] { new ResourceSlot(ResourceStruct.Stone, 2), new ResourceSlot(ResourceStruct.WoodenStick, 1) }, new ResourceSlot(ResourceStruct.Stone, 1), 1, sectionTypes.Blocks);
    //Decorations
    public static Recipe Flower => new Recipe(new ResourceSlot[1] { new ResourceSlot(ResourceStruct.Flower, 5) }, new ResourceSlot(ResourceStruct.Flower, 5), 0, sectionTypes.Decorations);
    //Nature
    //Weapons
    public static Recipe WoodenSword => new Recipe(new ResourceSlot[2] { new ResourceSlot(ResourceStruct.OakLog, 2), new ResourceSlot(ResourceStruct.WoodenStick, 1) }, new ResourceSlot(ResourceStruct.WoodenSword, 1), 1, sectionTypes.Weapons);
    //Tools
    public static Recipe CraftingTable => new Recipe(new ResourceSlot[1] { new ResourceSlot(ResourceStruct.OakLog, 4) }, new ResourceSlot(ResourceStruct.CraftingTable, 1), 0, sectionTypes.Tools);
    public static Recipe woodenHoe => new Recipe(new ResourceSlot[2] { new ResourceSlot(ResourceStruct.OakLog, 1), new ResourceSlot(ResourceStruct.WoodenStick, 1) }, new ResourceSlot(ResourceStruct.WoodenHoe, 1), 1, sectionTypes.Tools);
    //Food
    public static Recipe Apple => new Recipe(new ResourceSlot[1] { new ResourceSlot(ResourceStruct.OakLog, 1) }, new ResourceSlot(ResourceStruct.Apple, 1), 1, sectionTypes.Food);















}

