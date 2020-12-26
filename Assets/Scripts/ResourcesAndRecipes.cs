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
public class ItemPackSO : ScriptableObject
{
    public ItemSO[] resources;

}

[CreateAssetMenu(fileName = "New Resource", menuName = "Crafting/" + "Resource")]
public class ItemSO : ScriptableObject
{
    public int maxStackSize;
    public string description;
    public ResourceType resourceEnum;
    public Sprite sprite;
}
[Serializable]
public class ItemSlot
{
    public ItemSO resource;
    public int amount;

    public ItemSlot(ItemSO resource, int amount) {
        this.resource = resource;
        this.amount = amount;
    }
}

[CreateAssetMenu(fileName = "New Recipe Pack", menuName = "Crafting/" + "Recipe Pack")]
public class RecipePackSO : ScriptableObject
{
    public RecipeSO[] resources;
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/" + "Recipe")]
public class RecipeSO : ScriptableObject
{
    public ItemSlot[] ResourcesCost;
    public ItemSlot Resource;
    public int tier;
    public sectionTypes sectionType;
}


