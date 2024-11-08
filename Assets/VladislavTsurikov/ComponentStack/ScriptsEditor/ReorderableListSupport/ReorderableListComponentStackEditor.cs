#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem;
using Component = VladislavTsurikov.ComponentStack.Scripts.Component;

namespace VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport
{
    public class ReorderableListComponentStackEditor<T, N> : ComponentStackEditor<T, N>
        where T: Component
        where N: ReorderableListComponentEditor
    {
        private static class Styles
        {
            public static readonly Texture2D Move;
            
            static Styles()
            {
                Move = Resources.Load<Texture2D>("Images/move");
            }
        }

        private bool _headerFoldout = true;
        private ReorderableList _reorderableList;
        private GUIContent _reorderableListName;
        private bool _dragging;
        private ComponentStack<T> _stack;
        
        protected bool CreateIfMissing = true;
        public bool ShowActiveToggle = true;

        public ReorderableListComponentStackEditor(GUIContent reorderableListName, ComponentStack<T> stack, object[] args = null) : base(stack, args)
        {
            _stack = stack;
            _reorderableListName = reorderableListName;
            _reorderableList = new ReorderableList(stack.ComponentList, typeof(TransformComponent), true, false, true, true );

            SetupCallbacks();
        }
        
        public ReorderableListComponentStackEditor(GUIContent reorderableListName, ComponentStack<T> stack, bool displayAddButton, bool displayRemoveButton, object[] args = null) : base(stack, args)
        {
            _stack = stack;
            _reorderableListName = reorderableListName;
            _reorderableList = new ReorderableList(stack.ComponentList, typeof(TransformComponent), true, false, displayAddButton, displayRemoveButton);

            SetupCallbacks();
        }

        protected virtual void ShowAddManu()
        {
            GenericMenu menu = new GenericMenu();

            foreach (var type in AllSettingsEditorTypes<T>.Types)
            {
                Type settingsType = type.Key;
                
                string context = settingsType.GetAttribute<NameAttribute>().Name;

                if (CreateIfMissing)
                {
                    bool exists = Stack.HasComponent(settingsType);

                    if (!exists)
                    {
                        menu.AddItem(new GUIContent(context), false, () => Stack.CreateIfMissing(settingsType));
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent(context));
                    }
                }
                else
                {
                    menu.AddItem(new GUIContent(context), false, () => Stack.Create(settingsType));
                }
                
            }

            menu.ShowAsContext();
        }

        private void SetupCallbacks()
        {
            _reorderableList.drawElementCallback = DrawElementCB;
            _reorderableList.elementHeightCallback = ElementHeightCB;
            _reorderableList.onAddCallback = AddCB;
            _reorderableList.onRemoveCallback = RemoveFilter;
            _reorderableList.onChangedCallback = OnChangedCallback;
        }

        private void OnChangedCallback(ReorderableList list)
        {
            _stack.IsDirty = true;
        }

        protected override void OnGUI()
		{
            Rect rect = EditorGUILayout.GetControlRect(true, _reorderableList.GetHeight());
            rect = EditorGUI.IndentedRect(rect);
                
            _reorderableList.DoList(rect);
        }
        
        protected override void OnGUI(Rect rect)
        {
            _reorderableList.DoList(rect);
        }

        void RemoveFilter(ReorderableList list)
        {
            Stack.Remove(list.index);
        }

        private void DrawHeaderCB(Rect rect)
        {
            if(CustomEditorGUILayout.IsInspector == false)
            {
                rect.x -= 15;
            }

            _headerFoldout = EditorGUI.Foldout(rect, _headerFoldout, _reorderableListName.text, true);
        }

        private float ElementHeightCB(int index)
        {
            N componentEditor = Editors[index];

            float height = EditorGUIUtility.singleLineHeight * 1.5f;

            if(componentEditor == null)
            {
                return EditorGUIUtility.singleLineHeight * 2;
            }

            if(!componentEditor.SelectSettingsFoldout)
            {
                return EditorGUIUtility.singleLineHeight + 5;
            }
            else
            {
                height += componentEditor.GetElementHeight(index);
                return height;
            }
        }

