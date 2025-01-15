#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.SelectionWindow;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.Window;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem
{
    public class ToolMonoBehaviourEditor : EditorBase, IToolEditor
    {
	    private Vector2 _windowScrollPos;

        protected bool _groupSettingsFoldout = true;
		protected bool _prototypeSettingsFoldout = true;
        protected bool _toolSettingsFoldout = true;
		protected bool _commonSettingsFoldout = true;
		
		public DrawBasicData DrawBasicData;
		public ToolMonoBehaviour Target => (ToolMonoBehaviour)target;
		
		public BasicData BasicData => Target.Data;
		public Type TargetType => Target.GetType();
		
		protected virtual void OnInit(){}
		public virtual void DrawFirstSettings(){}
		public virtual void DrawToolSettings(){}
		public virtual void DrawToolButtons(){}
        
		public virtual void ChangeGUIGroup(Group group){}
		public virtual void ChangeGUIPrototype(Prototype prototype){}

		public void OnEnable()
		{
			DrawBasicDataAttribute drawBasicDataAttribute = GetType().GetAttribute<DrawBasicDataAttribute>();

			if(drawBasicDataAttribute == null)
			{
				DrawBasicData = new DrawBasicData(typeof(GeneralSelectionGroupWindow), typeof(GeneralSelectionPrototypeWindow),
					Target.Data, target.GetType());
			}
			else
			{
				DrawBasicData = new DrawBasicData(drawBasicDataAttribute.SelectionGroupWindowType, drawBasicDataAttribute.SelectionPrototypeWindowType, 
					Target.Data, target.GetType());
			}
			
			OnInit();
		}

		public override void OnInspectorGUI()
		{
			Target.Data.SelectedVariables.DeleteNullValueIfNecessary(Target.Data.GroupList);
			Target.Data.SelectedVariables.SetAllSelectedParameters(Target.Data.GroupList);
			
			EditorGUI.indentLevel = 0;

			CustomEditorGUILayout.IsInspector = true;
			
			OnGUI();
			
			Target.Data.SaveAllData();
		}

        public void OnGUI()
        {
	        Target.Data.OnGUI(DrawBasicData, target.GetType());

			if(Target.Data.SelectedVariables.SelectedGroupList.Count == 0)
            {
                return;
            }

            if(!ToolEditorUtility.DrawWarningAboutUnsupportedResourceType(BasicData, TargetType))
            {
                return;
            }

#if !INSTANT_RENDERER
			if(Target.Data.SelectedVariables.HasOneSelectedGroup())
            {
                if(Target.Data.SelectedVariables.SelectedGroup.PrototypeType == typeof(PrototypeLargeObject))
				{
					CustomEditorGUILayout.HelpBox("Instant Renderer is missing in the project. Instant Renderer is only available by sponsor through Patreon."); 

					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						CustomEditorGUILayout.DrawHelpBanner("https://docs.google.com/document/d/1jIPRTMlCR3jsuUrT9CedmDwRC8SsPAf0qc_flbhMOLM/edit#heading=h.1mzix67heftb", "Learn more about Instant Renderer");
						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();
					
					return;
				}
            }
#endif

			if(Target.Data.SelectedVariables.HasOneSelectedGroup())
			{
				if(Target.Data.SelectedVariables.SelectedGroup.PrototypeList.Count == 0)
				{
					CustomEditorGUILayout.HelpBox("This group does not contain more than one prototype."); 
					ToolEditorUtility.DrawResourceController(BasicData,false);
					return;
				}
				else if(WindowDataPackage.Instance.ResourcesControllerEditor.IsSyncError(Target.Data.SelectedVariables.SelectedGroup))
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

			AddGeneralGroupComponentsAttribute addGeneralGroupComponentsAttribute = target.GetType().GetAttribute<AddGeneralGroupComponentsAttribute>();
			AddGroupComponentsAttribute addGroupComponentsAttribute = target.GetType().GetAttribute<AddGroupComponentsAttribute>();
			
			AddGeneralPrototypeComponentsAttribute addGeneralPrototypeComponentsAttribute = target.GetType().GetAttribute<AddGeneralPrototypeComponentsAttribute>();
			AddPrototypeComponentsAttribute addPrototypeComponentsAttribute = target.GetType().GetAttribute<AddPrototypeComponentsAttribute>();

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
	        EditorGUI.BeginChangeCheck();

	        List<Type> drawTypes = ToolEditorUtility.GetGroupComponents(group, TargetType);
	        
	        if(drawTypes.Count != 0)
				group.ComponentStackEditor.OnGUI(drawTypes, Target.GetType());
	        
	        if(EditorGUI.EndChangeCheck())
	        {
		        ChangeGUIGroup(group);
	        }
        }

        protected virtual void DrawPrototypeSettings(Prototype proto)
        {
	        EditorGUI.BeginChangeCheck();

	        List<Type> drawTypes = ToolEditorUtility.GetPrototypeComponents(proto, TargetType);

	        if(drawTypes.Count != 0)
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
		
		protected string GetNameToolSettings()
		{
			return "Tool Settings (" + Target.GetType().GetAttribute<NameAttribute>().Name + ")";
		}
    }
}
#endif