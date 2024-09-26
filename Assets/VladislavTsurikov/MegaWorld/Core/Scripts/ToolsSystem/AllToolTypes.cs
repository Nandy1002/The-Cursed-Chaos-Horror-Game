using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.Utility;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem
{
    public static class AllToolTypes 
    {
        public static List<System.Type> TypeList = new List<System.Type>();

        static AllToolTypes()
        {
            var toolWindowTypes = ModulesUtility.GetAllTypesDerivedFrom<ToolWindow>()
                                .Where(
                                    t => t.IsDefined(typeof(WindowToolAttribute), false)
                                      && !t.IsAbstract
                                ); 
            
            var toolMonoBehaviourTypes = ModulesUtility.GetAllTypesDerivedFrom<ToolMonoBehaviour>()
                .Where(
                    t => !t.IsAbstract
                ); 
            
            TypeList.AddRange(toolWindowTypes);
            TypeList.AddRange(toolMonoBehaviourTypes);
        }
    }
}