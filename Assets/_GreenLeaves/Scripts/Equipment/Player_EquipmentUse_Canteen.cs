using UnityEngine;

/// <summary>
/// Inherits from the equipment script; this is the water canteen equipment script.<br/>
/// This is used to collect water, drink water, and utilize while the canteen is being held.
/// </summary>
public class Player_EquipmentUse_Canteen : Player_EquipmentUse
{
    [Header("Canteen Specific")]
    [Tooltip("Determines which stat the canteen will refill")]
    public ResourceContainer_Cosume.TypeOfCosumption.ConsumeType m_energyRefilType;
    //Note: May want to just substitue the normal canteen for a special canteen when using the boiling system. When that crafted canteen is fully empty, 
    //replace with a normal empty canteen to revert back to the original canteen.


    [Header("Detection Variables")]
    public LayerMask m_waterSourceMask;
    public float m_detectRadius;
    public Transform m_detectionOrigin;

    [Header("Debugging")]
    public bool m_isDebugging;
    public Color m_gizmosColor;


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            UseEquipment();
        }
    }
    public override void UseEquipment()
    {

        int requiredEnergy = 0;

        ///Player at full energy?
        if (EnergyController.Instance.IsFullMainEnergy(out requiredEnergy))
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
                    EnergyController.Instance.AddAmount(m_energyRefilType, m_durability);
                    AdjustCanteenCapacity(0);
                }
                else
                {
                    PlayAnimation("Drink water: Canteen still has some");
                    EnergyController.Instance.AddAmount(m_energyRefilType, (float)requiredEnergy);
                    AdjustCanteenCapacity(m_durability - requiredEnergy);
                }
            }
            else
            {
                ///Look for water source
                if (WaterNearby())
                {
                    PlayAnimation("Drink directly from water source");
                    EnergyController.Instance.AddAmount(m_energyRefilType, 1000);
                }
                else
                {
                    PlayAnimation("Look for water");
                }
            }
        }
    }


    /// <summary>
    /// Checks if theres water near the player
    /// </summary>
    /// <returns></returns>
    private bool WaterNearby()
    {
       return Physics.OverlapSphere(m_detectionOrigin.position, m_detectRadius, m_waterSourceMask).Length > 0;
    }

    /// <summary>
    /// Adjusts the canteen's durability, which serves as it's water capacity
    /// Also updates the inventory icon
    /// </summary>
    public void AdjustCanteenCapacity(int p_newAmount)
    {
        m_durability = p_newAmount;
        if(m_durability > m_startingDurability)
        {
            m_durability = m_startingDurability;
        }else if(m_durability < 0)
        {
            m_durability = 0;
        }
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
        Gizmos.DrawWireSphere(m_detectionOrigin.position, m_detectRadius);
    }
}
