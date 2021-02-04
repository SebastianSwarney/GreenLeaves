using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.Serialization.Internal;
using Sirenix.Serialization.Editor;
using Sirenix.Serialization.Utilities;

/*
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
*/

[CreateAssetMenu(menuName = "Object Brush/Object List")]
public class ObjectBrushObjectList : SerializedScriptableObject
{
	public GameObject[] m_objectsToPlace;

	[Header("Object Appearance Properties")]
	[MinMaxSlider(0.1f, 3, showFields: true)]
	public Vector2 m_scaleVaritation;
	public bool m_lockWidthToHeight = true;
	public bool m_randomYRotation = true;

	[Header("Ground Properties")]
	[MinMaxSlider(-1, 1, showFields: true)]
	public Vector2 m_localGroundOffset;
	public float m_worldOffsetFromGround = 0;
	public bool m_alignObjectToGroundNormal;

	[Range(0, 100)]
	public float m_spawnChance;

	[Header("Height Mask Poperties")]
	public HeightMask m_heightMask;

	[Header("Slope Mask Poperties")]
	public SlopeMask m_slopeMask;

	[Header("Curvature Mask Poperties")]
	public CurvatureMask m_curvatureMask;

	public void SpawnObject(Vector3 p_spawnPoint, Vector3 p_slopeNormal, Transform p_objectParent, GameObject p_objectToSpawn)
	{
		GameObject newObject = p_objectToSpawn;
		//newObject = (GameObject)PrefabUtility.InstantiatePrefab(p_objectToSpawn);

		if (newObject == null)
		{
			Debug.LogError("Error instantiating prefab");
			return;
		}

		newObject.transform.position = p_spawnPoint;
		ModifyObject(newObject, p_slopeNormal);
		newObject.transform.parent = p_objectParent;
	}

	public GameObject GetObjectFromList()
	{
		return m_objectsToPlace[Random.Range(0, m_objectsToPlace.Length)];
	}

    #region Spawn Checking
    public bool CheckSpawn(float p_normalizedHeight, float p_curvature, float p_slope)
    {
		if ((Random.value * 100f) >= m_spawnChance)
		{
			return false;
		}

		float heightSpawnChance = m_heightMask.GetMaskValue(p_normalizedHeight);

		if ((Random.value) >= heightSpawnChance)
		{
			return false;
		}

		float curvatureSpawnRange = m_curvatureMask.GetMaskValue(p_curvature);

		if ((Random.value) >= curvatureSpawnRange)
		{
			return false;
		}

		float slopeSpawnChance = m_slopeMask.GetMaskValue(p_slope);

		if ((Random.value) >= slopeSpawnChance)
		{
			return false;
		}

		return true;
	}

	public bool CheckSlope(float p_targetSlopeAngle)
	{
		/*
		if (p_targetSlopeAngle < m_slopeAngle.y && p_targetSlopeAngle > m_slopeAngle.x)
		{
			return true;
		}
		*/

		return false;
	}
    #endregion

    #region Object Modification
    public void ModifyObject(GameObject p_objectToModify, Vector3 p_slopeNormal)
	{
		ModifySize(p_objectToModify);
		ModifyHeight(p_objectToModify);
		ModifyRoation(p_objectToModify, p_slopeNormal);
	}

	public void ModifyHeight(GameObject p_objectToModify)
	{
		if (m_localGroundOffset.y != 0)
		{
			float offsetAmount = Random.Range(m_localGroundOffset.x, m_localGroundOffset.y);

			MeshRenderer[] foundMeshRenderers = p_objectToModify.GetComponents<MeshRenderer>();

			MeshRenderer tallestMesh = null;

			foreach (MeshRenderer meshRenderer in foundMeshRenderers)
			{
				if (tallestMesh == null)
				{
					tallestMesh = meshRenderer;
				}

				if (tallestMesh.bounds.size.y < meshRenderer.bounds.size.y)
				{
					tallestMesh = meshRenderer;
				}
			}

			float objectHeight = tallestMesh.bounds.size.y;
			p_objectToModify.transform.position += Vector3.up * (objectHeight * offsetAmount);
		}

		if (m_worldOffsetFromGround != 0)
		{
			p_objectToModify.transform.position += Vector3.up * m_worldOffsetFromGround;
		}
	}

	private void ModifySize(GameObject p_objectToModify)
	{
		if (m_scaleVaritation.y > 1)
		{
			if (m_lockWidthToHeight)
			{
				float newScale = Random.Range(m_scaleVaritation.x, m_scaleVaritation.y);
				p_objectToModify.transform.localScale = Vector3.one * newScale;
			}
			else
			{
				float newHeight = Random.Range(m_scaleVaritation.x, m_scaleVaritation.y);
				float newWidth = Random.Range(m_scaleVaritation.x, m_scaleVaritation.y);
				float newDepth = Random.Range(m_scaleVaritation.x, m_scaleVaritation.y);

				Vector3 newScale = new Vector3(newWidth, newHeight, newDepth);

				p_objectToModify.transform.localScale = newScale;
			}
		}
	}

