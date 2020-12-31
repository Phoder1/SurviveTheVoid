using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Items/" + "Consumable")]
public class ConsumableItemSO : ScriptableObject
{
	public enum ConsumableType
	{
		Oxygen,
		Thirst,
		Hunger
	}

	[SerializeField]
	private ConsumableType consumableType;
	public ConsumableType GetConsumableType => consumableType;

	[SerializeField]
	private int consumeAmount;
	public int GetConsumeAmount => consumeAmount;


	



}
