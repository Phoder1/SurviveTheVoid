
using UnityEngine;


public enum EquipType
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
    private EquipType equipType;
    public EquipType GetEquipType => equipType;

    [SerializeField]
    private int maxDurability;
    public int GetMaxDurability => maxDurability;


    // gear tier 
    [SerializeField]
    private int gearTier;
    public int GetGearTier => gearTier;



    public EffectData[] effectDatas;

}

