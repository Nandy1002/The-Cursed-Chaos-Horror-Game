#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.Utility;

namespace VladislavTsurikov.ColliderSystem.Scripts.Plugin
{
    [InitializeOnLoad]
    public class ColliderDefines
    {
        private static readonly string DEFINE_COLLIDER = "COLLIDER";

        static ColliderDefines()
        {
            ScriptingDefineSymbolsUtility.SetScriptingDefineSymbols(DEFINE_COLLIDER);
        }
    }
}
#endif