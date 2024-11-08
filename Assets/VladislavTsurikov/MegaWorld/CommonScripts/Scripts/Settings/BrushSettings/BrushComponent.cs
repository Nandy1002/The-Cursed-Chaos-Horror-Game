using System;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.BrushSettings
{
    public enum MaskType
    {
        Custom,
        Procedural
    }

    public enum SpacingEqualsType 
    { 
        BrushSize, 
        HalfBrushSize,
        Custom,
    }


    [Serializable]
    [Name("Brush Settings")]
    public class BrushComponent : BaseComponent
    {
        [OdinSerialize]
        private float _brushSpacing = 30;

        [OdinSerialize]
        private float _brushRotation = 0.0f;

        [OdinSerialize]
        private float _brushSize = 100;

        public ProceduralMask ProceduralMask = new ProceduralMask();
        public CustomMasks CustomMasks = new CustomMasks();
        public MaskType MaskType = MaskType.Procedural;
        public SpacingEqualsType SpacingEqualsType = SpacingEqualsType.HalfBrushSize;
        public BrushJitterSettings BrushJitterSettings = new BrushJitterSettings();

        public float Spacing
        {
            set => _brushSpacing = Mathf.Max(0.01f, value);
            get => _brushSpacing;
        }

        public float BrushRotation
        {
            get => _brushRotation;
            set => _brushRotation = value;
        }

        public float BrushSize
        {
            get => _brushSize;
            set => _brushSize = Mathf.Max(0.01f, value);
        }

        public float BrushRadius => _brushSize / 2;

        public float GetCurrentSpacing()
        {
            switch (SpacingEqualsType)
            {
                case SpacingEqualsType.BrushSize:
                {
                    return BrushSize;
                }
                case SpacingEqualsType.HalfBrushSize:
                {
                    return BrushSize / 2;
                }
                default:
                {
                    return Mathf.Max(0.01f, Spacing);
                }
            }
        }

        public Texture2D GetCurrentRaw()
        {
            switch (MaskType)
            {
                case MaskType.Custom:
                {
                    Texture2D texture = CustomMasks.GetSelectedBrush();

                    return texture;
                }
                case MaskType.Procedural:
                {
                    Texture2D texture = ProceduralMask.Mask;

                    return texture;
                }
            }

            return Texture2D.whiteTexture;
        }

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

            areaVariables.Mask = GetCurrentRaw();
            areaVariables.Size = BrushSize;
            areaVariables.Rotation = BrushRotation;
            areaVariables.TerrainUnderCursor = CommonUtility.GetTerrain(hit.Point);
            areaVariables.RayHit = hit;
            areaVariables.Bounds = new Bounds();
            areaVariables.Bounds.size = new Vector3(areaVariables.Size, areaVariables.Size, areaVariables.Size);
            areaVariables.Bounds.center = hit.Point;

            return areaVariables;
        }
    }
}
