using UnityEngine;
using EzySlice;

[System.Serializable] public class SlicedEvent: UnityEngine.Events.UnityEvent<GameObject> { }
public class SelfSlice : MonoBehaviour
{
    public Material m_crossSectionMaterials;

    [Header("Slicing Properties")]
    public int m_hitsToSlice = 1;
    private int m_currentHits;
    public bool m_freezeBottomHalf;

    [Header("Fall Properties")]
    public bool m_addRB;
    public bool m_fallForward;
    public float m_fallInitialForce;

    public SlicedEvent m_sliced;
    public void SliceMe(Vector3 p_worldPoint, Vector3 p_upVector, Vector3 p_forwardDir)
    {
        m_currentHits++;
        if (m_currentHits < m_hitsToSlice) return;
        SlicedHull hull = gameObject.Slice(p_worldPoint, p_upVector, m_crossSectionMaterials);

        if (hull == null)
        {
            Debug.Log("Couldnt cut object: " + gameObject + " as object doesnt exist in slice region", gameObject);
            return;
        }
        GameObject upperHull = hull.CreateUpperHull(gameObject, m_crossSectionMaterials);
        GameObject lowerHull = hull.CreateLowerHull(gameObject, m_crossSectionMaterials);

        Transform[] childs = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            childs[i] = transform.GetChild(i);
        }

        foreach (Transform chi in childs)
        {
            if (Vector3.Angle(chi.transform.position - p_worldPoint, p_upVector) < 90)
            {
                chi.transform.parent = upperHull.transform;
            }
            else
            {
                chi.transform.parent = lowerHull.transform;
            }
        }

        lowerHull.AddComponent<MeshCollider>().convex = true;
        upperHull.AddComponent<MeshCollider>().convex = true;
        if (m_addRB)
        {
            if (!m_freezeBottomHalf)
            {
                if (m_fallForward)
                {
                    lowerHull.AddComponent<Rigidbody>().AddForce(p_forwardDir * m_fallInitialForce, ForceMode.Impulse);
                }
                else
                {
                    lowerHull.AddComponent<Rigidbody>().AddExplosionForce(1, upperHull.transform.position, 3, .5f, ForceMode.Impulse);
                }
            }
            if (m_fallForward)
            {
                upperHull.AddComponent<Rigidbody>().AddForce(p_forwardDir * m_fallInitialForce, ForceMode.Impulse);
            }
            else
            {
                upperHull.AddComponent<Rigidbody>().AddExplosionForce(1, upperHull.transform.position, 3, .5f, ForceMode.Impulse);
            }
        }
        m_sliced.Invoke(upperHull);
        gameObject.SetActive(false);
    }
}
