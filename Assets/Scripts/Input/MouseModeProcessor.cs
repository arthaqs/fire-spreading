using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Utilities;

namespace Input
{
    public static class MouseModeProcessor
    {
        private static readonly Dictionary<MouseModeType, MouseMode> m_mouseModes = new Dictionary<MouseModeType, MouseMode>();
        private static bool m_initialized;
        
        public static int GetMouseModeCount => m_mouseModes.Count;

        private static void Initialize()
        {
            m_mouseModes.Clear();

            var allMouseModeTypes = Assembly.GetAssembly(typeof(MouseMode)).GetTypes()
                .Where(mm => typeof(MouseMode).IsAssignableFrom(mm) && mm.IsAbstract == false);

            foreach (var mouseModeType in allMouseModeTypes)
            {
                MouseMode mouseMode = Activator.CreateInstance(mouseModeType) as MouseMode;
                m_mouseModes.Add(mouseMode.MouseModeType, mouseMode);
            }

            m_initialized = true;
        }

        public static void UseMouseMode(MouseController mouseController, MouseModeType mouseModeType, Vector2Int pos)
        {
            if (!m_initialized)
                Initialize();

            var mouseMode = m_mouseModes[mouseModeType];
            mouseMode.UseMouseMode(mouseController, pos);
        }

        public static string GetModeName(MouseModeType mouseModeType)
        {
            if (!m_initialized)
                Initialize();

            var mouseMode = m_mouseModes[mouseModeType];
            return mouseMode.GetName();
        }
    }
}