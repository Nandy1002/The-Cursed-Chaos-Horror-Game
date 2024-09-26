#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.UnityIntegration;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.SceneDataSystem.Scripts;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ResourcesController 
{
    [Serializable]
    public static class GameObjectControllerEditor
    {
        public static void OnGUI()
		{
            CustomEditorGUILayout.HelpBox("If you manually changed the position of the GameObject without using MegaWorld, please click on this button, otherwise, for example, Brush Erase will not be able to delete the changed GameObject.");

            GUILayout.BeginHorizontal();
            {
				GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
				if(CustomEditorGUILayout.ClickButton("Refresh Cells", ButtonStyle.Add))
				{
                    foreach (var item in SceneDataManagerStack.GetAllSceneDataManager())
                    {
                        GameObjectCollider gameObjectCollider = (GameObjectCollider)item.GetSceneData(typeof(GameObjectCollider));

                        gameObjectCollider?.RefreshObjectTree();
                    }
				}
				GUILayout.Space(5);
			}
			GUILayout.EndHorizontal();
        }
    }
}
#endif