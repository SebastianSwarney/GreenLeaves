using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    public GameObject[] m_recipeObjects;
    public static RecipeBook Instance;
    private void Awake()
    {
        Instance = this;
        foreach(GameObject rec in m_recipeObjects)
        {
            rec.SetActive(false);
        }
    }

    public void UnlockRecipe(int p_recipeIndex)
    {
        if (p_recipeIndex < m_recipeObjects.Length && p_recipeIndex >= 0)
        {
            m_recipeObjects[p_recipeIndex].SetActive(true);
        }
        else
        {
            Debug.Log("No recipe book index at: " + p_recipeIndex + " exists in the list.", this);
        }
    }
}
