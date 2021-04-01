using System.Collections.Generic;
using UnityEngine;

public class TestAddItem : MonoBehaviour
{
    public List<DebugToolsComponents> m_debugComponents;
    public bool m_debugTools;
    [System.Serializable]
    public class DebugToolsComponents
    {
        public KeyCode m_unlockKey = KeyCode.L;
        public ResourceContainer_Equip.ToolType m_testToolType;
        public bool KeyPressed()
        {
            return Input.GetKeyDown(m_unlockKey);
        }
    }


#if UNITY_EDITOR
    private void Update()
    {

        if (m_debugTools)
        {
            foreach(DebugToolsComponents current in m_debugComponents)
            {
                if (current.KeyPressed())
                {
                    Crafting_Table.CraftingTable.m_toolComponents.NewToolFound(current.m_testToolType);
                }
            }
        }
    }
#endif


}
