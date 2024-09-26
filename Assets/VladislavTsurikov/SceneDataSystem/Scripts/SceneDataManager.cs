using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.BVH.Scripts.Math;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using VladislavTsurikov.OdinSerializer.Unity_Integration.SerializedUnityObjects;
using VladislavTsurikov.SceneDataSystem.Scripts.Attributes;
using GameObjectUtility = VladislavTsurikov.Utility.GameObjectUtility;

namespace VladislavTsurikov.SceneDataSystem.Scripts
{
    [ExecuteInEditMode]
    public class SceneDataManager : SerializedMonoBehaviour
    {
        [OdinSerialize] private AABB _streamingCell;

        [OdinSerialize] public List<SceneData> SceneDataList = new List<SceneData>();
        
        [NonSerialized] private bool _isSetup;

        public AABB StreamingCell 
        {
            get
            {                
                if(!_streamingCell.IsValid)
                {
                    List<UnityEngine.Object> terains = GameObjectUtility.FindObjectsOfType(typeof(Terrain), GetScene(), true);

                    Bounds currentBounds = new Bounds();

                    for (int i = 0; i < terains.Count; i++)
                    {
                        Terrain terrain = (Terrain)terains[i];

                        Bounds currentTerrainBounds = new Bounds(terrain.terrainData.bounds.center + terrain.transform.position, terrain.terrainData.bounds.size);

                        if(i == 0)
                        {
                            currentBounds = currentTerrainBounds;
                        }
                        else
                        {
                            currentBounds.Encapsulate(currentTerrainBounds);
                        }
                    }

                    _streamingCell = new AABB(currentBounds);

                    return _streamingCell;
                }

                return _streamingCell;
            }
            set
            {
                _streamingCell = value;
                FindScene.StreamingCells.ChangeNodeSize(this, _streamingCell);
            }
        }

        private void OnEnable()
        {
            if(!gameObject.scene.isLoaded)
            {
                return;
            }

            SceneDataManagerStack.AddSceneDataManager(this);
        }

        private void OnDisable() 
        {
            SceneDataManagerStack.RemoveSceneDataManager(gameObject.scene);
            _isSetup = false;
            
            if(!gameObject.scene.isLoaded)
            {
                return;
            }

            foreach (var item in SceneDataList)
            {
                item.InternalOnDisable(); 
            }
        }
        
        private void LateUpdate()
        {
            if (!_isSetup)
            {
                if(gameObject.scene.isLoaded)
                    SceneDataManagerStack.AddSceneDataManager(this);
            }

            foreach (var item in SceneDataList)
            {
                item?.LateUpdate();
            }
        }

        public void Setup()
        {
            _isSetup = true;
            
            SceneDataList.RemoveAll(obj => obj == null);
            
            if(!gameObject.scene.isLoaded)
            {
                return;
            }

            foreach (SceneData sceneData in SceneDataList)
            {
                sceneData.InternalSetup(this);    
            }
        }

        public Scene GetScene()
        {
            return gameObject.scene;
        }

        public SceneData AddSceneDataIfNecessary(Type type)
        {
            AllowInstanceAttribute allowInstanceAttribute = type.GetAttribute<AllowInstanceAttribute>();

            if(allowInstanceAttribute == null || allowInstanceAttribute.Allow(this))
            {
                SceneData findSceneData = GetSceneData(type);

                if(findSceneData != null)
                {
                    return findSceneData;
                }

                SceneData sceneData = (SceneData)Activator.CreateInstance(type); 
                
                SceneDataList.Add(sceneData);
                sceneData.InternalSetup(this);

                return sceneData;
            }

            return null;
        }

        public void RemoveSceneData(Type type)
        {
            SceneData sceneData = GetSceneData(type);
            SceneDataList.Remove(sceneData);
        }

        public SceneData GetSceneData(Type type)
        {
            foreach (SceneData sceneData in SceneDataList)
            {
                if(sceneData.GetType() == type)
                {
                    return sceneData;
                }
            }

            return null;
        }

        public static SceneData InstanceSceneData(Type type, Scene scene)
        {
            if (!scene.isLoaded)
                return null;
            
            SceneDataManager sceneDataManager = SceneDataManagerStack.InstanceSceneDataManager(scene);

            return sceneDataManager.AddSceneDataIfNecessary(type);
        }
    }
}