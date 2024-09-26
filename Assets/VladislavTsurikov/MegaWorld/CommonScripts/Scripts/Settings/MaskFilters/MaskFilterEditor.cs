#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters
{
    public abstract class MaskFilterEditor : ReorderableListComponentEditor
    {
        public virtual string GetAdditionalName()
        {
            return "";
        }
    }
}
#endif