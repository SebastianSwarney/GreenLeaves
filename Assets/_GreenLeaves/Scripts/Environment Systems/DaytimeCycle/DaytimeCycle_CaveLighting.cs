using System.Collections;
using UnityEngine;

public class DaytimeCycle_CaveLighting : MonoBehaviour
{
    public GameObject m_otherTrigger;
    public bool m_darken;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //StopAllCoroutines();
            //StartCoroutine(AdjustLighting());
            m_otherTrigger.gameObject.SetActive(true);
            DaytimeCycle_Update.Instance.ChangeLightingToCave(m_darken);
            transform.parent.gameObject.SetActive(false);
        }
    }




    /*private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StopAllCoroutines();
            DaytimeCycle_Update.Instance.AdjustCaveLighting(CloserToEntrence() ? 0 : 1);
        }
    }*/

   
}
