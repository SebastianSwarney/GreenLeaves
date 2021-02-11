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
	public WeightedTerrainPaletteItem[] m_objectListsInUse;

	public ObjectBrushObjectList GetObjectList()
    {
		return ((WeightedTerrainPaletteItem)GetItemFromWeightedList(m_objectListsInUse)).m_objectList;
	}

	private void OnValidate()
	{
		CalculatePercentages(m_objectListsInUse);
	}
}
