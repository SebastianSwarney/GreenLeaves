using UnityEngine;

public class TerrainDetector
{
    /*private TerrainData terrainData;
    private int alphamapWidth;
    private int alphamapHeight;
    private float[,,] splatmapData;
    private int numTextures;*/

    public TerrainDetector()
    {

    }

    private Vector3 ConvertToSplatMapCoordinate(Vector3 worldPosition)
    {
        Vector3 splatPosition = new Vector3();
        Terrain ter = Terrain.activeTerrain;
        Vector3 terPosition = ter.transform.position;
        splatPosition.x = ((worldPosition.x - terPosition.x) / ter.terrainData.size.x) * ter.terrainData.alphamapWidth;
        splatPosition.z = ((worldPosition.z - terPosition.z) / ter.terrainData.size.z) * ter.terrainData.alphamapHeight;
        return splatPosition;
    }

   

}