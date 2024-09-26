#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.UnityIntegration;
using VladislavTsurikov.Undo.ScriptsEditor.UndoActions;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.CallDelegate
{
    [InitializeOnLoad]
    public static class СallUndoDelegate 
    {
        static СallUndoDelegate()
        {
            DestroyedGameObject.UndoPerformed -= DestroyedGameObjectUndoPerformed;
            DestroyedGameObject.UndoPerformed += DestroyedGameObjectUndoPerformed;
        }

        private static void DestroyedGameObjectUndoPerformed(List<GameObject> gameObjectList)
        {
            foreach (GameObject go in gameObjectList)
            {
                GameObjectCollider.RegisterGameObjectToCurrentScene(go);
            }
        }
    }
}
#endif