using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Inherits from the equipment script; this is the water canteen equipment script.<br/>
/// This is used to collect water, drink water, and utilize while the canteen is being held.
/// </summary>
public class Player_EquipmentUse_Canteen : Player_EquipmentUse
{
    public static Player_EquipmentUse_Canteen Instance;

    [Header("Canteen Specific")]
    public ResourceContainer_Equip_Canteen m_defaultCanteenData;

    [Tooltip("Determines which stat the canteen will refill")]
    public List<RefillType> m_energyRefilType;

    public int m_unitsDrankPerSecond;
    private float m_drinkTimer;
    public Sprite m_defaultCanteenSprite;

    public bool m_canteenEquipped;

    [System.Serializable]
    public struct RefillType
    {
        public ResourceContainer_Cosume.TypeOfCosumption.ConsumeType m_energyRefilType;
        public float m_waterToStatRatio;
    }
    //Note: May want to just substitue the normal canteen for a special canteen when using the boiling system. When that crafted canteen is fully empty, 
    //replace with a normal empty canteen to revert back to the original canteen.

    private bool m_hasSpecialDrink = false;

    [Header("Detection Variables")]
    public LayerMask m_waterSourceMask;
    public float m_detectRadius;
    private bool m_gettingWater;
    public float m_timeToFullCanteen;

    [Header("Prompt Text")]
    public string m_controlText;
    public string m_controlFillText;
    public Transform m_promptAnchor;
    public float m_floatingDist;

    [Header("Drink Events")]
    public GenericWorldEvent m_drinkEvent;
    public GenericWorldEvent m_emptyEvent, m_fillEvent;

    [Header("Use UI")]
    public Durability_UI m_holdButtonUI;

    [Header("Debugging")]
    public bool m_isDebugging;
    public Color m_gizmosColor;

