using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeHeadCollision : MonoBehaviour
{
	public LayerMask m_treeMask;

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
			PlayerController.Instance.HitTree(transform.position + m_collider.center);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (CheckCollisionLayer(m_treeMask, other.gameObject))
		{
			PlayerController.Instance.HitTree(transform.position + m_collider.center);
		}
	}
}
