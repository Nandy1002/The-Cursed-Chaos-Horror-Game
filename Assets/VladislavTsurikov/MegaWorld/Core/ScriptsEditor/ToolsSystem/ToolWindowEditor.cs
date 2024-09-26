#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem.GlobalToolSettings;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.SelectionWindow;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem.GlobalToolSettings;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.Window;
using Component = VladislavTsurikov.ComponentStack.Scripts.Component;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem
{
    public class ToolWindowEditor : ComponentEditor, IToolEditor
    {
        private Vector2 _windowScrollPos;

        private bool _groupSettingsFoldout = true;
        private bool _prototypeSettingsFoldout = true;
        private bool _toolSettingsFoldout = true;
        private bool _commonSettingsFoldout = true;
		
		public DrawBasicData DrawBasicData;
		public BasicData BasicData => WindowDataPackage.Instance.BasicData;
		public Type TargetType => Target.GetType();

		public override void Init(object[] args)
		{
			DrawBasicDataAttribute drawBasicDataAttribute = GetType().GetAttribute<DrawBasicDataAttribute>();

			if(drawBasicDataAttribute == null)
			{
				DrawBasicData = new DrawBasicData(typeof(GeneralSelectionGroupWindow), typeof(GeneralSelectionPrototypeWindow),
					BasicData, Target.GetType());
			}
			else
			{
				DrawBasicData = new DrawBasicData(drawBasicDataAttribute.SelectionGroupWindowType, drawBasicDataAttribute.SelectionPrototypeWindowType, 
					BasicData, Target.GetType());
			}
			
            OnEnable();
        }

        public override void OnGUI()
        {
			if(!Window.SelectionWindow.IsOpen)
			{
				BasicData.OnGUI(DrawBasicData, Target.GetType());
			}

			if(BasicData.SelectedVariables.SelectedGroupList.Count == 0)
            {
                return;
            }

            if(!ToolEditorUtility.DrawWarningAboutUnsupportedResourceType(BasicData, TargetType))
            {
                return;
            }

#if !INSTANT_RENDERER
			if(BasicData.SelectedVariables.HasOneSelectedGroup())
            {
                if(BasicData.SelectedVariables.SelectedGroup.PrototypeType == typeof(PrototypeLargeObject))
				{
					CustomEditorGUILayout.HelpBox("Instant Renderer is a free hyper optimization tool that is by far the best alternative to Unity Terrain Tree."); 

					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						CustomEditorGUILayout.DrawHelpBanner("https://docs.google.com/document/d/1jIPRTMlCR3jsuUrT9CedmDwRC8SsPAf0qc_flbhMOLM/edit#heading=h.1mzix67heftb", "Learn more about Instant Renderer");
						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();
					
					CustomEditorGUILayout.HelpBox("This is only available to users of the Discord server, join, write to the developer Vladislav Tsurikov in PM to get all the tools, also tell something about yourself, you can also ask any questions."); 
					
					CustomEditorGUILayout.HelpBox("I hope you will become part of the community and be active on Discord, suggest ideas, write feedback, sponsor development by helping me literally create revolutionary tools, there are still a lot of ideas, this is just the beginning, I am creating a large ecosystem of tools.");
					
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						CustomEditorGUILayout.DrawHelpBanner("https://discord.gg/fVAmyXs8GH", "Join Discord server (get all tools)");
						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();

					return;
				}
            }
#endif
	        if(BasicData.SelectedVariables.HasOneSelectedGroup())
			{
				if(BasicData.SelectedVariables.SelectedGroup.PrototypeList.Count == 0)
				{
					CustomEditorGUILayout.HelpBox("This group does not contain more than one prototype."); 
					ToolEditorUtility.DrawResourceController(BasicData, false);
					return;
				}
				else if(WindowDataPackage.Instance.ResourcesControllerEditor.IsSyncError(BasicData.SelectedVariables.SelectedGroup))
				{
					ToolEditorUtility.DrawResourceController(BasicData, false);
					return;
				}
			}

			_windowScrollPos = EditorGUILayout.BeginScrollView(_windowScrollPos);
			
			DrawToolButtons();
			DrawFirstSettings();
			DrawToolSettings();
			ToolEditorUtility.DrawResourceController(BasicData);

			AddGeneralGroupComponentsAttribute addGeneralGroupComponentsAttribute = Target.GetType().GetAttribute<AddGeneralGroupComponentsAttribute>();
			AddGroupComponentsAttribute addGroupComponentsAttribute = Target.GetType().GetAttribute<AddGroupComponentsAttribute>();
			
			AddGeneralPrototypeComponentsAttribute addGeneralPrototypeComponentsAttribute = Target.GetType().GetAttribute<AddGeneralPrototypeComponentsAttribute>();
			AddPrototypeComponentsAttribute addPrototypeComponentsAttribute = Target.GetType().GetAttribute<AddPrototypeComponentsAttribute>();

			if(addGeneralGroupComponentsAttribute != null || addGroupComponentsAttribute != null)
			{
				DrawGroupSettings();
			} 

			if(addGeneralPrototypeComponentsAttribute != null || addPrototypeComponentsAttribute != null)
			{
				DrawPrototypeSettings();
			} 

			DrawCommonSettings();

			EditorGUILayout.EndScrollView();
        }
        
        public virtual void ChangeGUIGroup(Group group){}
        public virtual void ChangeGUIPrototype(Prototype prototype){}
        public virtual void DrawToolButtons(){}
        public virtual void DrawFirstSettings(){}

        protected virtual void DrawCommonSettings()
		{
			if(BasicData.SelectedVariables.HasOneSelectedGroup())
			{
				if (BasicData.SelectedVariables.SelectedGroup.PrototypeType == typeof(PrototypeLargeObject) || BasicData.SelectedVariables.SelectedGroup.PrototypeType == typeof(PrototypeGameObject))
				{
					_commonSettingsFoldout = CustomEditorGUILayout.Foldout(_commonSettingsFoldout, "Common Settings");

					if(_commonSettingsFoldout)
					{
						EditorGUI.indentLevel++;

						LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;
						layerSettings.OnGUI();

						EditorGUI.indentLevel--;
					}
				}
			}
			else
			{
				_commonSettingsFoldout = CustomEditorGUILayout.Foldout(_commonSettingsFoldout, "Common Settings");

				if(_commonSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;
					layerSettings.OnGUI();

					EditorGUI.indentLevel--;
				}
			}
		}

        protected virtual void DrawGroupSettings(Group group)
        {
	        List<Type> drawTypes = ToolEditorUtility.GetGroupComponents(group, TargetType);
	        
	        EditorGUI.BeginChangeCheck();

	        group.ComponentStackEditor.OnGUI(drawTypes, Target.GetType());
			        
	        if(EditorGUI.EndChangeCheck())
	        {
		        ChangeGUIGroup(group);
	        }
        }

        protected virtual void DrawPrototypeSettings(Prototype proto)
        {
	        List<Type> drawTypes = ToolEditorUtility.GetPrototypeComponents(proto, TargetType);
	        
	        EditorGUI.BeginChangeCheck();

	        proto.ComponentStackEditor.OnGUI(drawTypes, Target.GetType());
	        
	        if(EditorGUI.EndChangeCheck())
	        {
		        ChangeGUIPrototype(proto);
	        }
        }

        private void DrawPrototypeSettings()
        {
			if(!BasicData.SelectedVariables.HasOneSelectedGroup())
			{
				return;
			}
			
			if(BasicData.SelectedVariables.HasOneSelectedPrototype())
			{
				Prototype proto = BasicData.SelectedVariables.GetOneSelectedPrototype();
				
				List<Type> drawTypes = ToolEditorUtility.GetPrototypeComponents(proto, TargetType);

				if (drawTypes.Count != 0)
				{
					_prototypeSettingsFoldout = CustomEditorGUILayout.HeaderWithMenu("Prototype Settings (" + proto.GetName() + ")", _prototypeSettingsFoldout, () => ToolEditorUtility.ResetPrototypeSettingsMenu(proto, TargetType));

					if(_prototypeSettingsFoldout)
					{
						EditorGUI.indentLevel++;

						DrawPrototypeSettings(proto);
					
						EditorGUI.indentLevel--;
					}
				}
			}
			else
			{
				CustomEditorGUILayout.HelpBox("Select one prototype to display prototype settings.");
			}
        }

		private void DrawGroupSettings()
        {
            if(BasicData.SelectedVariables.HasOneSelectedGroup())
			{
                Group group = BasicData.SelectedVariables.SelectedGroup;
                
                List<Type> drawTypes = ToolEditorUtility.GetGroupComponents(group, TargetType);

                if (drawTypes.Count != 0)
                {
	                _groupSettingsFoldout = CustomEditorGUILayout.HeaderWithMenu("Group Settings (" + group.name + ")", _groupSettingsFoldout, () => ToolEditorUtility.ResetGroupSettingsMenu(group, TargetType));
                
	                if(_groupSettingsFoldout)
	                {
		                EditorGUI.indentLevel++;
			        
		                DrawGroupSettings(group);

		                EditorGUI.indentLevel--;
	                }
                }
			}
			else 
			{
                CustomEditorGUILayout.HelpBox("Select one group to display group settings");   
			}
        }

		protected virtual void DrawToolSettings()
        {
	        AddGlobalToolSettingsAttribute addGlobalToolSettingsAttribute = Target.GetType().GetAttribute<AddGlobalToolSettingsAttribute>();

	        if (addGlobalToolSettingsAttribute == null)
	        {
		        return;
	        }

	        if (GetType().GetAttribute<DontDrawToolSettingsFoldoutAttribute>() == null)
	        {
		        _toolSettingsFoldout = CustomEditorGUILayout.HeaderWithMenu(GetNameToolSettings(Target), _toolSettingsFoldout, () => ToolEditorUtility.ResetGlobalToolSettingsStackMenu(TargetType));
                
		        if(_toolSettingsFoldout)
		        {
			        EditorGUI.indentLevel++;

			        foreach (var type in addGlobalToolSettingsAttribute.Types)
			        {
				        GlobalToolComponentStackEditor.OnGUI(Target.GetType(), type);
			        }

			        EditorGUI.indentLevel--;
		        }
	        }
	        else
	        {
		        foreach (var type in addGlobalToolSettingsAttribute.Types)
		        {
			        GlobalToolComponentStackEditor.OnGUI(Target.GetType(), type);
		        }
	        }
        }

		private static string GetNameToolSettings(Component target)
		{
			return "Tool Settings (" + target.GetName() + ")";
		}
    }
}
#endif