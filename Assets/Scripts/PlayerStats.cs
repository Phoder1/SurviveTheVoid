using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    int HP;
    int hunger;
    int EXP;
    float speed = 5;
    int temperature;
    int attackDMG;
    int level;
    int thirst;
    int oxygen;
    int awakeness;
    float gatheringSpeed;
    public void Init()
    {
        ResetStats();
    }
    void ResetStats() {
        thirst = 100;
        hunger = 100;
        EXP = 0;
        speed = 5f;
        temperature = 50;
        attackDMG = 20;
        level = 1;
        oxygen = 100;
        awakeness = 100;
        gatheringSpeed = 2f;
    }


    public int GetSetHP { get { return HP; } set { HP = value; } }
    public int GetSetAwakeness { get { return awakeness; } set { awakeness = value; } }
    public int GetSetOxygen { get { return oxygen; } set { oxygen = value; } }
    public int GetSetThirst { get { return thirst; } set { thirst = value; } }
    public int GetSetHunger { get { return hunger; } set { hunger = value; } }
    public int GetSetTemperature { get { return temperature; } set { temperature = value; } }
    public int GetSetAttackDMG { get { return attackDMG; } set { attackDMG = value; } }
    public int GetSetEXP { get { return EXP; } set { EXP = value; } }
    public int GetSetLevel { get { return level; } set { level = value; } }
    public float GetSetSpeed { get { return speed; } set { speed = value; } }
    public float GetSetGatheringSpeed { get { return gatheringSpeed; } set { gatheringSpeed = value; } }




    public class EffectHandler {

        List<ConsumableEffect> ActiveEffect;

        public EffectHandler() {
            ActiveEffect = new List<ConsumableEffect>();
        }


        public void AddEffect(ConsumableEffect ConsumeEffect) {

            if (!ActiveEffect.Contains(ConsumeEffect))
                ActiveEffect.Add(ConsumeEffect);
            

            // add logic of not activating or removing the effect
        }

        public void RemoveEffect(ConsumableEffect ConsumeEffect) {
            ActiveEffect.Remove(ConsumeEffect);
        }
    }

}
