using Assets.TimeEvents;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ConsumeEffectHandler : MonoSingleton<ConsumeEffectHandler>
{

    static Dictionary<EffectCategory, AbstStat> StatEffectDict;
    [SerializeField] float cooldownBeforeStart;
    public override void Init()
    {
        StatEffectDict = new Dictionary<EffectCategory, AbstStat>() {
       {EffectCategory.Food, new HungerStat()},
       {EffectCategory.Food_Poisoning, new HungerStat()},
       {EffectCategory.HP, new HPStat()},
       {EffectCategory.HP_Regeneration, new HPStat()},
       {EffectCategory.Thirst, new ThirstStat()},
       {EffectCategory.Water, new ThirstStat()}

        };
    }
    private void Start()
    {
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
            tickTime = 0.5f,
            duration = Mathf.Infinity
          
        };
        ConsumableEffect thirstEffect = new ConsumableEffect() {
            effectCategory = EffectCategory.Thirst,
            isPresentage = false,
            isRelative = false,
            amount = -0.5f,
            tickTime = 0.5f,
            duration = Mathf.Infinity
           
        };
        ConsumableEffect oxygenEffect = new ConsumableEffect()
        {
            effectCategory = EffectCategory.Air,
            isPresentage = false,
            isRelative = false,
            amount = -0.5f,
            tickTime = 0.5f,
            duration = Mathf.Infinity
        };

        
        yield return new WaitForSeconds(cooldownBeforeStart);




        //Applying Effects:

        AbstStat survivalEffect;


        survivalEffect = new HungerStat();
        StopCoroutine(survivalEffect.AddEffectOverTime(hungerEffect.amount, hungerEffect.duration, hungerEffect.tickTime, hungerEffect.isPresentage, hungerEffect.isRelative));
        StartCoroutine(survivalEffect.AddEffectOverTime(hungerEffect.amount, hungerEffect.duration, hungerEffect.tickTime, hungerEffect.isPresentage, hungerEffect.isRelative));

        survivalEffect = new ThirstStat();
        StopCoroutine(survivalEffect.AddEffectOverTime(thirstEffect.amount, thirstEffect.duration, thirstEffect.tickTime, thirstEffect.isPresentage, thirstEffect.isRelative));
        StartCoroutine(survivalEffect.AddEffectOverTime(thirstEffect.amount, thirstEffect.duration, thirstEffect.tickTime, thirstEffect.isPresentage, thirstEffect.isRelative));

        survivalEffect = new OxygenStat();
        StopCoroutine(survivalEffect.AddEffectOverTime(oxygenEffect.amount, oxygenEffect.duration, oxygenEffect.tickTime, oxygenEffect.isPresentage, oxygenEffect.isRelative));
        StartCoroutine(survivalEffect.AddEffectOverTime(oxygenEffect.amount, oxygenEffect.duration, oxygenEffect.tickTime, oxygenEffect.isPresentage, oxygenEffect.isRelative));

    }

 
    public static AbstStat GetAbstStat(ConsumableEffect effect) {

        if (StatEffectDict.TryGetValue(effect.effectCategory, out AbstStat effectStatAbst))
            return effectStatAbst;

        else
           return null;

    }
    public bool GetEffectCoolDown(ConsumableItemSO Item) {

        if (Item == null || Item.Effects.Length == 0)
            return false;


        AbstStat abstStat;

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

        AbstStat effectCache = GetAbstStat(effect);

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
        public AbstStat statCache;

        public ResetCooldown(float triggerTime , AbstStat _statCache) : base(triggerTime)
        {
            statCache = _statCache;
        }

        public override void Trigger()
        {
            statCache.isOnCoolDown = false;
        }
    }

}

   
