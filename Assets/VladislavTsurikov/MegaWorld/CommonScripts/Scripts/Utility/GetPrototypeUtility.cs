using UnityEngine;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility
{
    public static class GetPrototypeUtility 
    {
        public static T GetPrototype<T>(Object obj)
            where T : Prototype
        {
            foreach (Group group in AllAvailableGroups.GetAllGroups())
            {
                if(group.PrototypeType != typeof(T))
                    continue;
                
                foreach (Prototype proto in group.PrototypeList)
                {
                    if(proto.IsSameNecessaryData(obj))
                    {
                        return (T)proto;
                    }
                }
            }
            
            return null;
        }

        public static T GetPrototype<T>(int ID)
            where T: Prototype
        {
            foreach (Group group in AllAvailableGroups.GetAllGroups())
            {
                if(group.PrototypeType != typeof(T))
                    continue;
                
                foreach (Prototype proto in group.PrototypeList)
                {
                    if(proto.GetID() == ID)
                    {
                        return (T)proto;
                    }
                }
            }
            
            return null;
        }
    }
}