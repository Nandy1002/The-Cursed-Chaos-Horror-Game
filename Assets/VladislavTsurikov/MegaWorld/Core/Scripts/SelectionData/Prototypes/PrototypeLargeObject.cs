using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.Utility;
using Object = UnityEngine.Object;
#if INSTANT_RENDERER
using VladislavTsurikov.InstantRenderer.Core.Scripts.PrototypeRendererSystem.SelectionData;
using VladislavTsurikov.InstantRenderer.LargeObjectRenderer.Scripts;
using VladislavTsurikov.InstantRenderer.LargeObjectRenderer.Scripts.API;
#endif

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes
{
    [Serializable]
    [Name("Large Object")]
    [ZeroIconsWarning("Drag & Drop Prefabs Here")]
    public class PrototypeLargeObject : PlacedObjectPrototype
    {
#if INSTANT_RENDERER
        public InstantRenderer.LargeObjectRenderer.Scripts.PrototypeLargeObject RendererPrototype => 
            (InstantRenderer.LargeObjectRenderer.Scripts.PrototypeLargeObject)LargeObjectRendererAPI.AddMissingPrototype(Prefab);

        public override int GetID()
        {
            InstantRenderer.LargeObjectRenderer.Scripts.PrototypeLargeObject rendererPrototype = 
                (InstantRenderer.LargeObjectRenderer.Scripts.PrototypeLargeObject)AllPrototypeStack.Instance.GetPrototype(Prefab, typeof(LargeObjectRenderer));
            
            if (rendererPrototype == null)
                return _id;
            
            return rendererPrototype.ID;
        }
#endif
        
        public override bool IsSameNecessaryData(Object obj)
        {
            GameObject go = (GameObject)obj;

            return GameObjectUtility.IsSameGameObject(go, Prefab);
        }
    }
}