using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes
{
    [Serializable]
    [Name("Unity Integration/Terrain Texture")]
    [ZeroIconsWarning("Drag & Drop Texture or Terrain Layers Here")]
    [ComponentStack.Scripts.Icon(new[]{typeof(Texture2D), typeof(TerrainLayer)})]
    public class PrototypeTerrainTexture : Prototype
    {
	    public TerrainTextureSettings TerrainTextureSettings = new TerrainTextureSettings();
        public TerrainLayer TerrainLayer;

        public override string GetName()
        {
	        if (TerrainTextureSettings.DiffuseTexture != null)
            {
				return TerrainTextureSettings.DiffuseTexture.name;
            }

	        return "Missing Texture";
        }
        
	    public override void Init(Object obj)
	    {
		    if (obj is Texture2D)
		    {
			    TerrainTextureSettings = new TerrainTextureSettings((Texture2D)obj);
		    }
		    else
		    {
			    TerrainLayer = (TerrainLayer)obj;
			    TerrainTextureSettings = new TerrainTextureSettings(TerrainLayer);
		    }
	    }
	    
#if UNITY_EDITOR
	    public override Texture2D GetPreviewTexture()
	    {
		    if (TerrainTextureSettings.DiffuseTexture != null)
		    {
			    return TerrainTextureSettings.DiffuseTexture;  
		    }

		    return null;
	    }
#endif
	    
	    public override bool IsSameNecessaryData(Object obj)
	    {
		    return GetNecessaryData() == obj;
	    }

	    public override Object GetNecessaryData()
	    {
		    if (TerrainLayer == null)
			    return TerrainTextureSettings.DiffuseTexture;
		    
            return TerrainLayer;
        }
    }
}