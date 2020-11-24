using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class WeightedScatterGroupItem : WeightedListItem
{
	[InlineEditor(InlineEditorModes.GUIAndPreview)]
	public ObjectBrushObjectList m_scatterObject;
}

[CreateAssetMenu(menuName = "Object Brush/Palette Item")]
public class ObjectBrushPaletteItem : ObjectBrushWeightedList<WeightedScatterGroupItem>
{
	[Header("Scatter Object Properties")]
	public int m_amountOfScatterObjectsMax;
	public float m_scatterObjectPlacementRadius;
	public float m_scatterObjectsSpacingRadius;

	[Header("Main Object Properties")]
	[InlineEditor(InlineEditorModes.GUIAndPreview)]
	public ObjectBrushObjectList m_mainObject;
	public float m_mainObjectsSpacingRadius;
}