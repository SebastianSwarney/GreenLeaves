using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    public Recipes[] m_recipes;
    public static RecipeBook Instance;

    public UnityEngine.UI.Image[] m_recipePlaces;
    public int m_currentStartingIndex = 0;

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
        }
        else
        {
            Debug.Log("No recipe book index at: " + p_recipeIndex + " exists in the list.", this);
        }
    }

    public void FlipPage(int p_dir)
    {
        m_currentStartingIndex += (m_recipePlaces.Length) * p_dir;
        if (m_currentStartingIndex > m_recipes.Length)
        {
            m_currentStartingIndex = 0;
        }else if (m_currentStartingIndex < 0)
        {
            m_currentStartingIndex = m_recipes.Length-1 - (m_recipes.Length % m_recipePlaces.Length);
        }
        UpdateRecipeImages();
    }
    public void OpenRecipeBook()
    {
        m_currentStartingIndex = 0;
        UpdateRecipeImages();
    }
    public void UpdateRecipeImages()
    {
        for (int x = 0; x < m_recipePlaces.Length; x++)
        {
            if (x + m_currentStartingIndex < m_recipes.Length)
            {
                if (m_recipes[x + m_currentStartingIndex].m_unlocked)
                {
                    m_recipePlaces[x].sprite = m_recipes[x + m_currentStartingIndex].m_recipeSprite;
                    m_recipePlaces[x].color = Color.white;
                    continue;
                }
            }
            m_recipePlaces[x].sprite = null;
            m_recipePlaces[x].color = Color.clear;
        }
    }

}
