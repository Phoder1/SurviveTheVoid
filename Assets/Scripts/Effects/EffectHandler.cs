using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EffectHandler : MonoSingleton<EffectHandler>
{

    PlayerStats playerStats;
    static Dictionary<StatType, StatControllers> StatsEffectsDict;
    [SerializeField] float cooldownBeforeStart;
    public override void Init() {
        StopAllCoroutines();
        playerStats = PlayerStats._instance;
        
        FillDictionary();
        
        StopCoroutine(SurvivalEffects());
        StartCoroutine(SurvivalEffects());
    }

    private void FillDictionary() {
        StatsEffectsDict = new Dictionary<StatType, StatControllers>();
        foreach (StatType statType in Enum.GetValues(typeof(StatType))) {
            Stat stat = playerStats.GetStat(statType);
            StatsEffectsDict.Add(statType,
                new StatControllers(new EffectController(stat, stat.cooldown), new EffectController(stat, stat.overtimeCooldown)));
        }
    }

    IEnumerator SurvivalEffects() {

        //Declartion of Effects

        EffectData hungerEffect = new EffectData() {
            effectStatType = StatType.Food,
            isPresentage = false,
            isRelative = false,
            amount = -0.5f,
            tickTime = 1f,
            duration = Mathf.Infinity
        };
        EffectData thirstEffect = new EffectData() {
            effectStatType = StatType.Water,
            isPresentage = false,
            isRelative = false,
            amount = -0.5f,
            tickTime = 1f,
            duration = Mathf.Infinity
        };
        EffectData oxygenEffect = new EffectData() {
            effectStatType = StatType.Air,
            isPresentage = false,
            isRelative = false,
            amount = -0.5f,
            tickTime = 1f,
            duration = Mathf.Infinity
        };
        EffectData sleepEffect = new EffectData() {
            effectStatType = StatType.Sleep,
            isPresentage = false,
            isRelative = false,
            amount = -0.5f,
            tickTime = 1f,
            duration = Mathf.Infinity
        };


        yield return new WaitForSeconds(cooldownBeforeStart);




        //Applying Effects:

        EffectController worldEffect;


        worldEffect = new EffectController(playerStats.GetStat(StatType.Food), 3f);
        StopCoroutine(worldEffect.AddEffectOverTime(hungerEffect));
        StartCoroutine(worldEffect.AddEffectOverTime(hungerEffect));

        worldEffect = new EffectController(playerStats.GetStat(StatType.Water), 3f);
        StopCoroutine(worldEffect.AddEffectOverTime(thirstEffect));
        StartCoroutine(worldEffect.AddEffectOverTime(thirstEffect));

        worldEffect = new EffectController(playerStats.GetStat(StatType.Air), 3f);
        StopCoroutine(worldEffect.AddEffectOverTime(oxygenEffect));
        StartCoroutine(worldEffect.AddEffectOverTime(oxygenEffect));

        worldEffect = new EffectController(playerStats.GetStat(StatType.Sleep), 3f);
        StopCoroutine(worldEffect.AddEffectOverTime(sleepEffect));
        StartCoroutine(worldEffect.AddEffectOverTime(sleepEffect));

    }


    public static EffectController GetStatController(EffectData effect) {
        if (StatsEffectsDict.TryGetValue(effect.effectStatType, out StatControllers effectControllers)) 
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
            case EffectType.ToggleOverTime:
                return valueController;
            case EffectType.OverTimeSmallPortion:
                return regenerationController;
            default:
                return null;
        }
    }
}


