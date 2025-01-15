#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Scripts;

namespace VladislavTsurikov.ComponentStack.ScriptsEditor
{
    public class ComponentEditor
    {
        public Component Target { get; private set; }
        public bool SelectSettingsFoldout = true;

        internal void InternalInit(Component target, object[] args = null)
        {
            Target = target;
            Init(args);
            OnEnable();
        }
        
        public virtual void Init(object[] args){}
        public virtual void OnGUI(){}
        public virtual void OnEnable(){}
        public virtual void OnDisable(){}
    }
}
#endif