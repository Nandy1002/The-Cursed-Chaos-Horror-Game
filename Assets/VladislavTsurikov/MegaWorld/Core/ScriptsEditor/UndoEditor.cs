#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor 
{
    [Serializable]
    public static class UndoEditor
    {
        public static void OnGUI()
		{
            GUILayout.BeginHorizontal();
            {
				GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
				if(CustomEditorGUILayout.ClickButton("Undo (" + Undo.ScriptsEditor.Undo.UndoRecordCount + "/" + Undo.ScriptsEditor.Undo.MaxNumberOfUndo + ")"))
				{
					Undo.ScriptsEditor.Undo.PerformUndo();
				}
				GUILayout.Space(3);
				if(CustomEditorGUILayout.ClickButton("Undo All"))
				{
					Undo.ScriptsEditor.Undo.PerformUndoAll();
				}
				GUILayout.Space(5);
			}
			GUILayout.EndHorizontal();
        }
    }
}
#endif