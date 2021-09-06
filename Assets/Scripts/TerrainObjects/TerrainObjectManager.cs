using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace TerrainObjects
{
    public class TerrainObjectManager : MonoBehaviour
    {
        [SerializeField] private Grid.Grid m_grid;
    
        [Header("Terrain Object Materials")]
        public Material m_aliveMaterial;
        public Material m_onFireMaterial;
        public Material m_burntMaterial;

        [SerializeField] private GameObject m_treePrefab;
        [SerializeField] private Terrain m_terrain;
        [SerializeField] private Transform m_poolParent;
    
        private Queue<GameObject> m_terrainObjectPool;
        private Dictionary<Vector2Int, TerrainObject> m_terrainObjects;
        private List<Vector2Int> m_aliveTerrainObjects;
    
        private float m_terrainLengthX;
        private float m_terrainLengthZ;
        private float m_offsetX;
        private float m_offsetZ;

        private float m_density;
        private int m_spread;
    
        public int GridCellCount => m_grid.CellCount;
        public int TerrainObjectCount => m_terrainObjects.Count;
        public int AliveObjectCount => m_aliveTerrainObjects.Count;

        public int TerrainObjectSpread
        {
            set => m_spread = value;
        }

        public float TerrainObjectDensity
        {
            set => m_density = value;
        }
    
        private void OnEnable()
        {
            Initialize();
            PreloadObjectPool();
        }

        public void AddAliveTerrainObject(Vector2Int terrainObjectKey)
        {
            m_aliveTerrainObjects.Add(terrainObjectKey);
        }

        public void RemoveAliveTerrainObject(Vector2Int terrainObjectKey)
        {
            m_aliveTerrainObjects.Remove(terrainObjectKey);
        }
    
        /// <summary>
        /// Checks whether requested key exists in Terrain Object dictionary.
        /// </summary>
        /// <param name="key">Key of a Terrain Object</param>
        /// <returns></returns>
        public bool IsKeyValid(Vector2Int key)
        {
            return m_terrainObjects.ContainsKey(key);
        }
    
        private void Initialize()
        {
            var terrainData = m_terrain.terrainData;
            m_terrainLengthX = terrainData.size.x;
            m_terrainLengthZ = terrainData.size.z;
        
            m_terrainObjectPool = new Queue<GameObject>();
            m_terrainObjects = new Dictionary<Vector2Int, TerrainObject>();
            m_aliveTerrainObjects = new List<Vector2Int>();
        }

        private void PreloadObjectPool()
        {
            var maxObjectCount = m_terrainLengthX * m_terrainLengthZ;
        
            for (int i = 0; i < maxObjectCount; i++)
            {
                var terrainObjectGO = Instantiate(m_treePrefab, Vector3.zero, Quaternion.identity, m_poolParent);
                var terrainObject = terrainObjectGO.GetComponent<TerrainObject>();
            
                terrainObjectGO.SetActive(false);
                terrainObject.TerrainObjectManager = this;
                m_terrainObjectPool.Enqueue(terrainObjectGO);
            }
        }
    
        public void Generate()
        {
            ClearAll();
            SpawnTerrainObjects();
        }
    
        public void UpdateTerrainObjectState(Vector2Int terrainObjectKey, TerrainObjectState terrainObjectState)
        {
            if (!m_terrainObjects.ContainsKey(terrainObjectKey)) return;
        
            var terrainObject = m_terrainObjects[terrainObjectKey];
        
            m_grid.DisbandCellMesh(terrainObject.GridCellPosition);
            terrainObject.SetState(terrainObjectState);

            if (terrainObjectState == TerrainObjectState.Alive)
                m_aliveTerrainObjects.Add(terrainObjectKey);
        }
    
        public void ClearAll()
        {
            if (m_terrainObjects.Count == 0) return;

            for (int i = 0; i < m_grid.CellCount; i++)
            {
                var gridCell = m_grid[i];
                gridCell.StopAllCoroutines();
                gridCell.AssignedTerrainObjects.Clear();
            
                var mesh = gridCell.gameObject.GetComponent<MeshFilter>().mesh;
                var material = gridCell.gameObject.GetComponent<MeshRenderer>().materials[0];
                Destroy(mesh);
                Destroy(material);
            }

            foreach (var terrainObject in m_terrainObjects)
            {
                terrainObject.Value.gameObject.SetActive(false);
                terrainObject.Value.SetState(TerrainObjectState.Alive);

                m_terrainObjectPool.Enqueue(terrainObject.Value.gameObject);
            }
        
            m_aliveTerrainObjects.Clear();
            m_terrainObjects.Clear();
        
            GameSettings.TotalTerrainObjects = 0;
            GameSettings.AliveTerrainObjects = 0;
            GameSettings.OnFireTerrainObjects = 0;
            GameSettings.BurntTerrainObjects = 0;
        }

        /// <summary>
        /// Spawns Terrain Objects with Perlin Noise
        /// </summary>
        private void SpawnTerrainObjects()
        {
            RandomizePerlinNoiseOffset();
        
            for (int z = 0; z < m_terrainLengthZ; z++)
            {
                for (int x = 0; x < m_terrainLengthX; x++)
                {
                    if (!(PerlinNoiseCalc(x, z) < m_density)) continue;
                
                    SpawnTerrainObject(x, z);
                }
            }
        
            m_grid.CombineAllMeshes();
        
            GameSettings.BurntTerrainObjects = 0;
            GameSettings.OnFireTerrainObjects = 0;
        }

        public void SpawnTerrainObject(int spawnPosX, int spawnPosZ)
        {
            Vector2Int v2Int = new Vector2Int(spawnPosX, spawnPosZ);
            if (m_terrainObjects.Keys.Contains(v2Int)) return;
        
            GameObject terrainObjectGO = null;
            var ySpawnPoint = m_terrain.SampleHeight(new Vector3(spawnPosX, 0, spawnPosZ));
            var spawnPos = new Vector3(spawnPosX, ySpawnPoint + 0.5f, spawnPosZ);
            var posInGrid = new Vector3Int(spawnPosX / GameSettings.GRID_SIZE, 0, spawnPosZ / GameSettings.GRID_SIZE);

            if (m_terrainObjectPool.Count > 0)
            {
                terrainObjectGO = m_terrainObjectPool.Dequeue();
                terrainObjectGO.SetActive(true);
            }
            else
            {
                terrainObjectGO = Instantiate(m_treePrefab, m_poolParent);
                terrainObjectGO.transform.rotation = Quaternion.identity;
            }

            GameSettings.TotalTerrainObjects++;
            GameSettings.AliveTerrainObjects++;
                
            ResetSpawnedObject(terrainObjectGO, posInGrid, spawnPos);
        }

        private void ResetSpawnedObject(GameObject terrainObjectGO, Vector3Int gridPosition, Vector3 spawnPos)
        {
            var terrainObject = terrainObjectGO.GetComponent<TerrainObject>();
            terrainObject.GridCellPosition = gridPosition;
            terrainObject.ParentCell = m_grid[gridPosition];
            terrainObject.transform.position = spawnPos;
        
            m_grid[gridPosition].AssignedTerrainObjects.Add(terrainObjectGO);
            m_terrainObjects.Add(new Vector2Int((int)spawnPos.x, (int)spawnPos.z), terrainObject);
            m_aliveTerrainObjects.Add(new Vector2Int((int)spawnPos.x, (int)spawnPos.z));
        }

        public void RemoveTerrainObject(Vector2Int hitPoint)
        {
            if (!m_terrainObjects.ContainsKey(hitPoint)) return;
        
            var terrainObjectObject = m_terrainObjects[hitPoint];
            var terrainObjectPosInGrid = terrainObjectObject.GridCellPosition;
        
            var gridCell = m_grid[terrainObjectPosInGrid];
            gridCell.AssignedTerrainObjects.Remove(terrainObjectObject.gameObject);
        
            var terrainObjectGO = terrainObjectObject.gameObject;
            terrainObjectGO.SetActive(false);
        
            m_grid.DisbandCellMesh(terrainObjectPosInGrid);
            m_terrainObjectPool.Enqueue(terrainObjectGO);

            switch (terrainObjectObject.TerrainObjectState)
            {
                case TerrainObjectState.Alive:
                    m_aliveTerrainObjects.Remove(hitPoint);
                    GameSettings.AliveTerrainObjects--;
                    break;
                case TerrainObjectState.Burnt:
                    GameSettings.BurntTerrainObjects--;
                    break;
                case TerrainObjectState.OnFire:
                    GameSettings.OnFireTerrainObjects--;
                    break;
            }
        
            m_terrainObjects.Remove(hitPoint);
        
            GameSettings.TotalTerrainObjects--;
        }
    
        private void RandomizePerlinNoiseOffset()
        {
            m_offsetX = Random.Range(0, 1000);
            m_offsetZ = Random.Range(0, 1000);
        }
    
        private float PerlinNoiseCalc(int x, int z)
        {
            float xCoord = x / m_terrainLengthX * m_spread + m_offsetX;
            float zCoord = z / m_terrainLengthZ * m_spread + m_offsetZ;
        
            return Mathf.PerlinNoise(xCoord, zCoord);
        }
    
        public Vector2Int GetAliveTerrainObject(int index)
        {
            return m_aliveTerrainObjects[index];
        }

        public TerrainObject GetTerrainObject(Vector2Int key)
        {
            return m_terrainObjects.ContainsKey(key) ? m_terrainObjects[key] : null;
        }
    }
}
