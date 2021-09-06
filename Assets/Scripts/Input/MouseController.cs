using TerrainObjects;
using UnityEngine;
using Utilities;

namespace Input
{
    public class MouseController : MonoBehaviour
    {
        [SerializeField] private Camera m_mainCamera;
    
        [SerializeField] private TerrainObjectManager m_terrainObjectManager;
        [SerializeField] private FireController m_fireController;

        private int m_currentMouseMode;

        public string GetMouseModeTypeName => MouseModeProcessor.GetModeName((MouseModeType) m_currentMouseMode);
        public int GetMouseModeInt => m_currentMouseMode;
    
        private void Update()
        {
            CheckForInput();
        }

        private void CheckForInput()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0) || 
                UnityEngine.Input.GetKey(KeyCode.Mouse0) && UnityEngine.Input.GetKey(KeyCode.LeftShift))
            {
                Ray ray = m_mainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, 10000))
                {
                    var v2Int= new Vector2Int((int)hit.point.x, (int)hit.point.z);
                
                    MouseModeProcessor.UseMouseMode(this, (MouseModeType)m_currentMouseMode, v2Int);
                }
            }
        }
    
        public void ChangeLMBMode()
        {
            m_currentMouseMode++;
            if (m_currentMouseMode == MouseModeProcessor.GetMouseModeCount)
                m_currentMouseMode = 0;
        }

        public void Action_SpawnTree(Vector2Int hitPoint)
        {
            m_terrainObjectManager.SpawnTerrainObject(hitPoint.x, hitPoint.y);
        }

        public void Action_RemoveTree(Vector2Int hitPoint)
        {
            m_terrainObjectManager.RemoveTerrainObject(hitPoint);
        }

        public void Action_FireWater(Vector2Int hitPoint)
        {
            m_fireController.IgniteOrExtinguishTerrainObject(hitPoint);
        }
    }
}
