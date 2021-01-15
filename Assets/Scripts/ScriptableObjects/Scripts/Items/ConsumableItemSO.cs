using System;
using UnityEngine;



public enum EffectType
{
	HP,
	Food,
	HP_Regeneration,
	Water,
	Air,
	Sleep,
	XP,
	Speed,
	Slow,
	Strength,
	Weakness,
	Gathering,
	Defence,
	Poison,
	Light,
	Flight,
	Hunger,
	Thirst,
	Fatigue,
	Panting

}

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Items/" + "Consumable")]
public class ConsumableItemSO : ItemSO
{

	public InstantEffect[] instantEffects;
	public OverTimeEffect[] OverTimeEffects;

	//[SerializeField]
	//private EffectType effectTypeType;
	//public EffectType GetEffectType => effectTypeType;

	//[SerializeField]
	//private int consumeAmount;
	//public int GetConsumeAmount => consumeAmount;
}



[Serializable]
public class InstantEffect
{
	public EffectType effectType;
	public int Amount;
}


[Serializable]
public class OverTimeEffect
{
	public EffectType effectType;
	public int Strength;
	public float Duration;
}