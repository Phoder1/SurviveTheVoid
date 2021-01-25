
using System;
using UnityEngine;


public enum GearType
        {
            Helmet,
            Chest,
            Gloves,
            Legging,
            Shoes
        }
[CreateAssetMenu(fileName = "New Gear Item", menuName = "Items/" + "Gear")]
public class GearItemSO : ItemSO
{
    [SerializeField]
    private GearType equipType;
    public GearType GetEquipType => equipType;

    [SerializeField]
    private int maxDurability;
    public int GetMaxDurability => maxDurability;


    // gear tier 
    [SerializeField]
    private int gearTier;
    public int GetGearTier => gearTier;



    public EquipStats[] equipstats;

}
[Serializable]
public class EquipStats {
    public StatType statType;
    public bool isPercentage;
     public float amount;
}