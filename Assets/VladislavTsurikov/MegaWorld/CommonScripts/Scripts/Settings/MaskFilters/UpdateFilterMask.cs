using System.Collections.Generic;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters
{
    public static class UpdateFilterMask 
    {
        public static void UpdateFilterMaskTextureForAllTerrainDetail(List<Prototype> prototypes, AreaVariables areaVariables)
        {
            if(areaVariables.TerrainUnderCursor == null)
            {
                return;
            }
            
            foreach (Prototype proto in prototypes)
            {
                MaskFilterComponent maskFilterComponent = (MaskFilterComponent)proto.GetSettings(typeof(MaskFilterComponent));
                FilterMaskOperation.UpdateMaskTexture(maskFilterComponent, areaVariables);
            }
        }
    }
}