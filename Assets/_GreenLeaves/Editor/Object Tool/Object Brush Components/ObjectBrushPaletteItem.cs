using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedScatterGroupItem : WeightedListItem
{
	public ObjectBrushObjectList scatterObject;
}

[CreateAssetMenu(menuName = "Object Brush/Palette Item")]
public class ObjectBrushPaletteItem : ObjectBrushWeightedList<WeightedScatterGroupItem>
{
	[Header("Scatter Object Properties")]
	public int amountOfScatterObjectsMax;
	public float scatterObjectPlacementRadius;
	public float scatterObjectsSpacingRadius;

	[Header("Main Object Properties")]
	public ObjectBrushObjectList mainObject;
	public float mainObjectsSpacingRadius;
}