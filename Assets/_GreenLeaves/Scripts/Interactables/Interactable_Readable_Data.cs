using UnityEngine;

[CreateAssetMenu(fileName = "ReadableData_", menuName = "ScriptableObjects/ReadableData", order = 0)]
public class Interactable_Readable_Data : ScriptableObject
{
    public string m_readableTitle;
    [TextAreaAttribute(15,20)]
    public string m_readableDescription;
    public Sprite m_sprite;

    [Header("Crafting Recipe Stuff")]
    public bool m_isCraftingRecipe;
    public int m_recipeIndex;

    [Header("Map Unlock")]
    public bool m_unlockMap = false;

}
