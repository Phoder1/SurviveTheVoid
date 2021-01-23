using System;
using UnityEngine;

public enum ToolType { Axe, Pickaxe, Hoe, Shovel, Hammer }

[CreateAssetMenu(fileName = "New Item Tool", menuName = "Items/" + "Tool")]
public class ToolItemSO : ItemSO
{

   



    [SerializeField]
    ToolType toolType;
    public ToolType GetToolType => toolType;


    [SerializeField]
    private float gatheringSpeed;
    public float getGatheringSpeed => gatheringSpeed;

    [SerializeField]
    private int maxDurability;
    public int getMaxDurability => maxDurability;
    private int? durabiltiy = null;
    public int GetSetDurability { set => durabiltiy = value; get
        {

            if (durabiltiy == null)
            {
                durabiltiy = maxDurability;
            }
            return durabiltiy.GetValueOrDefault();
        }  }
    // int tier
    [SerializeField]
    private int toolTier;
    public int getToolTier => toolTier;
    [SerializeField]
    private int Tier;
    [SerializeField]
    private float Precentage;

    bool isActive = false;
    public float GetPrecentage => Precentage;
    public int GetTier => Tier;

    public bool SetGetIsActive { set => isActive = value; get => isActive; }
}

