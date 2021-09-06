using System.Collections;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace TerrainObjects
{
    public class Tree : TerrainObject
    {
        public override void SetState(TerrainObjectState treeState)
        {
            switch (treeState)
            {
                case TerrainObjectState.Alive:
                    SetAlive();
                    break;
                case TerrainObjectState.OnFire:
                    SetOnFire();
                    break;
                case TerrainObjectState.Burnt:
                    SetBurnt();
                    break;
                default:
                    SetAlive();
                    break;
            }
        }

        protected override void SetAlive()
        {
            StopAllCoroutines();
            
            IsBurning = false;
            TerrainObjectState = TerrainObjectState.Alive;
            
            UpdateMaterial();
            TerrainObjectManager.AddAliveTerrainObject(new Vector2Int((int)transform.position.x, (int)transform.position.z));
            
            GameSettings.AliveTerrainObjects++;
        }
        
        protected override void SetOnFire()
        {
            if (IsBurning) return;
            if (TerrainObjectState == TerrainObjectState.Burnt) return;
            
            TerrainObjectState = TerrainObjectState.OnFire;
            
            UpdateMaterial();
            TerrainObjectManager.RemoveAliveTerrainObject(new Vector2Int((int)transform.position.x, (int)transform.position.z));
            gameObject.SetActive(true);
            
            StartCoroutine(Burn());
            
            GameSettings.AliveTerrainObjects--;
            GameSettings.OnFireTerrainObjects++;
        }
    
        protected override void SetBurnt()
        {
            if (TerrainObjectState == TerrainObjectState.Burnt || TerrainObjectState == TerrainObjectState.Alive) return;
            
            TerrainObjectState = TerrainObjectState.Burnt;
            IsBurning = false;
            UpdateMaterial();
            TerrainObjectManager.RemoveAliveTerrainObject(new Vector2Int((int)transform.position.x, (int)transform.position.z));

            GameSettings.BurntTerrainObjects++;
            GameSettings.OnFireTerrainObjects--;
            
            ParentCell.NewBurntTreeCount++;
        }
        
        private IEnumerator Burn()
        {
            IsBurning = true;
    
            // Stay OnFire and spread fire among neighbour terrain objects
            var rn = Random.Range(0.0f, 0.3f);
            var waitTime = GameSettings.WindSpeed + rn;
            
            yield return new WaitForSeconds(1.3f - waitTime); // WIND SPEED

            var pos = transform.position;
            Vector2Int up = new Vector2Int((int)pos.x, (int)pos.z + 1);
            Vector2Int down = new Vector2Int((int)pos.x, (int)pos.z - 1);
            Vector2Int left = new Vector2Int((int)pos.x - 1, (int)pos.z);
            Vector2Int right = new Vector2Int((int)pos.x + 1, (int)pos.z);
    
            var windDirection = GameSettings.WindDirection;
            var allDirectionsWind = GameSettings.AllDirectionsWind;
            
            bool northWind = (windDirection <= 90 || windDirection >= 270) || allDirectionsWind;
            bool southWind = (windDirection >= 90 && windDirection <= 270) || allDirectionsWind;
            bool westWind = (windDirection >= 180 || windDirection == 0) || allDirectionsWind;
            bool eastWind = (windDirection <= 180 || windDirection == 360) || allDirectionsWind;
    
    
            var humidity = GameSettings.Humidity;
            var fireChance = 100 - humidity;
            var randomNumber = Random.Range(0, 100);
            
            if (randomNumber < fireChance)
            {
                if (northWind)
                    if (TerrainObjectManager.IsKeyValid(up))
                        TerrainObjectManager.UpdateTerrainObjectState(up, TerrainObjectState.OnFire);
    
                if (southWind)
                    if (TerrainObjectManager.IsKeyValid(down))
                        TerrainObjectManager.UpdateTerrainObjectState(down, TerrainObjectState.OnFire);
    
                if (westWind)
                    if (TerrainObjectManager.IsKeyValid(left))
                        TerrainObjectManager.UpdateTerrainObjectState(left, TerrainObjectState.OnFire);
    
                if (eastWind)
                    if (TerrainObjectManager.IsKeyValid(right))
                        TerrainObjectManager.UpdateTerrainObjectState(right, TerrainObjectState.OnFire);
            }
            
            // Die
            yield return new WaitForSeconds(GameSettings.DURATION_ON_FIRE);
            
            SetBurnt();
        }
    }
}
