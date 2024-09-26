using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.Utility;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes
{
    [Serializable]
    [Name("Unity Integration/GameObject")]
    [ZeroIconsWarning("Drag & Drop Prefabs Here")]
    public class PrototypeGameObject : PlacedObjectPrototype
    {
        public override bool IsSameNecessaryData(Object obj)
        {
            GameObject go = (GameObject)obj;

            return GameObjectUtility.IsSameGameObject(go, Prefab);
        }
    }
}