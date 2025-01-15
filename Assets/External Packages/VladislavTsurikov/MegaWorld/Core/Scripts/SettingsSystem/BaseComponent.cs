using System;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using Component = VladislavTsurikov.ComponentStack.Scripts.Component;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem
{
    [Serializable]
    public abstract class BaseComponent : Component
    {
        [OdinSerialize] public List<Type> UsedTools = new List<Type>();
        public bool General;

        public void AddUsedTools(Type toolType)
        {
            if (!UsedTools.Contains(toolType))
            {
                UsedTools.Add(toolType);
            }
        }
    }
}