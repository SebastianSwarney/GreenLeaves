using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class WeightedObjectListItem : WeightedListItem
{
	public GameObject objectToPlace;
}

[System.Serializable]
public class ObjectListDisplay
{
	public WeightedObjectListItem[] listObjects;
}

[CreateAssetMenu(menuName = "Object Brush/Object List")]
public class ObjectBrushObjectList : ObjectBrushWeightedList<WeightedObjectListItem>
{
	[Header("Object Appearance Properties")]
	//[MinMaxSlider(1, 10, false)]
	public Vector2 scaleVaritation;
	public bool lockWidthToHeight = true;
	public bool randomYRotation = true;

	[Header("Ground Properties")]
	//[MinMaxSlider(-90, 90, true)]
	public Vector2 slopeAngle;
	//[MinMaxSlider(-1, 1, false)]
	public Vector2 localGroundOffset;
	public float worldOffsetFromGround = 0;
	public bool alignObjectToGroundNormal;

	private float spacingRadius;

	[ContextMenu("Test")]
	public void Test()
	{
		Debug.Log("ran");
	}

	public bool CheckSlope(float targetSlopeAngle)
	{
		if (targetSlopeAngle < slopeAngle.y && targetSlopeAngle > slopeAngle.x)
		{
			return true;
		}

		return false;
	}

	public void ModifyHeight(GameObject objectToModify, Vector3 slopeNormal)
	{
		if (localGroundOffset.y != 0)
		{
			float offsetAmount = Random.Range(localGroundOffset.x, localGroundOffset.y);

			MeshRenderer foundMeshRenderer = objectToModify.GetComponentInChildren<MeshRenderer>();

			float objectHeight = foundMeshRenderer.bounds.size.y;

			objectToModify.transform.position += Vector3.up * (objectHeight * offsetAmount);
		}

		if (worldOffsetFromGround != 0)
		{
			objectToModify.transform.position += Vector3.up * worldOffsetFromGround;
		}

		if (alignObjectToGroundNormal)
		{
			objectToModify.transform.rotation = Quaternion.LookRotation(Vector3.Cross(slopeNormal, Vector3.up), slopeNormal);
		}
	}

	public void ModifyObject(GameObject objectToModify, Vector3 slopeNormal)
	{
		if (scaleVaritation.y > 1)
		{
			if (lockWidthToHeight)
			{
				float newScale = Random.Range(scaleVaritation.x, scaleVaritation.y);
				objectToModify.transform.localScale = Vector3.one * newScale;
			}
			else
			{
				float newHeight = Random.Range(scaleVaritation.x, scaleVaritation.y);
				float newWidth = Random.Range(scaleVaritation.x, scaleVaritation.y);
				float newDepth = Random.Range(scaleVaritation.x, scaleVaritation.y);

				Vector3 newScale = new Vector3(newWidth, newHeight, newDepth);

				objectToModify.transform.localScale = newScale;
			}
		}

		ModifyHeight(objectToModify, slopeNormal);

		if (randomYRotation)
		{
			objectToModify.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
		}
	}

	public bool CheckIfObjectIsInGroup(GameObject objectToCheck)
	{
		if (objectToCheck != null)
		{
			if (PrefabUtility.IsPartOfAnyPrefab(objectToCheck))
			{
				GameObject foundPrefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(objectToCheck).transform.root.gameObject;

				for (int i = 0; i < itemList.Length; i++)
				{
					if (itemList[i].objectToPlace == foundPrefabAsset)
					{
						return true;
					}
				}
			}
		}

		return false;
	}

	public bool CheckIfObjectIsInGroup(Collider colliderOnObjectToCheck)
	{
		if (colliderOnObjectToCheck != null)
		{
			if (PrefabUtility.IsPartOfAnyPrefab(colliderOnObjectToCheck))
			{
				GameObject foundPrefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(colliderOnObjectToCheck).transform.root.gameObject;

				for (int i = 0; i < itemList.Length; i++)
				{
					if (itemList[i].objectToPlace == foundPrefabAsset)
					{
						return true;
					}
				}
			}
		}

		return false;
	}

	public bool CheckArea(Vector3 targetPosition, ref List<GameObject> alreadyPlacedObjects, ObjectBrushObjectList[] allObjectLists, float spacingRadius)
	{
		Collider[] foundColliders = Physics.OverlapSphere(targetPosition, spacingRadius);

		foreach (Collider collider in foundColliders)
		{
			for (int i = 0; i < allObjectLists.Length; i++)
			{
				if (allObjectLists[i].CheckIfObjectIsInGroup(collider))
				{
					return true;
				}
			}
		}

		foreach (GameObject placedObject in alreadyPlacedObjects)
		{
			MeshRenderer foundMeshRenderer = placedObject.GetComponentInChildren<MeshRenderer>();

			if ((Vector3.Distance(targetPosition, placedObject.transform.position) < spacingRadius) || foundMeshRenderer.bounds.Contains(targetPosition))
			{
				return true;
			}
		}

		return false;
	}

}
