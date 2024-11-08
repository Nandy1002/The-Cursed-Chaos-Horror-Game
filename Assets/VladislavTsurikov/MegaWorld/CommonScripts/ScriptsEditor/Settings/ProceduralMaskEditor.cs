#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.CPUNoise;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings
{
    [Serializable]
    public class ProceduralMaskEditor
    {
	    private ProceduralMask _target;
	    
        public bool proceduralMaskFoldout = true;
        public bool additionalNoiseSettingsFoldout = true;
        public bool proceduralBrushPreviewTextureFoldout = true;

        public ProceduralMaskEditor(ProceduralMask target)
        {
	        _target = target;
        }

        public void OnGUI()
        {
	        proceduralMaskFoldout = CustomEditorGUILayout.Foldout(proceduralMaskFoldout, "Procedural Mask");

	        if(proceduralMaskFoldout)
	        {
		        EditorGUI.indentLevel++;

		        EditorGUI.BeginChangeCheck();

		        proceduralBrushPreviewTextureFoldout = CustomEditorGUILayout.Foldout(proceduralBrushPreviewTextureFoldout, "Preview Texture");

		        if(proceduralBrushPreviewTextureFoldout)
		        {
			        EditorGUI.indentLevel++;

			        Rect textureRect = EditorGUILayout.GetControlRect(GUILayout.Height(200), GUILayout.Width(200), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
			        Rect indentedRect = EditorGUI.IndentedRect(textureRect);
			        Rect finalRect = new Rect(new Vector2(indentedRect.x, indentedRect.y), new Vector2(200, 200));

			        UnityEngine.GUI.DrawTexture(finalRect, _target.Mask);

			        EditorGUI.indentLevel--;
		        }

		        _target.Shape = (Shape)CustomEditorGUILayout.EnumPopup(shape, _target.Shape);
		        _target.Falloff = CustomEditorGUILayout.Slider(brushFalloff, _target.Falloff, 0f, 100f);
		        _target.Strength = CustomEditorGUILayout.Slider(brushStrength, _target.Strength, 0f, 100f);

		        DrawNoiseForProceduralBrush();

		        if (EditorGUI.EndChangeCheck())
		        {
			        _target.CreateProceduralTexture();
		        }

		        EditorGUI.indentLevel--;
	        }
        }

        public void DrawNoiseForProceduralBrush()
		{
			EditorGUI.BeginChangeCheck();

			_target.FractalNoise = CustomEditorGUILayout.Toggle(fractalNoise, _target.FractalNoise);

			if(_target.FractalNoise)
			{
				EditorGUI.indentLevel++;

				_target.NoiseType = (NoiseType)CustomEditorGUILayout.EnumPopup(new GUIContent("Check Fractal Noise"), _target.NoiseType);

				_target.Seed = CustomEditorGUILayout.IntSlider(new GUIContent("Seed"), _target.Seed, 0, 65000);
				_target.Octaves = CustomEditorGUILayout.IntSlider(new GUIContent("Octaves"), _target.Octaves, 1, 12);
				_target.Frequency = CustomEditorGUILayout.Slider(new GUIContent("Frequency"), _target.Frequency, 0f, 0.1f);

				_target.Persistence = CustomEditorGUILayout.Slider(new GUIContent("Persistence"), _target.Persistence, 0f, 1f);
				_target.Lacunarity = CustomEditorGUILayout.Slider(new GUIContent("Lacunarity"), _target.Lacunarity, 1f, 3.5f);

				additionalNoiseSettingsFoldout = CustomEditorGUILayout.Foldout(additionalNoiseSettingsFoldout, "Additional Settings");

				if(additionalNoiseSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					_target.RemapMin = CustomEditorGUILayout.Slider(new GUIContent("Remap Min"), _target.RemapMin, 0f, 1f);
					_target.RemapMax = CustomEditorGUILayout.Slider(new GUIContent("Remap Max"), _target.RemapMax, 0f, 1f);
					
					_target.Invert = CustomEditorGUILayout.Toggle(new GUIContent("Invert"), _target.Invert);

					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}

			if (EditorGUI.EndChangeCheck())
            {
                _target.Fractal = new FractalNoiseCPU(_target.GetNoiseForProceduralBrush(), _target.Octaves, _target.Frequency / 7, _target.Lacunarity, _target.Persistence);

				_target.FindNoiseRangeMinMaxForProceduralNoise(150, 150);
			}
		}

		[NonSerialized]
		private GUIContent shape = new GUIContent("Shape", "Allows you to select the geometric shape of the mask.");
		[NonSerialized]
		private GUIContent brushFalloff = new GUIContent("Brush Falloff (%)", "Allows you to control the brush fall by creating a gradient.");
		[NonSerialized]
		private GUIContent brushStrength = new GUIContent("Brush Strength (%)", "Allows you to change the maximum strength of the brush the lower this parameter, the closer the value.");
		[NonSerialized]
		private GUIContent fractalNoise = new GUIContent("Fractal Noise", "Mathematical algorithm for generating a procedural texture by a pseudo-random method.");
    }
}
#endif
