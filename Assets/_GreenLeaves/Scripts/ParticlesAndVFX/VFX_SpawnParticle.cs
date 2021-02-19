using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_SpawnParticle : MonoBehaviour
{
    public GameObject m_spawnedParticle;

    public List<GameObject> m_spawnedParticleList;
    public Vector3 m_particleOffset;

    public Transform m_refScaleTransform;

    public bool m_rotateParticleWithY;
    

    /// <summary>
    /// Spawns an unparented particle object at the p_position
    /// </summary>
    /// <param name="p_positon"></param>
    public void SpawnParticlePrefab(Vector3 p_positon)
    {
        ObjectPooler.Instance.NewObject(m_spawnedParticle, p_positon, Quaternion.identity);
    }
    /// <summary>
    /// Spawns an unparented particle object at this objects postion.
    /// </summary>
    public void SpawnParticlePrefab()
    {

        Transform newParticle = ObjectPooler.Instance.NewObject(m_spawnedParticle, transform.position, Quaternion.identity).transform;



        if (m_particleOffset != Vector3.zero)
        {
            newParticle.transform.position += transform.rotation * m_particleOffset * transform.parent.localScale.y;
        }

        newParticle.transform.localScale = Vector3.one;

        if (m_refScaleTransform!= null)
        {
            if (m_refScaleTransform == transform)
            {

                newParticle.transform.localScale = transform.parent.localScale;
            }
            else
            {
                newParticle.transform.localScale = m_refScaleTransform.localScale;
            }
        }

        if (m_rotateParticleWithY)
        {
            
            newParticle.transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0);
        }

    }

    public void SpawnParticleWithAngle(Vector3 p_upCoord)
    {
        ObjectPooler.Instance.NewObject(m_spawnedParticle, transform.position, Quaternion.FromToRotation(Vector3.up, p_upCoord));
    }


    public void SpawnParticleFromList(int p_index)
    {
        Transform newParticle = ObjectPooler.Instance.NewObject(m_spawnedParticleList[p_index], transform.position, Quaternion.identity).transform;



        if (m_particleOffset != Vector3.zero)
        {
            newParticle.transform.position += transform.rotation * m_particleOffset * transform.parent.localScale.y;
        }

        newParticle.transform.localScale = Vector3.one;

        if (m_refScaleTransform != null)
        {
            if (m_refScaleTransform == transform)
            {

                newParticle.transform.localScale = transform.parent.localScale;
            }
            else
            {
                newParticle.transform.localScale = m_refScaleTransform.localScale;
            }
        }

        if (m_rotateParticleWithY)
        {

            newParticle.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }


#if UNITY_EDITOR
    [Header("Debugging")]
    public bool m_isDebugging;
    public Color m_gizmosColor1 = Color.yellow;
    public float m_gizRadius = .5f;
    public bool m_drawWire;
    private void OnDrawGizmos()
    {
        if (!m_isDebugging) return;
        Gizmos.color = m_gizmosColor1;
        Gizmos.matrix = transform.localToWorldMatrix;
        if (m_drawWire)
        {
            Gizmos.DrawWireSphere(m_particleOffset, m_gizRadius);
        }
        else
        {
            Gizmos.DrawSphere(m_particleOffset, m_gizRadius);
        }
    }
#endif
}