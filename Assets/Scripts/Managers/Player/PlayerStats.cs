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
    EXP,
    EXPAmountToLevelUp,
    Level,
    MoveSpeed,
    GatheringSpeed
}
public class PlayerStats : MonoSingleton<PlayerStats>
{
    [SerializeField] SurvivalStat[] survivalStats;
    [SerializeField] PlayerStat[] playerStats;
    [SerializeField] ExpStat expStat;
    private Dictionary<SurvivalStatType, SurvivalStat> survivalStatsDict;
    private Dictionary<PlayerStatType, PlayerStat> playerStatsDict;
    public override void Init() {
        foreach (SurvivalStat stat in survivalStats)
            survivalStatsDict.Add(stat.stat, stat);
        foreach (PlayerStat stat in playerStats)
            playerStatsDict.Add(stat.stat, stat);
        playerStatsDict.Add(expStat.stat, expStat);
        ResetStats();
    }

    void ResetStats() {
        foreach (SurvivalStat stat in survivalStatsDict.Values)
            stat.Reset();
        foreach (PlayerStat stat in playerStatsDict.Values)
            stat.Reset();
    }
    private Stat GetStat(SurvivalStatType statType) => survivalStatsDict[statType];
    private Stat GetStat(PlayerStatType statType) => playerStatsDict[statType];
    public float GetStatValue(SurvivalStatType statType) => GetStat(statType).GetSetValue;
    public float GetStatValue(PlayerStatType statType) => GetStat(statType).GetSetValue;
    public void SetStatValue(SurvivalStatType statType, float value) => GetStat(statType).GetSetValue = value;
    public void SetStatValue(PlayerStatType statType, float value) => GetStat(statType).GetSetValue = value;
    public void AddStatValue(SurvivalStatType statType, float value) => GetStat(statType).GetSetValue += value;
    public void AddStatValue(PlayerStatType statType, float value) => GetStat(statType).GetSetValue += value;
    public float GetStatMax(SurvivalStatType statType) => survivalStatsDict[statType].maxValue;
    public void SetStatMax(SurvivalStatType statType, float value) => survivalStatsDict[statType].maxValue = value;
    private abstract class Stat
    {
        public float defaultValue;
        [SerializeField] private protected float value;
        public abstract float GetSetValue { get; set; }


        public void Reset() {
            value = defaultValue;
        }
    }
    [System.Serializable]
    private class SurvivalStat : Stat
    {
        public SurvivalStatType stat;
        public float maxValue;
        public override float GetSetValue {
            get => value;
            set {
                if (value != this.value) {
                    this.value = Mathf.Clamp(value, 0, maxValue);
                    UIManager._instance.UpdateSurvivalBar(stat, value);
                }
            }
        }
    }
    [System.Serializable]
    private class PlayerStat : Stat
    {
        public PlayerStatType stat;
        protected PlayerStat(PlayerStatType stat) {
            this.stat = stat;
        }
        
        public override float GetSetValue {
            get => value;
            set {
                this.value = Mathf.Max(value, 0);
            }
        }
    }
    [System.Serializable]
    private class ExpStat : PlayerStat
    {
        protected ExpStat(PlayerStatType stat) : base(stat) {
        }

        public override float GetSetValue {
            get => value;
            set {
                if (value != this.value) {
                    this.value = Mathf.Max(value, 0);
                    float XPtoLevel = _instance.GetStatValue(PlayerStatType.EXPAmountToLevelUp);
                    if (this.value > XPtoLevel) {
                        _instance.AddStatValue(PlayerStatType.Level, 1);
                        this.value = this.value - XPtoLevel;
                        _instance.AddStatValue(PlayerStatType.EXPAmountToLevelUp, XPtoLevel * 0.5f);
                    }
                    UIManager._instance.UpdateEXPbar();
                }
            }
        }
    }
}
