using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.Utility;
using Group = VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Group;

namespace VladislavTsurikov.MegaWorld.TerrainSpawner.Scripts.Utility
{
    public class TerrainsMaskItem
    {
        private readonly Dictionary<Object, Texture2D> _terrainsMasks = new Dictionary<Object, Texture2D>();

        public readonly MaskFilterComponent MaskFilterComponent;

        public TerrainsMaskItem(MaskFilterComponent maskFilterComponent)
        {
            MaskFilterComponent = maskFilterComponent;
        }

        public float GetFitness(Group group, Terrain terrain, Vector3 point)
        {
            if (terrain == null)
                return 0;

            Bounds terrainBounds = new Bounds(terrain.terrainData.bounds.center + terrain.transform.position,
                terrain.terrainData.bounds.size);
            
            TerrainMask[] terrainMasks = terrain.GetComponents<TerrainMask>();

            float maskFitness = 1;

            foreach (var terrainMask in terrainMasks)
            {
                if (terrainMask != null && terrainMask.IsFit() && terrainMask.Group == group)
                {
                    maskFitness = GrayscaleFromTexture.GetFromWorldPosition(terrainBounds, point, terrainMask.Mask);
                    break;
                }
            }

            if (_terrainsMasks.TryGetValue(terrain, out Texture2D mask))
            {
                return CommonScripts.Scripts.Utility.GetFitness.GetFromMaskFilter(terrainBounds, MaskFilterComponent.Stack, mask,
                    point) * maskFitness;
            }
            else
            {
                RayHit terrainCenterRayHit = RaycastUtility.Raycast(
                    RayUtility.GetRayDown(terrainBounds.center + new Vector3(0, 20, 0)),
                    LayerMask.GetMask(LayerMask.LayerToName(terrain.gameObject.layer)));

                if (terrainCenterRayHit == null)
                {
                    return 0;
                }

                AreaVariables areaVariables = new AreaVariables(terrainBounds, terrainCenterRayHit);
                Texture2D texture2D = FilterMaskOperation.UpdateMaskTexture(MaskFilterComponent, areaVariables);
                _terrainsMasks.Add(terrain, texture2D);

                return CommonScripts.Scripts.Utility.GetFitness.GetFromMaskFilter(terrainBounds, MaskFilterComponent.Stack, texture2D,
                    point) * maskFitness;
            }
        }

        public float GetFitness(Group group, Vector3 point)
        {
            return GetFitness(group, CommonUtility.GetTerrain(point), point);
        }
    }
}