using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Tool", menuName = "Items/" + "Tool")]
public class ToolItemSO : ItemSO
{

    public enum ToolType
    {
        Sword,
        Axe
    }



    [SerializeField]
    ToolType equipType;
    public ToolType GetEquipType => equipType;


    [SerializeField]
    private float gatheringSpeed;
    public float getGatheringSpeed => gatheringSpeed;

    [SerializeField]
    private int maxDurability;
    public int getMaxDurability => maxDurability;
    
    // int tier
    [SerializeField]
    private int toolTier;
    public int getToolTier => toolTier;

}
