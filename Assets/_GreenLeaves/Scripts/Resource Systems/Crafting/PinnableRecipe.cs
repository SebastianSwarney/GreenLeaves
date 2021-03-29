using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinnableRecipe : MonoBehaviour
{
    public UnityEngine.UI.Image m_recipeImage;

    public GameObject m_recipeMenu, m_openButton;
    public void AssignRecipe(UnityEngine.UI.Image p_sprite)
    {
        gameObject.SetActive(true);
        m_recipeImage.sprite = p_sprite.sprite;
        m_recipeMenu.SetActive(true);
        m_openButton.SetActive(false);
    }

    public void ToggleMenu(bool p_newState)
    {
        m_recipeMenu.SetActive(p_newState);
        m_openButton.SetActive(!p_newState);
    }
}
