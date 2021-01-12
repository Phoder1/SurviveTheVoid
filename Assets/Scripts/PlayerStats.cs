
using UnityEngine;

public class PlayerStats : MonoBehaviour
{


    int HP;
    int hunger;
    int EXP;
    float speed=5;
    int temperature;
    int attackDMG;
    int level;
    public void Init()
    {
     
    }

    public int GetSetHP { get { return HP; } set { HP = value; } }
    public int GetSetHunger { get { return hunger; } set { hunger = value; } }
    public int GetSetTemperature { get { return temperature; } set { temperature = value; } }
    public int GetSetAttackDMG { get { return attackDMG; } set { attackDMG = value; } }
    public int GetSetEXP { get { return EXP; } set { EXP = value; } }
    public int GetSetLevel { get { return level; } set { level = value; } }
    public float GetSetSpeed { get { return speed; } set { speed = value; } }
}