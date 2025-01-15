#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.Utility;

namespace VladislavTsurikov.ComponentStack.ScriptsEditor
{
    public static class AllSettingsEditorTypes<T> where T: Scripts.Component
    {
        public static Dictionary<Type, Type> Types = new Dictionary<Type, Type>(); // SettingsType => EditorType

        static AllSettingsEditorTypes()
        {
            var types = ModulesUtility.GetAllTypesDerivedFrom<T>()
                .Where(
                    t => t.IsDefined(typeof(NameAttribute), false)
                         && !t.IsAbstract
                );

            var editorTypes = ModulesUtility.GetAllTypesDerivedFrom<ComponentEditor>()
                .Where(
                    t => t.IsDefined(typeof(SettingsEditorAttribute), false)
                         && !t.IsAbstract
                );

            foreach (var type in editorTypes)
            {
                var attribute = type.GetAttribute<SettingsEditorAttribute>();

                if (types.Contains(attribute.SettingsType))
                {
                    if (!Types.Keys.Contains(attribute.SettingsType))
                    {
                        Types.Add(attribute.SettingsType, type);
                    }
                }
            }
        }

        public static List<Type> GetTypes(List<Type> removeTypes)
        {
            List<Type> types = new List<Type>();
            
            foreach (var type in Types)
            {
                if(!removeTypes.Contains(type.Key))
                    types.Add(type.Key);
            }

            return types;
        }
    }
}
#endif