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
//public class ThirstStat : Effect
//{
//    public override IEnumerator AddEffectOverTime(float amount, float duration, float tickTime, bool isPrecentage, bool isRelative) {
//        float startingTime = Time.time;
//        isOnCoolDown = true;
//        if (isPrecentage) {
//            float amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetThirst, amount) / tickTime;
//            ;
//            while (startingTime + duration > Time.time) {

//                if (isRelative)
//                    amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetThirst - amountFromPrecentage, amount);


//                PlayerStats._instance.GetSetThirst += amountFromPrecentage;
//                yield return new WaitForSeconds(duration / tickTime);
//                amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetThirst, amount) / tickTime;
//            }

//        }
//        else {

//            while (startingTime + duration > Time.time) {
//                PlayerStats._instance.GetSetThirst += amount / tickTime;
//                yield return new WaitForSeconds(duration / tickTime);
//            }

//        }


//    }

//    public override void AddFixedAmount(float amount, bool isPrecentage) {
//        isOnCoolDown = true;

//        if (isPrecentage)
//            PlayerStats._instance.GetSetThirst += AmountFromPrecentage(PlayerStats._instance.GetSetThirst, amount);

//        else
//            PlayerStats._instance.GetSetThirst += amount;
//    }

//    public override IEnumerator ToggleAmountOverTime(float amount, float duration, bool isPrecentage, bool isRelative) {

//        isOnCoolDown = true;

//        if (isPrecentage) {
//            float amountFromPrecentage;
//            amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetThirst, amount);
//            PlayerStats._instance.GetSetThirst += amountFromPrecentage;
//            yield return new WaitForSeconds(duration);

//            if (isRelative)
//                amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetHunger - amountFromPrecentage, amount);

//            PlayerStats._instance.GetSetThirst += -amountFromPrecentage;
//        }
//        else {
//            PlayerStats._instance.GetSetThirst += amount;
//            yield return new WaitForSeconds(duration);
//            PlayerStats._instance.GetSetThirst += -amount;

//        }

//    }
//}
//public class HPStat : Effect
//{
//    public override IEnumerator AddEffectOverTime(float amount, float duration, float tickTime, bool isPrecentage, bool isRelative) {
//        float startingTime = Time.time;
//        isOnCoolDown = true;
//        if (isPrecentage) {

//            float amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetHP, amount) / tickTime;
//            while (startingTime + duration > Time.time) {

//                if (isRelative)
//                    amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetHP - amountFromPrecentage, amount);

//                PlayerStats._instance.GetSetHP += AmountFromPrecentage(PlayerStats._instance.GetSetHP, amount) / tickTime;
//                yield return new WaitForSeconds(duration / tickTime);
//                amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetHP, amount) / tickTime;
//            }
//        }
//        else {

//            while (startingTime + duration > Time.time) {
//                PlayerStats._instance.GetSetHP += amount / tickTime;
//                yield return new WaitForSeconds(duration / tickTime);
//            }
//        }


//    }

//    public override void AddFixedAmount(float amount, bool isPrecentage) {
//        isOnCoolDown = true;

//        if (isPrecentage)
//            PlayerStats._instance.GetSetHP += AmountFromPrecentage(PlayerStats._instance.GetSetHP, amount);

//        else
//            PlayerStats._instance.GetSetHP += amount;
//    }

//    public override IEnumerator ToggleAmountOverTime(float amount, float duration, bool isPrecentage, bool isRelative) {
//        isOnCoolDown = true;

//        float amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetHP, amount);

//        if (isPrecentage) {
//            PlayerStats._instance.GetSetHP += amountFromPrecentage;
//            yield return new WaitForSeconds(duration);

//            if (isRelative)
//                amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetHP - amountFromPrecentage, amount);

//            PlayerStats._instance.GetSetHP += -amountFromPrecentage;
//        }
//        else {
//            PlayerStats._instance.GetSetHP += amount;
//            yield return new WaitForSeconds(duration);
//            PlayerStats._instance.GetSetHP += -amount;

//        }
//    }
//}
//public class OxygenStat : Effect
//{
//    public override IEnumerator AddEffectOverTime(float amount, float duration, float tickTime, bool isPrecentage, bool isRelative) {
//        float startingTime = Time.time;
//        isOnCoolDown = true;
//        if (isPrecentage) {

//            float amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetOxygen, amount) / tickTime;
//            while (startingTime + duration > Time.time) {

//                if (isRelative)
//                    amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetOxygen - amountFromPrecentage, amount);

//                PlayerStats._instance.GetSetHP += AmountFromPrecentage(PlayerStats._instance.GetSetOxygen, amount) / tickTime;
//                yield return new WaitForSeconds(duration / tickTime);
//                amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetOxygen, amount) / tickTime;
//            }
//        }
//        else {

//            while (startingTime + duration > Time.time) {
//                PlayerStats._instance.GetSetOxygen += amount / tickTime;
//                yield return new WaitForSeconds(duration / tickTime);
//            }
//        }


//    }

//    public override void AddFixedAmount(float amount, bool isPrecentage) {
//        isOnCoolDown = true;

//        if (isPrecentage)
//            PlayerStats._instance.GetSetOxygen += AmountFromPrecentage(PlayerStats._instance.GetSetOxygen, amount);

//        else
//            PlayerStats._instance.GetSetOxygen += amount;
//    }

//    public override IEnumerator ToggleAmountOverTime(float amount, float duration, bool isPrecentage, bool isRelative) {
//        isOnCoolDown = true;

//        float amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetOxygen, amount);

//        if (isPrecentage) {
//            PlayerStats._instance.GetSetOxygen += amountFromPrecentage;
//            yield return new WaitForSeconds(duration);

//            if (isRelative)
//                amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetOxygen - amountFromPrecentage, amount);

//            PlayerStats._instance.GetSetOxygen += -amountFromPrecentage;
//        }
//        else {
//            PlayerStats._instance.GetSetOxygen += amount;
//            yield return new WaitForSeconds(duration);
//            PlayerStats._instance.GetSetOxygen += -amount;

//        }
//    }
//}