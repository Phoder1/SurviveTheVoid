using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemName
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



public enum ItemType
{
    None,

    Consumable,
    Building,
    Equipable

}

public enum EquipableType
{
    None,

    Helmet,
    Chest,
    HandOne,
    HandTwo,
    Legging,

    Sword,
    Axe
}





[CreateAssetMenu(fileName = "New Item", menuName = "Crafting/" + "Item")]
public class ItemSO : ScriptableObject
{
    [SerializeField]
    private int maxStackSize;
    public int getmaxStackSize => maxStackSize; 

    [SerializeField]
    private string description;
    public string getDescription => description; 

    [SerializeField]
    private ItemName itemEnum;
    public ItemName getItemEnum => itemEnum; 

    [SerializeField]
    private Sprite sprite;
    public Sprite getsprite { get => sprite; }

}


