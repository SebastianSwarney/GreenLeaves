using UnityEngine;

public class Crafting_ToolComponents : MonoBehaviour
{
    public Inventory_Icon_ToolResource m_axeResource, m_torchResource, m_canteenResource, m_bowResource, m_climbingAxeResource;

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
                break;

            case ResourceContainer_Equip.ToolType.Torch:
                m_torchResource.gameObject.SetActive(true);
                break;

            case ResourceContainer_Equip.ToolType.Canteen:
                m_canteenResource.gameObject.SetActive(true);
                break;

            case ResourceContainer_Equip.ToolType.Bow:
                m_bowResource.gameObject.SetActive(true);
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

                m_axeResource.gameObject.SetActive(true);
                break;

            case ResourceContainer_Equip.ToolType.Torch:

                m_torchResource.gameObject.SetActive(true);
                break;

            case ResourceContainer_Equip.ToolType.Canteen:

                m_canteenResource.gameObject.SetActive(true);
                break;

            case ResourceContainer_Equip.ToolType.Bow:
                m_bowResource.gameObject.SetActive(true);
                break;

            case ResourceContainer_Equip.ToolType.ClimbingAxe:
                m_climbingAxeResource.gameObject.SetActive(true);
                break;
        }
    }
}
