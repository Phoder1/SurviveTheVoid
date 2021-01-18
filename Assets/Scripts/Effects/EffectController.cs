using System.Collections;
using UnityEngine;
using Assets.TimeEvents;
using System.Collections.Generic;


public class EffectController
{
    public Stat stat;
    public bool isOnCoolDown = false;
    bool stopImmidiatly;
    public readonly float cooldown;
    private float value { get => stat.GetSetValue; set => stat.GetSetValue = value; }
    public void SetStop()
        => stopImmidiatly = true;
    public EffectController(Stat stat, float cooldown) {
        this.stat = stat;
        this.cooldown = cooldown;
    }
    public float AmountFromPrecentage(float amountOfStat, float precentage) => amountOfStat * precentage / 100f;
    // instantly add/remove fixed amount 
    public void AddFixedAmount(EffectData data) {
        isOnCoolDown = true;
        value += (data.isPresentage) ? AmountFromPrecentage(value, data.amount) : data.amount;
    }
    //  Add/remove fixed amount -> wait -> return to previous State
    public IEnumerator ToggleAmountOverTime(EffectData data) {
        float amount = data.amount,
            duration = data.duration;
        bool isPrecentage = data.isPresentage,
            isRelative = data.isRelative;
        isOnCoolDown = true;

        if (isPrecentage) {
            float amountFromPrecentage = AmountFromPrecentage(value, amount);

            value += amountFromPrecentage;
            
            yield return new WaitForSeconds(duration);

            if (isRelative)
                amountFromPrecentage = AmountFromPrecentage(value - amountFromPrecentage, amount);

            value += -amountFromPrecentage;

        }
        else {
            value += amount;
            yield return new WaitForSeconds(duration);
            value += -amount;

        }

    }
    // Add/remove small amount -> wait -> return to previous State
    public IEnumerator AddEffectOverTime(EffectData data) {
        float amount = data.amount,
    duration = data.duration,
    tickTime = data.tickTime;
        bool isPrecentage = data.isPresentage,
            isRelative = data.isRelative;
        float startingTime = Time.time;
        isOnCoolDown = true;
        if (isPrecentage) {
            float amountFromPrecentage = AmountFromPrecentage(value, amount) / tickTime;
            while (startingTime + duration > Time.time) {

                if (isRelative)
                    amountFromPrecentage = AmountFromPrecentage(value - amountFromPrecentage, amount);


                value += amountFromPrecentage;
                yield return new WaitForSeconds(duration / tickTime);
                amountFromPrecentage = AmountFromPrecentage(value, amount) / tickTime;
            }
        }
        else {

            while (startingTime + duration > Time.time) {
                value += amount;
                yield return new WaitForSeconds(tickTime);
            }

        }

    }
    public Coroutine Begin(EffectData effectData) {
        EffectHandler effectHandler = EffectHandler._instance;

        if (isOnCoolDown)
            return null;

        if (effectData.tickTime == 0)
            effectData.tickTime = 1f;

        new ResetCooldown(Time.time + this.cooldown, this);
        switch (effectData.effectType) {
            case EffectType.Instant:
                AddFixedAmount(effectData);
                return null;
            case EffectType.ToggleOverTime:
                effectHandler.StopCoroutine(this.ToggleAmountOverTime(effectData));
                return effectHandler.StartCoroutine(this.ToggleAmountOverTime(effectData));
            case EffectType.OverTimeSmallPortion:
                effectHandler.StopCoroutine(this.AddEffectOverTime(effectData));
                return effectHandler.StartCoroutine(this.AddEffectOverTime(effectData));
            default:
                return null;
        }

    }

    public class ResetCooldown : TimeEvent
    {
        public EffectController statCache;

        public ResetCooldown(float triggerTime, EffectController _statCache) : base(triggerTime) {
            statCache = _statCache;
        }

        public override void Trigger() {
            statCache.isOnCoolDown = false;
        }
    }
}