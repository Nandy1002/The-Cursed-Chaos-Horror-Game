#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.SelectionWindow;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.Window;

namespace VladislavTsurikov.MegaWorld.TextureStamperTool.ScriptsEditor
{
    [CustomEditor(typeof(Scripts.TextureStamperTool))]
    public class TextureStamperToolEditor : ToolMonoBehaviourEditor
    {
		private Scripts.TextureStamperTool _textureStamperTool;

        #region UI Settings
        private bool _stamperToolControllerFoldout = true;
		#endregion

		protected override void OnInit()
        {
            _textureStamperTool = (Scripts.TextureStamperTool)target;
            
	        _textureStamperTool.Area.SetAreaBounds(_textureStamperTool);

			DrawBasicData = new DrawBasicData(typeof(GeneralSelectionGroupWindow), typeof(GeneralSelectionPrototypeWindow),
				_textureStamperTool.Data, target.GetType());
        }
		
		public override void ChangeGUIGroup(Group group)
		{
			if (MaskFilterComponentEditor.ChangedGUI)
			{
				_textureStamperTool.StamperVisualisation.UpdateMask = true;
				MaskFilterComponentEditor.ChangedGUI = false;
			}
			
			_textureStamperTool.AutoRespawnController.StartAutoRespawn(group, _textureStamperTool);
		}

		public override void ChangeGUIPrototype(Prototype proto)
		{
			if (MaskFilterComponentEditor.ChangedGUI)
			{
				_textureStamperTool.StamperVisualisation.UpdateMask = true;
				MaskFilterComponentEditor.ChangedGUI = false;
			}

			_textureStamperTool.AutoRespawnController.StartAutoRespawn(_textureStamperTool.Data.SelectedVariables.SelectedGroup, _textureStamperTool);
		}

        public override void DrawToolSettings()
		{
			_toolSettingsFoldout = CustomEditorGUILayout.Foldout(_toolSettingsFoldout, GetNameToolSettings());

			if(_toolSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				_textureStamperTool.Area.OnGUI(_textureStamperTool);
				StamperToolControllerWindowGUI();

				EditorGUI.indentLevel--;
			}
		}
        
