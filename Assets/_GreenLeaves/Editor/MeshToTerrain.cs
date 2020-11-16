using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshToTerrain : EditorWindow
{
	public bool m_deleteAfterMerge;

	[MenuItem("Tools/Mesh To Terrain", false, 2000)]
	static void OpenWindow()
	{
		EditorWindow.GetWindow<MeshToTerrain>(true);
	}

	private void OnGUI()
	{
		if (GUILayout.Button("Merge Selected Into Terrain"))
		{
			if (Selection.activeGameObject == null)
			{
				EditorUtility.DisplayDialog("No object selected", "Please select an object.", "Ok");
				return;
			}
			else
			{
				MergeObject();
			}
		}

		m_deleteAfterMerge = EditorGUILayout.Toggle("Delete selection", m_deleteAfterMerge);
	}

	private void MergeObject()
	{
		Undo.SetCurrentGroupName("Merge with Terrain");

		ShowProgressBar(1, 100, "Merging object with terrain...");

		Terrain[] foundTerrains = FindObjectsOfType<Terrain>();

		if (foundTerrains == null)
		{
			EditorUtility.DisplayDialog("No terrain in scene", "No terrain in scene.", "Ok");
			return;
		}

		foreach (Terrain existingTerrain in foundTerrains)
		{
			GameObject terrainObject = existingTerrain.gameObject;
			TerrainData terrainData = existingTerrain.terrainData;

			Undo.RegisterCompleteObjectUndo(terrainData, "Merge Object with Terrain");

			List<Collider> colliders = new List<Collider>();

			foreach (var selectedObject in Selection.gameObjects)
			{
				Terrain foundTerrain = selectedObject.GetComponent<Terrain>();

				if (selectedObject != terrainObject && foundTerrain == null)
				{
					Collider[] foundColliders = selectedObject.GetComponentsInChildren<Collider>();
					colliders.AddRange(foundColliders);
				}
			}

			Bounds terrainBounds = new Bounds(terrainObject.transform.position + (terrainData.size / 2), terrainData.size);

			Vector2 stepXZ = new Vector2(terrainData.size.x / terrainData.heightmapResolution, terrainData.size.z / terrainData.heightmapResolution);

			float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

			Ray ray = new Ray(origin: new Vector3(terrainBounds.min.x + (stepXZ[0] / 2.0f), terrainBounds.max.y + terrainBounds.size.y, terrainBounds.min.z + (stepXZ[1] / 2.0f)), direction: -Vector3.up);
			RaycastHit hit = new RaycastHit();

			Vector3 rayOrigin = ray.origin;

			for (int zCount = 0; zCount < terrainData.heightmapResolution; zCount++)
			{
				ShowProgressBar(zCount, terrainData.heightmapResolution, "Casting Rays against " + colliders.Count + " colliders...");

				for (int xCount = 0; xCount < terrainData.heightmapResolution; xCount++)
				{
					foreach (var collider in colliders)
					{
						if (collider.Raycast(ray, out hit, maxDistance: terrainBounds.size.y * 3))
						{
							float bottomOfTerrainY = terrainObject.transform.position.y;
							float height = Mathf.Abs(hit.point.y - bottomOfTerrainY) / terrainBounds.size.y;

							var oldHeight = heights[zCount, xCount];
							heights[zCount, xCount] = Mathf.Max(height, oldHeight);
						}
					}

					rayOrigin.x += stepXZ[0];
					ray.origin = rayOrigin;
				}

				rayOrigin.z += stepXZ[1];
				rayOrigin.x = terrainBounds.min.x;
				ray.origin = rayOrigin;
			}

			terrainData.SetHeights(0, 0, heights);
		}

		if (m_deleteAfterMerge)
		{
			foreach (var go in Selection.gameObjects)
			{
				Undo.DestroyObjectImmediate(go);
			}
		}
		EditorUtility.ClearProgressBar();
	}

	private void ShowProgressBar(float p_progress, float p_maxProgress, string p_message = "MeshToTerrian")
	{
		float displayProgress = p_progress / p_maxProgress;
		EditorUtility.DisplayProgressBar("Object to Terrain", p_message, displayProgress);
	}
}