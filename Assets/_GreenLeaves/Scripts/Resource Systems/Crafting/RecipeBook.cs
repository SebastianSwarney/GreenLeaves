using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    public Recipes[] m_recipes;
    public static RecipeBook Instance;

    public UnityEngine.UI.Image[] m_recipePlaces;
    public int m_currentStartingIndex = 0;

    public bool m_debug;

    private void OnValidate()
    {
        if (m_debug)
        {
            m_debug = false;
            for (int i = 0; i < m_recipes.Length; i++)
            {
                if (m_recipes[i].m_unlocked)
                {
                    UnlockRecipe(i);
                }
            }
        }
    }
    [System.Serializable]
    public class Recipes
    {
        public Sprite m_recipeSprite;
        public bool m_unlocked;
    }
    private void Awake()
    {
        Instance = this;
        for (int i = 0; i < m_recipes.Length; i++)
        {
            if (m_recipes[i].m_unlocked)
            {
                UnlockRecipe(i);
            }
        }
    }

    public void UnlockRecipe(int p_recipeIndex)
    {
        if (p_recipeIndex < m_recipes.Length && p_recipeIndex >= 0)
        {
            m_recipes[p_recipeIndex].m_unlocked = true;
            m_recipePlaces[p_recipeIndex].sprite = m_recipes[p_recipeIndex].m_recipeSprite;
            m_recipePlaces[p_recipeIndex].color = new Vector4(1, 1, 1, 1);
        }
        else
        {
            Debug.Log("No recipe book index at: " + p_recipeIndex + " exists in the list.", this);
        }
    }


}
