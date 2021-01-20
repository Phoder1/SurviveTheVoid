using System;
using UnityEngine;

public enum EffectType {
Instant, Toggle, OverTime
}

//public enum EffectCategory
//{
//	HP, // + HP instant
//	HP_Regeneration, // + small amount of HP over duration time
//	MaxHP, // HP maximum
//	Food,  // + Hunger instant
//	Food_Regeneration, // - small amount of hunger over duration time
//	MaxFood,
//	Water, // + Thirst instant
//	Water_Regeneration, // - Thirst instant
//	MaxWater,
//	Air, // + oxygen instant
//	Air_Regeneration, // - small amount of oxygen over duration time
//	MaxAir,
//	Sleep, // + instant awakeness
//	Sleep_Regeneration, // - small amount of awakeness over duration time
//	MaxSleep,
//	EXP, // + EXP instant
//	MovementSpeed, // + speed instant over duration time
//	Strength, // + attackDMG instant over duration time
//	GatheringSpeed, // + gatheringSpeed instant over duration of time
//	Defence, // + 
//	Light, // + Special abillity
//	Flight, // + Special abillity
//}

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Items/" + "Consumable")]
public class ConsumableItemSO : ItemSO
{
	public EffectData[] Effects;
	public void ApplyEffect() {
		EffectHandler._instance.BeginAllConsumeableEffects(Effects);
	}
}
[Serializable]
public class EffectData
{
	public StatType effectStatType;
	[Tooltip("OVERTIME: it will Add the amount every tick time\n" +
		"INSTANT: it will Add the amount once\n" +
		"TOGGLE: Add the amount, wait the duration, remove the amount.")]
	public EffectType effectType;
	//[Header("OverTime: it will Add the amount every tick time")]
	//[Header("Instant: it will Add the amount once")]
	//[Header("Toggle: Add the amount, wait the duration, remove the amount")]
	//[Space]
	//[Header(" please write the precentage like :")]
	//[Header("if Precentage:")]
	[Tooltip("Whether to use a fixed amount in precentage.")]
	
	public bool inPercentage;
	[Tooltip("Whether to use precentage relative to max amount.")]
	public bool isRelativeToMax; 
	[Space (20f)]
	[Tooltip("If was set to percentage make sure to use whole numbers:\n" +
		"(10 = 10%, 110 = 110%)\n" +
		"In % per second, not per tick")]
	public float amount;
	[Tooltip("The total duration of the effect.")]
	public float duration;
	[Tooltip("The time between additions of the amount, when set to overtime.")] 
	[Min(0.03f)]
	public float tickTime;
}



