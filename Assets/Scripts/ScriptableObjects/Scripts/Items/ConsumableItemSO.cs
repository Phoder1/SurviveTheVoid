using System;
using UnityEngine;

public enum EffectType {
Instant, ToggleOverTime, OverTimeSmallPortion
}

public enum EffectCategory
{
	HP, // + HP instant
	HP_Regeneration, // + small amount of HP over duration time
	Food,  // + Hunger instant
	Water, // + Thirst instant
	Air, // + oxygen instant
	Sleep, // + instant awakeness
	EXP, // + EXP instant
	Speed, // + speed instant over duration time
	Slow, // - speed instant over duration time
	Strength, // + attackDMG instant over duration time
	Weakness, // - attackDMG instant over duration time
	Gathering, // + gatheringSpeed instant over duration of time
	Defence, // + 
	Poison, // - small amount of HP over duration time
	Light, // + 
	Flight, // + Special ability
	Food_Poisoning, // - small amount of hunger over duration time , 
	Thirst, // - Thirst instant
	Fatigue, // - small amount of awakeness over duration time
	Panting // - small amount of oxygen over duration time 

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
	public bool isRelative; 
	public bool isPresentage;
	public float amount;
	public float duration;
	public float tickTime;
}



