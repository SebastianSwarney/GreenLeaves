using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_ItemUsage : MonoBehaviour
{

    public static Inventory_ItemUsage Instance;


    [Header("Equipment")]
    public ResourceContainer_Equip m_currentEquippedItem = null;
    public GameObject m_equipedItemObject = null;
    public Inventory_Icon m_currentEquippedIcon;

    private void Awake()
    {
        Instance = this;
    }
    #region Item Usage Functions


    public void EquipNewItem(Inventory_Icon p_currentIcon, ResourceContainer_Equip p_currentEquipment, GameObject p_heldObject)
    {
        if (m_currentEquippedItem != null)
        {
            UnEquipCurrent();
        }

        m_currentEquippedItem = p_currentEquipment;
        m_equipedItemObject = p_heldObject;
        m_currentEquippedIcon = p_currentIcon;

        Player_Inventory.Instance.EquipItem(p_heldObject);
        Inventory_2DMenu.Instance.ChangeSelectedButtonText("Unequip");
        //m_playerEquipment.Unequip();
        //m_playerEquipment.Equip(p_currentIcon);
    }

    public void UnEquipCurrent()
    {
        m_currentEquippedItem.ItemUnequipped();
        Player_Inventory.Instance.UnEquipItem();
        ObjectPooler.Instance.ReturnToPool(m_equipedItemObject);
        m_currentEquippedItem = null;
        m_currentEquippedIcon = null;
        m_equipedItemObject = null;
    }

    public void HeldEquipmentBroke()
    {
        Inventory_2DMenu.Instance.m_inventoryGrid.RemoveSingleIcon(m_currentEquippedIcon);
        m_currentEquippedItem.ItemUnequipped();
        Player_Inventory.Instance.UnEquipItem();
        ObjectPooler.Instance.ReturnToPool(m_equipedItemObject);
        m_currentEquippedItem = null;
        m_currentEquippedIcon = null;
        m_equipedItemObject = null;
    }
    public void ConsumeItem(Inventory_Icon p_currentIcon, List<ResourceContainer_Cosume.TypeOfCosumption> p_currentStats)
    {
        foreach (ResourceContainer_Cosume.TypeOfCosumption consume in p_currentStats)
        {
            if (EnergyController.Instance != null)
            {
                EnergyController.Instance.AddAmount(consume.m_typeOfConsume, consume.m_replenishAmount, consume.m_increasePastAmount);
            }
            else
            {
                Debug.LogError("Energy Controlelr singleton is not initailized");
            }
        }
        Inventory_2DMenu.Instance.ItemUsed(p_currentIcon);
    }
    #endregion
}
