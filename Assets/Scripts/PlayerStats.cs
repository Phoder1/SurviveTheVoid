using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoSingleton<PlayerStats>
{

    float HP;
    float hunger;
    float EXP;
    float moveSpeed;
    float temperature;
    float attackDMG;  
    int level;
    float thirst;
    float oxygen;
    float awakeness;
    float gatheringSpeed;

    void ResetStats()
    {
        thirst = 100f;
        hunger = 100f;
        EXP = 0;
        moveSpeed = 5f;
        temperature = 50;
        attackDMG = 5f;
        level = 1;
        oxygen = 100f;
        awakeness = 100f;
        gatheringSpeed = 2f;
        HP = 100f;
       
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
    public float GetSetSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    public float GetSetGatheringSpeed { get { return gatheringSpeed; } set { gatheringSpeed = value; } }


}
