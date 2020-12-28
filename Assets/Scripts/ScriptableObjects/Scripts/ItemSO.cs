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
    [SerializeField]
    private int maxStackSize;
    public int getmaxStackSize { get => maxStackSize; }

    [SerializeField]
    private string description;
    public string getDescription { get => description; }

    [SerializeField]
    private ResourceType itemEnum;
    public ResourceType getItemEnum { get => itemEnum; }

    [SerializeField]
    private Sprite sprite;
    public Sprite getsprite { get => sprite; }

}
