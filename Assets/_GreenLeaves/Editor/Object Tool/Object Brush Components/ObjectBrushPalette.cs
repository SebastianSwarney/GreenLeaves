using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[System.Serializable]
public class WeightedPaletteItem : WeightedListItem
{
	public ObjectBrushPaletteItem m_palletLoadoutItem;
}

[CreateAssetMenu(menuName = "Object Brush/Palette")]
public class ObjectBrushPalette : ObjectBrushWeightedList<WeightedPaletteItem>
{
	[ListItemSelector("SetSelected")]
	public WeightedPaletteItem[] itemList;

	[BoxGroup("Titles", ShowLabel = false)]
	[TitleGroup("Titles/Current Palette Item")]
	[ShowInInspector, InlineEditor(InlineEditorModes.GUIAndPreview, InlineEditorObjectFieldModes.Hidden)]
	private ObjectBrushPaletteItem m_selectedPaletteItem;

	public void SetSelected(int index)
	{
		this.m_selectedPaletteItem = index >= 0 ? this.itemList[index].m_palletLoadoutItem : null;
	}

	public void RunPalettePlacement(Vector3 p_brushOrgin, float p_placementRadus, ref List<GameObject> p_alreadyPlacedObjects, ObjectBrushObjectList[] p_allObjectLists, Transform p_objectRoot, LayerMask p_groundMask)
	{
		ObjectBrushPaletteItem selectedPaletteItem = ((WeightedPaletteItem)GetItemFromWeightedList(itemList)).m_palletLoadoutItem;
		Vector3 rootObjectPos = selectedPaletteItem.m_mainObject.PlaceObject(p_brushOrgin, p_placementRadus, ref p_alreadyPlacedObjects, p_allObjectLists, p_objectRoot, selectedPaletteItem.m_mainObjectsSpacingRadius, p_groundMask);

		if (rootObjectPos != Vector3.zero)
		{
			if (selectedPaletteItem.m_amountOfScatterObjectsMax != 0)
			{
				int amountToSpawn = Random.Range(1, selectedPaletteItem.m_amountOfScatterObjectsMax);

				for (int i = 0; i < amountToSpawn; i++)
				{
					//ObjectBrushObjectList foundScatterObject = ((WeightedScatterGroupItem)GetItemFromWeightedList(selectedPaletteItem.itemList)).m_scatterObject;
					//foundScatterObject.PlaceObject(rootObjectPos, selectedPaletteItem.m_scatterObjectPlacementRadius, ref p_alreadyPlacedObjects, p_allObjectLists, p_objectRoot, selectedPaletteItem.m_scatterObjectsSpacingRadius, p_groundMask);
				}
			}
		}
	}

	/*
	public Vector3 PlaceObjectFromPalette(Vector3 p_originPos, float p_placementRadus, ref List<GameObject> p_alreadyPlacedObjects, ObjectBrushObjectList[] p_allObjectLists, Transform p_objectRoot, ObjectBrushObjectList p_listToUse, float p_spacingRadius)
	{
		bool blockPlacement = false;

		Vector2 random = Random.insideUnitCircle * p_placementRadus;
		Vector3 randomFlat = new Vector3(random.x, 0, random.y);
		Vector3 posOffsetFromRandom = p_originPos + randomFlat;

		RaycastHit hit;

		if (Physics.Raycast(posOffsetFromRandom + (Vector3.up * 10000), Vector3.down, out hit, Mathf.Infinity))
		{
			Vector3 placePos = hit.point;
			float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

			if (p_listToUse.CheckSlope(slopeAngle))
			{
				if (p_listToUse.CheckArea(placePos, p_alreadyPlacedObjects, p_allObjectLists, p_spacingRadius))
				{
					blockPlacement = true;
				}

				if (!blockPlacement)
				{
					//WeightedObjectListItem objectListItem = ((WeightedObjectListItem)listToUse.GetItemFromWeightedList(listToUse.itemList));
					//GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(objectListItem.objectToPlace);
					GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(p_listToUse.GetObjectFromList());

					if (newObject == null)
					{
						Debug.LogError("Error instantiating prefab");
						return Vector3.zero;
					}

					Undo.RegisterCreatedObjectUndo(newObject, "Object Brush");

					p_alreadyPlacedObjects.Add(newObject);
					newObject.transform.localPosition = placePos;
					p_listToUse.ModifyObject(newObject, hit.normal);
					newObject.transform.parent = p_objectRoot;
					return newObject.transform.position;
				}
			}
		}

		return Vector3.zero;
	}
	*/
}