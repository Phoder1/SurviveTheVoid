using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Tool", menuName = "Crafting/" + "Tool")]
public class ToolItemSO : ItemSO
{
    [SerializeField]
    EquipableType equipType;

    [SerializeField]
    private float gatheringSpeed;
    public float getGatheringSpeed => gatheringSpeed;

    [SerializeField]
    private int durability;
    public int getDurability => durability;

    // int tier
    [SerializeField]
    private int toolTier;
    public int getToolTier => toolTier;

}
