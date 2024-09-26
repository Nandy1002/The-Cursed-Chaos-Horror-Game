using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using GUIUtility = VladislavTsurikov.Utility.GUIUtility;
#endif

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes
{
    public enum PrefabType
    {
        Mesh = 0,
        Texture = 1
    }
    
    [Serializable]
    [Name("Unity Integration/Terrain Detail")]
    [ZeroIconsWarning("Drag & Drop Prefabs or Textures Here")]
    [ComponentStack.Scripts.Icon(new[]{typeof(Texture2D), typeof(GameObject)})]
    public class PrototypeTerrainDetail : Prototype
    {
        public int TerrainProtoId;

        public PrefabType PrefabType = PrefabType.Mesh;
        public Texture2D DetailTexture;
        public GameObject Prefab;
        
        public override string GetName()
        {
	        if(PrefabType == PrefabType.Mesh)
	        {
		        if (Prefab != null)
                {
			    	return Prefab.name;
                }

		        return "Missing Prefab";
	        }

	        if (DetailTexture != null)
	        {
		        return DetailTexture.name;
	        }

	        return "Missing Texture";
        }


	    public override void Init(Object obj)
	    {
		    if (obj is GameObject)
		    {
			    PrefabType = PrefabType.Mesh;
            
			    Prefab = (GameObject)obj;
		    }
		    else
		    {
			    PrefabType = PrefabType.Texture;

			    DetailTexture = (Texture2D)obj;
		    }
	    }
	    
	    public override bool IsSameNecessaryData(Object obj)
	    {
		    return GetNecessaryData() == obj;
	    }
	    
	    public override Object GetNecessaryData()
        {
	        if(PrefabType == PrefabType.Texture)
            {
                return DetailTexture;
            }

	        return Prefab;
        }
	    
#if UNITY_EDITOR
	    public override Texture2D GetPreviewTexture()
	    {
		    if (Prefab != null)
		    {
			    return GUIUtility.GetPrefabPreviewTexture(Prefab);      
		    }

		    return DetailTexture;
	    }
#endif
    }
}