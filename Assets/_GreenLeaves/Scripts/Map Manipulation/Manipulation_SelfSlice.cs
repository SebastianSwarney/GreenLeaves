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

    [Header("Slicing Properties")]
    public int m_hitsToSlice = 1;
    private int m_currentHits;
    public bool m_freezeBottomHalf;

    [Header("Fall Properties")]
    public bool m_addRB;
    public bool m_fallForward;
    public float m_fallInitialForce;

    public GenericWorldEvent m_hitEvent;
    public SlicedEvent m_slicedEvent;


    /// <summary>
    /// <para>Called to slice the mesh. The parameters passed will affect the direction of the slice. </para>
    /// World point is the origin point of the slice<br/>
    /// Up Vecotor is the up vector of the slice. Determines the angle of the slice<br/>
    /// Forward Dir is used to push the mesh in a direction after it's been sliced<br/>
    /// </summary>
    public void SliceMe(Vector3 p_worldPoint, Vector3 p_upVector, Vector3 p_forwardDir)
    {

        #region Determine the hit amount
        m_currentHits++;
        if (m_currentHits < m_hitsToSlice)
        {
            m_hitEvent.Invoke();
            return;
        }
        #endregion

        #region Create the sliced hulls
        SlicedHull hull = gameObject.Slice(p_worldPoint, p_upVector, m_crossSectionMaterials);

        if (hull == null)
        {
            Debug.Log("Couldnt cut object: " + gameObject + " as object doesnt exist in slice region", gameObject);
            return;
        }
        GameObject upperHull = hull.CreateUpperHull(gameObject, m_crossSectionMaterials);
        GameObject lowerHull = hull.CreateLowerHull(gameObject, m_crossSectionMaterials);

        #endregion

        #region prepare the different slices


        ///Assigns the children to the hulls, depending on their relative position to the cut and it's angle
        #region Children assignment
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

        #endregion
        
        
        ///Disable this later if we are adding our own collider for the cut parts
        Debug.Log("Disable this later if we are adding our own collider for the cut parts");
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

        #endregion

        m_slicedEvent.Invoke(upperHull);
        gameObject.SetActive(false);
    }
}
