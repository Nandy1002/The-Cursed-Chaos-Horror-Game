#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.BrushSettings;
using VladislavTsurikov.MegaWorld.TextureStamperTool.Scripts;

namespace VladislavTsurikov.MegaWorld.TextureStamperTool.ScriptsEditor
{
    [Serializable]
    public class AreaEditor
    {
	    private ProceduralMaskEditor _proceduralMaskEditor;
	    private CustomMasksEditor _customMasksEditor;
	    private bool _isInit = false;

	    public void OnGUI(Scripts.TextureStamperTool textureStamper, Area area)
        {
	        if (!_isInit)
	        {
		        _proceduralMaskEditor = new ProceduralMaskEditor(area.ProceduralMask);
		        _customMasksEditor = new CustomMasksEditor(area.CustomMasks); 
	        }
	        
            DrawAreaSettings(area, textureStamper);
        }

	    public void DrawAreaSettings(Area area, Scripts.TextureStamperTool textureStamper)
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
    			    	area.FitToTerrainSize(textureStamper);
    			    }
            	    GUILayout.Space(3);
            	}
            	GUILayout.EndHorizontal();

    			GUILayout.Space(3);

				area.UseSpawnCells = CustomEditorGUILayout.Toggle(new GUIContent("Use Spawn Cells"), area.UseSpawnCells);

				if(area.UseSpawnCells)
				{
					CustomEditorGUILayout.HelpBox("It is recommended to enable \"Use Cells\" when your terrain is more than 4 km * 4 km. This parameter creates smaller cells, \"Stamper Tool\" will spawn each cell in turn. Why this parameter is needed, too long spawn delay can disable Unity.");

					GUILayout.BeginHorizontal();
            		{
            		    GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
            		    if(CustomEditorGUILayout.ClickButton("Refresh Cells"))
    				    {
    				    	area.CreateCells();
    				    }
            		    GUILayout.Space(3);
            		}
            		GUILayout.EndHorizontal();

    				GUILayout.Space(3);

					area.CellSize = CustomEditorGUILayout.FloatField(cellSize, area.CellSize);
					CustomEditorGUILayout.Label("Cell Count: " + area.CellList.Count);
					area.ShowCells = CustomEditorGUILayout.Toggle(showCells, area.ShowCells); 
				}
				else
				{
					area.UseMask = CustomEditorGUILayout.Toggle(new GUIContent("Use Mask"), area.UseMask);

            		if(area.UseMask)
            		{
                	    area.MaskType = (MaskType)CustomEditorGUILayout.EnumPopup(new GUIContent("Mask Type"), area.MaskType);

            		    switch (area.MaskType)
			    	    {
			    	    	case MaskType.Custom:
			    	    	{
				                _customMasksEditor.OnGUI();

			    	    		break;
			    	    	}
			    	    	case MaskType.Procedural:
			    	    	{
				                _proceduralMaskEditor.OnGUI();

			    	    		break;
			    	    	}
			    	    }
            		}
				}
				
    			EditorGUI.indentLevel--;
    		}
        }

		public GUIContent cellSize = new GUIContent("Cell Size", "Sets the cell size in meters.");
		public GUIContent showCells = new GUIContent("Show Cells", "Shows all available cells.");
    }
}
#endif