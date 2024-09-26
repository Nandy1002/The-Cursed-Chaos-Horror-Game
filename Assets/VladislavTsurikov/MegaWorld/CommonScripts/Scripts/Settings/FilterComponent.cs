using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings
{
    public enum FilterType
    {
        SimpleFilter,
        MaskFilter
    }
    
    [Name("Filter Settings")]
    public class FilterComponent : BaseComponent
    {
        public FilterType FilterType = FilterType.MaskFilter;
        
        [OdinSerialize] public SimpleFilterComponent SimpleFilterComponent = new SimpleFilterComponent();
        [OdinSerialize] public MaskFilterComponent MaskFilterComponent = new MaskFilterComponent();
    }
}