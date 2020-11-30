using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[System.Serializable]
public class WeightedListItem
{
	[Range(0, 100)]
	public float sliderValue;
	[ReadOnly]
	[Range(0, 100)]
	public float percentageChance;
}

public class ObjectBrushWeightedList<ListItemType> : ScriptableObject where ListItemType : WeightedListItem
{
	/*
	public ListItemType[] itemList;

	private void OnValidate()
	{
		CalculatePercentages();
	}
	*/

	public WeightedListItem GetItemFromWeightedList(WeightedListItem[] listType)
	{
		WeightedListItem fallbackItem = null;

		float minRange = 0;
		float maxRange = listType[0].percentageChance;

		float randomValue = Random.Range(0, 100);

		for (int i = 0; i < listType.Length; i++)
		{
			if (i != 0)
			{
				minRange += listType[i - 1].percentageChance;
				maxRange = minRange + listType[i].percentageChance;
			}

			if (listType[i].percentageChance != 0)
			{
				if (randomValue >= minRange && randomValue <= maxRange)
				{
					return listType[i];
				}
			}
		}

		return fallbackItem;
	}

	public void CalculatePercentages(WeightedListItem[] listType)
	{
		float totalValue = 0;

		for (int i = 0; i < listType.Length; i++)
		{
			totalValue += listType[i].sliderValue;
		}

		for (int i = 0; i < listType.Length; i++)
		{
			listType[i].percentageChance = (listType[i].sliderValue / totalValue) * 100;
		}
	}
}
