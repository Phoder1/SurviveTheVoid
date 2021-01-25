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
    private int maxDurability;
    public int GetMaxDurability => maxDurability;

    // int tier
    [SerializeField]
    private int toolTier;
    public int GetToolTier => toolTier;

    [SerializeField]
    private float speedPrecentage;

    bool isActive = false;
    public float GetPrecentage => speedPrecentage;


    public bool SetGetIsActive { set { if (isActive != value) isActive = value; } get => isActive; }
}

