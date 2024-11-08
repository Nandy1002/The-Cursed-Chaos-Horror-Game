#if UNITY_EDITOR
using UnityEditor;

namespace VladislavTsurikov.Scripts
{
    public class EditorBase : Editor
    {
        public override bool RequiresConstantRepaint() { return true; }
    }
}
#endif