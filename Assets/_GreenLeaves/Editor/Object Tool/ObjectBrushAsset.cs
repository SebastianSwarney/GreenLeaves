using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class ObjectBrushSettings : StandardToolSettings
{
	public KeyCode placeObjectsKey;
}

[CreateAssetMenu(menuName = "Object Brush/Brush Asset")]
public class ObjectBrushAsset : ObjectToolStandardBase<ObjectBrushSettings>
{
	public int numberOfObjectsToPlace;

	[Space]
	[Space]

	public ObjectBrushPalette currentPalette;

	private bool canPlaceObjects;

	public override void RunTool(RaycastHit hit, Event currentEvent, SceneView screenView, Transform placedObjectRoot)
	{
		base.RunTool(hit, currentEvent, screenView, placedObjectRoot);
		RunObjectPlacement(hit, currentEvent, placedObjectRoot);
	}

	private void RunObjectPlacement(RaycastHit hit, Event currentEvent, Transform placedObjectRoot)
	{
		if (currentEvent.type == EventType.KeyDown)
		{
			if (currentEvent.keyCode == settings.placeObjectsKey)
			{
				if (canPlaceObjects)
				{
					PlaceObjects(numberOfObjectsToPlace, brushRadius, hit, placedObjectRoot);
					canPlaceObjects = false;
				}
			}
		}

		if (currentEvent.type == EventType.KeyUp)
		{
			if (currentEvent.keyCode == settings.placeObjectsKey)
			{
				canPlaceObjects = true;
			}
		}
	}

	public void PlaceObjects(int amountToSpawn, float placeRadius, RaycastHit hitPosition, Transform objectRoot)
	{
		List<GameObject> alreadyPlacedObjects = new List<GameObject>();

		for (int i = 0; i < amountToSpawn; i++)
		{
			currentPalette.RunPalettePlacement(hitPosition.point, placeRadius, ref alreadyPlacedObjects, allObjectLists, objectRoot, groundMask);
		}
	}
}