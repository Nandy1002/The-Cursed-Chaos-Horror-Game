using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.MegaWorld.SprayBrushTool.ScriptsEditor
{
    [Name("Brush Settings")]
    public class BrushComponent : BaseComponent
    {
        [OdinSerialize]
        private float _brushSpacing = 30; 

        [OdinSerialize]
        private float _brushSize = 100;

        public float Spacing
        {
            set => _brushSpacing = Mathf.Max(0.01f, value);
            get => _brushSpacing;
        }

        public float BrushSize
        {
            get => _brushSize;
            set => _brushSize = Mathf.Max(0.01f, value);
        }

        public float BrushRadius => _brushSize / 2;

        public void ScrollBrushRadiusEvent()
        {
            if(Event.current.shift)
            {
                if (Event.current.type == EventType.ScrollWheel) 
                {
                    BrushSize += Event.current.delta.y;
                    Event.current.Use();
			    }
            }
        }

        public AreaVariables GetAreaVariables(RayHit hit)
        {
            AreaVariables areaVariables = new AreaVariables();

            areaVariables.Mask = Texture2D.whiteTexture;
            areaVariables.Size = BrushSize;
            areaVariables.TerrainUnderCursor = CommonUtility.GetTerrain(hit.Point);
            areaVariables.RayHit = hit;
            areaVariables.Bounds = new Bounds();
            areaVariables.Bounds.size = new Vector3(areaVariables.Size, areaVariables.Size, areaVariables.Size);
            areaVariables.Bounds.center = hit.Point;

            return areaVariables;
        }
    }
}