    public void AssignSingleton()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        Player_EquipmentToolsUi.Instance.ToggleCanteenUi(true);
        Player_EquipmentToolsUi.Instance.AdjustCanteenUI((float)m_durability / (float)m_startingDurability);
    }
    private void OnDisable()
    {
        Player_EquipmentToolsUi.Instance.ToggleCanteenUi(false);
    }
    public void Update()
    {

        if (Inventory_2DMenu.Instance.m_isOpen || PlayerUIManager.Instance.m_isPaused || Building_PlayerPlacement.Instance.m_isPlacing || Daytime_WaitMenu.Instance.m_isWaiting || Interactable_Readable_Menu.Instance.m_isOpen)
        {
            m_holdButtonUI.HideUI();

            return;
        }

        if (WaterNearby() && m_durability <= 0)
        {
            if (!m_holdButtonUI.m_appearing)
            {
                m_holdButtonUI.ShowUI(false);
            }
        }else if (m_holdButtonUI.m_appearing && !WaterNearby())
        {
            m_holdButtonUI.HideUI();
        }

        if (Input.GetMouseButtonDown(0) && m_durability <= 0)
        {
            if (WaterNearby())
            {
                PlayerInputToggle.Instance.ToggleInputFromGameplay(false, false);
                m_linkedIcon.m_itemData = m_defaultCanteenData;
                m_energyRefilType = new List<RefillType>(m_defaultCanteenData.m_energyRefilType);
                m_linkedIcon.m_itemIcon.sprite = m_defaultCanteenSprite;
                m_hasSpecialDrink = false;

                m_gettingWater = true;
                m_fillEvent.Invoke();
            }
            else
            {
                m_emptyEvent.Invoke();
            }

        }
        else if (Input.GetMouseButton(0) && !m_gettingWater && m_durability > 0)
        {
            PlayerInputToggle.Instance.ToggleInputFromGameplay(false, false);
            UseEquipment();

            Player_EquipmentToolsUi.Instance.AdjustCanteenUI((float)m_durability / (float)m_startingDurability);
            UpdateIconDurability();
        }
        else if (Input.GetMouseButton(0) && m_gettingWater)
        {
            if (m_durability < m_startingDurability)
            {
                if (m_drinkTimer > 1)
                {
                    int amountToIncrease = (int)((m_startingDurability / m_timeToFullCanteen));
                    m_durability += amountToIncrease;
                    UpdateIconDurability();
                    Player_EquipmentToolsUi.Instance.AdjustCanteenUI((float)m_durability / (float)m_startingDurability);
                    m_drinkTimer = 0;
                }
                m_drinkTimer += Time.deltaTime;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_holdButtonUI.HideUI();
            PlayerInputToggle.Instance.ToggleInputFromGameplay(true, false);
            m_gettingWater = false;
        }

        m_drinkTimer += Time.deltaTime;

    }

    public override void UseEquipment()
    {
        if (m_drinkTimer < .5f) return;
        m_drinkTimer = 0;
        if (m_durability > 0)
        {
            if (m_durability - m_unitsDrankPerSecond >= 0)
            {
                foreach (RefillType reff in m_energyRefilType)
                {
                    PlayerStatsController.Instance.AddAmount(reff.m_energyRefilType, m_unitsDrankPerSecond * reff.m_waterToStatRatio);
                }
                m_drinkEvent.Invoke();
                m_durability -= m_unitsDrankPerSecond / 2;
            }
            else
            {
                m_durability -= m_unitsDrankPerSecond / 2;
                foreach (RefillType reff in m_energyRefilType)
                {
                    PlayerStatsController.Instance.AddAmount(reff.m_energyRefilType, (m_unitsDrankPerSecond - Mathf.Abs(m_durability)) * reff.m_waterToStatRatio);
                }
                m_drinkEvent.Invoke();
                m_durability = 0;
            }
        }

    }



    /*
     
    /// <summary>
    /// Old use Equipment
    /// </summary>
    public override void UseEquipment()
    {

        int requiredEnergy = (int)(PlayerStatsController.Instance.GetMaxStat(m_energyRefilType[0].m_energyRefilType) - PlayerStatsController.Instance.GetCurrentStat(m_energyRefilType[0].m_energyRefilType));
        requiredEnergy = requiredEnergy / (int)m_energyRefilType[0].m_waterToStatRatio;
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
                if (m_hasSpecialDrink) return;
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
                    foreach (RefillType reff in m_energyRefilType)
                    {
                        PlayerStatsController.Instance.AddAmount(reff.m_energyRefilType, m_durability * reff.m_waterToStatRatio);
                    }
                    AdjustCanteenCapacity(0);
                }
                else
                {
                    PlayAnimation("Drink water: Canteen still has some");
                    foreach (RefillType reff in m_energyRefilType)
                    {
                        PlayerStatsController.Instance.AddAmount(reff.m_energyRefilType, (float)requiredEnergy * reff.m_waterToStatRatio);
                    }
                    AdjustCanteenCapacity(m_durability - requiredEnergy);
                }
            }
            else
            {
                ///Look for water source
                if (WaterNearby())
                {
                    PlayAnimation("Drink directly from water source");
                    foreach (RefillType reff in m_energyRefilType)
                    {
                        PlayerStatsController.Instance.AddAmount(reff.m_energyRefilType, 1000);
                    }
                    AdjustCanteenCapacity(m_startingDurability);
                }
                else
                {
                    PlayAnimation("Look for water");
                }
            }
        }
    }*/

    public override void EquipObject(Inventory_Icon_Durability p_linkedIcon)
    {
        base.EquipObject(p_linkedIcon);
        m_canteenEquipped = true;
    }
    public override void UnEquipObject()
    {
        base.UnEquipObject();
        m_canteenEquipped = false;
    }


    public void ChangeEdibleType(List<Player_EquipmentUse_Canteen.RefillType> p_newType, bool p_specialDrink)
    {
        m_hasSpecialDrink = p_specialDrink;
        m_energyRefilType = new List<RefillType>(p_newType);
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
        else if (m_durability <= 0)
        {
            m_durability = 0;
            m_linkedIcon.m_itemData = m_defaultCanteenData;
            m_energyRefilType = new List<RefillType>(m_defaultCanteenData.m_energyRefilType);
            m_hasSpecialDrink = false;
        }

        Player_EquipmentToolsUi.Instance.AdjustCanteenUI((float)m_durability / (float)m_startingDurability);
        UpdateIconDurability();
    }

    /// <summary>
    /// Disables the breaking from no durability, as durability is used as water capacity
    /// </summary>
    public override void ObjectBroke()
    {
        m_canteenEquipped = false;
        //Debug.Log("Canteen Empty");
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
