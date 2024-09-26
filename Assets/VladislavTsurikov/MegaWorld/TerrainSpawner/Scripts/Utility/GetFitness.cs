using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;

namespace VladislavTsurikov.MegaWorld.TerrainSpawner.Scripts.Utility
{
    public static class GetFitness
    {
        public static float Get(Group group, RayHit rayHit)
        {
            FilterComponent filterComponent = (FilterComponent)group.GetSettings(typeof(FilterComponent));
            
            if(filterComponent.FilterType == FilterType.MaskFilter)
            {
                return TerrainsMaskManager.GetFitness(group, filterComponent.MaskFilterComponent, rayHit.Point);
            }
            else
            {
                return CommonScripts.Scripts.Utility.GetFitness.GetFromSimpleFilter(filterComponent.SimpleFilterComponent, rayHit);
            }
        }
        
        public static float Get(Group group, MaskFilterComponent maskFilterComponent, Terrain terrain, Vector3 point)
        {
            return TerrainsMaskManager.GetFitness(group, maskFilterComponent, terrain, point);
        }
    }
}