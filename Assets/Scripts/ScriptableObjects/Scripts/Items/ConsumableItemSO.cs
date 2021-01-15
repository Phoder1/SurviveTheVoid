using System;
using System.Collections;
using UnityEngine;



public enum EffectType
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

}





