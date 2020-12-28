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

    [SerializeField]
    private ItemSlot[] ItemCostArr;
    public ItemSlot[] getitemCostArr {get => ItemCostArr; }

    [SerializeField]
    private ItemSlot outcomeItem;
    public ItemSlot getoutcomeItem { get => outcomeItem; }

    [SerializeField]
    private int tier;
    public int getTier { get => tier; }

    [SerializeField]
    private SectionEnum section;
    public SectionEnum getSection { get => section; }

    [SerializeField]
    private bool isUnlocked;
    public bool getisUnlocked { get => isUnlocked; }
  
    public void UpdateIfRecipeUnlocked(bool _unlocked)
    {
        isUnlocked = _unlocked;
    }
}

