using Assets.TimeEvents;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ConsumeEffectHandler : MonoSingleton<ConsumeEffectHandler>
{

    PlayerStats playerStats;
    static Dictionary<EffectCategory, Effect> StatEffectDict;
    [SerializeField] float cooldownBeforeStart;
    public override void Init()
    {
        playerStats = PlayerStats._instance;
        StatEffectDict = new Dictionary<EffectCategory, Effect>() {
       {EffectCategory.Food, new Effect(playerStats.GetStat(SurvivalStatType.Hunger),3f)},
       {EffectCategory.Food_Regeneration, new Effect(playerStats.GetStat(SurvivalStatType.Hunger),3f)},
       {EffectCategory.MaxFood, new Effect(playerStats.GetStatMax(SurvivalStatType.Hunger),3f)},
       {EffectCategory.HP, new Effect(playerStats.GetStat(SurvivalStatType.HP),3f)},
       {EffectCategory.HP_Regeneration, new Effect(playerStats.GetStat(SurvivalStatType.HP),3f)},
       {EffectCategory.MaxHP, new Effect(playerStats.GetStatMax(SurvivalStatType.HP),3f)},
       {EffectCategory.Water, new Effect(playerStats.GetStat(SurvivalStatType.Thirst),3f)},
       {EffectCategory.Water_Regeneration, new Effect(playerStats.GetStat(SurvivalStatType.Thirst),3f)},
       {EffectCategory.MaxWater, new Effect(playerStats.GetStatMax(SurvivalStatType.Thirst),3f)},
       {EffectCategory.EXP, new Effect(playerStats.GetStat(PlayerStatType.EXP),3f)},
       {EffectCategory.Sleep, new Effect(playerStats.GetStat(SurvivalStatType.Awakeness),3f)},


        };
        StopCoroutine(SurvivalEffects());
        StartCoroutine(SurvivalEffects());
    }

    IEnumerator SurvivalEffects() {

        //Declartion of Effects
     
        ConsumableEffect hungerEffect = new ConsumableEffect() {
            effectCategory = EffectCategory.Food,
            isPresentage = false,
            isRelative = false,
            amount = -0.5f,
            tickTime = 1f,
            duration = Mathf.Infinity
          
        };
        ConsumableEffect thirstEffect = new ConsumableEffect() {
            effectCategory = EffectCategory.Water_Regeneration,
            isPresentage = false,
            isRelative = false,
            amount = -0.5f,
            tickTime = 1f,
            duration = Mathf.Infinity
           
        };
        ConsumableEffect oxygenEffect = new ConsumableEffect()
        {
            effectCategory = EffectCategory.Air,
            isPresentage = false,
            isRelative = false,
            amount = -0.5f,
            tickTime = 1f,
            duration = Mathf.Infinity
        };

        
        yield return new WaitForSeconds(cooldownBeforeStart);




        //Applying Effects:

        Effect worldEffect;


        worldEffect = new Effect(playerStats.GetStat(SurvivalStatType.Hunger), 3f);
        StopCoroutine(worldEffect.AddEffectOverTime(hungerEffect.amount, hungerEffect.duration, hungerEffect.tickTime, hungerEffect.isPresentage, hungerEffect.isRelative));
        StartCoroutine(worldEffect.AddEffectOverTime(hungerEffect.amount, hungerEffect.duration, hungerEffect.tickTime, hungerEffect.isPresentage, hungerEffect.isRelative));

        worldEffect = new Effect(playerStats.GetStat(SurvivalStatType.Thirst), 3f);
        StopCoroutine(worldEffect.AddEffectOverTime(thirstEffect.amount, thirstEffect.duration, thirstEffect.tickTime, thirstEffect.isPresentage, thirstEffect.isRelative));
        StartCoroutine(worldEffect.AddEffectOverTime(thirstEffect.amount, thirstEffect.duration, thirstEffect.tickTime, thirstEffect.isPresentage, thirstEffect.isRelative));

        worldEffect = new Effect(playerStats.GetStat(SurvivalStatType.Oxygen), 3f);
        StopCoroutine(worldEffect.AddEffectOverTime(oxygenEffect.amount, oxygenEffect.duration, oxygenEffect.tickTime, oxygenEffect.isPresentage, oxygenEffect.isRelative));
        StartCoroutine(worldEffect.AddEffectOverTime(oxygenEffect.amount, oxygenEffect.duration, oxygenEffect.tickTime, oxygenEffect.isPresentage, oxygenEffect.isRelative));

    }

 
    public static Effect GetAbstStat(ConsumableEffect effect) {

        if (StatEffectDict.TryGetValue(effect.effectCategory, out Effect effectStatAbst))
            return effectStatAbst;

        else
           return null;

    }
    public bool GetEffectCoolDown(ConsumableItemSO Item) {

        if (Item == null || Item.Effects.Length == 0)
            return false;


        Effect abstStat;

        bool CanUseTheItem = true;
        foreach (var effect in Item.Effects)
        {

            abstStat = GetAbstStat(effect);

            if (abstStat ==null)
                continue;


            CanUseTheItem = CanUseTheItem && !abstStat.isOnCoolDown;


            if (CanUseTheItem == false)
                return false;
            
        }

        return CanUseTheItem;

    }
    public void StartEffect(ConsumableEffect effect)
    {

        Effect effectCache = GetAbstStat(effect);

        if (effectCache == null || effectCache.isOnCoolDown)
            return;

        if (effect.tickTime == 0)
            effect.tickTime = 1f;
        
        switch (effect.effectType)
        {
            case EffectType.Instant:
                effectCache.AddFixedAmount(effect.amount, effect.isPresentage);
                break;
            case EffectType.ToggleOverTime:
                StopCoroutine(effectCache.ToggleAmountOverTime(effect.amount, effect.duration, effect.isPresentage, effect.isRelative));
                StartCoroutine(effectCache.ToggleAmountOverTime(effect.amount, effect.duration , effect.isPresentage, effect.isRelative));
                break;
            case EffectType.OverTimeSmallPortion:
                StopCoroutine(effectCache.AddEffectOverTime(effect.amount, effect.duration, effect.tickTime, effect.isPresentage, effect.isRelative));
                StartCoroutine(effectCache.AddEffectOverTime(effect.amount, effect.duration , effect.tickTime, effect.isPresentage,effect.isRelative));
               
                break;
            default:
                break;
        }

        new ResetCooldown(Time.time + effectCache.cooldown, effectCache);

    }

    public class ResetCooldown : TimeEvent
    {
        public Effect statCache;

        public ResetCooldown(float triggerTime , Effect _statCache) : base(triggerTime)
        {
            statCache = _statCache;
        }

        public override void Trigger()
        {
            statCache.isOnCoolDown = false;
        }
    }

}

   
