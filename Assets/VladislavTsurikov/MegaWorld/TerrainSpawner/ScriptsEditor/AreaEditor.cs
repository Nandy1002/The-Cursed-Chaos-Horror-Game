#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.TerrainSpawner.Scripts;

namespace VladislavTsurikov.MegaWorld.TerrainSpawner.ScriptsEditor
{
    [Serializable]
    public class AreaEditor
    {
        public void OnGUI(Scripts.TerrainSpawner stamper, Area area)
        {
            DrawAreaSettings(area, stamper);
        }

        public void DrawAreaSettings(Area area, Scripts.TerrainSpawner stamper)
        {
    		area.AreaSettingsFoldout = CustomEditorGUILayout.Foldout(area.AreaSettingsFoldout, "Area Settings");

    		if(area.AreaSettingsFoldout)
    		{
    			EditorGUI.indentLevel++;

    			GUILayout.BeginHorizontal();
            	{
            	    GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
            	    if(CustomEditorGUILayout.ClickButton("Fit To Terrain Size"))
    			    {
    			    	area.FitToTerrainSize(stamper);
    			    }
            	    GUILayout.Space(3);
            	}
            	GUILayout.EndHorizontal();

    			GUILayout.Space(3);
                
    			EditorGUI.indentLevel--;
    		}
        }
    }
}
#endif