using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.Utility;

namespace VladislavTsurikov.ComponentStack.Scripts
{
    public static class AllComponentTypes<T>
    {
        public static readonly List<System.Type> TypeList;

        static AllComponentTypes()
        {
            var types = ModulesUtility.GetAllTypesDerivedFrom<T>()
                .Where(
                    t => t.IsDefined(typeof(NameAttribute), false)
                         && !t.IsAbstract
                );

            TypeList = types.ToList();
        }
    }
}