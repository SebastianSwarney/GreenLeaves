using UnityEngine;

public class RecipeBook_Unlock : MonoBehaviour
{
    public int m_recipeUnlockIndex;
    
    public void UnlockSetRecipe()
    {
        RecipeBook.Instance.UnlockRecipe(m_recipeUnlockIndex);
    }

    public void UnlockPassedRecipeIndex(int p_givenIndex)
    {
        RecipeBook.Instance.UnlockRecipe(p_givenIndex);
    }
}
