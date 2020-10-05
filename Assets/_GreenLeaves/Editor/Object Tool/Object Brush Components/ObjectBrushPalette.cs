using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[System.Serializable]
public class WeightedPaletteItem : WeightedListItem
{
	public ObjectBrushPaletteItem palletLoadoutItem;
}

[CreateAssetMenu(menuName = "Object Brush/Palette")]
public class ObjectBrushPalette : ObjectBrushWeightedList<WeightedPaletteItem>
{
	public void RunPalettePlacement(Vector3 brushOrgin, float placementRadus, ref List<GameObject> alreadyPlacedObjects, ObjectBrushObjectList[] allObjectLists, Transform objectRoot)
	{
		ObjectBrushPaletteItem selectedPaletteItem = ((WeightedPaletteItem)GetItemFromWeightedList(itemList)).palletLoadoutItem;

		Vector3 rootObjectPos = PlaceObjectFromPalette(brushOrgin, placementRadus, ref alreadyPlacedObjects, allObjectLists, objectRoot, selectedPaletteItem.mainObject, selectedPaletteItem.mainObjectsSpacingRadius);

		if (rootObjectPos != Vector3.zero)
		{
			if (selectedPaletteItem.amountOfScatterObjectsMax != 0)
			{
				int amountToSpawn = Random.Range(1, selectedPaletteItem.amountOfScatterObjectsMax);

				for (int i = 0; i < amountToSpawn; i++)
				{
					ObjectBrushObjectList foundScatterGroup = ((WeightedScatterGroupItem)GetItemFromWeightedList(selectedPaletteItem.itemList)).scatterObject;
					PlaceObjectFromPalette(rootObjectPos, selectedPaletteItem.scatterObjectPlacementRadius, ref alreadyPlacedObjects, allObjectLists, objectRoot, foundScatterGroup, selectedPaletteItem.scatterObjectsSpacingRadius);
				}
			}
		}
	}

	public Vector3 PlaceObjectFromPalette(Vector3 originPos, float placementRadus, ref List<GameObject> alreadyPlacedObjects, ObjectBrushObjectList[] allObjectLists, Transform objectRoot, ObjectBrushObjectList listToUse, float spacingRadius)
	{
		bool blockPlacement = false;

		Vector2 random = Random.insideUnitCircle * placementRadus;
		Vector3 randomFlat = new Vector3(random.x, 0, random.y);
		Vector3 posOffsetFromRandom = originPos + randomFlat;

		RaycastHit hit;

		if (Physics.Raycast(posOffsetFromRandom + (Vector3.up * 10000), Vector3.down, out hit, Mathf.Infinity))
		{
			Vector3 placePos = hit.point;
			float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

			if (slopeAngle < listToUse.slopeAngle.y && slopeAngle > listToUse.slopeAngle.x)
			{
				if (listToUse.CheckArea(placePos, ref alreadyPlacedObjects, allObjectLists, spacingRadius))
				{
					blockPlacement = true;
				}

				if (!blockPlacement)
				{
					WeightedObjectListItem objectListItem = ((WeightedObjectListItem)listToUse.GetItemFromWeightedList(listToUse.itemList));

					GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(objectListItem.objectToPlace);

					if (newObject == null)
					{
						Debug.LogError("Error instantiating prefab");
						return Vector3.zero;
					}

					Undo.RegisterCreatedObjectUndo(newObject, "Object Brush");

					alreadyPlacedObjects.Add(newObject);
					newObject.transform.localPosition = placePos;
					listToUse.ModifyObject(newObject, hit.normal);
					newObject.transform.parent = objectRoot;
					return newObject.transform.position;
				}
			}
		}

		return Vector3.zero;
	}
}