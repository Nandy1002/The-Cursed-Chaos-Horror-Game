#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.Extensions.Scripts;
using VladislavTsurikov.MegaWorld.Core.Scripts.ResourcesController;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using GUIUtility = VladislavTsurikov.Utility.GUIUtility;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ResourcesController 
{
	[Serializable]
    public static class TerrainResourcesControllerEditor 
    {
		[Serializable]
		public class Layer
		{
			public bool selected = false;
			public TerrainLayer AssignedLayer = null;
		}

		private static List<Layer> paletteLayers = new List<Layer>();
        
		private static int protoIconWidth  = 64;
        private static int protoIconHeight = 76;
		private static float prototypeWindowHeight = 100f;

		private static Vector2 prototypeWindowsScroll = Vector2.zero;
		private static InternalDragAndDrop _dragAndDrop = new InternalDragAndDrop();

		static void UpdateLayerPalette(Terrain terrain)
		{
			if (terrain == null)
			{
				return;
			}

			bool[] selectedList = new bool[paletteLayers.Count];
			for(int i = 0; i < paletteLayers.Count; i++)
			{
				if (paletteLayers[i] != null)
				{
					selectedList[i] = paletteLayers[i].selected;
				}				
			}

			paletteLayers.Clear();

			int index = 0;
			foreach (TerrainLayer layer in terrain.terrainData.terrainLayers)
			{
				if(layer != null) 
				{
					Layer paletteLayer = new Layer();//ScriptableObject.CreateInstance<Layer>();
					paletteLayer.AssignedLayer = layer; 
					paletteLayer.selected = selectedList.ElementAtOrDefault(index);
					paletteLayers.Add(paletteLayer); 
					index++;
				}
			}
		}

		public static void OnGUI(Group group)
		{
			if(Terrain.activeTerrains.Length != 0)
			{	
				TerrainResourcesController.DetectSyncError(group, Terrain.activeTerrain);

				switch (TerrainResourcesController.SyncError)
				{
					case TerrainResourcesController.TerrainResourcesSyncError.None:
					{
						string getResourcesFromTerrain = "Get/Update Resources From Terrain";
		
						GUILayout.BeginHorizontal();
						{
							GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
							GUILayout.BeginVertical();
							{
								if(CustomEditorGUILayout.ClickButton(getResourcesFromTerrain, ButtonStyle.ButtonClick, ButtonSize.ClickButton))
								{
									TerrainResourcesController.UpdatePrototypesFromTerrain(Terrain.activeTerrain, group);
								}
	
								GUILayout.Space(3);
	
								GUILayout.BeginHorizontal();
								{
									if(CustomEditorGUILayout.ClickButton("Add Missing Resources", ButtonStyle.Add))
									{
										TerrainResourcesController.AddMissingPrototypesToTerrains(group);
									}
	
									GUILayout.Space(2);
	
									if(CustomEditorGUILayout.ClickButton("Remove All Resources", ButtonStyle.Remove))
									{
										if (EditorUtility.DisplayDialog("WARNING!",
											"Are you sure you want to remove all Terrain Resources from the scene?",
											"OK", "Cancel"))
										{
											TerrainResourcesController.RemoveAllPrototypesFromTerrains(group);
										}
									}
								}
								GUILayout.EndHorizontal();
							}
							GUILayout.EndVertical();
							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();

						break;
					}
					case TerrainResourcesController.TerrainResourcesSyncError.NotAllProtoAvailable:
					{
						if (group.PrototypeType == typeof(PrototypeTerrainDetail))
						{
							CustomEditorGUILayout.WarningBox("You need all Terrain Details prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");   
						}
						else if (group.PrototypeType == typeof(PrototypeTerrainTexture))
						{
							CustomEditorGUILayout.WarningBox("You need all Terrain Textures prototypes to be in the terrain. Click \"Add Missing Resources To Terrain\"");   
						}
	
						string getResourcesFromTerrain = "Get/Update Resources From Terrain";
		
						GUILayout.BeginHorizontal();
						{
							GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
							GUILayout.BeginVertical();
							{
								if(CustomEditorGUILayout.ClickButton(getResourcesFromTerrain, ButtonStyle.ButtonClick, ButtonSize.ClickButton))
								{
									TerrainResourcesController.UpdatePrototypesFromTerrain(Terrain.activeTerrain, group);
								}
	
								GUILayout.Space(3);
	
								GUILayout.BeginHorizontal();
								{
									if(CustomEditorGUILayout.ClickButton("Add Missing Resources", ButtonStyle.Add))
									{
										TerrainResourcesController.AddMissingPrototypesToTerrains(group);
									}
	
									GUILayout.Space(2);
	
									if(CustomEditorGUILayout.ClickButton("Remove All Resources", ButtonStyle.Remove))
									{
										if (EditorUtility.DisplayDialog("WARNING!",
											"Are you sure you want to remove all Terrain Resources from the scene?",
											"OK", "Cancel"))
										{
											TerrainResourcesController.RemoveAllPrototypesFromTerrains(group);
										}
									}
								}
								GUILayout.EndHorizontal();
							}
							GUILayout.EndVertical();
							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();

						break;
					}
					case TerrainResourcesController.TerrainResourcesSyncError.MissingPrototypes:
					{
						string getResourcesFromTerrain = "Get/Update Resources From Terrain";
	
						GUILayout.BeginHorizontal();
						{
							GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
							GUILayout.BeginVertical();
							{
								if(CustomEditorGUILayout.ClickButton(getResourcesFromTerrain, ButtonStyle.ButtonClick, ButtonSize.ClickButton))
								{
									TerrainResourcesController.UpdatePrototypesFromTerrain(Terrain.activeTerrain, group);
								}
							}
							GUILayout.EndVertical();
							GUILayout.Space(5);
						}
						GUILayout.EndHorizontal();

						break;
					}
				}
				
				if (group.PrototypeType == typeof(PrototypeTerrainTexture))
				{
					CustomEditorGUILayout.Header("Active Terrain: Layer Palette");   

					if(Terrain.activeTerrain != null)
					{
						DrawSelectedWindowForTerrainTextureResources(Terrain.activeTerrain, group);
					}
				}

				GUILayout.Space(3);
			}
			else
			{
				CustomEditorGUILayout.WarningBox("There is no active terrain in the scene.");
			}
		}

		public static void DrawSelectedWindowForTerrainTextureResources(Terrain terrain, Group group)
		{
			bool dragAndDrop = false;

			Color InitialGUIColor = UnityEngine.GUI.color;

			Event e = Event.current;

			Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, prototypeWindowHeight)) );
			windowRect = EditorGUI.IndentedRect(windowRect);

			Rect virtualRect = new Rect(windowRect);

			if(IsNecessaryToDrawIconsForPrototypes(windowRect, InitialGUIColor, terrain, group, ref dragAndDrop) == true)
			{
				SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect);

				UpdateLayerPalette(terrain);
				DrawProtoTerrainTextureIcons(e, group, paletteLayers, windowRect);

				SelectedWindowUtility.DrawResizeBar(e, protoIconHeight, ref prototypeWindowHeight);
			}
			else
			{
				SelectedWindowUtility.DrawResizeBar(e, protoIconHeight, ref prototypeWindowHeight);
			}
		}

		private static bool IsNecessaryToDrawIconsForPrototypes(Rect brushWindowRect, Color InitialGUIColor, Terrain terrain, Group group, ref bool dragAndDrop)
		{
			if (group.PrototypeType == typeof(PrototypeTerrainDetail))
			{
				if(terrain.terrainData.detailPrototypes.Length == 0)
				{
					SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, brushWindowRect, "Missing \"Terrain Detail\" on terrain");
					dragAndDrop = true;
					return false;
				}
			}
			else if (group.PrototypeType == typeof(PrototypeTerrainTexture))
			{
				if(terrain.terrainData.terrainLayers.Length == 0)
				{
					SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, brushWindowRect, "Missing \"Terrain Layers\" on terrain");
					dragAndDrop = true;
					return false;
				}
			}

			dragAndDrop = true;
			return true;
		}

		private static void DrawProtoTerrainTextureIcons(Event e, Group group, List<Layer> paletteLayers, Rect windowRect)
		{
			Layer draggingPrototypeTerrainTexture = null;
			if (_dragAndDrop.IsDragging() || _dragAndDrop.IsDragPerform())
            {
                if(_dragAndDrop.GetData() is Layer)
				{
					draggingPrototypeTerrainTexture = (Layer)_dragAndDrop.GetData();
				}      
            }

			Rect virtualRect = SelectedWindowUtility.GetVirtualRect(windowRect, paletteLayers.Count, protoIconWidth, protoIconHeight);

			Vector2 windowScrollPos = prototypeWindowsScroll;
            windowScrollPos = UnityEngine.GUI.BeginScrollView(windowRect, windowScrollPos, virtualRect, false, true);

			Rect dragRect = new Rect(0, 0, 0, 0);

			int y = (int)virtualRect.yMin;
			int x = (int)virtualRect.xMin;

			for (int protoIndexForGUI = 0; protoIndexForGUI < paletteLayers.Count; protoIndexForGUI++)
			{
				Rect iconRect = new Rect(x, y, protoIconWidth, protoIconHeight);

				Color textColor;
				Color rectColor;

				Texture2D preview;
				string name;

				SetColorForIcon(paletteLayers[protoIndexForGUI].selected, out textColor, out rectColor);
				SetIconInfoForTexture(paletteLayers[protoIndexForGUI].AssignedLayer, out preview, out name);

				Rect iconRectScrolled = new Rect(iconRect);
            	iconRectScrolled.position -= windowScrollPos;

            	// only visible icons
            	if(iconRectScrolled.Overlaps(windowRect))
            	{
            	    if(iconRect.Contains(e.mousePosition))
					{
            			_dragAndDrop.AddDragObject(paletteLayers[protoIndexForGUI]);

						if (draggingPrototypeTerrainTexture != null && e.type == EventType.Repaint)
						{
							if(_dragAndDrop.IsDragPerform())
                			{
								Move(paletteLayers, GetSelectedIndexLayer(), protoIndexForGUI);
								SetToTerrainLayers(group);
								TerrainResourcesController.SyncTerrainID(Terrain.activeTerrain, group);
                			}

							EditorGUI.DrawRect(iconRect, Color.white.WithAlpha(0.3f));
						}

						HandleSelectLayer(protoIndexForGUI, group, e);
					}

					DrawIcon(e, iconRect, preview, name, textColor, rectColor, protoIconWidth, false);
				}

				SelectedWindowUtility.SetNextXYIcon(virtualRect, protoIconWidth, protoIconHeight, ref y, ref x);
			}

			prototypeWindowsScroll = windowScrollPos;

			UnityEngine.GUI.EndScrollView();
		}

		private static void DrawIcon(Event e, Rect iconRect, Texture2D preview, string name, Color textColor, Color rectColor, 
			int iconWidth, bool drawTextureWithAlphaChannel)
		{
			GUIStyle LabelTextForIcon = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForIcon);

			EditorGUI.DrawRect(iconRect, rectColor);
                
            if(e.type == EventType.Repaint)
            {
                Rect previewRect = new Rect(iconRect.x+2, iconRect.y+2, iconRect.width-4, iconRect.width-4);

				if (preview != null)
            	{						
					if(drawTextureWithAlphaChannel)
					{
						UnityEngine.GUI.DrawTexture(previewRect, preview);
					}
					else
					{
						EditorGUI.DrawPreviewTexture(previewRect, preview);
					}						
            	}
            	else
            	{
            	    Color dimmedColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);
					EditorGUI.DrawRect(previewRect, dimmedColor);
            	}

				LabelTextForIcon.normal.textColor = textColor;
				LabelTextForIcon.Draw(iconRect, SelectedWindowUtility.GetShortNameForIcon(name, iconWidth), false, false, false, false);
            }
		}

		private static void SetColorForIcon(bool selected, out Color textColor, out Color rectColor)
		{
			textColor = EditorColors.Instance.LabelColor;

			if(selected)
			{
				rectColor = EditorColors.Instance.ToggleButtonActiveColor;
			}
			else
			{
				rectColor = EditorColors.Instance.ToggleButtonInactiveColor;
			}
		}

		private static void SetIconInfoForTexture(TerrainLayer protoTerrainTexture, out Texture2D preview, out string name)
		{
            if (protoTerrainTexture.diffuseTexture != null)
            {
                preview = protoTerrainTexture.diffuseTexture;      
				name = protoTerrainTexture.name;
            }
			else
			{
				preview = null;
				name = "Missing Texture";
			}
		}

		public static void HandleSelectLayer(int prototypeIndex, Group group, Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
				{
					if(e.button == 0)
					{										
						SelectLayer(prototypeIndex);

            	    	e.Use();
					}

            	    break;
				}
				case EventType.ContextClick:
				{
					PrototypeTerrainTextureMenu(group).ShowAsContext();

					e.Use();

            		break;
				}
			}
		}

		public static void SelectLayer(int prototypeIndex)
        {
            SetSelectedAllLayer(false);

            if(prototypeIndex < 0 && prototypeIndex >= paletteLayers.Count)
            {
                return;
            }

            paletteLayers[prototypeIndex].selected = true;
        }

		public static void SetSelectedAllLayer(bool select)
        {
            foreach (Layer proto in paletteLayers)
            {
                proto.selected = select;
            }
        }

		public static int GetSelectedIndexLayer()
        {
			for (int i = 0; i < paletteLayers.Count; i++)
			{
				if(paletteLayers[i].selected)
				{
					return i;
				}
			}

			return 0;
		}

		public static void SetToTerrainLayers(Group group)
		{
			List<TerrainLayer> layers = new List<TerrainLayer>();

			foreach (Layer item in paletteLayers)
			{
				layers.Add(item.AssignedLayer);
			}

#if UNITY_2019_2_OR_NEWER
			Terrain.activeTerrain.terrainData.SetTerrainLayersRegisterUndo(layers.ToArray(), "Reorder Terrain Layers");
#else
			Terrain.activeTerrain.terrainData.terrainLayers = layers.ToArray();
#endif

			TerrainResourcesController.SyncAllTerrains(group, Terrain.activeTerrain);
		}

		private static GenericMenu PrototypeTerrainTextureMenu(Group group)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Delete"), false, GUIUtility.ContextMenuCallback, new Action(() => DeleteSelectedProtoTerrainTexture(group)));			

            return menu;
        }

        public static void DeleteSelectedProtoTerrainTexture(Group group)
        {
            paletteLayers.RemoveAll((prototype) => prototype.selected);
			SetToTerrainLayers(group);
        }

		private static void Move(IList elements, int sourceIndex, int destIndex)
        {
            if (destIndex >= elements.Count) 
            {
                return;
            }

            Icon item = (Icon)elements[sourceIndex];
            elements.RemoveAt(sourceIndex);
            elements.Insert(destIndex, item);
        }
	}
}
#endif