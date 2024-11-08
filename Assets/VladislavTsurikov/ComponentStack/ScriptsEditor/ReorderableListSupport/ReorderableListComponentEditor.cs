#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport
{
    public abstract class ReorderableListComponentEditor : ComponentEditor
    {
        public virtual void OnGUI(Rect rect, int index) {}
        public virtual float GetElementHeight(int index) => EditorGUIUtility.singleLineHeight * 2;
    }
}
#endif