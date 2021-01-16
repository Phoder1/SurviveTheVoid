using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoSingleton<PlayerStats>
{

    float HP;
    float hunger;
    float EXP;
    float speed = 5;
    float temperature;
    float attackDMG;
    int level;
    float thirst;
    float oxygen;
    float awakeness;
    float gatheringSpeed;

    void ResetStats()
    {
        thirst = 100;
        hunger = 50;
        EXP = 0;
        speed = 5f;
        temperature = 50;
        attackDMG = 20;
        level = 1;
        oxygen = 100;
        awakeness = 100;
        gatheringSpeed = 2f;
        HP = 50;
       
    }

    public override void Init()
    {
        ResetStats();
    }

    public float GetSetHP { get { return HP; } set { 
            Debug.Log("HP is : " + HP);
            HP = value; 
            Debug.Log("HP is Now : " + HP); }
    }
    public float GetSetAwakeness { get { return awakeness; } set { awakeness = value; } }
    public float GetSetOxygen { get { return oxygen; } set { oxygen = value; } }
    public float GetSetThirst { get { return thirst; } set { thirst = value; } }
    public float GetSetHunger { get { return hunger; } set { Debug.Log("Hunger is Now : " + hunger); hunger = value; Debug.Log("Hunger is Now : " + hunger); } }
    public float GetSetTemperature { get { return temperature; } set { temperature = value; } }
    public float GetSetAttackDMG { get { return attackDMG; } set { attackDMG = value; } }
    public float GetSetEXP { get { return EXP; } set { EXP = value; } }
    public int GetLevel { get { return level; } }
    public float GetSetSpeed { get { return speed; } set { speed = value; } }
    public float GetSetGatheringSpeed { get { return gatheringSpeed; } set { gatheringSpeed = value; } }


}
