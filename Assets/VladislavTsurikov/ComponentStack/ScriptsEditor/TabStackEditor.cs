#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using GUIUtility = VladislavTsurikov.Utility.GUIUtility;

namespace VladislavTsurikov.ComponentStack.ScriptsEditor
{
    public class TabStackEditor
    {
        static string s_tabTextFieldName = "TabNameTextField";
        static int s_TabsWindowHash = "TabsWindow".GetHashCode();

        private InternalDragAndDrop _dragAndDrop = new InternalDragAndDrop();

        private IList _elements;

        private bool _draggable;
        private bool _renameFunctionEnable;
        
        private int _renameIndexTab = -1; 
        private bool _onBeginTabRename = false;

        //This variable is required because function "EditorGUILayout.GetControl" returns an invalid Rect, which causes an anomaly in the interface
        private Rect _unityRectFix;
        
        public bool TabWidthFromName = true;

        public int TabWidth = 130;
		public int TabHeight = 25;
        public int TabPlusWidth = 50;
        public int OffsetTabWidth = 30;
        
        public delegate void AddCallbackDelegate();
        public delegate GenericMenu AddTabMenuCallbackDelegate(int currentTabIndex);
        public delegate void SelectCallbackDelegate(int currentTabIndex);
        public delegate void HappenedMoveCallbackDelegate();
        public delegate void TabSelectedDelegate(ITab tab);
        public delegate void TabDisabledDelegate(ITab tab);
        public delegate void SetTabColorDelegate(ITab tab, out Color barColor, out Color labelColor);
        
        public AddCallbackDelegate AddCallback;
        public AddTabMenuCallbackDelegate AddTabMenuCallback;
        public SelectCallbackDelegate SelectCallback;
        public HappenedMoveCallbackDelegate HappenedMoveCallback;
		public TabSelectedDelegate TabSelected;
        public TabDisabledDelegate TabDisabled;
        public SetTabColorDelegate IconColor;

        public TabStackEditor(IList elements, bool draggable, bool renameFunctionEnableFunction)
        {
            _elements = elements;
            _draggable = draggable;

            _renameFunctionEnable = renameFunctionEnableFunction;
        }

        public void OnGUI()
        {
            int initialIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            int tabCount = _elements.Count;
            if(tabCount != 0)
            {
                if(!(_elements[_elements.Count - 1] is ITab))
                {
                    EditorGUI.indentLevel = initialIndentLevel;

                    CustomEditorGUILayout.Label("This List does not have a base type \"Tab\"");
                    return;
                }
            }

            _dragAndDrop.OnBeginGUI();

            Event e = Event.current;

			Color InitialGUIColor = GUI.color;

            if(AddCallback != null)
            {
                tabCount += 1;
            }

            float windowWidth = EditorGUIUtility.currentViewWidth;

			int tabUnderCursor = -1;

            ITab draggingTab = null;
            if (_dragAndDrop.IsDragging() || _dragAndDrop.IsDragPerform())
            {
                if(_dragAndDrop.GetData() is ITab)
				{
					draggingTab = (ITab)_dragAndDrop.GetData();
				}      
            }

            Rect lineRect = EditorGUILayout.GetControlRect(GUILayout.Height(TabHeight));
            lineRect.x += initialIndentLevel * 15;

            Rect dragRect = new Rect(0, 0, 0, 0);

            if(lineRect.y == 0 && _unityRectFix != null)
            {
                lineRect = _unityRectFix;
            }
            else
            {
                _unityRectFix = lineRect;
            }
            
            Rect tabRect = new Rect(lineRect.x, lineRect.y, TabWidth, TabHeight);
            int tabWindowControlID = UnityEngine.GUIUtility.GetControlID(s_TabsWindowHash, FocusType.Passive);

            for (int tabIndex = 0; tabIndex < tabCount; tabIndex++)
            {
                if(AddCallback != null)
                {
                    if(tabIndex == tabCount - 1)
                    {
                        Rect tabPlusRect = tabRect;
                        tabPlusRect.width = TabPlusWidth;

                        float inspectorOffset = CustomEditorGUILayout.IsInspector ? 15 : 0;
                        if(tabPlusRect.width + tabPlusRect.x + inspectorOffset > windowWidth)
                        {
                            EditorGUILayout.GetControlRect(GUILayout.Height(TabHeight));

                            tabPlusRect.x = lineRect.x;
                            tabPlusRect.y += TabHeight;
                        }

                        CustomEditorGUILayout.RectTab(tabPlusRect, "+", ButtonStyle.Add, TabHeight, 28);

                        if(tabPlusRect.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 0)
                        {
                            UnityEngine.GUIUtility.keyboardControl = tabWindowControlID;
                            UnityEngine.GUIUtility.hotControl = 0;
                                
                            AddCallback();
                        }

                        break;
                    }
                }

                ITab tab = (ITab)_elements[tabIndex];

                float localTabWidth = TabWidth;

                if(TabWidthFromName)
                {
                    GUIStyle labelStyle = CustomEditorGUILayout.GetStyle(StyleName.LabelButton);
                    localTabWidth = labelStyle.CalcSize(new GUIContent(tab.GetName())).x + OffsetTabWidth;
                    tabRect.width = localTabWidth;
                }

                float addWidth = CustomEditorGUILayout.IsInspector ? 15 : 0;
                if(tabRect.width + tabRect.x + addWidth > windowWidth)
                {
                    EditorGUILayout.GetControlRect(GUILayout.Height(TabHeight));

                    tabRect.x = lineRect.x;
                    tabRect.y += TabHeight;
                }
                
                if (IconColor != null)
                {
                    Color barColor;
                    Color labelColor;

                    IconColor(tab, out barColor, out labelColor);
                    CustomEditorGUILayout.RectTab(tabRect, tab.GetName(), barColor, labelColor, TabHeight);
                }
                else
                {
                    CustomEditorGUILayout.RectTab(tabRect, tab.GetName(), tab.Selected, TabHeight);
                }

                if (_renameFunctionEnable)
                {
                    // Tab Rename
                    if (tabIndex == _renameIndexTab)
                    {
                        // make TextField and set focus to it
                        if (_onBeginTabRename)
                        {
                            GUI.SetNextControlName(s_tabTextFieldName);

                            GUI.color = EditorColors.Instance.orangeNormal;

                            tab.SetName(SelectedWindowUtility.DelayedTextField(tabRect, tab.GetName()));

                            GUI.color = InitialGUIColor;

                            TextEditor textEditor =
                                (TextEditor)UnityEngine.GUIUtility.GetStateObject(typeof(TextEditor),
                                    UnityEngine.GUIUtility.keyboardControl);
                            if (textEditor != null)
                            {
                                textEditor.SelectAll();
                            }

                            GUI.FocusControl(s_tabTextFieldName);
                            _onBeginTabRename = false;
                        }
                        else
                        {
                            // if TextField still in focus - continue rename 
                            if (GUI.GetNameOfFocusedControl() == s_tabTextFieldName)
                            {
                                GUI.SetNextControlName(s_tabTextFieldName);
                                EditorGUI.BeginChangeCheck();

                                GUI.color = EditorColors.Instance.orangeNormal;

                                string newTabName = SelectedWindowUtility.DelayedTextField(tabRect, tab.GetName());

                                GUI.color = InitialGUIColor;

                                if (EditorGUI.EndChangeCheck())
                                {
                                    tab.SetName(newTabName);
                                }

                                // Unfocus TextField - finish rename
                                if (e.isKey && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.Escape))
                                {
                                    UnityEngine.GUIUtility.keyboardControl = 0;
                                    UnityEngine.GUIUtility.hotControl = 0;
                                    _renameIndexTab = -1;
                                    e.Use();
                                }
                            }

                        }
                    }
                }

                // Tab under cursor
                if(tabRect.Contains(e.mousePosition))
                {
					tabUnderCursor = tabIndex;
					
                    if(_draggable)
                    {
                        _dragAndDrop.AddDragObject(_elements[tabUnderCursor]);

                        if (draggingTab != null) 
                        {
                            bool isAfter;
							SelectedWindowUtility.SetDragRect(e, tabRect, ref dragRect, out isAfter);

					        EditorGUIUtility.AddCursorRect(tabRect, MouseCursor.MoveArrow);

                            if(_dragAndDrop.IsDragPerform())
                            {
                                Move(_elements, GetSelectedIndex(), tabIndex, isAfter);
                            }
                        }
                    }
                }

                tabRect.x += localTabWidth;
            }

