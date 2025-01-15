using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.Utility;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility
{
    public static class GetFitness
    {
        public static float Get(Group group, Bounds bounds, RayHit rayHit)
        {
            FilterComponent filterComponent = (FilterComponent)group.GetSettings(typeof(FilterComponent));
            
            if(filterComponent.FilterType == FilterType.MaskFilter)
            {
                return GetFromMaskFilter(bounds, filterComponent.MaskFilterComponent.Stack, filterComponent.MaskFilterComponent.FilterMaskTexture2D, rayHit.Point);
            }
            else
            {
                return GetFromSimpleFilter(filterComponent.SimpleFilterComponent, rayHit);
            }
        }

        public static float GetFromSimpleFilter(SimpleFilterComponent simpleFilterComponent, RayHit rayHit)
        {
            return simpleFilterComponent.GetFitness(rayHit.Point, rayHit.Normal);
        }

        public static float GetFromMaskFilter(Bounds bounds, MaskFilterStack stack, Texture2D filterMask, Vector3 point)
        {
            if(stack.ComponentList.Count != 0)
            {
                return GrayscaleFromTexture.GetFromWorldPosition(bounds, point, filterMask);
            }

            return 1;
        }
    }
}