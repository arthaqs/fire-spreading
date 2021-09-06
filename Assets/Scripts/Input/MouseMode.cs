using UnityEngine;
using Utilities;

namespace Input
{
    public abstract class MouseMode
    {
        public abstract MouseModeType MouseModeType { get; }
        // public abstract void UseMouseMode(TerrainObjectManager terrainObjectManager, Vector2 hitPoint);
        public abstract void UseMouseMode(MouseController mouseController, Vector2Int hitPoint);
        public abstract string GetName();
    }

    public class AddMode : MouseMode
    {
        public override MouseModeType MouseModeType => MouseModeType.Add;
        public override void UseMouseMode(MouseController mouseController, Vector2Int hitPoint)
        {
            mouseController.Action_SpawnTree(hitPoint);
        }

        public override string GetName()
        {
            return "ADD";
        }
    }

    public class RemoveMode : MouseMode
    {
        public override MouseModeType MouseModeType => MouseModeType.Remove;
        public override void UseMouseMode(MouseController mouseController, Vector2Int hitPoint)
        {
            mouseController.Action_RemoveTree(hitPoint);
        }
        
        public override string GetName()
        {
            return "REMOVE";
        }
    }

    public class FireWaterMode : MouseMode
    {
        public override MouseModeType MouseModeType => MouseModeType.FireWater;
        public override void UseMouseMode(MouseController mouseController, Vector2Int hitPoint)
        {
            mouseController.Action_FireWater(hitPoint);
        }
        
        public override string GetName()
        {
            return "FIRE\nWATER";
        }
    }
}