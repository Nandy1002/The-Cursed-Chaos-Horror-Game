using System;
using System.Collections.Generic;

namespace VladislavTsurikov.SceneDataSystem.Scripts
{
    [Serializable]
    public abstract class SceneData
    {
        protected bool _init = false;

        public bool Init => _init;

        public SceneDataManager SceneDataManager;

        public void InternalSetup(SceneDataManager sceneDataManager)
        {
            SceneDataManager = sceneDataManager;

            if(!_init)
            {
                Setup();
            }

            _init = true;
        }

        public virtual void InternalOnDisable()
        {
            _init = false;
            OnDisable();
        }

        public void SetupForce()
        {
            _init = false;
            Setup();
            _init = true;
        }

        protected virtual void Setup(){}
        protected virtual void OnDisable(){}
        public virtual void DrawDebug(){}
        public virtual bool CheckDelete(){return false;}
        public virtual void LateUpdate(){}
        public static void SetupForce(Type sceneDataType)
        {
            foreach (SceneDataManager sceneDataManager in SceneDataManagerStack.GetAllSceneDataManager())
            {
                SceneData sceneData = sceneDataManager.GetSceneData(sceneDataType);
                if(sceneData != null)
                {
                    sceneData.SetupForce();
                }
            }
        }

        public static void OnDisable(Type sceneDataType)
        {
            foreach (SceneDataManager sceneDataManager in SceneDataManagerStack.GetAllSceneDataManager())
            {
                SceneData sceneData = sceneDataManager.GetSceneData(sceneDataType);
                if(sceneData != null)
                {
                    sceneData.OnDisable();
                }
            }
        }

        public static List<T> GetAllSceneData<T>() where T: SceneData
        {
            List<T> sceneDataList = new List<T>();

            foreach (SceneDataManager sceneDataManager in SceneDataManagerStack.GetAllSceneDataManager())
            {
                T cellsOcclusionCulling = (T)sceneDataManager.GetSceneData(typeof(T));

                if(cellsOcclusionCulling != null)
                {
                    sceneDataList.Add(cellsOcclusionCulling);
                }
            }

            return sceneDataList;
        }
    }
}