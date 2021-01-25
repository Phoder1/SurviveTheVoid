using System.Collections.Generic;
using UnityEngine;
public enum StatType
{
    None,
    HP,
    Food,
    Water,
    Air,
    Sleep,
    Temperature,

    MaxHP,
    MaxFood,
    MaxWater,
    MaxAir,
    MaxSleep,

    EXP,
    EXPtoNextLevel,
    Level,
    MoveSpeed,
    AttackDMG,
    GatheringSpeed
}
public class PlayerStats : MonoSingleton<PlayerStats>
{
    private Dictionary<StatType, Stat> StatsDict;
    #region comfort zone
    public float GetSetHP { get => GetStat(StatType.HP).GetSetValue; set => GetStat(StatType.HP).GetSetValue = value; }
    public float GetSetFood { get => GetStat(StatType.Food).GetSetValue; set => GetStat(StatType.Food).GetSetValue = value; }
    public float GetSetWater { get => GetStat(StatType.Water).GetSetValue; set => GetStat(StatType.Water).GetSetValue = value; }
    public float GetSetAir { get => GetStat(StatType.Air).GetSetValue; set => GetStat(StatType.Air).GetSetValue = value; }
    public float GetSetSleep { get => GetStat(StatType.Sleep).GetSetValue; set => GetStat(StatType.Sleep).GetSetValue = value; }
    public float GetSetMaxHP { get => GetStat(StatType.MaxHP).GetSetValue; set => GetStat(StatType.MaxHP).GetSetValue = value; }
    public float GetSetMaxFood { get => GetStat(StatType.MaxFood).GetSetValue; set => GetStat(StatType.MaxFood).GetSetValue = value; }
    public float GetSetMaxWater { get => GetStat(StatType.MaxWater).GetSetValue; set => GetStat(StatType.MaxWater).GetSetValue = value; }
    public float GetSetMaxAir { get => GetStat(StatType.MaxAir).GetSetValue; set => GetStat(StatType.MaxAir).GetSetValue = value; }
    public float GetSetMaxSleep { get => GetStat(StatType.MaxSleep).GetSetValue; set => GetStat(StatType.MaxSleep).GetSetValue = value; }
    public float GetSetEXP { get => GetStat(StatType.EXP).GetSetValue; set => GetStat(StatType.EXP).GetSetValue = value; }
    public float GetSetEXPtoNextLevel { get => GetStat(StatType.EXPtoNextLevel).GetSetValue; set => GetStat(StatType.EXPtoNextLevel).GetSetValue = value; }
    public float GetSetLevel { get => GetStat(StatType.Level).GetSetValue; set => GetStat(StatType.Level).GetSetValue = value; }
    public float GetSetMoveSpeed { get => GetStat(StatType.MoveSpeed).GetSetValue; set => GetStat(StatType.MoveSpeed).GetSetValue = value; }
    public float GetSetAttackDMG { get => GetStat(StatType.AttackDMG).GetSetValue; set => GetStat(StatType.AttackDMG).GetSetValue = value; }
    public float GetSetGatheringSpeed { get => GetStat(StatType.GatheringSpeed).GetSetValue; set => GetStat(StatType.GatheringSpeed).GetSetValue = value; }
    #endregion
    public override void Init() {
        StatsDict = new Dictionary<StatType, Stat>();
        FillDictionary();
        AddReactions();
        GameManager.RespawnEvent += DeathReset;
        ResetStats();

    }

    public void ResetStats() {
        foreach (Stat stat in StatsDict.Values)
            stat.Reset();
    }

    private void FillDictionary() {
        Stat maxStat;
        //HP
        maxStat = AddToDict(StatType.MaxHP, 100);
        AddToDict(StatType.HP, 2, maxStat);
        //Food
        maxStat = AddToDict(StatType.MaxFood, 100);
        AddToDict(StatType.Food, 20, maxStat);
        //Water
        maxStat = AddToDict(StatType.MaxWater, 100);
        AddToDict(StatType.Water, 100, maxStat);
        //Air
        maxStat = AddToDict(StatType.MaxAir, 100);
        AddToDict(StatType.Air, 100, maxStat);
        //Sleep
        maxStat = AddToDict(StatType.MaxSleep, 100);
        AddToDict(StatType.Sleep, 100, maxStat);

        //Other
        AddToDict(StatType.Temperature, 100);
        AddToDict(StatType.EXPtoNextLevel, 100);
        AddToDict(StatType.Level, 1);
        AddToDict(StatType.MoveSpeed, 1);
        AddToDict(StatType.GatheringSpeed, 1);
        AddToDict(StatType.AttackDMG, 100);

        ExpStat stat = new ExpStat(
            StatType.EXP,
            0,
            maxStat
        );
        StatsDict.Add(StatType.EXP, stat);
    }
    private Stat AddToDict(StatType statType, float defaultValue, Stat maxStat = null) {
        Stat stat = new Stat(
                statType,
                defaultValue,
                maxStat
            );
        StatsDict.Add(statType, stat);
        return stat;
    }
    private void AddReactions() {
        EffectData hpLoseEffect = new EffectData(StatType.HP, EffectType.OverTime, -1f, Mathf.Infinity, 1f, false, false);
        EffectData hpRegenEffect = new EffectData(StatType.HP, EffectType.OverTime, 1f, Mathf.Infinity, 1f, false, false);

        AddReaction(StatType.Food, new ReactionEffect(true, true, 4, new EffectData[1] { hpLoseEffect }));
        AddReaction(StatType.Food, new ReactionEffect(true, false, 95, new EffectData[1] { hpRegenEffect }));
        AddReaction(StatType.HP, new DeathReaction(false, true, 0));

        void AddReaction(StatType statType, Reaction reaction) => GetStat(statType).AddReaction(reaction);
    }
    public Stat GetStat(StatType statType) => StatsDict[statType];
    public float GetStatValue(StatType statType) => GetStat(statType).GetSetValue;
    public float AddToStatValue(StatType statType, float amount) => GetStat(statType).GetSetValue += amount;
    public Stat GetMaxStat(StatType statType)
        => StatsDict[statType].maxStat;
    private void DeathReset() {
        ResetStats();
        EquipManager.GetInstance.ReEquipStats();
    }
    class DeathReaction : Reaction
    {
        public DeathReaction(bool triggerInPercentage, bool triggerIfSmaller, float reactionTriggerValue)
            : base(triggerInPercentage, triggerIfSmaller, reactionTriggerValue) { }

