#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.PreferencesSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility.Repaint;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;

namespace VladislavTsurikov.MegaWorld.SprayBrushTool.ScriptsEditor
{
    public static class SprayBrushToolVisualisation 
    {
        public static void Draw(AreaVariables areaVariables)
        {
            if(areaVariables == null)
            {
                return;
            }

            if(areaVariables.RayHit == null)
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

                SimpleFilterComponent simpleFilterComponent = (SimpleFilterComponent)group.GetSettings(typeof(SprayBrushTool), typeof(SimpleFilterComponent));

                if(simpleFilterComponent.HasOneActiveFilter())
                {
                    VisualisationUtility.DrawSimpleFilter(group, areaVariables.RayHit.Point, areaVariables, simpleFilterComponent, true);
                }
                
                VisualisationUtility.DrawCircleHandles(areaVariables.Size, areaVariables.RayHit);
            }
            else
            {
                VisualisationUtility.DrawCircleHandles(areaVariables.Size, areaVariables.RayHit);
            }
        }
    }
}
#endif
