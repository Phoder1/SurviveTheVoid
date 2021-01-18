using System.Collections;
using UnityEngine;


public class Effect
{
    public Stat stat;
    public bool isOnCoolDown = false;
    public readonly float cooldown;
    private float value { get => stat.GetSetValue; set => stat.GetSetValue = value; }
    public Effect(Stat stat, float cooldown) {
        this.stat = stat;
        this.cooldown = cooldown;
    }

    public float AmountFromPrecentage(float amountOfStat, float precentage) => amountOfStat * precentage / 100f;
    // instantly add/remove fixed amount 
    public void AddFixedAmount(float amount, bool isPrecentage) {
        isOnCoolDown = true;
        value += (isPrecentage) ? AmountFromPrecentage(value, amount) : amount;
    }
    //  Add/remove fixed amount -> wait -> return to previous State
    public IEnumerator ToggleAmountOverTime(float amount, float duration, bool isPrecentage, bool isRelative) {
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
    public IEnumerator AddEffectOverTime(float amount, float duration, float tickTime, bool isPrecentage, bool isRelative) {
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
}