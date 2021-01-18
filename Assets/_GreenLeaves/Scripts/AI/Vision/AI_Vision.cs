using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Vision : MonoBehaviour
{
    public bool m_playerInView;
    public string m_playerLayer;
    public GenericWorldEvent m_playerSpotted;

    public LayerMask m_blockingMask;

    public float m_appearTime;
    private Transform m_playerTransform;
    private void Start()
    {
        m_playerTransform = Player_Inventory.Instance.transform;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(m_playerLayer))
        {
            StopAllCoroutines();
            StartCoroutine(CheckPlayerPos());
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer(m_playerLayer))
        {
            StopAllCoroutines();
            m_playerInView = false;
        }
    }

    private IEnumerator CheckPlayerPos()
    {
        yield return null;
        bool playerSpotted = false;
        float timer = 0;
        while (playerSpotted)
        {
            if(timer > m_appearTime)
            {
                playerSpotted = !Physics.Linecast(transform.position, m_playerTransform.position, m_blockingMask);
                timer = 0;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        m_playerInView = true;
        m_playerSpotted.Invoke();
    }
}
