using UnityEngine;

public class Interactable_LockedChest : Interactable
{
    public Animator m_animator;
    private bool m_isLocked = true;
    public GenericWorldEvent m_chestOpened, m_attemptOpen, m_initialUnlock;
    public override void LeftButtonPressed()
    {
        if (Inventory_2DMenu.Instance.m_containsKey)
        {
            m_initialUnlock.Invoke();
            m_animator.enabled = true;
            m_canBeInteractedWith = false;
            Interactable_Manager.Instance.HideButtonMenu(this, true);
        }
        else
        {
            m_attemptOpen.Invoke();
        }


    }

    public void ChestOpened()
    {
        m_chestOpened.Invoke();
    }

}
