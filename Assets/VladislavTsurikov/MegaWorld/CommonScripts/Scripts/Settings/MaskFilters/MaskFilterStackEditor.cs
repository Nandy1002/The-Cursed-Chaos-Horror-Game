#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor.ReorderableListSupport;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters
{
    public class MaskFilterStackEditor : ReorderableListComponentStackEditor<MaskFilter, MaskFilterEditor>
    {
        public MaskFilterStackEditor(GUIContent label, MaskFilterStack filterStack) : base(label, filterStack)
        {
            CreateIfMissing = false;
        }
    }
}
#endif