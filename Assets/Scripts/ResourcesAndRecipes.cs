using System.Collections;
using System;
using UnityEngine;

public enum ResourceType
{
    Flower,
    OakLog,
    CraftingTable,
    WoodenStick,
    WoodenSword,
    WoodenHoe,
    Apple,
    WoodWall,
    Stone
}

public enum sectionTypes
{
    Blocks,
    Decorations,
    Nature,
    Weapons,
    Tools,
    Food
}

[CreateAssetMenu(fileName = "New Resource Pack",menuName = "Crafting/" + "Resource Pack")]
public class ResourcesPackSO : ScriptableObject
{
    public ResourceSO[] resources;

}

[CreateAssetMenu(fileName = "New Resource", menuName = "Crafting/" + "Resource")]
public class ResourceSO : ScriptableObject
{
    public int maxStackSize;
    public string description;
    public ResourceType resourceEnum;
    public Sprite sprite;
}
[Serializable]
public struct ResourceSlot
{
    public ResourceSO resource;
    public int amount;
}

[CreateAssetMenu(fileName = "New Recipe Pack", menuName = "Crafting/" + "Recipe Pack")]
public class RecipePackSO : ScriptableObject
{
    public RecipeSO[] resources;
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/" + "Recipe")]
public class RecipeSO : ScriptableObject
{
    public ResourceSlot[] ResourcesCost;
    public ResourceSlot Resource;
    public int tier;
    public sectionTypes sectionType;
}


