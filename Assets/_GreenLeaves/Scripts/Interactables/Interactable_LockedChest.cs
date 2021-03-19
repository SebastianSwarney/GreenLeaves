using UnityEngine;

public class Interactable_LockedChest : Interactable
{
    public Animator m_animator;
    public GenericWorldEvent m_chestOpened;
    public override void LeftButtonPressed()
    {
        if (Inventory_2DMenu.Instance.m_containsKey)
        {
            m_animator.enabled = true;
            m_canBeInteractedWith = false;
            Interactable_Manager.Instance.HideButtonMenu(this, true);
        }


    }

    public void ChestOpened()
    {
        m_chestOpened.Invoke();
    }

}
