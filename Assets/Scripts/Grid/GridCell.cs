using System.Collections.Generic;
using TerrainObjects;
using UnityEngine;
using Utilities;

namespace Grid
{
    public class GridCell : MonoBehaviour
    {
        public List<GameObject> AssignedTerrainObjects;
        public TerrainObjectManager TerrainObjectManager;
        public Vector3Int GridPosition;

        public int NewBurntTreeCount;

        private const float TIMER_MAX = 3f;
        private float m_timerCount = 0;


        private void Start()
        {
            m_timerCount = TIMER_MAX;
        }

        private void Update()
        {
            CombineBurntMeshes(); // Check every TIMER_MAX interval for new burnt objects in a grid cell and if all all burnt, combine their meshes into one to improve performance.
        }

        private void CombineBurntMeshes()
        {
            if (m_timerCount <= 0)
            {
                if (NewBurntTreeCount == AssignedTerrainObjects.Count && NewBurntTreeCount > 0)
                {
                    m_timerCount = TIMER_MAX;
                    CombineMeshes(TerrainObjectState.Burnt, TerrainObjectManager.m_burntMaterial);
                }
            }
            else m_timerCount -= Time.time;
        }

        /// <summary>
        /// Combines meshes into selected TreeState and Material.
        /// </summary>
        /// <param name="terrainObjectState">Desired TreeState</param>
        /// <param name="material">Desired Material</param>
        public void CombineMeshes(TerrainObjectState terrainObjectState, Material material)
        {
            var mesh = gameObject.GetComponent<MeshFilter>().mesh;
            Destroy(mesh);
        
            CombineInstance[] combine = new CombineInstance[AssignedTerrainObjects.Count];
            for (int i = 0; i < AssignedTerrainObjects.Count; i++)
            {
                combine[i].mesh = AssignedTerrainObjects[i].GetComponent<MeshFilter>().sharedMesh;
                combine[i].transform = AssignedTerrainObjects[i].transform.localToWorldMatrix;
                AssignedTerrainObjects[i].gameObject.SetActive(false);
            }
        
            transform.GetComponent<MeshFilter>().mesh = new Mesh();
            transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
            transform.GetComponent<MeshRenderer>().enabled = true;
            transform.GetComponent<MeshRenderer>().material = material;
            transform.gameObject.SetActive(true);

            NewBurntTreeCount = 0;
        }
    }
}
