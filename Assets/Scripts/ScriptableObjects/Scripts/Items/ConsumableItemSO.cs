using System;
using UnityEngine;

public enum EffectType {
Instant, ToggleOverTime, OverTimeSmallPortion
}

public enum EffectCategory
{
	HP, // + HP instant
	HP_Regeneration, // + small amount of HP over duration time
	MaxHP, // HP maximum
	Food,  // + Hunger instant
	Food_Regeneration, // - small amount of hunger over duration time
	MaxFood,
	Water, // + Thirst instant
	Water_Regeneration, // - Thirst instant
	MaxWater,
	Air, // + oxygen instant
	Air_Regeneration, // - small amount of oxygen over duration time
	MaxAir,
	Sleep, // + instant awakeness
	Sleep_Regeneration, // - small amount of awakeness over duration time
	MaxSleep,
	EXP, // + EXP instant
	MovementSpeed, // + speed instant over duration time
	Strength, // + attackDMG instant over duration time
	GatheringSpeed, // + gatheringSpeed instant over duration of time
	Defence, // + 
	Light, // + Special abillity
	Flight, // + Special abillity
}

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Items/" + "Consumable")]
public class ConsumableItemSO : ItemSO
{
	
	public ConsumableEffect[] Effects;
	public void ApplyEffect() {

		if (Effects.Length == 0)
			return;
	

		for (int i = 0; i < Effects.Length; i++)
		{
			if (Effects[i] == null)
				continue;
			
			ConsumeEffectHandler._instance.StartEffect(Effects[i]);
		}

		
	}
}
[Serializable]
public class ConsumableEffect
{
	public EffectCategory effectCategory;
	public EffectType effectType;
	[Header("OverTimeSmallPortion: it will Add the amount every tick time")]
	[Header("nstant: it will Add the amount every tick time:")]
	[Header("ToggleOverTime: Add the amount,wait the duration, remove the amount")]
	[Header("it will update his adding from the current amount of the stat:")]
	[Header("if Relative:")]
	public bool isRelative; 
	[Header(" please write the precentage like 10 = 10%, 110 = 110%:")]
	[Header("if Precentage:")]
	public bool isPresentage;
	[Space (20f)]
	[Header("Reminder to check the setting above before adding amount:")]
	public float amount;
	[Header("The total duration of the buff:")]
	public float duration;
	[Header("The time between addition of the amount:")] 
	public float tickTime;
}



