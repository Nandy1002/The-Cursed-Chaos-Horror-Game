#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.SelectionWindow
{
    public static class SelectionPrototypeUtility
    {
        public static void DeleteSelectedPrototype(Group group)
        {
            group.PrototypeList.RemoveAll(proto => proto.Selected);
        }

        public static void SetSelectedAllPrototypes(Group group, bool select)
        {
            group.PrototypeList.ForEach(proto => proto.Selected = select);
        }
    }
}
#endif