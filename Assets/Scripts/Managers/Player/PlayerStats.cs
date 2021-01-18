using System.Collections.Generic;
using UnityEngine;
public enum StatType
{
    HP,
    Food,
    Temperature,
    Water,
    Air,
    Sleep,

    MaxHP,
    MaxFood,
    MaxTemperature,
    MaxWater,
    MaxAir,
    MaxSleep,

    EXP,
    EXPAmountToLevelUp,
    Level,
    MoveSpeed,
    AttackDMG,
    GatheringSpeed
}
public class PlayerStats : MonoSingleton<PlayerStats>
{
    [SerializeField] private PlayerStatsSO playerStatsSO;
    private Dictionary<StatType, Stat> StatsDict;
    public override void Init() {
        StatsDict = new Dictionary<StatType, Stat>();
        foreach (SurvivalStat stat in playerStatsSO.GetSurvivalStats) {
            StatsDict.Add(stat.statType, stat);
            StatsDict.Add(stat.maxStat.statType, stat.maxStat);
        }
        foreach (PlayerStat stat in playerStatsSO.GetPlayerStats)
            StatsDict.Add(stat.statType, stat);
        StatsDict.Add(playerStatsSO.GetExpStat.statType, playerStatsSO.GetExpStat);
        ResetStats();
    }

    void ResetStats() {
        foreach (Stat stat in StatsDict.Values)
            stat.Reset();
    }
    public Stat GetStat(StatType statType) => StatsDict[statType];
    public bool TryGetMaxStat(StatType statType, out PlayerStat statMax) {
        if(StatsDict[statType] is SurvivalStat survivalStat) {
            statMax = survivalStat.maxStat;
            return true;
        }
        statMax = null;
        return false;
    }
    public float GetStatValue(StatType statType) => GetStat(statType).GetSetValue;
    public void SetStatValue(StatType statType, float value) => GetStat(statType).GetSetValue = value;
    public void AddStatValue(StatType statType, float value) => GetStat(statType).GetSetValue += value;
}
public abstract class Stat
{
    public float cooldown;
    public float overtimeCooldown;
    public float defaultValue;
    private protected float value;
    public abstract float GetSetValue { get; set; }


    public void Reset() {
        GetSetValue = defaultValue;
    }
}
[System.Serializable]
public class SurvivalStat : Stat
{
    public StatType statType;
    public PlayerStat maxStat;
    [Header("High reaction activates above certain percentage")]
    [SerializeField] private bool highReactionEnabled;
    [Header("Value in precentage:")]
    [SerializeField] private float highReactionValue;
    [SerializeField] private EffectData[] highEffectsData;
    private EffectController[] highEffectsCont;
    private bool highEffectsRunning;
    private EffectController[] GetHighEffectsCont {
        get {
            if (highEffectsCont == null) {

                highEffectsCont = EffectHandler._instance.CreateControllers(highEffectsData, new float[highEffectsData.Length]);
            }
            return highEffectsCont;
        }
    }

    [Header("Mid reaction activates below a fixed value")]
    [SerializeField] private bool midReactionEnabled;
    [Header("Fixed value:")]
    [SerializeField] private float midReactionValue;
    [SerializeField] private EffectData[] midEffectsData;
    private EffectController[] midEffectsCont;
    private EffectController[] GetMidEffectsCont {
        get {
            if (midEffectsCont == null) {
                midEffectsCont = EffectHandler._instance.CreateControllers(midEffectsData, new float[midEffectsData.Length]);
            }
            return midEffectsCont;
        }
    }
    private bool midEffectsRunning;


    [Header("Low reaction activates When value at 0")]
    [SerializeField] private bool lowReactionEnabled;
    [SerializeField] EffectData[] lowEffectsData;
    private EffectController[] lowEffectsCont;
    private EffectController[] GetLowEffectsCont {
        get {
            if (lowEffectsCont == null) {

                lowEffectsCont = EffectHandler._instance.CreateControllers(lowEffectsData, new float[lowEffectsData.Length]);
            }
            return lowEffectsCont;
        }
    }
    private bool lowEffectsRunning;


    public override float GetSetValue {
        get => value;
        set {
            if (value != this.value) {
                this.value = Mathf.Clamp(value, 0, maxStat.GetSetValue);
                UIManager._instance.UpdateSurvivalBar(this, value);
                //if (highReactionEnabled) {
                //    if (!highEffectsRunning && this.value >= highReactionValue)
                //        StartHighReaction();
                //    else if (highEffectsRunning && this.value < highReactionValue)
                //        StopHighReaction();
                //}
                //if (midReactionEnabled) {
                //    if (!midEffectsRunning && this.value <= midReactionValue)
                //        StartMidReaction();
                //    else if (midEffectsRunning && this.value > midReactionValue)
                //        StopMidReaction();
                //}
                //if (lowReactionEnabled) {
                //    if (!lowEffectsRunning && this.value == 0)
                //        StartLowReaction();
                //    else if (lowEffectsRunning && this.value > 0)
                //        StopLowReaction();
                //}
            }
        }
    }
    ////Starts when stat is lower then midReactionValue
    //private void StartHighReaction() {
    //    EffectHandler._instance.BeginAllEffects(highEffectsData, GetHighEffectsCont);
    //    highEffectsRunning = true;
    //}

    //private void StopHighReaction() {
    //    EffectHandler._instance.StopAllEffects(highEffectsCont);
    //    highEffectsRunning = false;
    //}
    ////Starts when stat is lower then midReactionValue
    //private void StartMidReaction() {
    //    EffectHandler._instance.BeginAllEffects(midEffectsData, GetMidEffectsCont);
    //    midEffectsRunning = true;
    //}
    //private void StopMidReaction() {
    //    EffectHandler._instance.StopAllEffects(midEffectsCont);
    //    midEffectsRunning = false;

    //}
    ////Starts when stat is at 0
    //private void StartLowReaction() {
    //    EffectHandler._instance.BeginAllEffects(lowEffectsData, GetLowEffectsCont);
    //    lowEffectsRunning = true;
    //}
    //private void StopLowReaction() {
    //    EffectHandler._instance.StopAllEffects(lowEffectsCont);
    //    lowEffectsRunning = false;
    //}
}
[System.Serializable]
public class PlayerStat : Stat
{
    public StatType statType;
    protected PlayerStat(StatType stat) {
        this.statType = stat;
    }

    public override float GetSetValue {
        get => value;
        set {
            this.value = Mathf.Max(value, 0);
        }
    }
}
[System.Serializable]
public class ExpStat : PlayerStat
{

    protected ExpStat(StatType stat) : base(stat) {
    }

    public override float GetSetValue {
        get => value;
        set {
            PlayerStats playerStats = PlayerStats._instance;
            this.value = Mathf.Max(value, 0);
            float XPtoLevel = playerStats.GetStatValue(StatType.EXPAmountToLevelUp);
            if (this.value >= XPtoLevel) {
                playerStats.AddStatValue(StatType.Level, 1);
                //Needs to be recursive
                playerStats.AddStatValue(StatType.EXPAmountToLevelUp, XPtoLevel * 0.5f);
                GetSetValue -= XPtoLevel;
            }
            else {
                UIManager._instance.UpdateExpAndLvlBar();
            }
        }
    }
}
