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
    private float AmountFromPercentage(float amountOfStat, float precentage, bool isRelativeToMax) => ((isRelativeToMax && stat.GetIsCapped) ? stat.maxStat.GetSetValue : amountOfStat) * precentage / 100f;
    // instantly add/remove fixed amount 
    private void AddFixedAmount(EffectData data) {
        value += (data.inPercentage) ? AmountFromPercentage(value, data.amount, data.isRelativeToMax) : data.amount;
    }
    //  Add/remove fixed amount -> wait -> return to previous State
    private void ToggleAmountOverTime(EffectData data) {
        float toggleAmount = data.amount;
        if (data.inPercentage) {
            toggleAmount = AmountFromPercentage(value, data.amount, data.isRelativeToMax);

            value += toggleAmount;
        }
        else {
            value += toggleAmount;
        }
        ToggleEvent = new ToggleTimeEvent(Time.time + data.duration, this, toggleAmount);

    }
    // Add/remove small amount -> wait -> return to previous State
    private IEnumerator AddEffectOverTime(EffectData data) {
        float startingTime = Time.time;
        if (data.inPercentage) {
            while (startingTime + data.duration > Time.time) {
                value += (Mathf.Pow((1+(data.amount/100)),data.tickTime) - 1) * ((data.isRelativeToMax && stat.GetIsCapped) ? stat.maxStat.GetSetValue : value);
                yield return new WaitForSeconds(data.tickTime);
            }
        }
        else {
            while (startingTime + data.duration > Time.time) {
                value += data.amount*data.tickTime;
                yield return new WaitForSeconds(data.tickTime);
            }

        }
    }
    public void Begin(EffectData effectData) {
        if (effectData.effectStatType == StatType.None || isOnCoolDown)
            return;
        Stop();
        if (effectData.tickTime == 0)
            effectData.tickTime = 0.03f;

        if (cooldown != 0) {
            isOnCoolDown = true;
            new ResetCooldown(Time.time + cooldown, this);
        }
        switch (effectData.effectType) {
            case EffectType.Instant:
                AddFixedAmount(effectData);
                return;
            case EffectType.Toggle:
                this.ToggleAmountOverTime(effectData);
                this.ToggleAmountOverTime(effectData);
                break;
            case EffectType.OverTime:

                overtimeEffectCoro = EffectHandler._instance.StartCoroutine(AddEffectOverTime(effectData));
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
        float toggleAmount;
        public ToggleTimeEvent(float triggerTime, EffectController triggeringEffect, float toggleAmount) : base(triggerTime) {
            this.triggeringEffect = triggeringEffect;
            this.toggleAmount = toggleAmount;
        }

        protected override void OnCancel() {
            TriggerBehaviour();
        }

        protected override void TriggerBehaviour()
            => triggeringEffect.value -= toggleAmount;


    }
}