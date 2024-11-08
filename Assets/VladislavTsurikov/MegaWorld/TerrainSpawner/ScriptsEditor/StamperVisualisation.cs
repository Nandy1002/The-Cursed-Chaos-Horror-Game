#if UNITY_EDITOR
using System;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility.Repaint;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;

namespace VladislavTsurikov.MegaWorld.TerrainSpawner.ScriptsEditor
{
    public class StamperVisualisation 
    {
        [NonSerialized] public bool UpdateMask;
        [NonSerialized] public MaskFilterContext FilterContext;
        [NonSerialized] public MaskFilterStack PastMaskFilterStack;

        public void Draw(AreaVariables areaVariables, BasicData data, float multiplyAlpha)
        {
            if(areaVariables == null)
            {
                return;
            }

            if(areaVariables.RayHit == null)
            {
                return;
            }

            if(data.SelectedVariables.HasOneSelectedGroup())
            {
                Group group = data.SelectedVariables.SelectedGroup;

                if (group.PrototypeType == typeof(PrototypeGameObject) || group.PrototypeType == typeof(PrototypeLargeObject))
                {
                    FilterComponent filterComponent = (FilterComponent)group.GetSettings(typeof(FilterComponent));
                        
                    if(filterComponent.FilterType != FilterType.MaskFilter)
                    {
                        VisualisationUtility.DrawSimpleFilter(group, areaVariables.RayHit.Point, areaVariables, filterComponent.SimpleFilterComponent);
                    }
                    else
                    {
                        DrawVisualization(filterComponent.MaskFilterComponent.Stack, areaVariables, multiplyAlpha);
                    }
                }
                else
                {
                    if(data.SelectedVariables.HasOneSelectedPrototype())
                    {
                        DrawVisualization(VisualisationUtility.GetMaskFilterFromSelectedPrototype(data), areaVariables, multiplyAlpha);
                    }
                    else
                    {
                        VisualisationUtility.DrawAreaPreview(areaVariables);
                    }
                }
            }
        }

        public void DrawVisualization(MaskFilterStack maskFilterStack, AreaVariables areaVariables, float multiplyAlpha = 1)
        {
            if(areaVariables.TerrainUnderCursor == null)
            {
                return;
            }

            if (maskFilterStack.ComponentList.Count > 0)
            {
                UpdateMaskIfNecessary(maskFilterStack, areaVariables);

                VisualisationUtility.DrawMaskFilter(FilterContext, areaVariables, multiplyAlpha);
            }
            else
            {
                VisualisationUtility.DrawAreaPreview(areaVariables);
            }
        }

        public void UpdateMaskIfNecessary(MaskFilterStack maskFilterStack, AreaVariables areaVariables)
        {
            if(FilterContext == null)
            {
                FilterContext = new MaskFilterContext(areaVariables);
                FilterMaskOperation.UpdateFilterContext(ref FilterContext, maskFilterStack, areaVariables);

                UpdateMask = false;

                return;
            }
            
            if(PastMaskFilterStack != maskFilterStack || UpdateMask)   
            {
                FilterContext.DisposeUnmanagedMemory();
                FilterMaskOperation.UpdateFilterContext(ref FilterContext, maskFilterStack, areaVariables);
            
                PastMaskFilterStack = maskFilterStack;
            
                UpdateMask = false;
            }
        }
    }
}
#endif