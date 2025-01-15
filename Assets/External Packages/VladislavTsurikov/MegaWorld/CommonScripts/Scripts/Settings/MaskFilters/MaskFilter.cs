using VladislavTsurikov.ComponentStack.Scripts;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters
{
    public abstract class MaskFilter : Component
    {
        public virtual void Eval( MaskFilterContext filterContext, int index) {}
    }
}