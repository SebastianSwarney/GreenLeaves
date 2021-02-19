using UnityEngine;

public class Crafting_ToolComponents : MonoBehaviour
{
    public Inventory_Icon_ToolResource m_axeResource, m_torchResource, m_canteenResource, m_bowResource, m_climbingAxeResource;

    public bool m_axeUnlocked, m_torchUnlocked, m_canteenUnlocked, m_climbingAxeUnlocked;
    private void Start()
    {
        //m_climbingAxePos = m_climbingAxeResource.transform.localPosition;
        //m_bowPos = m_bowResource.transform.localPosition;
    }
    /// <summary>
    /// Inital Collection of the resource used to craft the tool.
    /// </summary>
    public void NewToolFound(ResourceContainer_Equip.ToolType p_toolType)
    {
        switch (p_toolType)
        {
            case ResourceContainer_Equip.ToolType.Axe:
                m_axeResource.gameObject.SetActive(true);
                m_axeUnlocked = true;
                break;

            case ResourceContainer_Equip.ToolType.Torch:
                m_torchResource.gameObject.SetActive(true);
                m_torchUnlocked = true;
                break;

            case ResourceContainer_Equip.ToolType.Canteen:
                m_canteenResource.gameObject.SetActive(true);
                m_canteenUnlocked = true;
                break;

            case ResourceContainer_Equip.ToolType.Bow:
                m_bowResource.gameObject.SetActive(true);
                break;

            case ResourceContainer_Equip.ToolType.ClimbingAxe:
                m_climbingAxeResource.gameObject.SetActive(true);
                m_climbingAxeUnlocked = true;
                break;
        }
    }

    /// <summary>
    /// ReEnabling the tool icon, after it has already been collected.
    /// </summary>
    public void EnableToolResource(ResourceContainer_Equip.ToolType p_toolType)
    {
        switch (p_toolType)
        {
            case ResourceContainer_Equip.ToolType.Axe:

                m_axeResource.gameObject.SetActive(true);
                if (m_axeUnlocked)
                {
                    m_axeResource.ResetPlacement();
                }
                m_axeUnlocked = true;
                break;

            case ResourceContainer_Equip.ToolType.Torch:

                m_torchResource.gameObject.SetActive(true);
                if (m_torchUnlocked)
                {
                    m_torchResource.ResetPlacement();
                }
                m_torchUnlocked = true;
                break;

            case ResourceContainer_Equip.ToolType.Canteen:

                m_canteenResource.gameObject.SetActive(true);
                if (m_canteenUnlocked)
                {
                    m_canteenResource.ResetPlacement();
                }
                m_canteenUnlocked = true;
                break;

            case ResourceContainer_Equip.ToolType.Bow:
                m_bowResource.gameObject.SetActive(true);
                break;

            case ResourceContainer_Equip.ToolType.ClimbingAxe:
                m_climbingAxeResource.gameObject.SetActive(true);
                if (m_climbingAxeUnlocked)
                {
                    m_climbingAxeResource.ResetPlacement();
                }
                m_climbingAxeUnlocked = true;
                break;
        }
    }
}
