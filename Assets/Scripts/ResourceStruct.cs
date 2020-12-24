using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStruct
{

    public int maxStackSize;

    public string description;
    public ResourceType resourceEnum;
    ResourceStruct(ResourceType _resourceEnum, string _description, int _maxStackSize)
    {
        resourceEnum = _resourceEnum;
        description = _description;
        maxStackSize = _maxStackSize;
    }



    public static ResourceStruct Flower { get { return new ResourceStruct(ResourceType.Flower, "Just a flower so you won't feel lonely.", 69); } }
    public static ResourceStruct OakLog { get { return new ResourceStruct(ResourceType.OakLog, "A log from a oak tree.", 69); } }
    public static ResourceStruct CraftingTable { get { return new ResourceStruct(ResourceType.CraftingTable, "Work you lazy ass.", 69); } }
    public static ResourceStruct WoodenStick { get { return new ResourceStruct(ResourceType.WoodenStick, "A Stick", 69); } }
    public static ResourceStruct WoodenSword { get { return new ResourceStruct(ResourceType.WoodenSword, "A Wooden Sword", 1); } }
    public static ResourceStruct WoodenHoe { get { return new ResourceStruct(ResourceType.WoodenHoe, "A Wooden Hoe", 1); } }
    public static ResourceStruct Apple { get { return new ResourceStruct(ResourceType.Apple, "GoldenApple", 69); } }
    public static ResourceStruct WoodWall { get { return new ResourceStruct(ResourceType.WoodWall, "A wall made out of wood", 69); } }
    public static ResourceStruct Stone { get { return new ResourceStruct(ResourceType.Stone, "Stone age baby!", 69); } }



}

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


