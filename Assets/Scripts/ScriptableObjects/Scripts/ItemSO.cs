using System.Collections;
using System.Collections.Generic;
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
    WoodBlock,
    Stone,
    StoneBlock
}



[CreateAssetMenu(fileName = "New Resource", menuName = "Crafting/" + "Resource")]
public class ItemSO : ScriptableObject
{
    public int maxStackSize;
    public string description;
    public ResourceType resourceEnum;
    public Sprite sprite;
}
