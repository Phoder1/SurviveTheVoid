using System.Collections;
using UnityEngine;


public abstract class AbstStat
{
    public bool isOnCoolDown = false;
    public readonly float cooldown = 3f;

    public abstract void AddFixedAmount(float amount , bool isPrecentage); // instantly add/remove fixed amount 
    public abstract IEnumerator ToggleAmountOverTime(float amount, float duration, bool isPrecentage, bool isRelative); //  Add/remove fixed amount -> wait -> return to previous State
    public abstract IEnumerator AddEffectOverTime(float amount, float duration , float tickTime, bool isPrecentage, bool isRelative); // Add/remove small amount -> wait -> return to previous State

    public float AmountFromPrecentage(float amountOfStat,float precentage)  => amountOfStat * precentage / 100f;

   
}
public class HungerStat : AbstStat
{
    public new readonly float cooldown = 5f;
    public override IEnumerator AddEffectOverTime(float amount, float duration, float tickTime, bool isPrecentage, bool isRelative)
    {
        float startingTime = Time.time;
        isOnCoolDown = true;
        PlayerStats playerStats = PlayerStats._instance;
        if (isPrecentage)
        {
            float amountFromPrecentage= AmountFromPrecentage(playerStats.GetStatValue(SurvivalStatType.Hunger), amount) / tickTime;
            while (startingTime + duration > Time.time)
            {

                if (isRelative)
                    amountFromPrecentage = AmountFromPrecentage(playerStats.GetStatValue(SurvivalStatType.Hunger) - amountFromPrecentage, amount);


                playerStats.AddStatValue(SurvivalStatType.Hunger, amountFromPrecentage);
                yield return new WaitForSeconds(duration / tickTime);
                amountFromPrecentage = AmountFromPrecentage(playerStats.GetStatValue(SurvivalStatType.Hunger), amount) / tickTime;
            }
        }
        else
        {

            while (startingTime + duration > Time.time)
            {
                playerStats.AddStatValue(SurvivalStatType.Hunger, amount / tickTime);
                yield return new WaitForSeconds(duration / tickTime);
            }

        }

    }

    public override void AddFixedAmount(float amount, bool isPrecentage)
    {
        isOnCoolDown = true;

        if (isPrecentage)
            PlayerStats._instance.AddStatValue(SurvivalStatType.Hunger, AmountFromPrecentage(PlayerStats._instance.GetStatValue(SurvivalStatType.Hunger), amount));

        else
            PlayerStats._instance.AddStatValue(SurvivalStatType.Hunger, amount);
    }

