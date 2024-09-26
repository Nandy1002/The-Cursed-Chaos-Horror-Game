#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.PreferencesSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility.Repaint;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;

namespace VladislavTsurikov.MegaWorld.AdvancedBrushTool.ScriptsEditor
{
    public static class AdvancedBrushToolVisualisation 
    {
        public static void Draw(AreaVariables areaVariables)
        {
            if(areaVariables == null || areaVariables.RayHit == null)
            {
                return;
            }

            if(AdvancedSettings.Instance.VisualisationSettings.VisualizeOverlapCheckSettings)
            { 
                if(WindowDataPackage.Instance.SelectedVariables.GetSelectedPrototypes(typeof(PrototypeGameObject)).Count != 0)
                {
                    OverlapCheckComponent.VisualizeOverlapForGameObject(areaVariables.Bounds);
                }

                if(WindowDataPackage.Instance.SelectedVariables.GetSelectedPrototypes(typeof(PrototypeLargeObject)).Count != 0)
                {
                    OverlapCheckComponent.VisualizeOverlapForInstantItem(areaVariables.Bounds);
                }
            }

            if(WindowDataPackage.Instance.SelectedVariables.HasOneSelectedGroup())
            {
                Group group = WindowDataPackage.Instance.SelectedVariables.SelectedGroup;
                
                if (group.PrototypeType == typeof(PrototypeGameObject) || group.PrototypeType == typeof(PrototypeLargeObject))
                {
                    FilterComponent filterComponent = (FilterComponent)group.GetSettings(typeof(FilterComponent));
                        
                    if(filterComponent.FilterType != FilterType.MaskFilter)
                    {
                        if(filterComponent.SimpleFilterComponent.HasOneActiveFilter())
                        {
                            VisualisationUtility.DrawSimpleFilter(group, areaVariables.RayHit.Point, areaVariables, filterComponent.SimpleFilterComponent);
                        }

                        VisualisationUtility.DrawCircleHandles(areaVariables.Size, areaVariables.RayHit);
                    }
                    else
                    {
                        VisualisationUtility.DrawMaskFilterVisualization(filterComponent.MaskFilterComponent.Stack, areaVariables);
                    }
                }
                else
                {
                    if(WindowDataPackage.Instance.SelectedVariables.HasOneSelectedPrototype())
                    {
                        VisualisationUtility.DrawMaskFilterVisualization(VisualisationUtility.GetMaskFilterFromSelectedPrototype(WindowDataPackage.Instance.BasicData), areaVariables);
                    }
                    else
                    {
                        VisualisationUtility.DrawAreaPreview(areaVariables);
                    }
                }
            }
            else
            {
                if(VisualisationUtility.IsActiveSimpleFilter(typeof(AdvancedBrushTool), WindowDataPackage.Instance.SelectedVariables))
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