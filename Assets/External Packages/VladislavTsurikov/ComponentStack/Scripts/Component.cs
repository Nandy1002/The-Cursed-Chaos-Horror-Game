using System;
using System.Linq;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.ComponentStack.Scripts
{
    [Serializable]
    public abstract class Component : ITab
    {        
        [OdinSerialize]
        private bool _selected = false;
        
        [OdinSerialize]
        private bool _active = true;
        
        public bool Selected
        {
            get => _selected;
            set => _selected = value;
        }
        
        public bool Active
        {
            get => _active;
            set
            {
                if (_active != value)
                {
                    _active = value;
                    OnChangeActive();
                }
            }
        }
        
        [NonSerialized]
        public bool IsInit = false;

        internal void InternalInit(object[] args = null)
        {
            IsInit = false;
            Init(args);
            IsInit = true;
        }
        
        public virtual void Init(object[] args = null){}
        public virtual void OnCreate(){}
        public virtual void OnDelete(){}
        public virtual void OnChangeActive(){}
        public virtual void OnDisable(){}
        
        public string GetName()
        {
            return GetType().GetAttribute<NameAttribute>().Name.Split('/').Last();
        }

        public void SetName(string newName)
        {
        }
    }
}