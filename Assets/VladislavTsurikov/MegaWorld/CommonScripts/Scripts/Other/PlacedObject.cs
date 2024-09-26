using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other
{
    [Serializable]
    public class PlacedObject
    {
        public GameObject GameObject;
        public Bounds Bounds;
        public Scene Scene;

        public PlacedObject(GameObject gameObject, Bounds bounds, Scene scene)
        {
            GameObject = gameObject;
            Bounds = bounds;
            Scene = scene;
        }

        public void CopyTransform(GameObject gameObject)
        {
            GameObject.transform.position = gameObject.transform.position;
            GameObject.transform.rotation = gameObject.transform.rotation;
            GameObject.transform.localScale = gameObject.transform.localScale;
        }
    }
}
    