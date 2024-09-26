#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.BrushSettings;
using GUIUtility = VladislavTsurikov.Utility.GUIUtility;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.BrushSettings
{
    [Serializable]
    public class CustomMasksEditor
    {
	    private CustomMasks _target;
	    
        private int alphaBrushesIconWidth  = 60;
        private int alphaBrushesIconHeight = 60;

		public Vector2 alphaBrushesWindowsScroll = Vector2.zero;
        public float alphaBrushesWindowHeight = 100.0f;
        
        public CustomMasksEditor(CustomMasks target)
        {
	        _target = target;
        }

        public void OnGUI()
        {
	        Event e = Event.current;

	        // Settings
	        Color initialGUIColor = UnityEngine.GUI.backgroundColor;

	        Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, alphaBrushesWindowHeight)) );
	        windowRect = EditorGUI.IndentedRect(windowRect);

	        Rect virtualRect = new Rect(windowRect);

	        if(IsNecessaryToDrawIconsForCustomBrushes(windowRect, initialGUIColor))
	        {
		        SelectedWindowUtility.DrawLabelForIcons(initialGUIColor, windowRect);
		        DrawBrushIcons(e, windowRect);
	        }

	        SelectedWindowUtility.DrawResizeBar(e, alphaBrushesIconHeight, ref alphaBrushesWindowHeight);

	        DropOperationForBrush(e, virtualRect);

	        switch (e.type)
	        {
		        case EventType.ContextClick:
		        {
			        if(virtualRect.Contains(e.mousePosition))
			        {
				        BrushesWindowMenu().ShowAsContext();
				        e.Use();
			        }
			        break;
		        }
	        }

	        GUILayout.Space(3);
        }

        private void DrawBrushIcons(Event e, Rect windowRect)
		{
			Rect virtualRect = SelectedWindowUtility.GetVirtualRect(windowRect, _target.Masks.Count, alphaBrushesIconWidth, alphaBrushesIconHeight);

			Vector2 brushWindowScrollPos = alphaBrushesWindowsScroll;
            brushWindowScrollPos = UnityEngine.GUI.BeginScrollView(windowRect, brushWindowScrollPos, virtualRect, false, true);

			int y = (int)virtualRect.yMin;
			int x = (int)virtualRect.xMin;

			for (int alphaBrushesIndex = 0; alphaBrushesIndex < _target.Masks.Count; alphaBrushesIndex++)
			{
				Rect brushIconRect = new Rect(x, y, alphaBrushesIconWidth, alphaBrushesIconHeight);

				Color rectColor;

				if (alphaBrushesIndex == _target.selectedCustomBrush)
				{
					rectColor = EditorColors.Instance.ToggleButtonActiveColor;
				}
        	    else 
				{ 
					rectColor = Color.white; 
				}

				DrawIconRectForBrush(brushIconRect, _target.Masks[alphaBrushesIndex], rectColor, e, windowRect, brushWindowScrollPos, () =>
				{
					HandleSelectBrush(alphaBrushesIndex, e);
				});

				SelectedWindowUtility.SetNextXYIcon(virtualRect, alphaBrushesIconWidth, alphaBrushesIconHeight, ref y, ref x);
			}

			alphaBrushesWindowsScroll = brushWindowScrollPos;

			UnityEngine.GUI.EndScrollView();
		}

        public void HandleSelectBrush(int brushIndex, Event e)
		{
			switch (e.type)
			{
				case EventType.MouseDown:
				{
					if(e.button == 0)
					{																		
						_target.selectedCustomBrush = brushIndex; 
            			e.Use();
					}

            	    break;
				}
				case EventType.ContextClick:
				{
					BrushesMenu(brushIndex).ShowAsContext();
					e.Use();

            		break;
				}
			}
		}

        private bool IsNecessaryToDrawIconsForCustomBrushes(Rect brushWindowRect, Color initialGUIColor)
		{
			if(_target.Masks.Count == 0)
			{
				SelectedWindowUtility.DrawLabelForIcons(initialGUIColor, brushWindowRect, "Drag & Drop Textures Here");
				return false;
			}

			return true;
		}

        private void DrawIconRectForBrush(Rect brushIconRect, Texture2D preview, Color rectColor, Event e, Rect brushWindowRect, Vector2 brushWindowScrollPos, UnityAction clickAction)
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
						UnityEngine.GUI.DrawTexture(previewRect, preview);
					}
					else
					{
						Color dimmedColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);
						EditorGUI.DrawRect(previewRect, dimmedColor);
					}
                }
			}
		}

        private void DropOperationForBrush(Event e, Rect virtualRect)
		{
			if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
            {
                // Add Prefab
                if(virtualRect.Contains (e.mousePosition))
                {
                    if (DragAndDrop.objectReferences.Length > 0)
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					}

                    if (e.type == EventType.DragPerform)
					{
                        DragAndDrop.AcceptDrag();

                        foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                        {
							if (draggedObject is Texture2D)
                            {
								_target.Masks.Add((Texture2D)draggedObject);
                            }
                        }

                    }
                    e.Use();
                } 
			}
		}

		private GenericMenu BrushesWindowMenu()
        {
            GenericMenu menu = new GenericMenu();

			menu.AddItem(new GUIContent("Get Polaris Brushes"), false, GUIUtility.ContextMenuCallback, new Action(() => _target.GetPolarisBrushes()));

			menu.AddSeparator ("");

			menu.AddItem(new GUIContent("Delete All"), false, GUIUtility.ContextMenuCallback, new Action(() => { _target.Masks.Clear(); }));

            return menu;
        }

        private GenericMenu BrushesMenu(int selectedAlphaBrush)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Delete"), false, GUIUtility.ContextMenuCallback, new Action(() => _target.Masks.RemoveAt((selectedAlphaBrush))));

            return menu;
        }
    }
}
#endif
