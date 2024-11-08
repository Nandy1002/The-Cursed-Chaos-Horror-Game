using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings;
#endif

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings
{
    [Serializable]
    public class LayerSettings
    {
        public LayerMask PaintLayers = 1;

#if UNITY_EDITOR
        public LayerSettingsEditor LayerSettingsEditor = new LayerSettingsEditor();

        public void OnGUI(bool useOnlyCustomRaycast = false)
        {
            LayerSettingsEditor.OnGUI(this, useOnlyCustomRaycast);
        }
#endif

        public LayerMask GetCurrentPaintLayers(Type prototypeType)
        {
            if (prototypeType == typeof(PrototypeTerrainDetail) || prototypeType == typeof(PrototypeTerrainTexture))
            {
                if(Terrain.activeTerrain == null)
                {
                    Debug.LogWarning("Not present in the scene with an active Unity Terrain.");
                }

                return LayerMask.GetMask(LayerMask.LayerToName(Terrain.activeTerrain.gameObject.layer));
            }
            else
            {
                return PaintLayers;
            }
        }
    }
}