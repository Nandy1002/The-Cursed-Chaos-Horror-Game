using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility
{
    public static class GetRandomPrototype 
    {
        public static Prototype GetMaxSuccessProto(List<Prototype> protoList)
        {
            if(protoList.Count == 0)
            {
                Debug.Log("You have not selected more than one prototype.");
                return null;
            }

            if(protoList.Count == 1)
            {
                return protoList[0];
            }

            List<float> successProto = new List<float>();

            foreach (Prototype proto in protoList)
            {
                SuccessComponent successComponent = (SuccessComponent)proto.GetSettings(typeof(SuccessComponent));

                if(proto.Active == false)
                {
                    successProto.Add(-1);
                    continue;
                }

                float randomSuccess = Random.Range(0.0f, 1.0f);

                if(randomSuccess > successComponent.SuccessValue / 100)
                {
                    randomSuccess = successComponent.SuccessValue / 100;
                }

                successProto.Add(randomSuccess);
            }

            float maxSuccessProto = successProto.Max();
            int maxIndexProto = 0;

            maxIndexProto = successProto.IndexOf(maxSuccessProto);
            return protoList[maxIndexProto];
        }

        public static PlacedObjectPrototype GetRandomSelectedPrototype(Group group)
        {
            List<Prototype> protoList = group.GetAllSelectedPrototypes();
            
            return (PlacedObjectPrototype)protoList[Random.Range(0, protoList.Count)];
        }
    }
}