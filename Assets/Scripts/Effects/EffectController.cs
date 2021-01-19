using Assets.TimeEvents;
using System.Collections;
using UnityEngine;


public class EffectController
{
    TimeEvent ToggleEvent;
    EffectData overtimeEffectData;
    public Stat stat;
    public bool isOnCoolDown = false;
    public readonly float cooldown;
    private float value { get => stat.GetSetValue; set => stat.GetSetValue = value; }
    public void Stop() {
        if (ToggleEvent != null) {
            ToggleEvent.Cancel();
            ToggleEvent = null;
        }
        if (overtimeEffectData != null) {
            EffectHandler._instance.StartCoroutine(AddEffectOverTime(overtimeEffectData));
        }
    }
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
    public void ToggleAmountOverTime(EffectData data) {
        float amount = data.amount,
            duration = data.duration;
        bool isPrecentage = data.isPresentage,
            isRelative = data.isRelative;
        isOnCoolDown = true;

        if (isPrecentage) {
            float amountFromPrecentage = AmountFromPrecentage(value, amount);

            value += amountFromPrecentage;
        }
        else {
            value += amount;
        }
        ToggleEvent = new ToggleTimeEvent(Time.time + duration, this, data, AmountFromPrecentage(value, amount));

    }
    // Add/remove small amount -> wait -> return to previous State
    public IEnumerator AddEffectOverTime(EffectData data) {
        overtimeEffectData = data;
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
    public void Begin(EffectData effectData) {
        EffectHandler effectHandler = EffectHandler._instance;

        if (isOnCoolDown)
            return;

        if (effectData.tickTime == 0)
            effectData.tickTime = 1f;

        new ResetCooldown(Time.time + this.cooldown, this);
        switch (effectData.effectType) {
            case EffectType.Instant:
                AddFixedAmount(effectData);
                return;
            case EffectType.ToggleOverTime:
                this.ToggleAmountOverTime(effectData);
                this.ToggleAmountOverTime(effectData);
                break;
            case EffectType.OverTimeSmallPortion:
                effectHandler.StopCoroutine(this.AddEffectOverTime(effectData));
                effectHandler.StartCoroutine(this.AddEffectOverTime(effectData));
                break;
            default:
                return;
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

    public class ToggleTimeEvent : TimeEvent
    {
        private EffectController triggeringEffect;
        private EffectData data;
        float amountFromPrecentage;
        public ToggleTimeEvent(float triggerTime, EffectController triggeringEffect, EffectData data, float amountFromPrecentage) : base(triggerTime) {
            this.triggeringEffect = triggeringEffect;
            this.data = data;
            this.amountFromPrecentage = amountFromPrecentage;
        }

        public override void Cancel() {
            base.Cancel();
            Trigger();
        }

        public override void Trigger() {
            if (data.isPresentage) {
                if (data.isRelative)
                    amountFromPrecentage = triggeringEffect.AmountFromPrecentage(triggeringEffect.value - amountFromPrecentage, data.amount);

                triggeringEffect.value += -amountFromPrecentage;

            }
            else {
                triggeringEffect.value += -data.amount;
            }
        }

    }
}