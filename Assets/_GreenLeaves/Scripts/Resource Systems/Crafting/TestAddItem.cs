using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAddItem : MonoBehaviour
{
    public ResourceContainer_Equip.ToolType m_testToolType;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Crafting_Table.Instance.m_toolComponents.NewToolFound(m_testToolType);
        }
    }
}
