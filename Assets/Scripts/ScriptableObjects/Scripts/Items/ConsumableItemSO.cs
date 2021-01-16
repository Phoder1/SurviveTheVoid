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

	public ConsumableEffect[] instantEffects;
	public ConsumableEffect[] OverTimeEffects;

	//[SerializeField]
	//private EffectType effectTypeType;
	//public EffectType GetEffectType => effectTypeType;

	//[SerializeField]
	//private int consumeAmount;
	//public int GetConsumeAmount => consumeAmount;
}



[Serializable]
public class ConsumableEffect
{
	public EffectType effectType;
	public float Duration;
	public int Amount;
}


