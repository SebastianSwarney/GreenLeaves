using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassDisperser : MonoBehaviour
{

    public MeshFilter filter;
    public float HeightOffset;
    public List<GrassPatch> grass = new List<GrassPatch>();

    Mesh mesh;
    public float m_grassScaleMultiplier;

    public bool m_updateGrass;

    [Header("Debugging")]
    public bool m_drawGrassDebugger;
    public bool m_drawHeightDebugger;

    public LayerMask m_hitLayer;

    public float m_grassGroundOffset;
    
    public enum GrowType { WorldUp, HitNormal}
    private void OnValidate()
    {
        if (m_updateGrass)
        {
            m_updateGrass = false;
            FillTerrain();
        }
    }
    // Use this for initialization
    void Start()
    {

        //FillTerrain();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Vector3> hitPoints, posPoints;
    public Vector3 m_origin;

    void FillTerrain()
    {
        int maxStrands = 0;
        for (int i = 0; i < grass.Count; i++)
        {
            maxStrands += grass[i].Number;
        }


        List<Vector3> positions = new List<Vector3>();
        int[] indicies = new int[maxStrands];
        List<Color> colors = new List<Color>();
        List<Vector3> normals = new List<Vector3>();

        int indices_counter = 0;
        hitPoints.Clear();
        posPoints.Clear();
        for (int i = 0; i < grass.Count; i++)
        {
            Vector3 origin = transform.position + transform.up * 10;
            m_origin = origin;
            for (int j = 0; j < grass[i].Number; j++)
            {
                float r = Random.Range(0.0f, grass[i].Radius);
                float angle = Random.Range(0.0f, 360.0f);

                Vector3 pos = origin + new Vector3(r * Mathf.Cos(angle), 0, r * Mathf.Sin(angle));
                posPoints.Add(pos);
                //Ray ray = new Ray(pos, -transform.up*300);
                RaycastHit hit;

                RaycastHit[] hits = Physics.RaycastAll(pos, -transform.up,  100, m_hitLayer);
                if (/*Physics.Raycast(ray, out hit)*/hits.Length > 0)
                {
                    hit = hits[0];
                    Debug.Log("Hit: " + hit.transform.gameObject.name, hit.transform.gameObject);
                    pos = hit.point;
                    hitPoints.Add(pos);
                    positions.Add(pos - transform.position);
                    normals.Add(hit.normal);
                    indicies[indices_counter] = indices_counter;
                    indices_counter += 1;
                }
            }
        }
        mesh = new Mesh();
        mesh.SetVertices(positions);
        mesh.SetIndices(indicies, MeshTopology.Points, 0);
        mesh.SetColors(colors);
        mesh.SetNormals(normals);
        mesh.RecalculateBounds();
        mesh.bounds = new Bounds(transform.position, mesh.bounds.size * 5);
        filter.mesh = mesh;
    }


    void OnDrawGizmos()
    {
        if (m_drawGrassDebugger)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < grass.Count; i++)
            {
                Gizmos.DrawCube(transform.position + grass[i].Origin, Vector3.one);
                Gizmos.DrawWireSphere(transform.position + grass[i].Origin, grass[i].Radius);
            }

        }
        if (m_drawHeightDebugger)
        {
            Gizmos.color = Color.blue;
            foreach (Vector3 hit in hitPoints)
            {
                Gizmos.DrawSphere(hit, .1f);
                Gizmos.DrawLine(hit, hit + transform.up * 2);
            }

            Gizmos.color = Color.red;
            foreach (Vector3 hit in posPoints)
            {

                Gizmos.DrawSphere(hit, .1f);
            }

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(m_origin, .1f);
        }
    }
}