using UnityEngine;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using Group = VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Group;

namespace VladislavTsurikov.MegaWorld.TerrainSpawner.Scripts
{
    public class TerrainMask : MonoBehaviour
    {
        public Group Group;
        public Texture2D Mask;

        private MaskFilterComponent _maskFilterComponent;

        public MaskFilterComponent MaskFilterComponent
        {
            get
            {
                if (_maskFilterComponent == null)
                {
                    _maskFilterComponent = (MaskFilterComponent)Group.GetSettings(typeof(MaskFilterComponent));
                }
                return _maskFilterComponent;
            }
        }

        public bool IsFit()
        {
            return Group != null && Mask != null;
        }
    }
}