	private void ModifyRoation(GameObject p_objectToModify, Vector3 p_slopeNormal)
	{
		if (m_randomYRotation)
		{
			p_objectToModify.transform.rotation = Quaternion.Euler(p_objectToModify.transform.rotation.eulerAngles.x, Random.Range(0, 360), p_objectToModify.transform.rotation.eulerAngles.z);
		}


		if (m_alignObjectToGroundNormal)
		{
			Vector3 lookVector = Vector3.Cross(p_slopeNormal, Vector3.up);

            if (lookVector.magnitude != 0)
            {
				p_objectToModify.transform.rotation = Quaternion.LookRotation(lookVector, p_slopeNormal);
			}
		}
	}
    #endregion

    public Vector3 PlaceObject
	(
		Vector3 p_originPos,
		float p_placementRadus,
		ref List<GameObject> p_alreadyPlacedObjects,
		ObjectBrushObjectList[] p_allObjectLists,
		Transform p_objectRoot,
		float p_spacingRadius,
		LayerMask p_layerMask
	)
	{
		bool blockPlacement = false;

		Vector2 random = Random.insideUnitCircle * p_placementRadus;
		Vector3 randomFlat = new Vector3(random.x, 0, random.y);
		Vector3 posOffsetFromRandom = p_originPos + randomFlat;

		RaycastHit hit;

		if (Physics.Raycast(posOffsetFromRandom + (Vector3.up * 10000), Vector3.down, out hit, Mathf.Infinity, p_layerMask))
		{
			Vector3 placePos = hit.point;
			float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

			if (CheckSlope(slopeAngle))
			{
				if (CheckArea(placePos, p_alreadyPlacedObjects, p_allObjectLists, p_spacingRadius))
				{
					blockPlacement = true;
				}

				if (!blockPlacement)
				{
					//WeightedObjectListItem objectListItem = ((WeightedObjectListItem)listToUse.GetItemFromWeightedList(listToUse.itemList));
					//GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(objectListItem.objectToPlace);
					GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(GetObjectFromList());

					if (newObject == null)
					{
						Debug.LogError("Error instantiating prefab");
						return Vector3.zero;
					}

					Undo.RegisterCreatedObjectUndo(newObject, "Object Brush");

					p_alreadyPlacedObjects.Add(newObject);
					newObject.transform.localPosition = placePos;
					ModifyObject(newObject, hit.normal);
					newObject.transform.parent = p_objectRoot;
					return newObject.transform.position;
				}
			}
		}

		return Vector3.zero;
	}

	public bool CheckIfObjectIsInGroup(GameObject p_objectToCheck)
	{
		if (p_objectToCheck != null)
		{
			if (PrefabUtility.IsPartOfAnyPrefab(p_objectToCheck))
			{
				GameObject foundPrefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(p_objectToCheck).transform.root.gameObject;

				for (int i = 0; i < m_objectsToPlace.Length; i++)
				{
					if (m_objectsToPlace[i].gameObject == foundPrefabAsset)
					{
						return true;
					}
				}
			}
		}

		return false;
	}

	public bool CheckIfObjectIsInGroup(Collider p_colliderOnObjectToCheck)
	{
		if (p_colliderOnObjectToCheck != null)
		{
			if (PrefabUtility.IsPartOfAnyPrefab(p_colliderOnObjectToCheck))
			{
				GameObject foundPrefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(p_colliderOnObjectToCheck).transform.root.gameObject;

				for (int i = 0; i < m_objectsToPlace.Length; i++)
				{
					if (m_objectsToPlace[i].gameObject == foundPrefabAsset)
					{
						return true;
					}
				}
			}
		}

		return false;
	}

	public bool CheckArea(Vector3 p_targetPosition, List<GameObject> p_alreadyPlacedObjects, ObjectBrushObjectList[] p_allObjectLists, float p_spacingRadius)
	{
		Collider[] foundColliders = Physics.OverlapSphere(p_targetPosition, p_spacingRadius);

		foreach (Collider collider in foundColliders)
		{
			for (int i = 0; i < p_allObjectLists.Length; i++)
			{
				if (p_allObjectLists[i].CheckIfObjectIsInGroup(collider))
				{
					return true;
				}
			}
		}

		foreach (GameObject placedObject in p_alreadyPlacedObjects)
		{
			MeshRenderer[] foundMeshRenderers = placedObject.GetComponents<MeshRenderer>();

			foreach (MeshRenderer meshRenderer in foundMeshRenderers)
			{
				if ((Vector3.Distance(p_targetPosition, placedObject.transform.position) < p_spacingRadius) || meshRenderer.bounds.Contains(p_targetPosition))
				{
					return true;
				}
			}

			/*
			MeshRenderer foundMeshRenderer = placedObject.GetComponentInChildren<MeshRenderer>();

			if ((Vector3.Distance(targetPosition, placedObject.transform.position) < spacingRadius) || foundMeshRenderer.bounds.Contains(targetPosition))
			{
				return true;
			}
			*/
		}

		return false;
	}
}
