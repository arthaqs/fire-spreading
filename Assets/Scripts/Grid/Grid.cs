using System.Collections.Generic;
using System.Linq;
using TerrainObjects;
using UnityEngine;
using Utilities;

namespace Grid
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] private Transform m_transformParent ;
        [SerializeField] private Material m_defaultMaterial;
        [SerializeField] private TerrainObjectManager m_terrainObjectManager;

        private Dictionary<Vector3Int, GridCell> m_gridCells;
    
        public int CellCount => m_gridCells.Count;
        public GridCell this[Vector3Int key] => m_gridCells[key];
        public GridCell this[int elementIndex] => m_gridCells.ElementAt(elementIndex).Value;
    
    
        private void Awake()
        {
            m_gridCells = new Dictionary<Vector3Int, GridCell>();
        
            CreateGrid();
        }
    
        private void CreateGrid()
        {
            Vector3Int gridPos = Vector3Int.zero;
            Transform gridCellObject = null;
            GridCell gridCell = null;
        
            float gridX = GameSettings.TerrainWidth / GameSettings.GRID_SIZE;
            float gridZ = GameSettings.TerrainHeight / GameSettings.GRID_SIZE;

            for (int i = 0; i < gridZ; i++)
            {
                for (int j = 0; j < gridX; j++)
                {
                    List<GameObject> terrainObjectGameObjects = new List<GameObject>();
                
                    // Create GameObject in hierarchy
                    gridPos = new Vector3Int(i, 0, j);
                    gridCellObject = new GameObject("GridCell " + gridPos).transform;
                    gridCellObject.gameObject.AddComponent<MeshFilter>();
                    gridCellObject.gameObject.AddComponent<MeshRenderer>();
                    gridCellObject.transform.parent = m_transformParent;
                
                    // Attach and set script to created GameObject
                    gridCell = gridCellObject.gameObject.AddComponent<GridCell>();
                    gridCell.AssignedTerrainObjects = terrainObjectGameObjects;
                    gridCell.GridPosition = gridPos;
                    gridCell.TerrainObjectManager = m_terrainObjectManager;
                
                    m_gridCells.Add(gridPos, gridCell);
                }
            }
        }
    
        public void CombineAllMeshes()
        {
            for (int i = 0; i < m_gridCells.Count; i++)
            {
                var gridCell = m_gridCells.ElementAt(i).Value;
                gridCell.CombineMeshes(TerrainObjectState.Alive, m_defaultMaterial);
            }
        }
    
        public void DisbandCellMesh(Vector3Int cellKey)
        {
            GridCell gridCell = m_gridCells[cellKey];
            Mesh meshFilter = gridCell.transform.GetComponent<MeshFilter>().sharedMesh;

            if (meshFilter == null) return; // Continue only if there is still a mesh on the cell.

            Destroy(meshFilter);
        
            int terrainObjectsInCellCount = gridCell.AssignedTerrainObjects.Count;
            for (int i = 0; i < terrainObjectsInCellCount; i++)
            {
                gridCell.AssignedTerrainObjects[i].gameObject.SetActive(true);
            }
        }
    }
}
