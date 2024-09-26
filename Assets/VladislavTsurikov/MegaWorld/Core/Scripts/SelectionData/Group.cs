using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.OdinSerializer.Unity_Integration.SerializedUnityObjects;
#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.SettingsSystem;
#endif

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData
{
    public class Group : SerializedScriptableObject, Icon
    {
        [OdinSerialize] private bool _selected = false;
        [OdinSerialize] private Type _prototypeType;
        
        public bool Selected
        {
            get => _selected;
            set => _selected = value;
        }
        
        public Type PrototypeType => _prototypeType;
        [OdinSerialize] public List<Prototype> PrototypeList = new List<Prototype>();
        [OdinSerialize] public BaseComponentStack ComponentStack = new BaseComponentStack();
        [NonSerialized] public Dictionary<Scene, GameObject> ContainerForGameObjects = new Dictionary<Scene, GameObject>();
        public int RandomSeed = 0;
        public bool GenerateRandomSeed = false;
        
        public string RenamingName = "Group";
        public bool Renaming = false;
        
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

        public void OnEnable() 
        {
            AllAvailableGroups.AddGroup(this);
        }

        public void Init(Type prototypeType)
        {
            _prototypeType = prototypeType;
        }
        
        public Prototype AddMissingPrototype(UnityEngine.Object obj)
        {
            return AddMissingPrototype(GeneratePrototypeIfNecessary(obj, PrototypeType));
        }
        
        public Prototype AddMissingPrototype(Prototype proto)
        {
            if (proto == null || proto.GetNecessaryData() == null)
            {
                return null;
            }

            if (GetPrototype(proto.GetID()) != null)
            {
                return proto;
            }
            
            PrototypeList.Add(proto);

            return proto;
        }
        
        public Prototype GeneratePrototypeIfNecessary(UnityEngine.Object obj, Type prototypeType)
        {
            Prototype prototype = GetPrototype(obj);
            
            int id = obj.GetInstanceID();

            if (prototype == null)
            {
                prototype = (Prototype)Activator.CreateInstance(prototypeType);
                prototype.InternalInit(id, obj);
                PrototypeList.Add(prototype);

#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }

            return prototype;
        }

        public Prototype GetPrototype(UnityEngine.Object obj)
        {
            foreach (Prototype proto in PrototypeList)
            {
                if(proto.GetNecessaryData() == obj)
                {
                    return proto;
                }
            }

            return null;
        }
        
        public Prototype GetPrototype(int ID)
        {
            foreach (var proto in PrototypeList)
            {
                if (proto.GetID() == ID) return proto;
            }

            return null;
        }
        
        public List<Prototype> GetAllSelectedPrototypes()
        {
            List<Prototype> protoList = new List<Prototype>();

            foreach (var proto in PrototypeList)
            {
                if(proto.Selected)
                {
                    protoList.Add(proto);
                }
            }

            return protoList;
        }

        public BaseComponent GetSettings(Type settingsType)
        {
            return ComponentStack.GetGeneralComponent(settingsType);
        }

        public BaseComponent GetSettings(Type toolType, Type settingsType)
        {
            return ComponentStack.GetComponent(settingsType, toolType);
        }

        public void ChangeRandomSeed() 
        {
            UnityEngine.Random.InitState(UnityEngine.Random.Range(0, int.MaxValue));
        }

        public void Save() 
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        public Texture2D GetPreviewTexture()
		{
			return null;
		}

        public string GetName()
		{
			return name;
		}
        
#if UNITY_EDITOR
        public string GetPrototypeTypeName()
        {
            return PrototypeType.GetAttribute<NameAttribute>().Name.Split('/').Last();
        }
#endif
        
        public bool IsRedIcon()
        {
            return false;
        }
    }
}