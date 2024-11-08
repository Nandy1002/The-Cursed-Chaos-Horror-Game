#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem.GlobalToolSettings;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.SettingsSystem;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem.GlobalToolSettings
{
    [SettingsEditor(typeof(GlobalToolComponent))]
    public class GlobalToolComponentEditor : ComponentEditor
    {
        private GlobalToolComponent _globalToolComponent;
        public BaseComponentStackEditor ComponentStackEditor;
        
        public override void OnEnable()
        {
            _globalToolComponent = (GlobalToolComponent)Target;
            ComponentStackEditor = new BaseComponentStackEditor(_globalToolComponent.ComponentStack);
        }
    }
}
#endif