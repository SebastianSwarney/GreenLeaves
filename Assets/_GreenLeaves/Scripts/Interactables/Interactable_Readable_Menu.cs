using UnityEngine;
using UnityEngine.UI;


public class Interactable_Readable_Menu : MonoBehaviour
{
    public static Interactable_Readable_Menu Instance;
    public bool m_isOpen;

    public GameObject m_readableCanvas;
    public Text m_title, m_description;
    public Image m_sprite;

    public GenericWorldEvent m_readableOpened;
    private void Awake()
    {
        Instance = this;
    }


    public void OpenReadable(Interactable_Readable_Data p_passedData)
    {
        m_readableOpened.Invoke();
        if (p_passedData.m_unlockMap)
        {
            PlayerUIManager.Instance.m_mapUnlocked = true;
        }
        m_title.text = p_passedData.m_readableTitle;
        m_description.text = p_passedData.m_readableDescription;
        m_sprite.sprite = p_passedData.m_sprite;

        m_readableCanvas.SetActive(true);

        if (p_passedData.m_isCraftingRecipe)
        {
            RecipeBook.Instance.UnlockRecipe(p_passedData.m_recipeIndex);
        }
        m_isOpen = true;
        PlayerInputToggle.Instance.ToggleInput(false);
    }

    public void CloseReadableMenu()
    {
        m_readableCanvas.SetActive(false);
        m_isOpen = false;
        PlayerInputToggle.Instance.ToggleInput(true);
        Interactable_Manager.Instance.ClearInteractable();

    }
}
