using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeHeadCollision : MonoBehaviour
{
	public LayerMask m_treeMask;

	public bool m_isKnife;

	private BoxCollider m_collider;

	private void Start()
	{
		m_collider = GetComponent<BoxCollider>();
	}

	public bool CheckCollisionLayer(LayerMask p_layerMask, GameObject p_object)
	{
		if (p_layerMask == (p_layerMask | (1 << p_object.layer)))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (CheckCollisionLayer(m_treeMask, other.gameObject))
		{
			if (!m_isKnife)
			{
				PlayerController.Instance.HitTree(transform.position + m_collider.center);
			}
			else
			{
				PlayerController.Instance.HitBush();
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (CheckCollisionLayer(m_treeMask, other.gameObject))
		{
			if (!m_isKnife)
			{
				PlayerController.Instance.HitTree(transform.position + m_collider.center);
			}
			else
			{
				PlayerController.Instance.HitBush();
			}
		}
	}
}
