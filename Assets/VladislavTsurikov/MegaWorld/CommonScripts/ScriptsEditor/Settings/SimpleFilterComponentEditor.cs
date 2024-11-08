#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.CPUNoise;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings
{
	[SettingsEditor(typeof(SimpleFilterComponent))]
    public class SimpleFilterComponentEditor : ComponentEditor
    {
	    public bool AdditionalNoiseSettingsFoldout = true;
	    
        private SimpleFilterComponent _simpleFilterComponent;
        
        public override void OnEnable()
        {
            _simpleFilterComponent = (SimpleFilterComponent)Target;
        }
        
        public override void OnGUI()
        {
	        DrawCheckHeight(_simpleFilterComponent);
	        DrawCheckSlope(_simpleFilterComponent);

	        if(_simpleFilterComponent.UseFalloff)
	        {
		        DrawCheckFractalNoise(_simpleFilterComponent);
	        }
        }

		public void DrawCheckHeight(SimpleFilterComponent filterComponent)
		{
			filterComponent.CheckHeight = CustomEditorGUILayout.Toggle(checkHeight, filterComponent.CheckHeight);

			EditorGUI.indentLevel++;

			if(filterComponent.CheckHeight)
			{
				filterComponent.MinHeight = CustomEditorGUILayout.FloatField(new GUIContent("Min Height"), filterComponent.MinHeight);
				filterComponent.MaxHeight = CustomEditorGUILayout.FloatField(new GUIContent("Max Height"), filterComponent.MaxHeight);

				DrawHeightFalloff(filterComponent);
			}

			EditorGUI.indentLevel--;
		}

		public void DrawHeightFalloff(SimpleFilterComponent filterComponent)
		{
			if(!filterComponent.UseFalloff)
			{
				return;
			}

			filterComponent.HeightFalloffType = (FalloffType)CustomEditorGUILayout.EnumPopup(heightFalloffType, filterComponent.HeightFalloffType);

			if(filterComponent.HeightFalloffType != FalloffType.None)
			{
				filterComponent.HeightFalloffMinMax = CustomEditorGUILayout.Toggle(heightFalloffMinMax, filterComponent.HeightFalloffMinMax);
			
				if(filterComponent.HeightFalloffMinMax == true)
				{
					filterComponent.MinAddHeightFalloff = CustomEditorGUILayout.FloatField(minAddHeightFalloff, filterComponent.MinAddHeightFalloff);
					filterComponent.MaxAddHeightFalloff = CustomEditorGUILayout.FloatField(maxAddHeightFalloff, filterComponent.MaxAddHeightFalloff);
				}
				else
				{
					filterComponent.AddHeightFalloff = CustomEditorGUILayout.FloatField(addHeightFalloff, filterComponent.AddHeightFalloff);
				}
			}
		}

		public void DrawSlopeFalloff(SimpleFilterComponent filterComponent)
		{
			if(!filterComponent.UseFalloff)
			{
				return;
			}
			
			filterComponent.SlopeFalloffType = (FalloffType)CustomEditorGUILayout.EnumPopup(slopeFalloffType, filterComponent.SlopeFalloffType);

			if(filterComponent.SlopeFalloffType != FalloffType.None)
			{
				filterComponent.SlopeFalloffMinMax = CustomEditorGUILayout.Toggle(slopeFalloffMinMax, filterComponent.SlopeFalloffMinMax);

				if(filterComponent.SlopeFalloffMinMax)
				{
					filterComponent.MinAddSlopeFalloff = CustomEditorGUILayout.FloatField(minAddSlopeFalloff, filterComponent.MinAddSlopeFalloff);
					filterComponent.MaxAddSlopeFalloff = CustomEditorGUILayout.FloatField(maxAddSlopeFalloff, filterComponent.MaxAddSlopeFalloff);
				}
				else
				{
					filterComponent.AddSlopeFalloff = CustomEditorGUILayout.FloatField(addSlopeFalloff, filterComponent.AddSlopeFalloff);
				}
			}
		}

		void DrawCheckSlope(SimpleFilterComponent filterComponent)
		{
			filterComponent.CheckSlope = CustomEditorGUILayout.Toggle(checkSlope, filterComponent.CheckSlope);

			EditorGUI.indentLevel++;

			if(filterComponent.CheckSlope)
			{
				CustomEditorGUILayout.MinMaxSlider(slope, ref filterComponent.MinSlope, ref filterComponent.MaxSlope, 0f, 90);
				
				DrawSlopeFalloff(filterComponent);
			}

			EditorGUI.indentLevel--;
		}

		public void DrawCheckFractalNoise(SimpleFilterComponent filterComponent)
		{
			EditorGUI.BeginChangeCheck();

			int width = 150;
			int height = 150;

			filterComponent.CheckGlobalFractalNoise = CustomEditorGUILayout.Toggle(new GUIContent("Check Global Fractal Noise"), filterComponent.CheckGlobalFractalNoise);
			
			if(filterComponent.CheckGlobalFractalNoise)
			{
				EditorGUI.indentLevel++;

				filterComponent.NoisePreviewTexture = CustomEditorGUILayout.Foldout(filterComponent.NoisePreviewTexture, "Noise Preview Texture");

				GUILayout.BeginHorizontal();
				{
					if(filterComponent.NoisePreviewTexture )
					{
						EditorGUI.indentLevel++;

						GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());

						Rect textureRect = GUILayoutUtility.GetRect(250, 250, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
						UnityEngine.GUI.DrawTexture(textureRect, filterComponent.NoiseTexture);

						EditorGUI.indentLevel--;
					}
				}
				GUILayout.EndHorizontal();

				filterComponent.Fractal.NoiseType = (NoiseType)CustomEditorGUILayout.EnumPopup(new GUIContent("Noise Type"), filterComponent.Fractal.NoiseType);

				filterComponent.Fractal.Seed = CustomEditorGUILayout.IntSlider(seed, filterComponent.Fractal.Seed, 0, 65000);
				filterComponent.Fractal.Octaves = CustomEditorGUILayout.IntSlider(octaves, filterComponent.Fractal.Octaves, 1, 12);
				filterComponent.Fractal.Frequency = CustomEditorGUILayout.Slider(frequency, filterComponent.Fractal.Frequency, 0f, 0.01f);

				filterComponent.Fractal.Persistence = CustomEditorGUILayout.Slider(persistence, filterComponent.Fractal.Persistence, 0f, 1f);
				filterComponent.Fractal.Lacunarity = CustomEditorGUILayout.Slider(lacunarity, filterComponent.Fractal.Lacunarity, 1f, 3.5f);

				AdditionalNoiseSettingsFoldout = CustomEditorGUILayout.Foldout(AdditionalNoiseSettingsFoldout, "Additional Settings");

				if(AdditionalNoiseSettingsFoldout)
				{
					EditorGUI.indentLevel++;

					filterComponent.RemapNoiseMin = CustomEditorGUILayout.Slider(remapNoiseMin, filterComponent.RemapNoiseMin, 0f, 1f);
					filterComponent.RemapNoiseMax = CustomEditorGUILayout.Slider(remapNoiseMax, filterComponent.RemapNoiseMax, 0f, 1f);

					filterComponent.Invert = CustomEditorGUILayout.Toggle(invert, filterComponent.Invert);

					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}

			if (EditorGUI.EndChangeCheck())
            {		
				if(filterComponent.NoisePreviewTexture)
				{
					FractalNoiseCPU fractal = new FractalNoiseCPU(filterComponent.Fractal.GetNoise(), filterComponent.Fractal.Octaves, filterComponent.Fractal.Frequency / 7, filterComponent.Fractal.Lacunarity, filterComponent.Fractal.Persistence);
					filterComponent.NoiseTexture = new Texture2D(width, height);

                	float[,] arr = new float[width, height];

                	for(int y = 0; y < height; y++)
                	{
                	    for (int x = 0; x < width; x++)
                	    { 
							arr[x,y] = fractal.Sample2D(x, y);
                	    }
                	}

					NoiseUtility.NormalizeArray(arr, width, height, ref filterComponent.RangeMin, ref filterComponent.RangeMax);
	
                	for (int y = 0; y < height; y++)
                	{
                	    for (int x = 0; x < width; x++)
                	    {
                	        float fractalValue = arr[x, y];
							
							if (filterComponent.Invert == true)
                   			{
                   			    fractalValue = 1 - fractalValue;
                   			}

							if (fractalValue < filterComponent.RemapNoiseMin) 
                			{
                			    fractalValue = 0;
                			}
                			else if(fractalValue > filterComponent.RemapNoiseMax)
                			{
                			    fractalValue = 1;
                			}
							else
							{
								fractalValue = Mathf.InverseLerp(filterComponent.RemapNoiseMin, filterComponent.RemapNoiseMax, fractalValue);
							}

                	        filterComponent.NoiseTexture.SetPixel(x, y, new Color(fractalValue, fractalValue, fractalValue, 1));
                	    }
                	}

                	filterComponent.NoiseTexture.Apply();
				}
				else
				{
					FindNoiseRangeMinMax(filterComponent, width, height);
				}	
			}
		}

		private void FindNoiseRangeMinMax(SimpleFilterComponent filterComponent, int width, int height)
		{
			FractalNoiseCPU fractal = new FractalNoiseCPU(filterComponent.Fractal.GetNoise(), filterComponent.Fractal.Octaves, filterComponent.Fractal.Frequency / 7, filterComponent.Fractal.Lacunarity, filterComponent.Fractal.Persistence);
			filterComponent.NoiseTexture = new Texture2D(150, 150);

            float[,] arr = new float[width, height];

            for(int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                { 
					arr[x,y] = fractal.Sample2D(x, y);
                }
            }

			NoiseUtility.NormalizeArray(arr, width, height, ref filterComponent.RangeMin, ref filterComponent.RangeMax);
		}

		private GUIContent checkHeight = new GUIContent("Check Height");
		private GUIContent heightFalloffType = new GUIContent("Height Falloff Type");
		private GUIContent heightFalloffMinMax = new GUIContent("Height Falloff Min Max");
		private GUIContent minAddHeightFalloff = new GUIContent("Min Add Height Falloff");
		private GUIContent maxAddHeightFalloff = new GUIContent("Max Add Height Falloff");
		private GUIContent addHeightFalloff = new GUIContent("Add Height Falloff");

		private GUIContent seed = new GUIContent("Seed");
		private GUIContent octaves = new GUIContent("Octaves");
		private GUIContent frequency = new GUIContent("Frequency");
		private GUIContent persistence = new GUIContent("Persistence");
		private GUIContent lacunarity = new GUIContent("Lacunarity");
		private GUIContent remapNoiseMin = new GUIContent("Remap Noise Min");
		private GUIContent remapNoiseMax = new GUIContent("Remap Noise Max");
		private GUIContent invert = new GUIContent("Invert");

		private GUIContent checkSlope = new GUIContent("Check Slope");
		private GUIContent slope = new GUIContent("Slope");	

		private GUIContent slopeFalloffType = new GUIContent("Slope Falloff Type");
		private GUIContent slopeFalloffMinMax = new GUIContent("Slope Falloff Min Max");
		private GUIContent minAddSlopeFalloff = new GUIContent("Min Add Slope Falloff");
		private GUIContent maxAddSlopeFalloff = new GUIContent("Max Add Slope Falloff");
		private GUIContent addSlopeFalloff = new GUIContent("Add Slope Falloff");
    }
}
#endif