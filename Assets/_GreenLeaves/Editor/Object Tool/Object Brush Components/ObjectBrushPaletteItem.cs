using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class WeightedScatterGroupItem : WeightedListItem
{
	public ObjectBrushObjectList m_scatterObject;
}

[CreateAssetMenu(menuName = "Object Brush/Palette Item")]
public class ObjectBrushPaletteItem : ObjectBrushWeightedList<WeightedScatterGroupItem>
{
	[BoxGroup("Scatter Objects", centerLabel: true)]

	[ListItemSelector("SetSelected")]
	public WeightedScatterGroupItem[] itemList;

	[BoxGroup("Scatter Objects")]
	[ShowInInspector, InlineEditor(InlineEditorModes.GUIAndPreview)]
	public ObjectBrushObjectList m_selectedScatterObject;

	[BoxGroup("Scatter Objects")]
	public int m_amountOfScatterObjectsMax;
	[BoxGroup("Scatter Objects")]
	public float m_scatterObjectPlacementRadius;
	[BoxGroup("Scatter Objects")]
	public float m_scatterObjectsSpacingRadius;

	[BoxGroup("Main Object", centerLabel: true)]

	[ShowInInspector, InlineEditor(InlineEditorModes.GUIAndPreview)]
	public ObjectBrushObjectList m_mainObject;
	[BoxGroup("Main Object")]
	public float m_mainObjectsSpacingRadius;

	public void SetSelected(int index)
	{
		this.m_selectedScatterObject = index >= 0 ? this.itemList[index].m_scatterObject : null;
	}
}