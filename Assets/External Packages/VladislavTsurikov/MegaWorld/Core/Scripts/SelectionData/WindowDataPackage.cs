using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem.GlobalToolSettings;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.Scripts;
#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ResourcesController;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem.GlobalToolSettings;
#endif

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData
{
	[Location("MegaWorld/WindowDataPackage")]
    public class WindowDataPackage : SerializedScriptableObjectSingleton<WindowDataPackage>
    {
	    private static WindowDataPackage _windowDataPackage;
	    [OdinSerialize] private WindowToolStack _windowToolStack = new WindowToolStack();

	    public BasicData BasicData = new BasicData();

	    public SelectedVariables SelectedVariables => BasicData.SelectedVariables;
		
	    [OdinSerialize] public GlobalToolComponentStack globalToolComponentStack = new GlobalToolComponentStack();
		public WindowToolStack WindowToolStack => _windowToolStack;

		public LayerSettings layerSettings = new LayerSettings();
        public TransformSpace TransformSpace = TransformSpace.Global;
        public ToolWindow SelectedTool;
        
#if UNITY_EDITOR
	    private ToolStackEditor _toolComponentsEditor = null;
		
        public ResourcesControllerEditor ResourcesControllerEditor = new ResourcesControllerEditor();
        public GlobalToolComponentStackEditor GlobalToolComponentStackEditor;

		public ToolStackEditor ToolComponentsEditor
		{
			get
			{
				if(_toolComponentsEditor == null || _toolComponentsEditor.Stack == null)
				{
					_toolComponentsEditor = new ToolStackEditor(WindowToolStack);
				}

				return _toolComponentsEditor;
			}
		}

		private void OnEnable()
		{
			_windowToolStack.InternalSetup();
			globalToolComponentStack.InternalSetup();
			globalToolComponentStack.CreateGlobalToolSettings();
			
			GlobalToolComponentStackEditor = new GlobalToolComponentStackEditor(globalToolComponentStack);
			CreateMegaWorldSettings.CreateSettings();
		}
		
		private void OnDisable()
		{
			_windowToolStack.OnDisable();
			globalToolComponentStack.InternalOnDisable();  
		}

		public void Save()
		{
            EditorUtility.SetDirty(this);
            BasicData.SaveAllData();
		}
#endif
    }
}