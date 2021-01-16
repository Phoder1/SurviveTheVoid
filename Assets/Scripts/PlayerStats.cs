using UnityEngine;
using UnityEngine.Networking;

public class PlayerStats : MonoSingleton<PlayerStats>
{
    float EXPAmountToLevelUp;
    float maximumAmount;
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
         EXPAmountToLevelUp = 100;
         maximumAmount = 100f;
    }

    public override void Init()
    {
        ResetStats();
    }

    public float GetSetHP { get { return HP; } set { 
            Debug.Log("HP is : " + HP);
            HP = value;

            if (HP < 0)
            {
                // DEAD LOGIC
                HP = 0;
            }
            else if(HP > maximumAmount)
            {
                HP = maximumAmount;
            }


            Debug.Log("HP is Now : " + HP); }
    }
    public float GetSetAwakeness { get { return awakeness; } set {



            awakeness = value;


            if (awakeness < 0)
            {
                // TIRED LOGIC
                awakeness = 0;
            }
            else if (awakeness > maximumAmount)
            {
                awakeness = maximumAmount;
            }
        } 
    }
    public float GetSetOxygen { get { return oxygen; } set {
            oxygen = value;


            if (oxygen < 0)
            {
                // DEATH LOGIC
                oxygen = 0;
            }
            else if (oxygen > maximumAmount)
            {
                oxygen = maximumAmount;
            }

        } 
    }
    public float GetSetThirst { get { return thirst; } set {
            thirst = value;


            if (thirst < 0)
            {
                // Thirst LOGIC
                thirst = 0;
            }
            else if (thirst > maximumAmount)
            {
                thirst = maximumAmount;
            }
        }
    }
    public float GetSetHunger { get { return hunger; } set { 
            Debug.Log("Hunger is Now : " + hunger); 
            hunger = value;
            Debug.Log("Hunger is Now : " + hunger);


            if (hunger < 0)
            {
                // HUNGRY LOGIC
                hunger = 0;
            }
            else if (hunger > maximumAmount)
            {
                hunger = maximumAmount;
            }
        } 
    }
    public float GetSetTemperature { get { return temperature; } set {
            temperature = value;


            if (temperature < 0)
            {
                // COLD LOGIC
                temperature = 0;
            }
            else if (temperature > maximumAmount)
            {
                // HOT LOGIC
                temperature = maximumAmount;
            }
        }
    }
    public float GetSetAttackDMG { get { return attackDMG; } set {
            attackDMG = value;


            if (attackDMG<= 0)
            {
                attackDMG = 2f;
            }
        } 
    }
    public float GetSetEXP { get { return EXP; } set { 

            EXP = value;

            if (EXP> EXPAmountToLevelUp)
            {
                level++;
                float expLeftToAddCache = EXP - EXPAmountToLevelUp;
                EXP = 0;
                GetSetEXP += expLeftToAddCache;
            }
            else if (EXP == EXPAmountToLevelUp)
            {
                level++;
                EXP = 0;
            }


        }
    }
    public int GetLevel { get { 
            return level;
        }
    }
    public float GetSetSpeed { get { return moveSpeed; } set {
            moveSpeed = value;

            if (moveSpeed <= 0)
                moveSpeed = 5f;
            
        }
    }
    public float GetSetGatheringSpeed { get { return gatheringSpeed; } set {
            gatheringSpeed = value;
        } 
    }



}
