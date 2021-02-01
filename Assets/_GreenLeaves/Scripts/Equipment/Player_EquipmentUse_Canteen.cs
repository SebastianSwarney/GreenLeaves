using UnityEngine;

/// <summary>
/// Inherits from the equipment script; this is the water canteen equipment script.<br/>
/// This is used to collect water, drink water, and utilize while the canteen is being held.
/// </summary>
public class Player_EquipmentUse_Canteen : Player_EquipmentUse
{
    public static Player_EquipmentUse_Canteen Instance;
    [Header("Canteen Specific")]
    [Tooltip("Determines which stat the canteen will refill")]
    public ResourceContainer_Cosume.TypeOfCosumption.ConsumeType m_energyRefilType;
    public int m_waterToStaminaRatio;
    //Note: May want to just substitue the normal canteen for a special canteen when using the boiling system. When that crafted canteen is fully empty, 
    //replace with a normal empty canteen to revert back to the original canteen.


    [Header("Detection Variables")]
    public LayerMask m_waterSourceMask;
    public float m_detectRadius;

    [Header("Prompt Text")]
    public string m_controlText;
    public string m_controlFillText;
    public Transform m_promptAnchor;
    public Durability_UI m_durabilityUi;
    public float m_floatingDist;

    [Header("Debugging")]
    public bool m_isDebugging;
    public Color m_gizmosColor;

    public void Start()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        Player_EquipmentToolsUi.Instance.ToggleCanteenUi(true);
        Player_EquipmentToolsUi.Instance.AdjustCanteenUI((float)m_durability / (float)m_startingDurability);
        m_durabilityUi.UpdatePromptText(m_controlText);
        m_durabilityUi.UpdateText(m_durability);
        m_durabilityUi.transform.parent = m_promptAnchor;
        m_durabilityUi.transform.position = m_promptAnchor.position + Vector3.up * m_floatingDist;
    }
    private void OnDisable()
    {
        Player_EquipmentToolsUi.Instance.ToggleCanteenUi(false);
    }
    public void Update()
    {

        if (Inventory_2DMenu.Instance.m_isOpen || PlayerUIManager.Instance.m_isPaused || Building_PlayerPlacement.Instance.m_isPlacing || Daytime_WaitMenu.Instance.m_isWaiting || Interactable_Readable_Menu.Instance.m_isOpen) return;

        if (Input.GetMouseButtonDown(0))
        {
            UseEquipment();
        }

    }
    public override void UseEquipment()
    {

        int requiredEnergy = (int)(PlayerStatsController.Instance.GetMaxStat(m_energyRefilType) - PlayerStatsController.Instance.GetCurrentStat(m_energyRefilType));

        ///Player at full energy?
        if (requiredEnergy == 0)
        {
            ///Canteen has full water
            if (m_durability == m_startingDurability)
            {
                PlayAnimation("Shake Canteen cause full");
            }
            else
            {
                ///Look for water source
                if (WaterNearby())
                {
                    PlayAnimation("Fill canteen with water");
                    AdjustCanteenCapacity(m_startingDurability);

                }
                else
                {
                    PlayAnimation("Look for water");
                }
            }
        }
        else
        {
            ///Canteen has water
            if (m_durability > 0)
            {

                ///Determine how much energy to refil
                if (m_durability - requiredEnergy <= 0)
                {
                    PlayAnimation("Drink water: Canteen empty");
                    PlayerStatsController.Instance.AddAmount(m_energyRefilType, m_durability * m_waterToStaminaRatio);
                    AdjustCanteenCapacity(0);
                }
                else
                {
                    PlayAnimation("Drink water: Canteen still has some");
                    PlayerStatsController.Instance.AddAmount(m_energyRefilType, (float)requiredEnergy);
                    AdjustCanteenCapacity(m_durability - requiredEnergy);
                }
            }
            else
            {
                ///Look for water source
                if (WaterNearby())
                {
                    PlayAnimation("Drink directly from water source");
                    PlayerStatsController.Instance.AddAmount(m_energyRefilType, 1000);
                    AdjustCanteenCapacity(m_startingDurability);
                }
                else
                {
                    PlayAnimation("Look for water");
                }
            }
        }

        m_durabilityUi.UpdatePromptText(m_controlText);
        m_durabilityUi.UpdateText(m_durability);
    }

    private void OldUseEquipment()
    {
        int requiredEnergy = 0;

        ///Player at full energy?
        if (PlayerStatsController.Instance.IsFullMainEnergy(out requiredEnergy))
        {
            ///Canteen has water
            if (m_durability == m_startingDurability)
            {
                PlayAnimation("Shake Canteen cause full");
            }
            else
            {
                ///Look for water source
                if (WaterNearby())
                {
                    PlayAnimation("Fill canteen with water");
                    AdjustCanteenCapacity(m_startingDurability);

                }
                else
                {
                    PlayAnimation("Look for water");
                }
            }
        }
        else
        {
            ///Canteen has water
            if (m_durability > 0)
            {

                ///Determine how much energy to refil
                if (m_durability - requiredEnergy <= 0)
                {
                    PlayAnimation("Drink water: Canteen empty");
                    PlayerStatsController.Instance.AddAmount(m_energyRefilType, m_durability);
                    AdjustCanteenCapacity(0);
                }
                else
                {
                    PlayAnimation("Drink water: Canteen still has some");
                    PlayerStatsController.Instance.AddAmount(m_energyRefilType, (float)requiredEnergy);
                    AdjustCanteenCapacity(m_durability - requiredEnergy);
                }
            }
            else
            {
                ///Look for water source
                if (WaterNearby())
                {
                    PlayAnimation("Drink directly from water source");
                    PlayerStatsController.Instance.AddAmount(m_energyRefilType, 1000);
                }
                else
                {
                    PlayAnimation("Look for water");
                }
            }
        }
    }

    public override void UnEquipObject()
    {
        m_durabilityUi.transform.parent = transform;
        base.UnEquipObject();
    }

    /// <summary>
    /// Checks if theres water near the player
    /// </summary>
    /// <returns></returns>
    private bool WaterNearby()
    {
        return Physics.OverlapSphere(transform.position, m_detectRadius, m_waterSourceMask).Length > 0;
    }

    /// <summary>
    /// Adjusts the canteen's durability, which serves as it's water capacity
    /// Also updates the inventory icon
    /// </summary>
    public void AdjustCanteenCapacity(int p_newAmount)
    {
        m_durability = p_newAmount;
        if (m_durability > m_startingDurability)
        {
            m_durability = m_startingDurability;
        }
        else if (m_durability < 0)
        {
            m_durability = 0;
        }
        Player_EquipmentToolsUi.Instance.AdjustCanteenUI((float)m_durability / (float)m_startingDurability);
        UpdateIconDurability();
    }

    /// <summary>
    /// Disables the breaking from no durability, as durability is used as water capacity
    /// </summary>
    public override void ObjectBroke()
    {
        Debug.Log("Canteen Empty");
    }
    public override void ReEnableToolComponent()
    {
        Crafting_Table.CraftingTable.m_toolComponents.EnableToolResource(ResourceContainer_Equip.ToolType.Canteen);
    }
    private void OnDrawGizmos()
    {
        if (!m_isDebugging) return;
        Gizmos.color = m_gizmosColor;
        Gizmos.DrawWireSphere(transform.position, m_detectRadius);
    }
}