        protected override void StartReaction() => GameManager.OnDeath();
    }
}
[System.Serializable]
public class Stat
{
    public StatType statType;
    [HideInInspector] public Stat maxStat;
    private List<Reaction> reactions;
    public float defaultValue;
    private protected float value;
    public bool GetIsCapped => maxStat != null;

    public Stat(StatType statType, float defaultValue, Stat maxStat) {
        this.statType = statType;
        this.maxStat = maxStat;
        this.maxStat = maxStat;
        this.defaultValue = defaultValue;
    }
    public virtual float GetSetValue {
        get => value;
        set {
            this.value = Mathf.Max(value, 0);
            if (maxStat != null)
                this.value = Mathf.Min(this.value, maxStat.GetSetValue);
            UIManager._instance.UpdateSurvivalBar(this);
            if (reactions != null)
                foreach (Reaction reaction in reactions)
                    reaction.CheckIfReactionEligible(this);
        }
    }
    public void Reset() {
        if (reactions != null) {
            foreach (Reaction reaction in reactions) {
                reaction.Reset();
            }
        }

        GetSetValue = defaultValue;
    }
    public void AddReaction(Reaction reaction) {
        if (reactions == null)
            reactions = new List<Reaction>();
        reactions.Add(reaction);
    }
}







[System.Serializable]
public class ExpStat : Stat
{
    public ExpStat(StatType statType, float defaultValue, Stat maxStat) : base(statType, defaultValue, maxStat) {
    }

    public override float GetSetValue {
        get => value;
        set {
            PlayerStats playerStats = PlayerStats._instance;
            this.value = Mathf.Max(value, 0);
            float XPtoLevel = playerStats.GetStat(StatType.EXPtoNextLevel).GetSetValue;
            if (this.value >= XPtoLevel) {
                playerStats.GetStat(StatType.Level).GetSetValue += 1;
                //Needs to be recursive
                playerStats.GetStat(StatType.EXPtoNextLevel).GetSetValue *= 1.5f;
                GetSetValue -= XPtoLevel;
            }
            else {
                UIManager._instance.UpdateExpAndLvlBar();
            }
        }
    }
}

public abstract class Reaction
{
    private protected bool triggerInPercentage, triggerIfSmaller;
    private protected float reactionTriggerValue;
    public Reaction(bool triggerInPercentage, bool triggerIfSmaller, float reactionTriggerValue) {
        this.triggerInPercentage = triggerInPercentage;
        this.triggerIfSmaller = triggerIfSmaller;
        this.reactionTriggerValue = reactionTriggerValue;
    }
    public bool CheckIfReactionEligible(Stat stat) {
        float tempValueCheck = reactionTriggerValue;
        if (triggerInPercentage && stat.GetIsCapped) {
            tempValueCheck = stat.maxStat.GetSetValue * (reactionTriggerValue / 100);
        }
        if ((triggerIfSmaller && stat.GetSetValue <= tempValueCheck) || (!triggerIfSmaller && stat.GetSetValue >= tempValueCheck)) {
            StartReaction();
        }
        else {
            StopReaction();
        }
        return false;
    }
    protected virtual void StartReaction() { }
    protected virtual void StopReaction() { }
    public virtual void Reset() { }
}
public class ReactionEffect : Reaction
{
    private EffectData[] effectsData;
    private EffectController[] effectsCont;
    private bool reactionEffectRunning;

    public ReactionEffect(bool triggerInPercentage, bool triggerIfSmaller, float reactionTriggerValue, EffectData[] effectsData)
        : base(triggerInPercentage, triggerIfSmaller, reactionTriggerValue) {
        this.effectsData = effectsData;
    }

    private EffectController[] GetEffectsCont {
        get {
            if (effectsCont == null) {
                effectsCont = EffectHandler._instance.CreateControllers(effectsData, new float[effectsData.Length]);
            }
            return effectsCont;
        }
    }

    protected override void StartReaction() {
        if (!reactionEffectRunning) {
            EffectHandler._instance.BeginAllEffects(effectsData, GetEffectsCont);
            reactionEffectRunning = true;
        }
    }
    protected override void StopReaction() {
        if (reactionEffectRunning) {
            EffectHandler._instance.StopAllEffects(GetEffectsCont);
            reactionEffectRunning = false;
        }
    }
    public override void Reset() {
        StopReaction();
    }
}