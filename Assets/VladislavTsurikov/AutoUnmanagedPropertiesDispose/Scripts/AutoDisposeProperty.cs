using UnityEngine;

namespace VladislavTsurikov.AutoUnmanagedPropertiesDispose.Scripts
{
    public abstract class AutoDisposeProperty
    {
        public bool isDispose = false;
        
        public AutoDisposeProperty()
        {
            isDispose = false;
            //AutoDisposeManager.Instance.AddUnmanagedProperties(this);
        }

        public void DisposeUnmanagedMemory()
        {
            isDispose = true;
            
            InternalDisposeUnmanagedMemory();
            //AutoDisposeManager.Instance.RemoveAutoDisposeProperty(this);
        }

        // public void InternalUnmanagedProperty(T property)
        // {
        //     isDispose = false;
        //     ChangeNativeArray(property);
        // }
        //
        // protected virtual void ChangeNativeArray(T property) { }
        
        protected abstract void InternalDisposeUnmanagedMemory();
    }
}