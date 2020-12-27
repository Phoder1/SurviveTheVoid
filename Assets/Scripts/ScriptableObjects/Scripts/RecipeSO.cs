using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SectionEnum
{
    Blocks,
    Furnitures,
    Plants,
    Weapons,
    Tools,
    Food
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "Crafting/" + "Recipe")]
public class RecipeSO : ScriptableObject
{
    public ItemSlot[] itemCostArr;
    public ItemSlot outcomeItem;
    public int tier;
    public SectionEnum section;
    public bool isUnlocked;
  

}

