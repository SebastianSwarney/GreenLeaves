using UnityEngine;

public class Crafting_ToolComponents : MonoBehaviour
{
    public Inventory_Icon_ToolResource m_axeResource, m_torchResource, m_canteenResource, m_bootResource, m_climbingAxeResource;
    public Transform m_axePos, m_torchPos, m_canteenPos, m_bootPos, m_climbingAxePos;

    /// <summary>
    /// Inital Collection of the resource used to craft the tool.
    /// </summary>
    public void NewToolFound(ResourceContainer_Equip.ToolType p_toolType)
    {
        switch (p_toolType)
        {
            case ResourceContainer_Equip.ToolType.Axe:
                m_axeResource.gameObject.SetActive(true);
                break;

            case ResourceContainer_Equip.ToolType.Torch:
                m_torchResource.gameObject.SetActive(true);
                break;

            case ResourceContainer_Equip.ToolType.Canteen:
                m_canteenResource.gameObject.SetActive(true);
                break;

            case ResourceContainer_Equip.ToolType.Boots:
                m_bootResource.gameObject.SetActive(true);
                break;

            case ResourceContainer_Equip.ToolType.ClimbingAxe:
                m_climbingAxeResource.gameObject.SetActive(true);
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
                m_axeResource.transform.position = m_axePos.position;
                m_axeResource.gameObject.SetActive(true);
                break;

            case ResourceContainer_Equip.ToolType.Torch:
                m_torchResource.transform.position = m_torchPos.position;
                m_torchResource.gameObject.SetActive(true);
                break;

            case ResourceContainer_Equip.ToolType.Canteen:
                m_canteenResource.transform.position = m_canteenPos.position;
                m_canteenResource.gameObject.SetActive(true);
                break;

            case ResourceContainer_Equip.ToolType.Boots:
                m_bootResource.transform.position = m_bootPos.position;
                m_bootResource.gameObject.SetActive(true);
                break;

            case ResourceContainer_Equip.ToolType.ClimbingAxe:
                m_climbingAxeResource.transform.position = m_climbingAxePos.position;
                m_climbingAxeResource.gameObject.SetActive(true);
                break;
        }
    }
}
