using Assets.TimeEvents;
using System.Collections;
using UnityEngine;


public class EffectController
{
    TimeEvent ToggleEvent;
    Coroutine overtimeEffectCoro;
    public Stat stat;
    public bool isOnCoolDown = false;
    public readonly float cooldown;
    private float value { get => stat.GetSetValue; set => stat.GetSetValue = value; }
    public EffectController(Stat stat, float cooldown) {
        this.stat = stat;
        this.cooldown = cooldown;
    }
    private float AmountFromPrecentage(float amountOfStat, float precentage) => amountOfStat * precentage / 100f;
    // instantly add/remove fixed amount 
    private void AddFixedAmount(EffectData data) {
        value += (data.isPrecentage) ? AmountFromPrecentage(value, data.amount) : data.amount;
    }
    //  Add/remove fixed amount -> wait -> return to previous State
    private void ToggleAmountOverTime(EffectData data) {
        float amount = data.amount,
            duration = data.duration;
        bool isPrecentage = data.isPrecentage;

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
    private IEnumerator AddEffectOverTime(EffectData data) {
        float amount = data.amount,
    duration = data.duration,
    tickTime = data.tickTime;
        bool isPrecentage = data.isPrecentage,
            isRelative = data.isRelative;
        float startingTime = Time.time;
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
        Stop();
        EffectHandler effectHandler = EffectHandler._instance;

        if (isOnCoolDown)
            return;

        if (effectData.tickTime == 0)
            effectData.tickTime = 1f;

        if (cooldown != 0) {
            isOnCoolDown = true;
            new ResetCooldown(Time.time + cooldown, this);
        }
        switch (effectData.effectType) {
            case EffectType.Instant:
                AddFixedAmount(effectData);
                return;
            case EffectType.ToggleOverTime:
                this.ToggleAmountOverTime(effectData);
                this.ToggleAmountOverTime(effectData);
                break;
            case EffectType.OverTimeSmallPortion:

                overtimeEffectCoro = effectHandler.StartCoroutine(AddEffectOverTime(effectData));
                break;
            default:
                return;
        }

    }
    public void Stop() {
        if (ToggleEvent != null) {
            ToggleEvent.Cancel();
            ToggleEvent = null;
        }
        if (overtimeEffectCoro != null) {
            EffectHandler._instance.StopCoroutine(overtimeEffectCoro);
            overtimeEffectCoro = null;
        }
    }

    public class ResetCooldown : TimeEvent
    {
        public EffectController statCache;


        public ResetCooldown(float triggerTime, EffectController _statCache) : base(triggerTime) {
            statCache = _statCache;
        }

        protected override void TriggerBehaviour() {
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

        protected override void OnCancel() {
            TriggerBehaviour();
        }

        protected override void TriggerBehaviour() {
            if (data.isPrecentage) {
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