    public override IEnumerator ToggleAmountOverTime(float amount, float duration, bool isPrecentage, bool isRelative)
    {
        isOnCoolDown = true;

        if (isPrecentage)
        {
            float amountFromPrecentage= AmountFromPrecentage(PlayerStats._instance.GetSetHunger, amount);

            PlayerStats._instance.GetSetHunger += amountFromPrecentage;
            yield return new WaitForSeconds(duration);

            if (isRelative)
                amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetHunger - amountFromPrecentage, amount);

            PlayerStats._instance.GetSetHunger += -amountFromPrecentage;

        }
        else
        {
            PlayerStats._instance.GetSetHunger += amount;
            yield return new WaitForSeconds(duration);
            PlayerStats._instance.GetSetHunger += -amount;

        }

    }
}
public class ThirstStat : AbstStat
{
    public override IEnumerator AddEffectOverTime(float amount, float duration, float tickTime, bool isPrecentage, bool isRelative)
    {
        float startingTime = Time.time;
        isOnCoolDown = true;
        if (isPrecentage)
        {
            float amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetThirst, amount) / tickTime; ;
            while (startingTime + duration > Time.time)
            {

                if (isRelative)
                    amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetThirst - amountFromPrecentage, amount);


                PlayerStats._instance.GetSetThirst += amountFromPrecentage;
                yield return new WaitForSeconds(duration / tickTime);
                amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetThirst, amount) / tickTime;
            }
           
        }
        else
        {

            while (startingTime + duration > Time.time)
            {
                PlayerStats._instance.GetSetThirst += amount / tickTime;
                yield return new WaitForSeconds(duration / tickTime);
            }

        }


    }

    public override void AddFixedAmount(float amount, bool isPrecentage)
    {
        isOnCoolDown = true;

        if (isPrecentage)
            PlayerStats._instance.GetSetThirst += AmountFromPrecentage(PlayerStats._instance.GetSetThirst, amount);

        else
            PlayerStats._instance.GetSetThirst += amount;
    }

    public override IEnumerator ToggleAmountOverTime(float amount, float duration, bool isPrecentage,bool isRelative)
    {

        isOnCoolDown = true;

        if (isPrecentage)
        {
        float amountFromPrecentage;
             amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetThirst, amount);
            PlayerStats._instance.GetSetThirst += amountFromPrecentage;
            yield return new WaitForSeconds(duration);

            if (isRelative)
                amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetHunger - amountFromPrecentage, amount);

            PlayerStats._instance.GetSetThirst += -amountFromPrecentage;
        }
        else
        {
            PlayerStats._instance.GetSetThirst += amount;
            yield return new WaitForSeconds(duration);
            PlayerStats._instance.GetSetThirst += -amount;

        }

    }
}
public class HPStat : AbstStat
{
    public override IEnumerator AddEffectOverTime(float amount, float duration, float tickTime, bool isPrecentage , bool isRelative)
    {
        float startingTime = Time.time;
        isOnCoolDown = true;
        if (isPrecentage)
        {
            
            float amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetHP, amount) / tickTime;
            while (startingTime + duration > Time.time)
            {

                if (isRelative)
                   amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetHP - amountFromPrecentage, amount);

                PlayerStats._instance.GetSetHP += AmountFromPrecentage(PlayerStats._instance.GetSetHP, amount) / tickTime;
                yield return new WaitForSeconds(duration / tickTime);
                amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetHP, amount) / tickTime;
            }
        }
        else
        {

            while (startingTime + duration > Time.time)
            {
                PlayerStats._instance.GetSetHP += amount / tickTime;
                yield return new WaitForSeconds(duration / tickTime);
            }
        }
       
      
    }

    public override void AddFixedAmount(float amount, bool isPrecentage)
    {
        isOnCoolDown = true;

        if (isPrecentage)
            PlayerStats._instance.GetSetHP += AmountFromPrecentage(PlayerStats._instance.GetSetHP, amount);

        else
        PlayerStats._instance.GetSetHP += amount;
    }

    public override IEnumerator ToggleAmountOverTime(float amount, float duration,bool isPrecentage , bool isRelative)
    {
        isOnCoolDown = true;
   
            float amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetHP, amount);

        if (isPrecentage)
        {
            PlayerStats._instance.GetSetHP += amountFromPrecentage;
            yield return new WaitForSeconds(duration);

            if (isRelative)
                amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetHP - amountFromPrecentage, amount);
            
            PlayerStats._instance.GetSetHP += -amountFromPrecentage;
        }
        else
        {
            PlayerStats._instance.GetSetHP += amount;
            yield return new WaitForSeconds(duration);
            PlayerStats._instance.GetSetHP += -amount;

        }
    }
}
public class OxygenStat : AbstStat
{
    public override IEnumerator AddEffectOverTime(float amount, float duration, float tickTime, bool isPrecentage, bool isRelative)
    {
        float startingTime = Time.time;
        isOnCoolDown = true;
        if (isPrecentage)
        {

            float amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetOxygen, amount) / tickTime;
            while (startingTime + duration > Time.time)
            {

                if (isRelative)
                    amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetOxygen - amountFromPrecentage, amount);

                PlayerStats._instance.GetSetHP += AmountFromPrecentage(PlayerStats._instance.GetSetOxygen, amount) / tickTime;
                yield return new WaitForSeconds(duration / tickTime);
                amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetOxygen, amount) / tickTime;
            }
        }
        else
        {

            while (startingTime + duration > Time.time)
            {
                PlayerStats._instance.GetSetOxygen += amount / tickTime;
                yield return new WaitForSeconds(duration / tickTime);
            }
        }


    }

    public override void AddFixedAmount(float amount, bool isPrecentage)
    {
        isOnCoolDown = true;

        if (isPrecentage)
            PlayerStats._instance.GetSetOxygen += AmountFromPrecentage(PlayerStats._instance.GetSetOxygen, amount);

        else
            PlayerStats._instance.GetSetOxygen += amount;
    }

    public override IEnumerator ToggleAmountOverTime(float amount, float duration, bool isPrecentage, bool isRelative)
    {
        isOnCoolDown = true;

        float amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetOxygen, amount);

        if (isPrecentage)
        {
            PlayerStats._instance.GetSetOxygen += amountFromPrecentage;
            yield return new WaitForSeconds(duration);

            if (isRelative)
                amountFromPrecentage = AmountFromPrecentage(PlayerStats._instance.GetSetOxygen - amountFromPrecentage, amount);

            PlayerStats._instance.GetSetOxygen += -amountFromPrecentage;
        }
        else
        {
            PlayerStats._instance.GetSetOxygen += amount;
            yield return new WaitForSeconds(duration);
            PlayerStats._instance.GetSetOxygen += -amount;

        }
    }
}