        public void StamperToolControllerWindowGUI()
		{
			_stamperToolControllerFoldout = CustomEditorGUILayout.Foldout(_stamperToolControllerFoldout, "Texture Stamper Tool Controller");

			if(_stamperToolControllerFoldout)
			{
				EditorGUI.indentLevel++;

				_textureStamperTool.textureStamperToolControllerSettings.Visualisation = CustomEditorGUILayout.Toggle(visualisation, _textureStamperTool.textureStamperToolControllerSettings.Visualisation);

				if(_textureStamperTool.Area.UseSpawnCells == false)
				{
					_textureStamperTool.textureStamperToolControllerSettings.AutoRespawn = CustomEditorGUILayout.Toggle(autoRespawn, _textureStamperTool.textureStamperToolControllerSettings.AutoRespawn);

					if(_textureStamperTool.textureStamperToolControllerSettings.AutoRespawn)
					{
						EditorGUI.indentLevel++;
						_textureStamperTool.textureStamperToolControllerSettings.DelayAutoRespawn = CustomEditorGUILayout.Slider(delayAutoSpawn, _textureStamperTool.textureStamperToolControllerSettings.DelayAutoRespawn, 0, 3);
						EditorGUI.indentLevel--;
						
						GUILayout.BeginHorizontal();
         				{
							GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
							if(CustomEditorGUILayout.ClickButton("Respawn", ButtonStyle.Add, ButtonSize.ClickButton))
							{
								Unspawn.UnspawnAllProto(_textureStamperTool.Data.GroupList, false);
								_textureStamperTool.Spawn();
							}
							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();

						GUILayout.Space(3);
					}
					else
					{
						DrawSpawnControls();
					}
				}
				else
				{
					CustomEditorGUILayout.HelpBox("Auto Spawn does not support when \"Use Spawn Cells\" is enabled in \"Area Settings\".");
	
					DrawSpawnWithCellsControls();
				}

				EditorGUI.indentLevel--;
			}
		}

		private void DrawSpawnControls()
        {
            if (_textureStamperTool.SpawnProgress > 0f && _textureStamperTool.SpawnProgress < 1f)
           	{
				GUILayout.BeginHorizontal();
         		{
					GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
					if(CustomEditorGUILayout.ClickButton("Cancel", ButtonStyle.Remove))
					{
						CancelSpawn();
					}
					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);
           	}
           	else
           	{
				if(_textureStamperTool.Data.SelectedVariables.GetSelectedPrototypes(typeof(PrototypeTerrainTexture)).Count == 0)
				{
					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						if(CustomEditorGUILayout.ClickButton("Respawn", ButtonStyle.Add, ButtonSize.ClickButton))
						{
							Unspawn.UnspawnAllProto(_textureStamperTool.Data.GroupList, false);
							_textureStamperTool.Spawn();
						}

						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();

					GUILayout.Space(3);
				}

				GUILayout.BeginHorizontal();
         		{
					GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
					if(CustomEditorGUILayout.ClickButton("Spawn", ButtonStyle.Add))
					{
						_textureStamperTool.Spawn();
					}
					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);
           	}
        }

		private void DrawSpawnWithCellsControls()
        {
			if (_textureStamperTool.SpawnProgress > 0f && _textureStamperTool.SpawnProgress < 1f)
           	{
				GUILayout.BeginHorizontal();
         		{
					GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
					if(CustomEditorGUILayout.ClickButton("Cancel", ButtonStyle.Remove))
					{
						CancelSpawn();
					}
					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);
           	}
           	else
           	{
				if(_textureStamperTool.Data.SelectedVariables.GetSelectedPrototypes(typeof(PrototypeTerrainTexture)).Count == 0)
				{
					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						if(CustomEditorGUILayout.ClickButton("Refresh", ButtonStyle.Add, ButtonSize.ClickButton))
						{
							if(_textureStamperTool.Area.CellList.Count == 0)
							{
								_textureStamperTool.Area.CreateCells();
							}

							Unspawn.UnspawnAllProto(_textureStamperTool.Data.GroupList, false);
							_textureStamperTool.SpawnWithCells(_textureStamperTool.Area.CellList);
						}
	
						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();
	
					GUILayout.Space(3);
				}

				GUILayout.BeginHorizontal();
         		{
					GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
					if(CustomEditorGUILayout.ClickButton("Spawn", ButtonStyle.Add))
					{
						if(_textureStamperTool.Area.CellList.Count == 0)
						{
							_textureStamperTool.Area.CreateCells();
						}

						_textureStamperTool.SpawnWithCells(_textureStamperTool.Area.CellList);
					}
					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);
           	}
        }

		public void CancelSpawn()
        {
            _textureStamperTool.CancelSpawn = true;
            _textureStamperTool.SpawnProgress = 0f;
			EditorUtility.ClearProgressBar();
        }

        [MenuItem("GameObject/MegaWorld/Add Texture Stamper", false, 14)]
    	public static void AddStamper(MenuCommand menuCommand)
    	{
    		GameObject stamper = new GameObject("Texture Stamper");
            stamper.transform.localScale = new Vector3(150, 150, 150);
    		stamper.AddComponent<Scripts.TextureStamperTool>();
            UnityEditor.Undo.RegisterCreatedObjectUndo(stamper, "Created " + stamper.name);
    		Selection.activeObject = stamper;
    	}

		[NonSerialized]
		public GUIContent visualisation = new GUIContent("Visualisation", "Allows you to see the Mask Filter Settings visualization.");
		[NonSerialized]
		public GUIContent autoRespawn = new GUIContent("Auto Respawn", "Allows you to do automatic deletion and then spawn when you changed the settings.");
		[NonSerialized]
		public GUIContent delayAutoSpawn = new GUIContent("Delay Auto Spawn", "Respawn delay in seconds.");
    }
}
#endif