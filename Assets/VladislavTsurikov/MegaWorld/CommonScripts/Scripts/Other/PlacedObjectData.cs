#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other
{
    public class PlacedObjectData
    {
        public PlacedObjectPrototype Proto;
        public PlacedObject PlacedObject;

        public PlacedObjectData(PlacedObjectPrototype proto, PlacedObject placedObject)
        {
            Proto = proto; 
            PlacedObject = placedObject;
        }
    }
}
#endif