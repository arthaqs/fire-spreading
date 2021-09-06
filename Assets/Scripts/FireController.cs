using TerrainObjects;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

public class FireController : MonoBehaviour
{
    [SerializeField] private TerrainObjectManager m_terrainObjectManager;

    public void StartRandomFire()
    {
        var rn = Random.Range(1, GameSettings.MAX_RANDOM_FIRE_HITS);
        for (int i = 0; i < rn; i++)
        {
            IgniteRandomTerrainObject();
        }
    }
    
    public void IgniteRandomTerrainObject() 
    {
        var aliveObjectCount = m_terrainObjectManager.AliveObjectCount;
        
        if (aliveObjectCount == 0) return;

        var rn = Random.Range(0, aliveObjectCount);
        m_terrainObjectManager.UpdateTerrainObjectState(m_terrainObjectManager.GetAliveTerrainObject(rn), TerrainObjectState.OnFire);
    }
    
    public void IgniteOrExtinguishTerrainObject(Vector2Int hitPoint)
        {
            var terrainObject = m_terrainObjectManager.GetTerrainObject(hitPoint);

            if (!terrainObject) return;
            
            if (terrainObject.TerrainObjectState == TerrainObjectState.Alive)
            {
                m_terrainObjectManager.UpdateTerrainObjectState(hitPoint, TerrainObjectState.OnFire);
    
            }
            else if (terrainObject.TerrainObjectState == TerrainObjectState.OnFire)
            {
                m_terrainObjectManager.UpdateTerrainObjectState(hitPoint, TerrainObjectState.Alive);
            }
        } 
}
