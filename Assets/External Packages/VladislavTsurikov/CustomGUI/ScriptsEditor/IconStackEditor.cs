#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.ComponentStack.Scripts;
using IconAttribute = VladislavTsurikov.ComponentStack.Scripts.IconAttribute;

namespace VladislavTsurikov.CustomGUI.ScriptsEditor
{
    public class IconStackEditor 
    {   
		private Type _iconType;
		private InternalDragAndDrop _dragAndDrop = new InternalDragAndDrop();
        private Vector2 _windowsScroll = Vector2.zero;
		private bool _draggable;

		public int IconWidth  = 80;
        public int IconHeight = 95;
		public float WindowHeight = 100f;

		public string ZeroIconsWarning = "Has no icons";
		public bool DraggedIconType = false;

        public delegate Icon AddIconCallbackDelegate(UnityEngine.Object obj);
        public delegate GenericMenu IconMenuCallbackDelegate(Icon icon);
		public delegate void SetIconColorDelegate(Icon icon, out Color textColor, out Color rectColor);
		public delegate void DrawIconRectDelegate(Event e, Icon icon, Rect iconRect, Color textColor, Color rectColor);
		public delegate void DrawWindowMenuDelegate();
		public delegate void IconSelectedDelegate(Icon icon);

        public AddIconCallbackDelegate AddIconCallback;
        public IconMenuCallbackDelegate IconMenuCallback;
		public SetIconColorDelegate IconColor;
		public DrawIconRectDelegate DrawIconRect;
		public DrawWindowMenuDelegate DrawWindowMenu;
		public IconSelectedDelegate IconSelected;
		
		public IconStackEditor(bool draggable)
  		{   
			_draggable = draggable;
		}
		 
