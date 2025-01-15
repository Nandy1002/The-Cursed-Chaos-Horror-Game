using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Core.Scripts;
#if UNITY_EDITOR

#endif

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.BrushSettings
{
    [Serializable]
    public class CustomMasks
    {
        static readonly StringBuilder Builder = new StringBuilder();
    	public List<Texture2D> Masks = new List<Texture2D>();
        public int selectedCustomBrush = 0;

        public void GetBuiltinBrushes()
    	{    
#if UNITY_EDITOR
			GetBuiltinTextures(UnityEditor.Experimental.EditorResources.brushesPath, "builtin_brush_");
    		selectedCustomBrush = Mathf.Min(Masks.Count - 1, selectedCustomBrush);
#endif
        }

		public void GetPolarisBrushes()
    	{    
			Masks = new List<Texture2D>(Resources.LoadAll<Texture2D>(MegaWorldPath.PolarisBrushes));
    		selectedCustomBrush = Mathf.Min(Masks.Count - 1, selectedCustomBrush);
        }

    	private void GetBuiltinTextures(string path, string prefix)
    	{
#if UNITY_EDITOR
    		Texture2D texture;
    		int index = 1;
    		do // begin from ../Brush_1 to ../Brush_n until there is a file not found
    		{
    			// Build file path
    			Builder.Length = 0;
    			Builder.Append(path);
    			Builder.Append(prefix);
    			Builder.Append(index);
    			Builder.Append(".png");

    			// Increase index for next texture
    			index++;

    			// Add texture to list
    			texture = (Texture2D)EditorGUIUtility.Load(Builder.ToString());
    			if (texture != null)
				{
					Masks.Add(texture);
				}
    				
    		} while (texture != null);
#endif
    	}

        public Texture2D GetSelectedBrush()
        {
			if(Masks.Count == 0)
			{
				return null;
			}

			if(selectedCustomBrush > Masks.Count - 1)
			{
				return null;
			}
			
            return Masks[selectedCustomBrush];
        }
    }
}

