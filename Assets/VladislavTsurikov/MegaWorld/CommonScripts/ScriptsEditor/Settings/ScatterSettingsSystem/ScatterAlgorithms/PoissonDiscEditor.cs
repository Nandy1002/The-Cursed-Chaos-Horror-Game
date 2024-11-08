#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem.ScatterAlgorithms;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.ScatterSettingsSystem.ScatterAlgorithms
{
    [SettingsEditor(typeof(PoissonDisc))]
    public class PoissonDiscEditor : ReorderableListComponentEditor
    {
        private PoissonDisc _poissonDisc;

        public override void OnEnable()
        {
            _poissonDisc = (PoissonDisc)Target;
        }

        public override void OnGUI(Rect rect, int index) 
        {
            _poissonDisc.PoissonDiscSize = EditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Poisson Disc Size"), _poissonDisc.PoissonDiscSize);
            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif