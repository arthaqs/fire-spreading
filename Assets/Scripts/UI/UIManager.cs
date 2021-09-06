using Input;
using TerrainObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TerrainObjectManager m_terrainObjectManager;
        [SerializeField] private MouseController m_mouseController;
        [SerializeField] private FireController m_fireController;
        [SerializeField] private GameManager m_gameManager;

        [SerializeField] private GameObject m_windDirectionArrow;
        [SerializeField] private Sprite m_windArrowSprite;
        [SerializeField] private Sprite m_windAllDirectionsSprite;
    
        [SerializeField] private TextMeshProUGUI m_totalTreesValue;
        [SerializeField] private TextMeshProUGUI m_aliveTreesValue;
        [SerializeField] private TextMeshProUGUI m_onFireTreesValue;
        [SerializeField] private TextMeshProUGUI m_burntTreesValue;
        [SerializeField] private TextMeshProUGUI m_fpsValue;
        [SerializeField] private TextMeshProUGUI m_playPauseButtonText;
        [SerializeField] private TextMeshProUGUI m_modeText;

        [SerializeField] private Slider m_windSpeedSlider;
        [SerializeField] private Slider m_windDirectionSlider;
        [SerializeField] private Slider m_humiditySlider;
        [SerializeField] private Slider m_forestSpreadSlider;
        [SerializeField] private Slider m_forestDensitySlider;
    
        private Image m_windowDirectionArrowImgComponent;
        private float m_fps;

    
        private void Awake()
        {
            m_windowDirectionArrowImgComponent = m_windDirectionArrow.GetComponent<Image>();
        
            ResetTreeCounters();
            UpdateMouseModeText();
            OnValueChanged_UpdateForestSpread();
            OnValueChanged_UpdateForestDensity();
            OnValueChanged_UpdateWindSpeed();
        }

        private void Update()
        {
            FPSCounter();
            UpdateTreeCounters();
        }

        private void FPSCounter()
        {
            m_fps = (int) (1f / Time.unscaledDeltaTime);
            m_fpsValue.text = m_fps.ToString();
        }

        public void ResetTreeCounters()
        {
            m_aliveTreesValue.text = 0.ToString();
            m_onFireTreesValue.text = 0.ToString();
            m_burntTreesValue.text = 0.ToString();
        }
    
        private void WindToAllDirections(bool isOn)
        {
            m_windowDirectionArrowImgComponent.sprite = isOn ? m_windAllDirectionsSprite : m_windArrowSprite;
            GameSettings.AllDirectionsWind = isOn;
        }

        #region ***** Update Text Methods *****

        public void UpdateTreeCounters()
        {
            m_totalTreesValue.text = GameSettings.TotalTerrainObjects.ToString();
            m_aliveTreesValue.text = GameSettings.AliveTerrainObjects.ToString();
            m_onFireTreesValue.text = GameSettings.OnFireTerrainObjects.ToString();
            m_burntTreesValue.text = GameSettings.BurntTerrainObjects.ToString();
        }
    
        public void UpdateTotalCount(int treeCount)
        {
            m_totalTreesValue.text = treeCount.ToString();
        }

        public void UpdateAliveCount(int treeCount)
        {
            m_aliveTreesValue.text = treeCount.ToString();
        }

        public void UpdateOnFireCount(int treeCount)
        {
            m_onFireTreesValue.text = treeCount.ToString();
        }

        public void UpdateBurnt(int treeCount)
        {
            m_burntTreesValue.text = treeCount.ToString();
        }
    
        private void UpdateMouseModeText()
        {
            m_modeText.text = m_mouseController.GetMouseModeTypeName;
        }

        #endregion
    
        #region ***** ButtonClick Methods *****

        public void ButtonClick_StartFire()
        {
            m_fireController.StartRandomFire();
        }
    
        public void ButtonClick_Clear()
        {
            m_terrainObjectManager.ClearAll();
        }
    
        public void ButtonClick_PlayPauseSimulation()
        {
            if (GameSettings.IsPaused)
            {
                Time.timeScale = 1;
                GameSettings.IsPaused = false;
                m_playPauseButtonText.text = "Pause";
            }
            else
            {
                Time.timeScale = 0;
                GameSettings.IsPaused = true;
                m_playPauseButtonText.text = "Play";
            }
        }

        public void ButtonClick_Compass(int direction)
        {
            // 0 = North
            // 90 = East
            // 180 = South
            // 270 = West

            WindToAllDirections(false);
            m_windDirectionSlider.value = direction;
            m_windDirectionArrow.transform.rotation = Quaternion.Euler(0,0, -direction);
        }

        public void ButtonClick_WindToAllDirection()
        {
            WindToAllDirections(true);
        }
    
        public void ButtonClick_Quit()
        {
            m_gameManager.Quit();
        }

        public void ButtonClick_ChangeMouseMode()
        {
            m_mouseController.ChangeLMBMode();
            UpdateMouseModeText();
        }
    
        #endregion
    
        #region ***** OnValueChanged Methods *****
        public void OnValueChanged_UpdateWindSpeed()
        {
            GameSettings.WindSpeed = m_windSpeedSlider.value;
        }

        public void OnValueChanged_UpdateWindDirection()
        {
            var dirSliderValue = m_windDirectionSlider.value;
        
            GameSettings.WindDirection = (int) dirSliderValue;
            m_windDirectionArrow.transform.eulerAngles = new Vector3(0, 0, -dirSliderValue);

            if (GameSettings.AllDirectionsWind)
                WindToAllDirections(false);
        }

        public void OnValueChanged_UpdateHumidity()
        {
            GameSettings.Humidity = (int)m_humiditySlider.value;
        }
    
        public void OnValueChanged_UpdateForestSpread()
        {
            m_terrainObjectManager.TerrainObjectSpread = (int)m_forestSpreadSlider.value;
        }
    
        public void OnValueChanged_UpdateForestDensity()
        {
            m_terrainObjectManager.TerrainObjectDensity = m_forestDensitySlider.value;
        }

        #endregion
    }
}