		public void OnGUI(string listMissingWarning = "")
    	{
    		Color InitialGUIColor = GUI.color;

    		Event e = Event.current;

    		Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, WindowHeight)) );
    		windowRect = EditorGUI.IndentedRect(windowRect);

			SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect, listMissingWarning);
    		
			SelectedWindowUtility.DrawResizeBar(e, IconHeight, ref WindowHeight);
		}

        public void OnGUI(IList elements, Type iconType)
        {
	        ZeroIconsWarningAttribute zeroIconsWarningAttribute = iconType.GetAttribute<ZeroIconsWarningAttribute>();
	        
	        if(zeroIconsWarningAttribute != null)
		        ZeroIconsWarning = zeroIconsWarningAttribute.Text;
	        
			_dragAndDrop.OnBeginGUI();

			_iconType = iconType;

    		bool dragAndDrop = false;

    		Color InitialGUIColor = GUI.color;

    		Event e = Event.current;

    		Rect windowRect = EditorGUILayout.GetControlRect(GUILayout.Height(Mathf.Max(0.0f, WindowHeight)) );
    		windowRect = EditorGUI.IndentedRect(windowRect);

    		Rect virtualRect = new Rect(windowRect);

    		if(IsNecessaryToDrawIcons(elements, windowRect, InitialGUIColor, ref dragAndDrop) == true)
    		{
    			SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect);

    			DrawIcons(elements, e, windowRect);
    		}
            
    		SelectedWindowUtility.DrawResizeBar(e, IconHeight, ref WindowHeight);

    		if(AddIconCallback != null && dragAndDrop)
    		{
    			DropOperation(e, virtualRect);
    		} 

			if(DrawWindowMenu != null)
			{
				if(e.type == EventType.ContextClick)
				{
					if(virtualRect.Contains(e.mousePosition))
            		{
						DrawWindowMenu();
            		    e.Use();
            		}
				}
			}

			_dragAndDrop.OnEndGUI();
    	}

        private bool IsNecessaryToDrawIcons(IList iconList, Rect windowRect, Color InitialGUIColor, ref bool dragAndDrop)
    	{
    		if(iconList.Count == 0)
    		{
    			SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect, ZeroIconsWarning);
    			dragAndDrop = true;
    			return false;
    		}
			else if(!(iconList[iconList.Count - 1] is Icon))
            {                
				SelectedWindowUtility.DrawLabelForIcons(InitialGUIColor, windowRect, "This List does not have a base type \"icon\"");
    			dragAndDrop = false;
				return false;
            }

    		dragAndDrop = true;
    		return true;
    	}

        private void DropOperation(Event e, Rect virtualRect)
    	{
    		if (e.type == EventType.DragUpdated || e.type == EventType.DragPerform)
            {
                // Add Prefab
                if(virtualRect.Contains(e.mousePosition))
                {
                    if (DragAndDrop.objectReferences.Length > 0)
    				{
    					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
    				}

                    if (e.type == EventType.DragPerform)
    				{
                        DragAndDrop.AcceptDrag();

						IconAttribute iconAttribute = _iconType.GetAttribute<IconAttribute>();

						foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                        {
							if(DraggedIconType)
							{
								if (draggedObject.GetType() == _iconType)
                        		{
                        		    AddIconCallback(draggedObject);
                        		}
							}
							else
							{
								if(iconAttribute == null)
								{
                        			if (draggedObject is GameObject && 
                        			    PrefabUtility.GetPrefabAssetType(draggedObject as GameObject) != PrefabAssetType.NotAPrefab &&
                        			    AssetDatabase.Contains(draggedObject))
                        			{
                        			    AddIconCallback(draggedObject);
                        			}
								}
								else
								{
									foreach (Type type in iconAttribute.NecessaryTypes)
									{
										if(type == typeof(GameObject))
										{
											if (draggedObject is GameObject && 
                        			    		PrefabUtility.GetPrefabAssetType(draggedObject as GameObject) != PrefabAssetType.NotAPrefab &&
                        			    		AssetDatabase.Contains(draggedObject))
                        					{
                        					    AddIconCallback(draggedObject);
                        					}
										}
										else
										{
											if (type == draggedObject.GetType())
                        					{
                        					    AddIconCallback(draggedObject);
                        					}
										}
									}
								}
							}
						}
                    }
                    e.Use();
                } 
    		}
    	}

        private void DrawIcons(IList iconList, Event e, Rect windowRect)
    	{
			Icon draggingIcon = null;
			if (_dragAndDrop.IsDragging() || _dragAndDrop.IsDragPerform())
        	{
				if(_dragAndDrop.GetData().GetType() == _iconType)
				{
					draggingIcon = (Icon)_dragAndDrop.GetData();
				}      
        	}

    		Rect virtualRect = SelectedWindowUtility.GetVirtualRect(windowRect, iconList.Count, IconWidth, IconHeight);

    		Vector2 windowScrollPos = _windowsScroll;
            windowScrollPos = GUI.BeginScrollView(windowRect, windowScrollPos, virtualRect, false, true);

			Rect dragRect = new Rect(0, 0, 0, 0);

    		int y = (int)virtualRect.yMin;
    		int x = (int)virtualRect.xMin;

    		for (int index = 0; index < iconList.Count; index++)
    		{
                Icon icon = (Icon)iconList[index];
    			Rect iconRect = new Rect(x, y, IconWidth, IconHeight);

    			Color textColor;
    			Color rectColor;
    			
				if(IconColor != null)
				{
					IconColor(icon, out textColor, out rectColor);
				}
				else
				{
					SetIconColor(icon, out textColor, out rectColor);
				}

            	Rect iconRectScrolled = new Rect(iconRect);
            	iconRectScrolled.position -= windowScrollPos;

            	// only visible icons
            	if(iconRectScrolled.Overlaps(windowRect))
            	{
					if(DrawIconRect != null)
					{
						DrawIconRect(e, icon, iconRect, textColor, rectColor);
					}
					else
					{
						Texture2D preview = icon.GetPreviewTexture();

						DrawIcon(e, iconRect, preview, icon.GetName(), textColor, rectColor, IconWidth, false);
					}	

					if(iconRect.Contains(e.mousePosition))
					{
						if(_draggable)
						{
							_dragAndDrop.AddDragObject(icon);

							if (draggingIcon != null)
							{
								bool isAfter;
								SelectedWindowUtility.SetDragRect(e, iconRect, ref dragRect, out isAfter);

								if(_dragAndDrop.IsDragPerform())
            					{
									Move(iconList, GetSelectedIndex(iconList), index, isAfter);
            					}
							}
						}

						HandleSelect(iconList, index, e);
					}				
				}

    			SelectedWindowUtility.SetNextXYIcon(virtualRect, IconWidth, IconHeight, ref y, ref x);
    		}

			if(_draggable)
			{
				if (draggingIcon != null)
				{
					EditorGUI.DrawRect(dragRect, Color.white);
				}
			}

    		_windowsScroll = windowScrollPos;

    		GUI.EndScrollView();
    	}

		public void DrawIcon(Event e, Rect iconRect, Texture2D preview, string name, Color textColor, Color rectColor, 
			int iconWidth, bool drawTextureWithAlphaChannel)
		{
			GUIStyle LabelTextForIcon = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForIcon);

			EditorGUI.DrawRect(iconRect, rectColor);
			GUI.Label(iconRect, new GUIContent("", name));
                
            if(e.type == EventType.Repaint)
            {
                Rect previewRect = new Rect(iconRect.x+2, iconRect.y+2, iconRect.width-4, iconRect.width-4);

				if (preview != null)
            	{						
					if(drawTextureWithAlphaChannel)
					{
						GUI.DrawTexture(previewRect, preview);
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

    	public void HandleSelect(IList iconList, int index, Event e)
    	{
    		switch (e.type)
    		{
    			case EventType.MouseDown:
    			{
    				if(e.button == 0)
    				{										
    					if (e.control)
						{    
							SelectPrototypeAdditive(iconList, index);               
						}
						else if (e.shift)
						{          
							SelectPrototypeRange(iconList, index);                
						}
						else 
						{
							SelectPrototype(iconList, index);
						}

            	    	e.Use();
    				}

            	    break;
    			}
    			case EventType.ContextClick:
    			{
                    if(IconMenuCallback == null)
                    {
                        break;
                    }

                    Icon icon = (Icon)iconList[index];
                    IconMenuCallback(icon).ShowAsContext();

    				e.Use();

            		break;
    			}
    		}
    	}

        private void SetIconColor(Icon icon, out Color textColor, out Color rectColor)
		{
			textColor = EditorColors.Instance.LabelColor;

			if(icon.Selected)
			{
				rectColor = icon.IsRedIcon() ? EditorColors.Instance.redNormal : EditorColors.Instance.ToggleButtonActiveColor;
			}
			else
			{
				rectColor = icon.IsRedIcon() ? EditorColors.Instance.redDark : EditorColors.Instance.ToggleButtonInactiveColor;
			}
		}

		private int GetSelectedIndex(IList elements)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                Icon icon = (Icon)elements[i];
                if(icon.Selected)
                {
                    return i;
                }
            }

            return 0;
        }

		private void Move(IList elements, int sourceIndex, int currentIndex, bool isAfter)
        {
            if (currentIndex >= elements.Count || sourceIndex == currentIndex) 
            {
                return;
            }

			int destIndex = currentIndex;

			if(sourceIndex > currentIndex)
			{
				if(isAfter)
				{
					destIndex += 1;
				}
			}
			else
			{
				if(!isAfter)
				{
					destIndex -= 1;
				}
			}

            destIndex = Mathf.Clamp(destIndex, 0, elements.Count);

            Icon item = (Icon)elements[sourceIndex];
            elements.RemoveAt(sourceIndex);
            elements.Insert(destIndex, item);
        }

		public void SelectPrototype(IList elements, int index)
        {
            if(index < 0 && index >= elements.Count)
            {
                return;
            }

			foreach (Icon localIcon in elements)
            {
                localIcon.Selected = false;
				if(IconSelected != null) 
				{
					IconSelected(localIcon);
				}
            }

            Icon icon = (Icon)elements[index];
            icon.Selected = true;
			if(IconSelected != null) 
			{
				IconSelected(icon);
			}
        }

        public void SelectPrototypeAdditive(IList elements, int index)
        {
            if(index < 0 && index >= elements.Count)
            {
                return;
            }

            Icon icon = (Icon)elements[index];
        
            icon.Selected = !icon.Selected;
			if(IconSelected != null) 
			{
				IconSelected(icon);
			}
        }

        public void SelectPrototypeRange(IList elements, int index)
        {
            if(index < 0 && index >= elements.Count)
            {
                return;
            }

            int rangeMin = index;
            int rangeMax = index;

            for (int i = 0; i < elements.Count; i++)
            {
                Icon icon = (Icon)elements[i];

                if (icon.Selected)
                {
                    rangeMin = Mathf.Min(rangeMin, i);
                    rangeMax = Mathf.Max(rangeMax, i);
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                Icon icon = (Icon)elements[i];

                if (icon.Selected != true)
                {
                    break;
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++) 
            {
                Icon icon = (Icon)elements[i];
                icon.Selected = true;
				if(IconSelected != null) 
				{
					IconSelected(icon);
				}
            }
        }
    }
}
#endif