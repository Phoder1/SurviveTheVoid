using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStruct
{
        public int id;
    public int maxStackSize;
        public string name;
        public string description;
        ResourceStruct(int _id, string _name, string _description, int _maxStackSize)
        {
            id = _id;
            name = _name;
            description = _description;
        maxStackSize = _maxStackSize;
        }



        public static ResourceStruct Flower { get { return new ResourceStruct(0, "Flower", "Just a flower so you won't feel lonely.", 69); } }
        public static ResourceStruct OakLog { get { return new ResourceStruct(1, "Oak Log", "A log from a oak tree.", 69); } }
        public static ResourceStruct CraftingTable { get { return new ResourceStruct(2, "Crafting Table", "Work you lazy ass.", 69); } }
        public static ResourceStruct WoodenStick { get { return new ResourceStruct(3, "Wooden Stick", "A Stick", 69); } }
        public static ResourceStruct WoodenSword { get { return new ResourceStruct(4, "Wooden Sword", "A Wooden Sword", 69); } }
        public static ResourceStruct WoodenHoe { get { return new ResourceStruct(5, "Wooden Hoe", "A Wooden Hoe", 69); } }



    
}
