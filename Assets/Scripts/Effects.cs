using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Effect", menuName = "Effects/" + "HP")]
public class Effects : Effect
{
       /// <summary>
       /// Need to add 5 types of effects with different Functionality 
       /// 
       ///
       
       /// 
       /// </summary>
    public override void ConsumeEffect()
    {
     
    }
}

public abstract class Effect : ConsumableEffect
{
    public abstract void ConsumeEffect();

}
[Serializable]
public class ConsumableEffect :  ScriptableObject
{
    public EffectType effectType;
    public bool isInstantEffect;
    public bool isStackAble;
    public float duration;
    public int amount;
}