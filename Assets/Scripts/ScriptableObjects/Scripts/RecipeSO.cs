using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SectionEnum
{
    Blocks,
    Decorations,
    Nature,
    Weapons,
    Tools,
    Food
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/" + "Recipe")]
public class RecipeSO : ScriptableObject
{
    public ItemSlot[] ResourcesCost;
    public ItemSlot Resource;
    public int tier;
    public SectionEnum section;

  

}

