using System.Collections.Generic;
using UnityEngine;
public enum SurvivalStatType
{
    HP,
    Hunger,
    Temperature,
    AttackDMG,
    Thirst,
    Oxygen,
    Awakeness,
}
public enum PlayerStatType
{
    MaxHP,
    MaxHunger,
    MaxTemperature,
    MaxAttackDMG,
    MaxThirst,
    MaxOxygen,
    MaxAwakeness,
    EXP,
    EXPAmountToLevelUp,
    Level,
    MoveSpeed,
    GatheringSpeed
}
public class PlayerStats : MonoSingleton<PlayerStats>
{
    [SerializeField] private PlayerStatsSO playerStatsSO;
    private Dictionary<SurvivalStatType, SurvivalStat> survivalStatsDict;
    private Dictionary<PlayerStatType, PlayerStat> playerStatsDict;
    public override void Init() {
        survivalStatsDict = new Dictionary<SurvivalStatType, SurvivalStat>();
        foreach (SurvivalStat stat in playerStatsSO.GetSurvivalStats)
            survivalStatsDict.Add(stat.statType, stat);
        playerStatsDict = new Dictionary<PlayerStatType, PlayerStat>();
        foreach (SurvivalStat stat in playerStatsSO.GetSurvivalStats)
            playerStatsDict.Add(stat.maxStat.statType, stat.maxStat);
        foreach (PlayerStat stat in playerStatsSO.GetPlayerStats)
            playerStatsDict.Add(stat.statType, stat);
        playerStatsDict.Add(playerStatsSO.GetExpStat.statType, playerStatsSO.GetExpStat);
        ResetStats();
    }

    void ResetStats() {
        foreach (SurvivalStat stat in survivalStatsDict.Values)
            stat.Reset();
        foreach (PlayerStat stat in playerStatsDict.Values)
            stat.Reset();
    }
    public Stat GetStat(SurvivalStatType statType) => survivalStatsDict[statType];
    public Stat GetStat(PlayerStatType statType) => playerStatsDict[statType];
    public Stat GetStatMax(SurvivalStatType statType) => survivalStatsDict[statType].maxStat;
    public float GetStatValue(SurvivalStatType statType) => GetStat(statType).GetSetValue;
    public float GetStatValue(PlayerStatType statType) => GetStat(statType).GetSetValue;
    public void SetStatValue(SurvivalStatType statType, float value) => GetStat(statType).GetSetValue = value;
    public void SetStatValue(PlayerStatType statType, float value) => GetStat(statType).GetSetValue = value;
    public void AddStatValue(SurvivalStatType statType, float value) => GetStat(statType).GetSetValue += value;
    public void AddStatValue(PlayerStatType statType, float value) => GetStat(statType).GetSetValue += value;
    public float GetStatMaxValue(SurvivalStatType statType) => GetStatMax(statType).GetSetValue;
    public void SetStatMaxValue(SurvivalStatType statType, float value) => GetStatMax(statType).GetSetValue = value;
}
public abstract class Stat
{
    public float defaultValue;
    private protected float value;
    public abstract float GetSetValue { get; set; }


    public void Reset() {
        value = defaultValue;
    }
}
[System.Serializable]
public class SurvivalStat : Stat
{
    public SurvivalStatType statType;
    public PlayerStat maxStat;
    public override float GetSetValue {
        get => value;
        set {
            if (value != this.value) {
                this.value = Mathf.Clamp(value, 0, maxStat.GetSetValue);
                UIManager._instance.UpdateSurvivalBar(statType, value);
            }
        }
    }
}
[System.Serializable]
public class PlayerStat : Stat
{
    public PlayerStatType statType;
    protected PlayerStat(PlayerStatType stat) {
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
    protected ExpStat(PlayerStatType stat) : base(stat) {
    }

    public override float GetSetValue {
        get => value;
        set {
            if (value != this.value) {
                PlayerStats playerStats = PlayerStats._instance;
                this.value = Mathf.Max(value, 0);
                float XPtoLevel = playerStats.GetStatValue(PlayerStatType.EXPAmountToLevelUp);
                if (this.value >= XPtoLevel) {
                    playerStats.AddStatValue(PlayerStatType.Level, 1);
                    //Needs to be recursive
                    playerStats.AddStatValue(PlayerStatType.EXPAmountToLevelUp, XPtoLevel * 0.5f);
                    GetSetValue -= XPtoLevel;
                }
                else {
                    UIManager._instance.UpdateEXPbar();
                }
            }
        }
    }
}
