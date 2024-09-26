using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.OdinSerializer.Utilities.Extensions;

namespace VladislavTsurikov.ComponentStack.Scripts
{
    [Serializable]
    public class ComponentStack<T> where T: Component
    {
        [OdinSerialize]
        public List<T> ComponentList = new List<T>();
        
        [NonSerialized]
        public bool IsDirty = true;
        [NonSerialized]
        public bool IsSetup;
        
        public T SelectedSettings
        {
            get
            {
                return ComponentList.FirstOrDefault(t => t.Selected);
            }
        }

        public virtual void InternalRemoveUnused()
        {
            RemoveNullComponent();
            RemoveUnused();
        }
        
        protected virtual void Setup(){}
        
        public virtual void RemoveUnused(){}
        
        public virtual void OnDisable(){}

        public void InternalSetup()
        {
            IsDirty = true;
            IsSetup = true;

            Setup();
            InternalRemoveUnused();
            CreateNecessaryComponents();
            
            foreach (var setting in ComponentList)
            {
                setting?.InternalInit();
            }
        }

        public void InternalOnDisable()
        {
            InternalRemoveUnused();
            
            foreach (T setting in ComponentList) 
            {
                setting.OnDisable();
            }

            OnDisable();
        }
        
        public void Clear()
        {
            ComponentList.Clear();
            IsDirty = true;
        }
        
        public void CreateNecessaryComponents()
        {
            CreateNecessaryComponentsAttribute attribute = GetType().GetAttribute<CreateNecessaryComponentsAttribute>();
            
            if(attribute != null) 
                CreateIfMissing(attribute.NecessaryTypes.ToArray());
        }

        public T CreateIfMissing(Type type, object[] args = null)
        {
            T settings = GetComponent(type);
            if (settings == null)
            {
                return Create(type, args);
            }

            return settings;
        }
        
        public void CreateIfMissing(Type[] types)
        {
            foreach (Type type in types)
            {
                CreateIfMissing(type);
            }
        }
        
        public void CreateAllAvailableTypes()
        {
            foreach (Type type in AllComponentTypes<T>.TypeList)
            {
                CreateIfMissing(type);
            }
        }
        
        public T Create(Type type, object[] args = null)
        {
            var asset = (T)Activator.CreateInstance(type);
            ComponentList.Add(asset);
            asset.InternalInit(args);
            asset.OnCreate();
            
            if(ComponentList.Count == 1)
            {
                asset.Selected = true;
            }
            
            IsDirty = true;
            
            return asset;
        }
        
        public void Remove(Type type)
        {
            var asset = GetComponent(type);
            ComponentList.Remove(asset);
            
            asset.OnDisable();
            asset.OnDelete();

            IsDirty = true;
        }
        
        public void Remove(int index)
        {
            var asset = ComponentList[index];
            ComponentList.RemoveAt(index);
            
            asset.OnDisable();
            asset.OnDelete();
            
            IsDirty = true;
        }
        
        public void MoveComponent(int index, int offset)
        {
            var prev = ComponentList[index + offset];
            ComponentList[index + offset] = ComponentList[index];
            ComponentList[index] = prev;
            
            IsDirty = true;
        }

        public void SetComponent(T settings)
        {
            for (int i = 0; i < ComponentList.Count; i++)
            {
                if (ComponentList[i].GetType() == settings.GetType())
                {
                    ComponentList[i] = settings;
                    IsDirty = true;
                    return;
                }
            }

            ComponentList.Add(settings);
            IsDirty = true;
        }

        public T GetComponent(Type type)
        {
            foreach (var component in ComponentList)
            {
                if (component != null)
                {
                    if (component.GetType().ToString() == type.ToString())
                    {
                        return component;
                    }
                }
            }

            return null;
        }
        
        public bool HasComponent(Type type)
        {
            return GetComponent(type) != null;
        }
        
        public void Select(Component component)
        {
            ComponentList.ForEach(setting => setting.Selected = false);
            component.Selected = true;
        }
        
        public T GetSelected()
        {
            foreach (T item in ComponentList)
            {
                if(item.Selected)
                {
                    return item;
                }
            }

            return null;
        }
        
        public void RemoveNullComponent()
        {
            bool happenedRemove = false;

            for (int i = ComponentList.Count - 1; i >= 0; i--)
            {
                if(ComponentList[i] == null || ComponentList[i].GetType().IsAbstract)
                {
                    ComponentList.RemoveAt(i);
                    happenedRemove = true;
                }
            }

            if(happenedRemove)
            {
                IsDirty = true;
            }
        }
    }
}