            if(_draggable)
			{
				if (draggingTab != null)
				{
                    EditorGUI.DrawRect(dragRect, Color.white);
				}
			}

            switch(e.type)
            {
                case EventType.MouseDown:
                {
                    if(tabUnderCursor != -1 && e.button == 0)
                    {
                        UnityEngine.GUIUtility.keyboardControl = tabWindowControlID;
                        UnityEngine.GUIUtility.hotControl = 0;

                        Select(tabUnderCursor);

                        e.Use();
                    }
                    break;
                }
                case EventType.ContextClick:
				{
                    if(AddTabMenuCallback == null)
                    {
                        break;
                    }

					if(tabUnderCursor != -1)
                	{
                        UnityEngine.GUIUtility.keyboardControl = tabWindowControlID;
                        UnityEngine.GUIUtility.hotControl = 0;
                        
                        ITab tab = (ITab)_elements[tabUnderCursor];

            	    	if(tab.Selected == false)
            	    	{
            	    	    Select(tabUnderCursor);
            	    	} 
						else 
						{
            	    	    GenericMenu menu = AddTabMenuCallback(tabUnderCursor);
                            
                            if(_renameFunctionEnable)
                            {
                                RenameMenu(menu, tabUnderCursor);
                            }

                            menu.ShowAsContext();
            	    	}

						e.Use();
					}
					
            	    break;
				}
			}

            EditorGUI.indentLevel = initialIndentLevel;

            _dragAndDrop.OnEndGUI();
        }

        private void RenameMenu(GenericMenu menu, int currentTabIndex)
        {
            menu.AddItem(new GUIContent("Rename"), false, GUIUtility.ContextMenuCallback, new Action(() => { _onBeginTabRename = true; _renameIndexTab = currentTabIndex; }));
        }

        private void Select(int index)
        {
            if(SelectCallback != null)
            {
                SelectCallback(index);
                return;
            }

            for (int i = 0; i < _elements.Count; i++)
            {
                ITab localTab = (ITab)_elements[i];

                if(TabDisabled != null)
                {
                    if(localTab.Selected)
                    {
                        TabDisabled(localTab);
                    }
                }

                localTab.Selected = false;
            }

            ITab tab = (ITab)_elements[index];

            tab.Selected = true;
            if(TabSelected != null)
            {
                TabSelected(tab);
            }
        }

        private int GetSelectedIndex()
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                ITab tab = (ITab)_elements[i];
                if(tab.Selected)
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

            ITab item = (ITab)elements[sourceIndex];
            elements.RemoveAt(sourceIndex);
            elements.Insert(destIndex, item);

            if(HappenedMoveCallback != null) HappenedMoveCallback();
        }
    }
}
#endif