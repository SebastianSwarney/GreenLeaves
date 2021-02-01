using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class WeightedTerrainPaletteItem : WeightedListItem
{
	[InlineEditor]
	public ObjectBrushObjectList m_objectList;
}

[CreateAssetMenu(menuName = "Object Brush/Terrain Spawner Palette")]
public class TerrainObjectSpawnerPalette : ObjectBrushWeightedList<WeightedTerrainPaletteItem>
{
	public WeightedTerrainPaletteItem[] m_objectList;

	public ObjectBrushObjectList GetObjectList()
    {
		return ((WeightedTerrainPaletteItem)GetItemFromWeightedList(m_objectList)).m_objectList;
	}

	private void OnValidate()
	{
		CalculatePercentages(m_objectList);
	}
}
