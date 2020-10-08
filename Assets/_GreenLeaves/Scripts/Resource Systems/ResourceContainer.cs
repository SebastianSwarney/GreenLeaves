using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData_", menuName = "ScriptableObjects/ResourceData",order = 0)]
public class ResourceContainer : ScriptableObject
{
    public GameObject m_prefabObject;
    public ResourceData m_resourceData;
}
