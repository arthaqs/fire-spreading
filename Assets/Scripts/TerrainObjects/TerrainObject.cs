using Grid;
using UnityEngine;
using Utilities;

namespace TerrainObjects
{
    public abstract class TerrainObject : MonoBehaviour
    {
        public TerrainObjectManager TerrainObjectManager;
        public Vector3Int GridCellPosition;
        public GridCell ParentCell;

        public TerrainObjectState TerrainObjectState { get; protected set; }
        public bool IsBurning { get; protected set; }

        public abstract void SetState(TerrainObjectState terrainObjectState);
        protected abstract void SetAlive();
        protected abstract void SetOnFire();
        protected abstract void SetBurnt();

        protected virtual void UpdateMaterial()
        {
            var treeMeshRenderer = GetComponent<MeshRenderer>();
            switch (TerrainObjectState)
            {
                case TerrainObjectState.Alive:
                    treeMeshRenderer.material = TerrainObjectManager.m_aliveMaterial;
                    break;
                case TerrainObjectState.OnFire:
                    treeMeshRenderer.material = TerrainObjectManager.m_onFireMaterial;
                    break;
                case TerrainObjectState.Burnt:
                    treeMeshRenderer.material = TerrainObjectManager.m_burntMaterial;
                    break;
                default:
                    treeMeshRenderer.material = TerrainObjectManager.m_aliveMaterial;
                    break;
            }
        }

        
    }
}