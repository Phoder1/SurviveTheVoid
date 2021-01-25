using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class EffectHandler : MonoSingleton<EffectHandler>
{

    PlayerStats playerStats;
    
    [SerializeField] float cooldownBeforeStart;
    public override void Init() {
        playerStats = PlayerStats._instance;

        StopCoroutine(SurvivalEffects());
        StartCoroutine(SurvivalEffects());
    }

    [SerializeField]
    float hungerDropAmount;
    [SerializeField]
    float WaterDropAmount;
    [SerializeField]
    float airDropAmount;
    [SerializeField]
    float sleepDropAmount;
    IEnumerator SurvivalEffects() {

        //Declartion of Effects

        EffectData hungerEffect = new EffectData( StatType.Food, EffectType.OverTime, -hungerDropAmount, Mathf.Infinity, 1f, false, false );
        EffectData thirstEffect =  new EffectData(StatType.Water, EffectType.OverTime, -WaterDropAmount, Mathf.Infinity, 1f, false, false);
        EffectData oxygenEffect =  new EffectData(StatType.Air, EffectType.OverTime, -airDropAmount, Mathf.Infinity, 1f, false, false);
        EffectData sleepEffect = new EffectData(StatType.Sleep, EffectType.OverTime, -sleepDropAmount, Mathf.Infinity, 1f, false, false);


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
        EffectController[] controllers = new EffectController[effectsData.Length];
        for (int i = 0; i < effectsData.Length; i++)
            controllers[i] = new EffectController(playerStats.GetStat(effectsData[i].effectStatType), coolDowns[i]);
        return controllers;
    }
}



