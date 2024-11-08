#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.ComponentStack.Scripts;
using Component = VladislavTsurikov.ComponentStack.Scripts.Component;

namespace VladislavTsurikov.ComponentStack.ScriptsEditor
{
    public abstract class ComponentStackEditor<T, N> 
        where T: Component
        where N: ComponentEditor
    {
        private object[] _initSettingsEditorArgs = null;
        public ComponentStack<T> Stack {get;}
        public List<N> Editors;
        
        public N SelectedEditor
        {
            get
            {
                return Editors.FirstOrDefault(t => t.Target.Selected);
            }
        }
        
        public ComponentStackEditor(ComponentStack<T> stack, object[] args = null)
        {
            Stack = stack;
            Editors = new List<N>();
            _initSettingsEditorArgs = args;
            
            if(!Stack.IsSetup) 
            {
                Stack.InternalSetup();
            }
            
            RefreshEditors();
        }

        public virtual void RefreshEditors()
        {
            Editors = new List<N>();
            
            foreach (var t in Stack.ComponentList)
            {
                Create(t);
            }
        }
        
        public void Create(T settings, int index = -1)
        {
            var settingsType = settings.GetType();

            if (settingsType.GetAttribute(typeof(DontDrawGUIAttribute)) != null)
            {
                return;
            }

            if (!AllSettingsEditorTypes<T>.Types.ContainsKey(settingsType))
            {
                Debug.LogWarning("Does not contain Editor for type: " + settingsType); 
                return;
            }

            if (AllSettingsEditorTypes<T>.Types.TryGetValue(settingsType, out var editorType))
            {
                var editor = (N)Activator.CreateInstance(editorType);
                editor.InternalInit(settings, _initSettingsEditorArgs);
                
                if (index < 0)
                    Editors.Add(editor);
                else
                    Editors[index] = editor;
            }
        }

        public void InternalOnGUI()
        {
            Stack.RemoveNullComponent();  
            
            if(Stack.IsDirty)
            {
                RefreshEditors();
                Stack.IsDirty = false;
            }

            OnGUI();
        }
        
        public void InternalOnGUI(Rect rect)
        {
            Stack.RemoveNullComponent();  
            
            if(Stack.IsDirty)
            {
                RefreshEditors();
                Stack.IsDirty = false;
            }

            OnGUI(rect);
        }
        
        public N GetSelected()
        {
            foreach (var editor in Editors)
            {
                if(editor.Target.Selected)
                {
                    return editor;
                }
            }

            return null;
        }

        public void OnGUI(Type type)
        {
            for (int i = 0; i < Editors.Count; i++)
            {
                if(Editors[i].Target.GetType() == type)
                {
                    OnGUI(Editors[i], i);
                }
            }
        }

        public void Reset(T component, int index)
        {
            if(component == null)
            {
                return;
            }

            Editors[index].OnDisable();
            Editors[index] = null;

            T newComponent = (T)Activator.CreateInstance(component.GetType());
            newComponent.InternalInit();
            newComponent.OnCreate();
            
            Stack.ComponentList[index] = newComponent;

            Create(newComponent, index);

            OnReset(component, newComponent);
        }
        
        protected virtual void OnGUI(){}
        protected virtual void OnGUI(Rect rect){}
        protected virtual void OnGUI(N editor, int index){}
        public virtual void OnReset(T oldComponent, T newComponent){}
    }
}
#endif