        private void DrawElementCB(Rect totalRect, int index, bool isActive, bool isFocused)
        {
            float dividerSize = 1f;
            float paddingV = 6f;
            float paddingH = 4f;
            float iconSize = 14f;

            bool isSelected = _reorderableList.index == index;

            Color bgColor;

            if(EditorGUIUtility.isProSkin)
            {
                if(isSelected)
                {
                    ColorUtility.TryParseHtmlString("#424242", out bgColor);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#383838", out bgColor);
                }
            }
            else
            {
                if(isSelected)
                {
                    ColorUtility.TryParseHtmlString("#b4b4b4", out bgColor);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#c2c2c2", out bgColor);
                }
            }

            Color dividerColor;

            if(isSelected)
            {
                dividerColor = EditorColors.Instance.ToggleButtonActiveColor;
            }
            else
            {
                if(EditorGUIUtility.isProSkin)
                {
                    ColorUtility.TryParseHtmlString("#202020", out dividerColor);
                }
                else
                {
                    ColorUtility.TryParseHtmlString("#a8a8a8", out dividerColor);
                }
            }

            Color prevColor = GUI.color;

            // modify total rect so it hides the builtin list UI
            totalRect.xMin -= 20f;
            totalRect.xMax += 4f;
            
            bool containsMouse = totalRect.Contains(Event.current.mousePosition);

            // modify currently selected element if mouse down in this elements GUI rect
            if(containsMouse && Event.current.type == EventType.MouseDown)
            {
                _reorderableList.index = index;
            }

            // draw list element separator
            Rect separatorRect = totalRect;
            // separatorRect.height = dividerSize;
            GUI.color = dividerColor;
            GUI.DrawTexture(separatorRect, Texture2D.whiteTexture, ScaleMode.StretchToFill);
            GUI.color = prevColor;

            // Draw BG texture to hide ReorderableList highlight
            totalRect.yMin += dividerSize;
            totalRect.xMin += dividerSize;
            totalRect.xMax -= dividerSize;
            totalRect.yMax -= dividerSize;
            
            GUI.color = bgColor;
            GUI.DrawTexture(totalRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, false);

            GUI.color = new Color(.7f, .7f, .7f, 1f);

            N componentEditor = Editors[index];
            
            if(componentEditor == null)
            {
                return;
            }

            Rect moveRect = new Rect(totalRect.xMin + paddingH, totalRect.yMin + paddingV, iconSize, iconSize );

            // draw move handle rect
            EditorGUIUtility.AddCursorRect(moveRect, MouseCursor.Pan);
            GUI.DrawTexture(moveRect, Styles.Move, ScaleMode.StretchToFill);

            Rect headerRect = totalRect;
            
            headerRect.x += 15;
            headerRect.height = EditorGUIUtility.singleLineHeight * 1.3f;
            
            GUI.color = new Color(1f, 1f, 1f, 1f);

            DefaultAttribute defaultAttribute = (DefaultAttribute)Stack.ComponentList[index].GetType().GetAttribute(typeof(DefaultAttribute));

            if (ShowActiveToggle && defaultAttribute == null)
            {
                bool temporaryActive = componentEditor.Target.Active;
                
                componentEditor.SelectSettingsFoldout = CustomEditorGUI.HeaderWithMenu(headerRect,
                    componentEditor.Target.GetType().GetAttribute<NameAttribute>().Name,
                    componentEditor.SelectSettingsFoldout, ref temporaryActive, () => Menu(Stack.ComponentList[index], index));

                componentEditor.Target.Active = temporaryActive;
            }
            else
            {
                componentEditor.SelectSettingsFoldout = CustomEditorGUI.HeaderWithMenu(headerRect,
                    componentEditor.Target.GetType().GetAttribute<NameAttribute>().Name,
                    componentEditor.SelectSettingsFoldout, () => Menu(Stack.ComponentList[index], index));
            }

            // update dragging state
            if(containsMouse && isSelected)
            {
                if (Event.current.type == EventType.MouseDrag && !_dragging && isFocused)
                {
                    _dragging = true;
                    _reorderableList.index = index;
                }
            }

            if(_dragging)
            {
                if(Event.current.type == EventType.MouseUp)
                {
                    _dragging = false;
                }
            }

            using( new EditorGUI.DisabledScope(!Stack.ComponentList[index].Active))
            {
                float rectX = 35;

                totalRect.x += rectX;
                totalRect.y += EditorGUIUtility.singleLineHeight + 3;
                totalRect.width -= rectX + 15;
                totalRect.height = EditorGUIUtility.singleLineHeight;

                GUI.color = prevColor;

                if(componentEditor.SelectSettingsFoldout)
                {
                    componentEditor.OnGUI(totalRect, index);
                }
            }

            GUI.color = prevColor;
        }

        public float GetElementStackHeight()
        {
            float height = 0;
            
            for (int i = 0; i < Editors.Count; i++)
            {
                height += Editors[i].GetElementHeight(i);
            }

            return height;
        }

        private void AddCB(ReorderableList list)
        {
            ShowAddManu();
        }
        
        private void Menu(T component, int index)
        {
            DefaultAttribute defaultAttribute = (DefaultAttribute)component.GetType().GetAttribute(typeof(DefaultAttribute));
            
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Reset"), false, () => Reset(component, index));
            
            if(defaultAttribute == null)
                menu.AddItem(new GUIContent("Remove"), false, () => _stack.Remove(index));
            
            menu.ShowAsContext();
        }
    }
}
#endif