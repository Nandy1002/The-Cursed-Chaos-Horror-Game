using System;
using UnityEngine;
using VladislavTsurikov.Utility;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using GUIUtility = VladislavTsurikov.Utility.GUIUtility;
#endif

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes
{
    public abstract class PlacedObjectPrototype : Prototype
    {
        public GameObject Prefab;
        
        [NonSerialized] public PastTransform PastTransform;
        public Vector3 Extents = Vector3.one;
        
        public override string GetName()
        {
            if (Prefab != null)
            {
                return Prefab.name;
            }

            return "Missing Prefab";
        }
        
        public override void Init(Object obj)
        {
            Prefab = (GameObject)obj;
            PastTransform = new PastTransform(Prefab.transform);
            Extents = MeshUtility.CalculateBoundsInstantiate(Prefab).extents;    
        }

        public override Object GetNecessaryData()
        {
            return Prefab;
        }
        
#if UNITY_EDITOR
        public override Texture2D GetPreviewTexture()
        {
            if (Prefab != null)
            {
                return GUIUtility.GetPrefabPreviewTexture(Prefab);  
            }

            return null;
        }
#endif
    }
}