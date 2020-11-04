using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_SpawnParticle : MonoBehaviour
{
    public GameObject m_spawnedParticle;
    public Vector3 m_particleOffset;



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
            newParticle.transform.position += transform.rotation * m_particleOffset;
        }

    }

    /// <summary>
    /// Spawns a line renderer, and requires a starting position, and ending position.
    /// </summary>
    /// <param name="p_startingPos"></param>
    /// <param name="p_endingPos"></param>
    public void SpawnParticleLineRenderer(Vector3 p_startingPos, Vector3 p_endingPos)
    {
        LineRenderer newLine = ObjectPooler.Instance.NewObject(m_spawnedParticle, p_startingPos, Quaternion.identity).GetComponent<LineRenderer>();

        newLine.SetPosition(0, p_startingPos);
        newLine.SetPosition(1, p_endingPos);
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