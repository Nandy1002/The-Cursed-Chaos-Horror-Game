#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.PreferencesSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem.ScatterAlgorithms;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.ScatterSettingsSystem.ScatterAlgorithms
{
    [SettingsEditor(typeof(RandomPoint))]
    public class RandomPointEditor : ReorderableListComponentEditor
    {
        private RandomPoint _randomPoint;

        public override void OnEnable()
        {
            _randomPoint = (RandomPoint)Target;
        }

        public override void OnGUI(Rect rect, int index) 
        {
            GUIStyle alignmentStyleRight = new GUIStyle(UnityEngine.GUI.skin.label);
            alignmentStyleRight.alignment = TextAnchor.MiddleRight;
            alignmentStyleRight.stretchWidth = true;
            GUIStyle alignmentStyleLeft = new GUIStyle(UnityEngine.GUI.skin.label);
            alignmentStyleLeft.alignment = TextAnchor.MiddleLeft;
            alignmentStyleLeft.stretchWidth = true;

            float minimumTmp = _randomPoint.MinChecks;
            float maximumTmp = _randomPoint.MaxChecks;

			EditorGUI.MinMaxSlider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Checks"), ref minimumTmp, ref maximumTmp, 1, AdvancedSettings.Instance.EditorSettings.maxChecks);

            _randomPoint.MinChecks = (int)minimumTmp;
            _randomPoint.MaxChecks = (int)maximumTmp;

            rect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), new GUIContent(""));
            Rect numFieldRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, (rect.width - EditorGUIUtility.labelWidth) * 0.2f, EditorGUIUtility.singleLineHeight);
            GUIContent minContent = new GUIContent("");

            EditorGUI.LabelField(numFieldRect, minContent, alignmentStyleLeft);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

            _randomPoint.MinChecks = EditorGUI.IntField(numFieldRect, _randomPoint.MinChecks);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);
            
            EditorGUI.LabelField(numFieldRect, " ");
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

            _randomPoint.MaxChecks = EditorGUI.IntField(numFieldRect, _randomPoint.MaxChecks);
            numFieldRect = new Rect(numFieldRect.x + numFieldRect.width, rect.y, numFieldRect.width, EditorGUIUtility.singleLineHeight);

            GUIContent maxContent = new GUIContent("");
            EditorGUI.LabelField(numFieldRect, maxContent, alignmentStyleRight);

            rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif