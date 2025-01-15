#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters;
using VladislavTsurikov.MegaWorld.Core.Scripts;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.MaskFilters.Filters 
{
	[SettingsEditor(typeof(TexturesFilter))]
    public class TexturesFilterEditor : MaskFilterEditor
    {
	    private TexturesFilter _texturesFilter;

	    public override void OnEnable()
	    {
		    _texturesFilter = (TexturesFilter)Target;
	    }
	    
	    public override void OnGUI(Rect rect, int index)
	    {
		    if(Terrain.activeTerrain == null)
		    {
			    EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "There is no terrain in the scene", MessageType.Warning);

			    rect.y += EditorGUIUtility.singleLineHeight;
			    return;
		    }

		    Event e = Event.current;

		    // Settings
		    Color initialGUIColor = UnityEngine.GUI.backgroundColor;

		    if(index != 0)
		    {
			    _texturesFilter.BlendMode = (BlendMode)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Blend Mode"), _texturesFilter.BlendMode);
			    rect.y += EditorGUIUtility.singleLineHeight;
			    rect.y += EditorGUIUtility.singleLineHeight;
		    }

		    Rect windowRect = new Rect(rect.x, rect.y, rect.width, сheckTexturesWindowHeight);

		    Rect virtualRect = new Rect(windowRect);

		    if(_texturesFilter.IsSyncTerrain(Terrain.activeTerrain) == false)
		    {
			    _texturesFilter.UpdateCheckTerrainTextures(Terrain.activeTerrain);
		    }

		    if(IsNecessaryToDrawIconsForCheckTerrainTextures(windowRect, initialGUIColor, _texturesFilter.TerrainTextureList))
		    {
			    DrawLabelForIcons(initialGUIColor, windowRect);
			    DrawCheckTerrainTexturesIcons(e, windowRect);
		    }

		    switch (e.type)
		    {
			    case EventType.ContextClick:
			    {
				    if(virtualRect.Contains(e.mousePosition))
				    {
					    CheckTerrainTexturesWindowMenu().ShowAsContext();
					    e.Use();
				    }
				    break;
			    }
		    }

		    GUILayout.Space(3);
        }

        public override float GetElementHeight(int index) 
        {
            if(Terrain.activeTerrain == null)
            {
			    return EditorGUIUtility.singleLineHeight * 2;
            }

            if(index != 0)
            {
                return EditorGUIUtility.singleLineHeight * 12;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight * 11;
            }
        }

        private int сheckTexturesIconWidth  = 60;
        private int сheckTexturesIconHeight = 60;
		public Vector2 сheckTexturesWindowsScroll = Vector2.zero;
        public float сheckTexturesWindowHeight = 140.0f;

        private void DrawCheckTerrainTexturesIcons(Event e, Rect windowRect)
		{
			List<TerrainTexture> checkTerrainTextures = _texturesFilter.TerrainTextureList;

			Rect virtualRect = GetVirtualRect(windowRect, checkTerrainTextures.Count, сheckTexturesIconWidth, сheckTexturesIconHeight);

			Vector2 brushWindowScrollPos = сheckTexturesWindowsScroll;
            brushWindowScrollPos = UnityEngine.GUI.BeginScrollView(windowRect, brushWindowScrollPos, virtualRect, false, true);

			int y = (int)virtualRect.yMin;
			int x = (int)virtualRect.xMin;

			for (int i = 0; i < checkTerrainTextures.Count; i++)
			{
				Rect brushIconRect = new Rect(x, y, сheckTexturesIconWidth, сheckTexturesIconHeight);

				Color rectColor;

				if (checkTerrainTextures[i].selected)
				{
					rectColor = EditorColors.Instance.ToggleButtonActiveColor;
				}
        	    else 
				{ 
					rectColor = Color.white; 
				}

				DrawIconRectForCheckTerrainTextures(brushIconRect, checkTerrainTextures[i].texture, rectColor, e, windowRect, brushWindowScrollPos, () =>
				{
					HandleSelectCheckTerrainTextures(i, e);
				});

				SetNextXYIcon(virtualRect, сheckTexturesIconWidth, сheckTexturesIconHeight, ref y, ref x);
			}

			сheckTexturesWindowsScroll = brushWindowScrollPos;

			UnityEngine.GUI.EndScrollView();
		}

        private void DrawIconRectForCheckTerrainTextures(Rect brushIconRect, Texture2D preview, Color rectColor, Event e, Rect brushWindowRect, Vector2 brushWindowScrollPos, UnityAction clickAction)
		{
			Rect brushIconRectScrolled = new Rect(brushIconRect);
            brushIconRectScrolled.position -= brushWindowScrollPos;

            // only visible incons
            if(brushIconRectScrolled.Overlaps(brushWindowRect))
            {
                if(brushIconRect.Contains(e.mousePosition))
				{
					clickAction.Invoke();
				}

				EditorGUI.DrawRect(brushIconRect, rectColor);
                    
				// Prefab preview 
                if(e.type == EventType.Repaint)
                {
                    Rect previewRect = new Rect(brushIconRect.x+2, brushIconRect.y+2, brushIconRect.width-4, brushIconRect.width-4);

					if(preview != null)
					{
						EditorGUI.DrawPreviewTexture(previewRect, preview);
					}
					else
					{
						Color dimmedColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);
						EditorGUI.DrawRect(previewRect, dimmedColor);
					}
                }
			}
		}

		public void HandleSelectCheckTerrainTextures(int index, Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
				{
					UnityEngine.GUI.changed = true;
					
					if(e.button == 0)
					{										
						if (e.control)
						{    
							SelectCheckTerrainTextureAdditive(index);               
						}
						else if (e.shift)
						{          
							SelectCheckTerrainTextureRange(index);                
						}
						else 
						{
							SelectCheckTerrainTexture(index);
						}

            	    	e.Use();
					}

            	    break;
				}
			}
		}

        public void SelectCheckTerrainTexture(int index)
        {
            SetSelectedAllCheckTerrainTexture(false);

            if(index < 0 && index >= _texturesFilter.TerrainTextureList.Count)
            {
                return;
            }

            _texturesFilter.TerrainTextureList[index].selected = true;
        }

        public void SelectCheckTerrainTextureAdditive(int index)
        {
            if(index < 0 && index >= _texturesFilter.TerrainTextureList.Count)
            {
                return;
            }
        
            _texturesFilter.TerrainTextureList[index].selected = !_texturesFilter.TerrainTextureList[index].selected;
        }

        public void SelectCheckTerrainTextureRange(int index)
        {
            if(index < 0 && index >= _texturesFilter.TerrainTextureList.Count)
            {
                return;
            }

            int rangeMin = index;
            int rangeMax = index;

            for (int i = 0; i < _texturesFilter.TerrainTextureList.Count; i++)
            {
                if (_texturesFilter.TerrainTextureList[i].selected)
                {
                    rangeMin = Mathf.Min(rangeMin, i);
                    rangeMax = Mathf.Max(rangeMax, i);
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                if (_texturesFilter.TerrainTextureList[i].selected != true)
                {
                    break;
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                _texturesFilter.TerrainTextureList[i].selected = true;
            }
        }

        public void SetSelectedAllCheckTerrainTexture(bool select)
        {
            foreach (TerrainTexture checkTerrainTextures in _texturesFilter.TerrainTextureList)
            {
                checkTerrainTextures.selected = select;
            }
        }

        private void DrawLabelForIcons(Color InitialGUIColor, Rect windowRect, string text = null)
		{
			GUIStyle LabelTextForSelectedArea = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForSelectedArea);
			GUIStyle boxStyle = CustomEditorGUILayout.GetStyle(StyleName.Box);

			UnityEngine.GUI.color = EditorColors.Instance.boxColor;
			UnityEngine.GUI.Label(windowRect, "", boxStyle);
			UnityEngine.GUI.color = InitialGUIColor;

			if(text != null)
			{
				EditorGUI.LabelField(windowRect, text, LabelTextForSelectedArea);
			}
		}

        private bool IsNecessaryToDrawIconsForCheckTerrainTextures(Rect brushWindowRect, Color initialGUIColor, List<TerrainTexture> checkTerrainTextures)
		{
			if(checkTerrainTextures.Count == 0)
			{
				DrawLabelForIcons(initialGUIColor, brushWindowRect, "Missing textures on terrain");
				return false;
			}

			return true;
		}

        private Rect GetVirtualRect(Rect brushWindowRect, int count, int iconWidth, int iconHeight)
		{
			Rect virtualRect = new Rect(brushWindowRect);
            {
                virtualRect.width = Mathf.Max(virtualRect.width - 20, 1); // space for scroll 

                int presetColumns = Mathf.FloorToInt(Mathf.Max(1, (virtualRect.width) / iconWidth));
                int virtualRows   = Mathf.CeilToInt((float)count / presetColumns);

                virtualRect.height = Mathf.Max(virtualRect.height, iconHeight * virtualRows);
            }

			return virtualRect;
		}

        private void SetNextXYIcon(Rect virtualRect, int iconWidth, int iconHeight, ref int y, ref int x)
		{
			if(x + iconWidth < (int)virtualRect.xMax - iconWidth)
            {
                x += iconWidth;
            }
            else if(y < (int)virtualRect.yMax)
            {
                y += iconHeight;
                x = (int)virtualRect.xMin;
            }
		}

		GenericMenu CheckTerrainTexturesWindowMenu()
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Update Check Terrain Textures"), false, ContextMenuCallback, new Action(() => _texturesFilter.UpdateCheckTerrainTextures(Terrain.activeTerrain))); 

            return menu;
        }

		void ContextMenuCallback(object obj)
        {
            if (obj != null && obj is Action)
                (obj as Action).Invoke();
        }
    }
}
#endif
