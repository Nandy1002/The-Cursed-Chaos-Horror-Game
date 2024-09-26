#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem;

namespace VladislavTsurikov.MegaWorld.TerrainSpawner.ScriptsEditor
{
	[Name("Terrain Spawner")]
    [CustomEditor(typeof(Scripts.TerrainSpawner))]
	public sealed class TerrainSpawnerEditor : ToolMonoBehaviourEditor
    {
		private Scripts.TerrainSpawner _stamperTool;
		
        private bool _stamperToolControllerFoldout = true;

		protected override void OnInit()
        {
            _stamperTool = (Scripts.TerrainSpawner)target;

            _stamperTool.Area.SetAreaBounds(_stamperTool);
        }

        public override void ChangeGUIGroup(Group group)
		{
			if (MaskFilterComponentEditor.ChangedGUI)
			{
				_stamperTool.StamperVisualisation.UpdateMask = true;
				MaskFilterComponentEditor.ChangedGUI = false;
			}
			
			_stamperTool.AutoRespawnController.StartAutoRespawn(group, _stamperTool);
		}

		public override void ChangeGUIPrototype(Prototype proto)
		{
			if (MaskFilterComponentEditor.ChangedGUI)
			{
				_stamperTool.StamperVisualisation.UpdateMask = true;
				MaskFilterComponentEditor.ChangedGUI = false;
			}
			
			if (proto.GetType() == typeof(PrototypeTerrainDetail))
			{
				PrototypeTerrainDetail prototypeTerrainDetail = (PrototypeTerrainDetail)proto;
				_stamperTool.AutoRespawnController.StartAutoRespawn(prototypeTerrainDetail, _stamperTool);
				return;
			}

			_stamperTool.AutoRespawnController.StartAutoRespawn(_stamperTool.Data.SelectedVariables.SelectedGroup, _stamperTool);
		}

		public override void DrawToolSettings()
		{
			_toolSettingsFoldout = CustomEditorGUILayout.Foldout(_toolSettingsFoldout, GetNameToolSettings());

			if(_toolSettingsFoldout)
			{
				EditorGUI.indentLevel++;

				_stamperTool.Area.OnGUI(_stamperTool);
				StamperToolControllerWindowGUI();

				EditorGUI.indentLevel--;
			}
		}

		public void StamperToolControllerWindowGUI()
		{
			_stamperToolControllerFoldout = CustomEditorGUILayout.Foldout(_stamperToolControllerFoldout, "Stamper Tool Controller");

			if(_stamperToolControllerFoldout)
			{
				EditorGUI.indentLevel++;

				_stamperTool.StamperToolControllerSettings.Visualisation = CustomEditorGUILayout.Toggle(visualisation, _stamperTool.StamperToolControllerSettings.Visualisation);

				_stamperTool.StamperToolControllerSettings.AutoRespawn = CustomEditorGUILayout.Toggle(autoRespawn, _stamperTool.StamperToolControllerSettings.AutoRespawn);

				if(_stamperTool.StamperToolControllerSettings.AutoRespawn)
				{
					EditorGUI.indentLevel++;
					_stamperTool.StamperToolControllerSettings.DelayAutoRespawn = CustomEditorGUILayout.Slider(delayAutoSpawn, _stamperTool.StamperToolControllerSettings.DelayAutoRespawn, 0, 3);
					EditorGUI.indentLevel--;
						
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						if(CustomEditorGUILayout.ClickButton("Respawn", ButtonStyle.Add, ButtonSize.ClickButton))
						{
							Unspawn.UnspawnAllProto(_stamperTool.Data.GroupList, false);
							_stamperTool.Spawn();
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

				if (_stamperTool.SpawnProgress == 0)
				{
					GUILayout.BeginHorizontal();
					{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						if(CustomEditorGUILayout.ClickButton("Unspawn Selected Prototypes", ButtonStyle.Remove, ButtonSize.ClickButton))
						{
							if (EditorUtility.DisplayDialog("WARNING!",
								    "Are you sure you want to remove all resource instances that have been selected from the scene?",
								    "OK", "Cancel"))
							{
								Unspawn.UnspawnAllProto(_stamperTool.Data.SelectedVariables.SelectedGroupList, true);
								GUILayout.BeginHorizontal();
							}
						}

						GUILayout.Space(5);
					}
					GUILayout.EndHorizontal();

					GUILayout.Space(3);
				}

				EditorGUI.indentLevel--;
			}
		}

		private void DrawSpawnControls()
        {
            if (!_stamperTool.SpawnComplete)
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
	            if(_stamperTool.Data.SelectedVariables.GetSelectedPrototypes(typeof(PrototypeTerrainTexture)).Count == 0)
				{
					GUILayout.BeginHorizontal();
         			{
						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
						if(CustomEditorGUILayout.ClickButton("Respawn", ButtonStyle.Add, ButtonSize.ClickButton))
						{
							Unspawn.UnspawnAllProto(_stamperTool.Data.GroupList, false);
							_stamperTool.Spawn();
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
						_stamperTool.Spawn();
					}
					GUILayout.Space(5);
				}
				GUILayout.EndHorizontal();

				GUILayout.Space(3);
           	}
        }

		public void CancelSpawn()
        {
            _stamperTool.CancelSpawn = true;
            _stamperTool.SpawnComplete = true;
            _stamperTool.SpawnProgress = 0f;
            EditorCoroutines.ScriptsEditor.EditorCoroutines.StopAllCoroutines();
			EditorUtility.ClearProgressBar();
        }

        [MenuItem("GameObject/MegaWorld/Add Terrain Spawner", false, 14)]
    	public static void AddStamper(MenuCommand menuCommand)
    	{
    		GameObject stamper = new GameObject("Terrain Spawner");
            stamper.transform.localScale = new Vector3(500, 500, 500);
    		stamper.AddComponent<Scripts.TerrainSpawner>();
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