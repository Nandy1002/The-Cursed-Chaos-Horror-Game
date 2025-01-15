#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem.ScatterAlgorithms;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.ScatterSettingsSystem.ScatterAlgorithms
{
	[SettingsEditor(typeof(RandomGrid))]
    public class GridEditor : ReorderableListComponentEditor
    {
	    private RandomGrid _randomGrid;
	    public override void OnEnable()
	    {
		    _randomGrid = (RandomGrid)Target;
	    }

	    public override void OnGUI(Rect rect, int index) 
        {
	        _randomGrid.RandomisationType = (RandomisationType)EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Randomisation Type"), _randomGrid.RandomisationType);
            rect.y += EditorGUIUtility.singleLineHeight;

			if(_randomGrid.RandomisationType == RandomisationType.Sphere)
			{
				_randomGrid.Vastness = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Vastness"), _randomGrid.Vastness, 0f, 1f);
                rect.y += EditorGUIUtility.singleLineHeight;
			}

			float distance = _randomGrid.GridStep.x;

			_randomGrid.UniformGrid = EditorGUI.Toggle(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Uniform Grid"), _randomGrid.UniformGrid);
            rect.y += EditorGUIUtility.singleLineHeight;

			if(_randomGrid.UniformGrid)
			{
				distance = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Distance"), distance, 0.1f, 50f);
                rect.y += EditorGUIUtility.singleLineHeight;
				
                _randomGrid.GridAngle = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Angle"), _randomGrid.GridAngle, 0, 360);
                rect.y += EditorGUIUtility.singleLineHeight;

                _randomGrid.GridStep = Vector2.Max(new Vector2(0.5f, 0.5f), new Vector2(distance, distance));
                rect.y += EditorGUIUtility.singleLineHeight;
			}
			else
			{
				_randomGrid.GridStep = Vector2.Max(new Vector2(0.1f, 0.1f), EditorGUI.Vector2Field(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Step"), _randomGrid.GridStep));
				rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;

                _randomGrid.GridAngle = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Angle"), _randomGrid.GridAngle, 0, 360);
                rect.y += EditorGUIUtility.singleLineHeight;
			}
			
			_randomGrid.FailureRate = EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Failure Rate Value (%)"), _randomGrid.FailureRate, 0f, 100f);
			rect.y += EditorGUIUtility.singleLineHeight;
        }

        public override float GetElementHeight(int index) 
        {
            float height = EditorGUIUtility.singleLineHeight;

            height += EditorGUIUtility.singleLineHeight;

			if(_randomGrid.RandomisationType == RandomisationType.Sphere)
			{
                height += EditorGUIUtility.singleLineHeight;
			}

            height += EditorGUIUtility.singleLineHeight;

			if(_randomGrid.UniformGrid)
			{
				height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
			}
			else
			{
				height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
                height += EditorGUIUtility.singleLineHeight;
			}
			
			height += EditorGUIUtility.singleLineHeight;

            return height;
        }
    }
}
#endif