using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.Utility;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData
{
    public static class AllPrototypeTypes
    {
        public static readonly List<System.Type> TypeList;

        static AllPrototypeTypes()
        {
            var types = ModulesUtility.GetAllTypesDerivedFrom<Prototype>()
                .Where(
                    t => t.IsDefined(typeof(NameAttribute), false)
                         && !t.IsAbstract
                );

            TypeList = types.ToList();
        }
    }
}