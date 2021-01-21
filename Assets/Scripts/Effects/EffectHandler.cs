using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EffectHandler : MonoSingleton<EffectHandler>
{

    PlayerStats playerStats;
    static Dictionary<StatType, StatControllers> ConsumablesEffectsDict;
    [SerializeField] float cooldownBeforeStart;
    public override void Init() {
        playerStats = PlayerStats._instance;
        
        FillDictionary();
        
        StopCoroutine(SurvivalEffects());
        StartCoroutine(SurvivalEffects());
    }

    private void FillDictionary() {
        ConsumablesEffectsDict = new Dictionary<StatType, StatControllers>();

        AddToDict(StatType.HP, 3, 3);
        AddToDict(StatType.MaxHP, 3, 3);
        AddToDict(StatType.Food, 3, 3);
        AddToDict(StatType.MaxFood, 3, 3);
        AddToDict(StatType.Water, 3, 3);
        AddToDict(StatType.MaxWater, 3, 3);
        AddToDict(StatType.Air, 3, 3);
        AddToDict(StatType.MaxAir, 3, 3);
        AddToDict(StatType.Sleep, 3, 3);
        AddToDict(StatType.MaxSleep, 3, 3);
        AddToDict(StatType.Temperature, 3, 3);
        AddToDict(StatType.Level, 3, 3);
        AddToDict(StatType.EXP, 3, 3);
        AddToDict(StatType.EXPtoNextLevel, 3, 3);
        AddToDict(StatType.MoveSpeed, 3, 3);
        AddToDict(StatType.AttackDMG, 3, 3);
        AddToDict(StatType.GatheringSpeed, 3, 3);
    }
    private void AddToDict(StatType statType, float cooldown, float overtimeCooldown) {
        Stat stat = playerStats.GetStat(statType);
        ConsumablesEffectsDict.Add(statType, new StatControllers(new EffectController(stat, cooldown), new EffectController(stat, overtimeCooldown)));
    }


    IEnumerator SurvivalEffects() {

        //Declartion of Effects

        EffectData hungerEffect = new EffectData() {
            effectStatType = StatType.Food,
            effectType = EffectType.OverTime,
            inPercentage = false,
            isRelativeToMax = false,
            amount = -0.5f,
            tickTime = 1f,
            duration = Mathf.Infinity
        };
        EffectData thirstEffect = new EffectData() {
            effectStatType = StatType.Water,
            effectType = EffectType.OverTime,
            inPercentage = false,
            isRelativeToMax = false,
            amount = -0.5f,
            tickTime = 1f,
            duration = Mathf.Infinity
        };
        EffectData oxygenEffect = new EffectData() {
            effectStatType = StatType.Air,
            effectType = EffectType.OverTime,
            inPercentage = false,
            isRelativeToMax = false,
            amount = -0.5f,
            tickTime = 1f,
            duration = Mathf.Infinity
        };
        EffectData sleepEffect = new EffectData() {
            effectStatType = StatType.Sleep,
            effectType = EffectType.OverTime,
            inPercentage = false,
            isRelativeToMax = false,
            amount = -0.5f,
            tickTime = 1f,
            duration = Mathf.Infinity
        };


        yield return new WaitForSeconds(cooldownBeforeStart);




        //Applying Effects:

        EffectController worldEffect;


        worldEffect = new EffectController(playerStats.GetStat(StatType.Food), 3f);
        worldEffect.Begin(hungerEffect);

        worldEffect = new EffectController(playerStats.GetStat(StatType.Water), 3f);
        worldEffect.Begin(thirstEffect);

        worldEffect = new EffectController(playerStats.GetStat(StatType.Air), 3f);
        worldEffect.Begin(oxygenEffect);

        worldEffect = new EffectController(playerStats.GetStat(StatType.Sleep), 3f);
        worldEffect.Begin(sleepEffect);

    }


    public static EffectController GetStatController(EffectData effect) {
        if (ConsumablesEffectsDict.TryGetValue(effect.effectStatType, out StatControllers effectControllers)) 
            return effectControllers.GetController(effect.effectType);
        return null;
    }
    public static EffectController[] GetStatControllers(EffectData[] effectsData) {
        if (effectsData.Length == 0)
            return null;
        EffectController[] controllers = new EffectController[effectsData.Length];
        for (int i = 0; i < effectsData.Length; i++) 
            controllers[i] = GetStatController(effectsData[i]);
        return controllers;
    }
    public void BeginAllConsumeableEffects(EffectData[] effectsData)
        => BeginAllEffects(effectsData, GetStatControllers(effectsData));
    public void BeginAllEffects(EffectData[] effectsData, EffectController[] effectController) {
        for (int i = 0; i < effectsData.Length; i++) 
            if (effectsData[i] != null) 
                effectController[i].Begin(effectsData[i]);
    }
    public void StopAllEffects(EffectController[] effects) {
        foreach (EffectController effect in effects)
            effect.Stop();
    }
    public EffectController[] CreateControllers(EffectData[] effectsData, float[] coolDowns) {
        if (effectsData.Length == 0)
            return null;
        PlayerStats playerStats = PlayerStats._instance;
        EffectController[]  controllers = new EffectController[effectsData.Length];
        for (int i = 0; i < effectsData.Length; i++) 
            controllers[i] = new EffectController(playerStats.GetStat(effectsData[i].effectStatType), coolDowns[i]);
        return controllers;
    }
    public bool GetEffectCoolDown(ConsumableItemSO Item) {

        if (Item == null || Item.Effects.Length == 0)
            return false;

        EffectController abstStat;

        bool CanUseTheItem = true;
        foreach (var effect in Item.Effects) {

            abstStat = GetStatController(effect);

            if (abstStat == null)
                continue;


            CanUseTheItem = CanUseTheItem && !abstStat.isOnCoolDown;


            if (CanUseTheItem == false)
                return false;

        }

        return CanUseTheItem;

    }
}
public class StatControllers
{
    public readonly EffectController valueController;
    public readonly EffectController regenerationController;

    public StatControllers(EffectController valueController, EffectController overTimeController) {
        this.valueController = valueController;
        this.regenerationController = overTimeController;
    }
    public EffectController GetController(EffectType effectType) {
        switch (effectType) {
            case EffectType.Instant:
            case EffectType.Toggle:
                return valueController;
            case EffectType.OverTime:
                return regenerationController;
            default:
                return null;
        }
    }
}


