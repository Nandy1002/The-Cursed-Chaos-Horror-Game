#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility.Repaint;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;

namespace VladislavTsurikov.MegaWorld.BrushEraseTool.ScriptsEditor
{
    public static class BrushEraseToolVisualisation 
    {
        public static void Draw(AreaVariables areaVariables)
        {
            if(areaVariables == null || areaVariables.RayHit == null)
            {
                return;
            }

            if(WindowDataPackage.Instance.SelectedVariables.HasOneSelectedGroup())
            {
                Group group = WindowDataPackage.Instance.SelectedVariables.SelectedGroup;
                
                if (group.PrototypeType == typeof(PrototypeGameObject) || group.PrototypeType == typeof(PrototypeLargeObject))
                {
                    FilterComponent filterComponent = (FilterComponent)group.GetSettings(typeof(BrushEraseTool), typeof(FilterComponent));
                        
                    if(filterComponent.FilterType != FilterType.MaskFilter)
                    {
                        VisualisationUtility.DrawSimpleFilter(group, areaVariables.RayHit.Point, areaVariables, filterComponent.SimpleFilterComponent);
                        VisualisationUtility.DrawCircleHandles(areaVariables.Size, areaVariables.RayHit);
                    }
                    else
                    {
                        VisualisationUtility.DrawMaskFilterVisualization(filterComponent.MaskFilterComponent.Stack, areaVariables);
                    }
                }
                else if(group.PrototypeType == typeof(PrototypeTerrainDetail))
                {
                    MaskFilterComponent maskFilterComponent = (MaskFilterComponent)group.GetSettings(typeof(BrushEraseTool), typeof(MaskFilterComponent));
                    VisualisationUtility.DrawMaskFilterVisualization(maskFilterComponent.Stack, areaVariables);
                }
            }
            else
            {
                if(VisualisationUtility.IsActiveSimpleFilter(typeof(BrushEraseTool), WindowDataPackage.Instance.SelectedVariables))
                {
                    VisualisationUtility.DrawCircleHandles(areaVariables.Size, areaVariables.RayHit);
                }
                else
                {
                    VisualisationUtility.DrawAreaPreview(areaVariables);
                }
            }
        }
    }
}
#endif