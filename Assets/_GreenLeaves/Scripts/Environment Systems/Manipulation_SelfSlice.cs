using UnityEngine;
using EzySlice;


/// <summary>
/// This is the script that is on the objects that can be sliced.
/// The SliceMe function must be called in order to perform the slice.
/// Will slice the mesh after enough hits have been applied.
/// </summary>
public class Manipulation_SelfSlice : MonoBehaviour
{
    public Material m_crossSectionMaterials;
    public PhysicMaterial m_choppedTreePhysicsMaterial;

    [Header("Slicing Properties")]
    public int m_hitsToSlice = 1;
    private int m_currentHits;
    public VFX_SpawnParticle m_choppingParticle;

    [Header("Fall Properties")]
    public bool m_addRB;
    public bool m_fallForward;
    public float m_fallInitialForce;

    public GenericWorldEvent m_hitEvent, m_oneMoreHitEvent;

    public SlicedEvent m_slicedEvent;

    public GameObject m_mesh;
    public float m_applyForcePosition;
    public Transform m_durabilityObject;

    public bool m_meshCollider = true;
    public float m_startingTreeMass = 100;

    public bool m_cutAtSetPosition;
    public Vector3 m_cutPosition;

    /// <summary>
    /// <para>Called to slice the mesh. The parameters passed will affect the direction of the slice. </para>
    /// World point is the origin point of the slice<br/>
    /// Up Vecotor is the up vector of the slice. Determines the angle of the slice<br/>
    /// Forward Dir is used to push the mesh in a direction after it's been sliced<br/>
    /// </summary>
    public virtual void SliceMe(Vector3 p_worldPos, Vector3 p_forwardDir)
    {
        Vector3 hitPos = p_worldPos;
        if (m_cutAtSetPosition)
        {
            hitPos = transform.position + (transform.rotation * m_cutPosition);
        }
        #region Determine the hit amount
        m_choppingParticle.SpawnParticlePrefab(new Vector3(transform.position.x, hitPos.y, transform.position.z));
        m_currentHits++;
        if (m_currentHits < m_hitsToSlice)
        {
            Durability_UI.Instance.UpdateText(m_hitsToSlice - m_currentHits);
            if (m_hitsToSlice - m_currentHits == 1)
            {
                m_oneMoreHitEvent.Invoke();
            }
            else
            {
                m_hitEvent.Invoke();
            }
            return;
        }
        #endregion

        #region Create the sliced hulls
        Vector3 hitPoint = new Vector3(hitPos.x * transform.localScale.x, hitPos.y * transform.localScale.y, hitPos.z * transform.localScale.z);
        SlicedHull hull = null;
        if (m_cutAtSetPosition)
        {
            hull = m_mesh.Slice(hitPos, transform.rotation * Vector3.up, m_crossSectionMaterials);
        }
        else
        {

            hull = m_mesh.Slice(hitPos, Vector3.up, m_crossSectionMaterials);
        }

        Debug.DrawLine(transform.position, hitPos, Color.magenta, 5);
        if (hull == null)
        {
            Debug.Log("Couldnt cut object: " + m_mesh + " as object doesnt exist in slice region", gameObject);
            return;
        }
        GameObject upperHull = hull.CreateUpperHull(m_mesh, m_crossSectionMaterials);
        GameObject lowerHull = hull.CreateLowerHull(m_mesh, m_crossSectionMaterials);

        SetUpHulls(upperHull, lowerHull, p_forwardDir, p_worldPos);

    }

    public virtual void SetUpHulls(GameObject p_upperHull, GameObject p_lowerHull, Vector3 p_fallDir, Vector3 p_worldPos)
    {

        p_upperHull.transform.localScale = transform.localScale;
        p_lowerHull.transform.localScale = transform.localScale;
        p_upperHull.transform.parent = p_lowerHull.transform.parent = transform.parent;
        p_lowerHull.transform.rotation = p_upperHull.transform.rotation = transform.rotation;
        p_lowerHull.transform.position = p_upperHull.transform.position = transform.position;
        #endregion

        #region prepare the different slices


        ///Assigns the children to the hulls, depending on their relative position to the cut and it's angle
        #region Children assignment
        Transform[] childs = new Transform[m_mesh.transform.childCount + 1];
        childs[childs.Length - 1] = m_durabilityObject;
        m_durabilityObject.gameObject.SetActive(true);
        for (int i = 0; i < m_mesh.transform.childCount; i++)
        {
            childs[i] = m_mesh.transform.GetChild(i);
        }

        foreach (Transform chi in childs)
        {
            if (chi == null) continue;
            chi.transform.parent = p_upperHull.transform;
            /*if (Vector3.Angle(chi.transform.position - p_worldPos, Vector3.up) < 90)
            {
                chi.transform.parent = p_upperHull.transform;
            }
            else
            {
                chi.transform.parent = p_upperHull.transform;
            }/*/
        }

        #endregion




        p_lowerHull.AddComponent<MeshCollider>().convex = true;

        if (m_addRB)
        {
            if (m_fallForward)
            {
                Rigidbody newRb = p_upperHull.AddComponent<Rigidbody>();
                newRb.mass = m_startingTreeMass * transform.localScale.y;
                newRb.AddForceAtPosition(p_fallDir.normalized * m_fallInitialForce, m_mesh.transform.position + (Vector3.up * m_applyForcePosition * transform.localScale.y), ForceMode.Impulse);
                Debug.DrawLine(transform.position, m_mesh.transform.position + (Vector3.up * m_applyForcePosition * transform.localScale.y), Color.magenta, 2f);
                newRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
            else
            {
                Rigidbody newRb = p_upperHull.AddComponent<Rigidbody>();
                newRb.AddExplosionForce(1, p_upperHull.transform.position, 3, .5f, ForceMode.Impulse);
                newRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
        }

        #endregion


        m_slicedEvent.Invoke(p_upperHull.transform);
        /*MeshCollider col = upperHull.transform.GetChild(1).gameObject.AddComponent<MeshCollider>();
        col.sharedMesh = upperHull.GetComponent<MeshFilter>().mesh;
        col.convex = true;*/
            BoxCollider col = p_upperHull.transform.GetChild(1).gameObject.AddComponent<BoxCollider>();
            Mesh mesh = p_upperHull.GetComponent<MeshFilter>().mesh;
            col.material = m_choppedTreePhysicsMaterial;
            col.size = mesh.bounds.size;
            col.center = mesh.bounds.center;

            gameObject.SetActive(false);
            p_upperHull.transform.GetChild(1).transform.localPosition = Vector3.zero;
            p_upperHull.transform.GetChild(1).transform.localRotation = Quaternion.identity;
            p_upperHull.transform.GetComponentInChildren<Tree_VelocityCheck>().AssignTree();

        }

        public void HideUI()
        {
            Durability_UI.Instance.HideUI();
        }
        public void ShowUI()
        {
            Durability_UI.Instance.ShowUI(true);
            Durability_UI.Instance.UpdateText(m_hitsToSlice - m_currentHits);
        }

#if UNITY_EDITOR
        [Header("DEbug")]
        public bool m_debug;
    public Color m_debugColor, m_debugColor2;
    public float m_boxSize;

    public virtual void OnDrawGizmos()
    {
        if (!m_debug) return;
        Gizmos.color = m_debugColor;
        Gizmos.DrawCube(transform.position + Vector3.up * m_applyForcePosition * transform.localScale.y, Vector3.one * m_boxSize);
    }
#endif
}
