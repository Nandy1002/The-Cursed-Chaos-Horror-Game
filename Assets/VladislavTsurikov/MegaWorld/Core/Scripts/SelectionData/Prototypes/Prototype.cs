using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.SettingsSystem;
#endif

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes
{
    [Serializable]
    public abstract class Prototype : Icon
    {
        [OdinSerialize]
        protected int _id;

        [SerializeField]
        private bool _selected;
        
        public bool Active = true;

        public bool Selected
        {
            get => _selected;
            set => _selected = value;
        }
        
        [OdinSerialize] public BaseComponentStack ComponentStack = new BaseComponentStack();

#if UNITY_EDITOR
        [NonSerialized] private BaseComponentStackEditor _componentStackEditor;
        public BaseComponentStackEditor ComponentStackEditor
        {
            get
            {
                if (_componentStackEditor == null)
                    _componentStackEditor = new BaseComponentStackEditor(ComponentStack);

                return _componentStackEditor;
            }
        }
#endif

        public void InternalInit(int id, Object obj)
        {
            _id = id;
            Init(obj);
            
            CreateMegaWorldSettings.CreatePrototypeSettings(this);
        }

        public BaseComponent GetSettings(Type settingsType)
        {
            return ComponentStack.GetComponent(settingsType);
        }

        public BaseComponent GetSettings(Type toolSystem, Type settingsType)
        {
            return ComponentStack.GetComponent(settingsType, toolSystem);
        }

        public bool IsRedIcon()
        {
            return !Active;
        }

        public abstract void Init(Object obj);
        public abstract Object GetNecessaryData();
        public abstract bool IsSameNecessaryData(Object obj);
        public abstract string GetName();
        
#if UNITY_EDITOR
        public abstract Texture2D GetPreviewTexture();
#endif

        public virtual int GetID()
        {
            return _id;
        }
    }
}
