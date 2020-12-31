using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Gear Item", menuName = "Items/" + "Gear")]
public class GearItemSO : ItemSO
{


    public enum GearType
    {
        None,

        Helmet,
        Chest,
        Gloves,
        Legging,
        Shoes
    }
    [SerializeField]
    private GearType gearType;
    public GearType GetGearTpye => gearType;

    [SerializeField]
    private int maxDurability;
    public int GetMaxDurability => maxDurability;


    // gear tier 
    [SerializeField]
    private int gearTier;
    public int GetGearTier => gearTier;






}

