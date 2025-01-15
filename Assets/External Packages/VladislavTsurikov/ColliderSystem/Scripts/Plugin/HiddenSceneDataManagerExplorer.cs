#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.SceneDataSystem.Scripts;

namespace VladislavTsurikov.ColliderSystem.Scripts.Plugin
{
	public class HiddenSceneDataManagerExplorer : EditorWindow
	{
	    private List<SceneDataManager> _sceneDataManagerList = new List<SceneDataManager>();
		private Vector2 _scrollPos = Vector2.zero;

		[MenuItem("Tools/Collider System/Hidden Scene Data Manager Explorer")]
	    static void Init()
	    {
			GetWindow<HiddenSceneDataManagerExplorer>();
	    }

		void OnEnable()
		{
			FindAllSceneDataManager();
		}

		void FindAllSceneDataManager()
		{
			var objs = FindObjectsOfType(typeof(SceneDataManager)) as SceneDataManager[];
			_sceneDataManagerList.Clear();
			foreach(var obj in objs)
			{
				_sceneDataManagerList.Add(obj);
			}
		}

	    void OnGUI()
	    {
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Find Scene Data Managers"))
			{
				FindAllSceneDataManager();
			}
			GUILayout.EndHorizontal();
			_scrollPos = GUILayout.BeginScrollView(_scrollPos);

			foreach (SceneDataManager sceneDataManager in _sceneDataManagerList)
			{
				GameObject O = sceneDataManager.gameObject;
				if (O == null)
					continue;
				GUILayout.BeginHorizontal(); 
				CustomEditorGUILayout.Label(sceneDataManager.GetScene().name);
				GUILayout.Space(20);
				if (GUILayout.Button("DELETE"))
				{
					DestroyImmediate(O);
					FindAllSceneDataManager();
					GUIUtility.ExitGUI();
				}
				GUILayout.Space(20);
				GUILayout.EndHorizontal();
				DrawSceneDataList(sceneDataManager);
			}
			GUILayout.EndScrollView();
	    }

		public void DrawSceneDataList(SceneDataManager sceneDataManager) 
	    {			
	        foreach (var item in sceneDataManager.SceneDataList)
	        {
	            CustomEditorGUILayout.Label(item.GetType().ToString().Split('.').Last());
	        }
	    }
	}
}
#endif