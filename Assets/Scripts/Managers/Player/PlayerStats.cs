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
    [SerializeField] private Reaction[] reactions;



    public override float GetSetValue {
        get => value;
        set {
            if (value != this.value) {
                this.value = Mathf.Clamp(value, 0, maxStat.GetSetValue);
                UIManager._instance.UpdateSurvivalBar(this, value);
                //foreach (Reaction reaction in reactions)
                //    reaction.CheckIfReactionEligible(GetSetValue, maxStat.GetSetValue);
            }
        }
    }
    [System.Serializable]
    private class Reaction
    {
        [SerializeField] private bool isPercentage, checkSmaller;
        [SerializeField] private float reactionStartValue;
        [SerializeField] private EffectData[] effectsData;
        private EffectController[] effectsCont;
        private bool effectsRunning;
        private EffectController[] GetEffectsCont {
            get {
                if (effectsCont == null) {
                    effectsCont = EffectHandler._instance.CreateControllers(effectsData, new float[effectsData.Length]);
                }
                return effectsCont;
            }
        }
        public bool CheckIfReactionEligible(float value, float maxValue) {
            float tempValueCheck = reactionStartValue;
            if (isPercentage) {
                tempValueCheck = maxValue * (reactionStartValue / 100);
            }
            if ((checkSmaller && value < tempValueCheck) || (!checkSmaller && value > tempValueCheck)) {
                if (!effectsRunning)
                    StartReaction();
            }else {
                if (effectsRunning)
                    StopReaction();
            }
            return false;
        }
        private  void StartReaction() {
            EffectHandler._instance.BeginAllEffects(effectsData, effectsCont);
            effectsRunning = true;
        }
        private void StopReaction() {
            EffectHandler._instance.StopAllEffects(effectsCont);
            effectsRunning = false;

        }
    }
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
