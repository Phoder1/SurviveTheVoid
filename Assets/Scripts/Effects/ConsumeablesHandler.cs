using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumeablesHandler : MonoSingleton<ConsumeablesHandler>
{
    static Dictionary<StatType, StatControllers> ConsumablesEffectsDict;
    PlayerStats playerStats;
    EffectHandler effectHandler;
    public override void Init() {
        playerStats = PlayerStats._instance;
        effectHandler = EffectHandler._instance;
        FillDictionary();

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

    public static EffectController GetConsumableStatController(EffectData effect) {
        if (ConsumablesEffectsDict.TryGetValue(effect.effectStatType, out StatControllers effectControllers))
            return effectControllers.GetController(effect.effectType);
        return null;
    }
    public static EffectController[] GetConsumableStatControllers(EffectData[] effectsData) {
        if (effectsData.Length == 0)
            return null;
        EffectController[] controllers = new EffectController[effectsData.Length];
        for (int i = 0; i < effectsData.Length; i++)
            controllers[i] = GetConsumableStatController(effectsData[i]);
        return controllers;
    }
    public void BeginAllConsumeableEffects(EffectData[] effectsData)
        => effectHandler.BeginAllEffects(effectsData, GetConsumableStatControllers(effectsData));
    public bool GetEffectCoolDown(ConsumableItemSO Item) {

        if (Item == null || Item.Effects.Length == 0)
            return false;

        EffectController abstStat;

        bool CanUseTheItem = true;
        foreach (var effect in Item.Effects) {

            abstStat = GetConsumableStatController(effect);

            if (abstStat == null)
                continue;


            CanUseTheItem = CanUseTheItem && !abstStat.isOnCoolDown;


            if (CanUseTheItem == false)
                return false;

        }

        return CanUseTheItem;

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
}
