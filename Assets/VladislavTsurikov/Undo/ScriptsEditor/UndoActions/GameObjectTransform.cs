#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.Undo.ScriptsEditor.UndoActions
{
    public class TransformData
    {
        public GameObject GameObject;
        public InstanceData InstanceData;

        public TransformData(GameObject gameObject)
        {
            GameObject = gameObject;
            InstanceData = new InstanceData(gameObject);
        }

        public void SetTransform()
        {
            GameObject.transform.position = InstanceData.Position;
            GameObject.transform.localScale = InstanceData.Scale; 
            GameObject.transform.rotation = InstanceData.Rotation;
        }
    }

    public class GameObjectTransform : UndoRecord
    {
        private List<TransformData> _transformList = new List<TransformData>();

        public GameObjectTransform(GameObject gameObject) 
        {
            GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(gameObject);
            
            _transformList.Add(new TransformData(prefabRoot));
        }

        public override void Merge(UndoRecord record)
        {
            if (record is GameObjectTransform)
            {
                GameObjectTransform gameObjectUndo = (GameObjectTransform)record;
                _transformList.AddRange(gameObjectUndo._transformList);
            }
        }

        public override void Undo()
        {
            foreach (TransformData transformUndoData in _transformList)
            {
                transformUndoData?.SetTransform();
            }
        }
    }
}
#endif