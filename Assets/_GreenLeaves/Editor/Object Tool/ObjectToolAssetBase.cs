using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class ObjectToolAssetBase : ScriptableObject
{
	[HideInInspector]
	public ObjectBrushObjectList[] allObjectLists;

	public LayerMask groundMask;

	public void RunSceneGUI(SceneView screenView, Transform placedObjectRoot, ObjectBrushObjectList[] allObjectListsInput)
	{
		allObjectLists = allObjectListsInput;

		Event currentEvent = Event.current;
		Vector3 mousePos = currentEvent.mousePosition;
		mousePos.y = screenView.camera.scaledPixelHeight - mousePos.y;
		Ray ray = screenView.camera.ScreenPointToRay(mousePos);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
		{
			RunTool(hit, currentEvent, screenView, placedObjectRoot);
		}
	}

	public abstract void RunTool(RaycastHit hit, Event currentEvent, SceneView screenView, Transform placedObjectRoot);

	public struct ObjectListWithWorldObject
	{
		public ObjectListWithWorldObject(ObjectBrushObjectList objectList, GameObject worldObject)
		{
			ObjectList = objectList;
			WorldObject = worldObject;
		}

		public ObjectBrushObjectList ObjectList { get; }
		public GameObject WorldObject { get; }
	}

	public ObjectListWithWorldObject[] FindAllObjectListObjects(Vector3 castPoint, float castRadius)
	{
		List<ObjectListWithWorldObject> foundObjects = new List<ObjectListWithWorldObject>();

		Collider[] foundColliders = Physics.OverlapSphere(castPoint, castRadius);

		foreach (Collider collider in foundColliders)
		{
			for (int i = 0; i < allObjectLists.Length; i++)
			{
				if (allObjectLists[i].CheckIfObjectIsInGroup(collider))
				{
					GameObject foundPrefabAsset = PrefabUtility.GetOutermostPrefabInstanceRoot(collider);
					foundObjects.Add(new ObjectListWithWorldObject(allObjectLists[i], foundPrefabAsset));
				}
			}
		}

		return foundObjects.ToArray();
	}

	public ObjectListWithWorldObject[] FindAllObjectListObjects(Vector3 castPoint, float castRadius, float castHeight)
	{
		List<ObjectListWithWorldObject> foundObjects = new List<ObjectListWithWorldObject>();

		Collider[] foundColliders = Physics.OverlapCapsule(castPoint + (Vector3.down * (castHeight / 2)), castPoint + (Vector3.up * (castHeight / 2)), castRadius);

		foreach (Collider collider in foundColliders)
		{
			for (int i = 0; i < allObjectLists.Length; i++)
			{
				if (allObjectLists[i].CheckIfObjectIsInGroup(collider))
				{
					GameObject foundPrefabAsset = PrefabUtility.GetOutermostPrefabInstanceRoot(collider);
					foundObjects.Add(new ObjectListWithWorldObject(allObjectLists[i], foundPrefabAsset));
				}
			}
		}

		return foundObjects.ToArray();
	}

	public ObjectListWithWorldObject[] FindSpecificObjectListObjects(Vector3 castPoint, float castRadius, ObjectBrushObjectList[] specificObjects)
	{
		List<ObjectListWithWorldObject> foundObjects = new List<ObjectListWithWorldObject>();

		Collider[] foundColliders = Physics.OverlapSphere(castPoint, castRadius);

		foreach (Collider collider in foundColliders)
		{
			for (int i = 0; i < specificObjects.Length; i++)
			{
				if (specificObjects[i].CheckIfObjectIsInGroup(collider))
				{
					GameObject foundPrefabAsset = PrefabUtility.GetOutermostPrefabInstanceRoot(collider);
					foundObjects.Add(new ObjectListWithWorldObject(specificObjects[i], foundPrefabAsset));
				}
			}
		}

		return foundObjects.ToArray();
	}

	public ObjectListWithWorldObject[] FindSpecificObjectListObjects(Vector3 castPoint, float castRadius, float castHeight, ObjectBrushObjectList[] specificObjects)
	{
		List<ObjectListWithWorldObject> foundObjects = new List<ObjectListWithWorldObject>();

		Collider[] foundColliders = Physics.OverlapCapsule(castPoint + (Vector3.down * (castHeight / 2)), castPoint + (Vector3.up * (castHeight / 2)), castRadius);

		foreach (Collider collider in foundColliders)
		{
			for (int i = 0; i < specificObjects.Length; i++)
			{
				if (specificObjects[i].CheckIfObjectIsInGroup(collider))
				{
					GameObject foundPrefabAsset = PrefabUtility.GetOutermostPrefabInstanceRoot(collider);
					foundObjects.Add(new ObjectListWithWorldObject(specificObjects[i], foundPrefabAsset));
				}
			}
		}

		return foundObjects.ToArray();
	}
}