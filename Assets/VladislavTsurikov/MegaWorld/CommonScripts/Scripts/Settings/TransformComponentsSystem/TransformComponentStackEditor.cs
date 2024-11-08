#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem
{
    public class TransformComponentStackEditor : ReorderableListComponentStackEditor<TransformComponent, ReorderableListComponentEditor>
    {
        private bool _useSimpleComponent;
        
        private List<Type> _removeTransformTypes = new List<Type>();

        public TransformComponentStackEditor(GUIContent reorderableListName, TransformComponentStack stack, List<Type> removeTransformTypes, bool useSimpleComponent) : base(reorderableListName, stack)
        {
            _removeTransformTypes = removeTransformTypes;
            _useSimpleComponent = useSimpleComponent;
        }

        public TransformComponentStackEditor(GUIContent reorderableListName, TransformComponentStack stack, bool useSimpleComponent) : base(reorderableListName, stack)
        {
            _useSimpleComponent = useSimpleComponent;
        }

        public TransformComponentStackEditor(GUIContent reorderableListName, TransformComponentStack stack) : base(reorderableListName, stack)
        {
            _useSimpleComponent = false;
        }

        protected override void ShowAddManu()
        {
            GenericMenu menu = new GenericMenu();

            foreach (var type in AllSettingsEditorTypes<TransformComponent>.GetTypes(_removeTransformTypes))
            {
                string context = type.GetAttribute<NameAttribute>().Name;

                if (_useSimpleComponent)
                {
                    if(type.GetAttribute<SimpleComponentAttribute>() == null)
                        continue;
                }
                else
                {
                    if(type.GetAttribute<SimpleComponentAttribute>() != null)
                    {
                        context = "Simple/" + context;
                    }
                    else
                    {
                        context = "Advanced/" + context;
                    }
                }

                bool exists = Stack.HasComponent(type);

                if (!exists)
                {
                    menu.AddItem(new GUIContent(context), false, () => Stack.CreateIfMissing(type));
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent(context));
                }
            }

            menu.ShowAsContext();
        }
    }
}
#endif