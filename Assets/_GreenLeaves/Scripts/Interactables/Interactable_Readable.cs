using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Readable : Interactable
{
    public Interactable_Readable_Data m_data;
    public override void LeftButtonPressed()
    {
        Interactable_Readable_Menu.Instance.OpenReadable(m_data);
        Interactable_Manager.Instance.HideButtonMenu(this, true);
        gameObject.SetActive(false);

    }

    public override string GetInteractableName()
    {
        return m_data.m_readableTitle;
        //return m_interactableName;
    }
}
