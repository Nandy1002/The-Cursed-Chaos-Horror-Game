#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.TransformComponentsSystem.Components
{
    [SettingsEditor(typeof(CliffsAlign))]
    public class CliffsAlignEditor : ReorderableListComponentEditor
    {
        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif