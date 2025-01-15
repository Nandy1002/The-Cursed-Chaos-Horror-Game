#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.Utility;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.TemplatesSystem
{
	public static class AllTemplateTypes 
    {
        public static List<System.Type> TypeList = new List<System.Type>();

        static AllTemplateTypes()
        {
            var types = ModulesUtility.GetAllTypesDerivedFrom<Template>()
                                .Where(
                                    t => t.IsDefined(typeof(TemplateAttribute), false)
                                      && !t.IsAbstract
                                ); 

            foreach (var type in types)
            {
                TypeList.Add(type);
            }
        }
    }
}
#endif