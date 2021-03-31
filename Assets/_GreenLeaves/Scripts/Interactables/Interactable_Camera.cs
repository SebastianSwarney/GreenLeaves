using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Camera : Interactable
{

    public bool m_inTimerMode;
    public float m_timer;

    public float m_respawnDistance = 100f;

    public GenericWorldEvent m_cameraPickedUp, m_pictureTaken, m_cameraTick;

    private void Update()
    {
        if(Vector3.Distance(transform.position, PlayerInputToggle.Instance.transform.position) >= m_respawnDistance)
        {
            m_inTimerMode = false;
            LeftButtonPressed();
        }
    }
    public override void LeftButtonPressed()
    {
        if (m_inTimerMode) return;
        ObjectPooler.Instance.ReturnToPool(gameObject);
        Interactable_Manager.Instance.HideButtonMenu(this, true);
        m_cameraPickedUp.Invoke();
        Inventory_2DMenu.Instance.m_toolComponents.EnableToolResource(ResourceContainer_Equip.ToolType.Camera);

    }

    public override void TopButtonPressed()
    {
        if (m_inTimerMode) return;
        TakePicture();
    }

    public override void RightButtonPressed()
    {
        if (m_inTimerMode) return;
        m_inTimerMode = true;
        StopAllCoroutines();
        StartCoroutine(TimerMode());
    }

    private IEnumerator TimerMode()
    {
        m_cameraTick.Invoke();
        float timer = 0;
        float secondTimer = 0;
        while(timer < m_timer)
        {
            yield return null;
            timer += Time.deltaTime;
            secondTimer += Time.deltaTime;
            if(secondTimer >= 1)
            {
                m_cameraTick.Invoke();
                secondTimer = 0;
            }
        }
        TakePicture();
        m_inTimerMode = false;
    }


    public void TakePicture()
    {
        m_pictureTaken.Invoke();
    }
}
