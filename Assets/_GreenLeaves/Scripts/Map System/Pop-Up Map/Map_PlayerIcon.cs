using UnityEngine;
public class Map_PlayerIcon : MonoBehaviour
{
    public Color m_iconColor;
    private void Start()
    {
        Map_LevelMap.Instance.AddNewPlayerIcon(this, Color.black);

        Color newColr = new Color(Random.Range(0, 255f) / 255f, Random.Range(0, 255f) / 255f, Random.Range(0, 255f) / 255f, 1);
        AssignColor(newColr);
    }

    public void AssignColor(Color p_newColor)
    {
        //m_photonView.RPC("RPC_AssignColor", RpcTarget.AllBuffered, p_newColor.r, p_newColor.g, p_newColor.b, p_newColor.a);
    }

    /*
    [PunRPC]
    public void RPC_AssignColor(float p_colorR, float p_colorG, float p_colorB, float p_colorA)
    {
        
        m_iconColor = new Color(p_colorR, p_colorG, p_colorB, p_colorA);
        Map_LevelMap.Instance.AddNewPlayerIcon(this, m_iconColor);
    }
    */

    private void OnDestroy()
    {
        Map_LevelMap.Instance.RemoveIcon(this);
    }
}
