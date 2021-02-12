using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Staggart.VegetationSpawner;

#if UNITY_EDITOR
public class TerrainCollision : MonoBehaviour
{
    public bool VisualizeCells;

    public Dictionary<Terrain, Cell[,]> terrainCells = new Dictionary<Terrain, Cell[,]>();

    private void OnDrawGizmos()
    {
        if (VisualizeCells)
        {
            if (terrainCells == null) return;

            foreach (KeyValuePair<Terrain, Cell[,]> item in terrainCells)
            {
                foreach (Cell cell in item.Value)
                {
                    if ((UnityEditor.SceneView.lastActiveSceneView.camera.transform.position - cell.bounds.center).magnitude > 150f) continue;

                    foreach (Cell subCell in cell.subCells)
                    {
                        if (subCell == null) continue;
                        Gizmos.color = new Color(1f, 0.05f, 0.05f, 1f);
                        Gizmos.DrawWireCube(new Vector3(subCell.bounds.center.x, subCell.bounds.center.y, subCell.bounds.center.z),
                            new Vector3(subCell.bounds.size.x, subCell.bounds.size.y, subCell.bounds.size.z));
                    }

                    Gizmos.color = new Color(0.66f, 0.66f, 1f, 0.25f);
                    Gizmos.DrawWireCube(
                        new Vector3(cell.bounds.center.x, cell.bounds.center.y, cell.bounds.center.z),
                        new Vector3(cell.bounds.size.x, cell.bounds.size.y, cell.bounds.size.z)
                        );
                }
            }
        }
    }
}

#endif