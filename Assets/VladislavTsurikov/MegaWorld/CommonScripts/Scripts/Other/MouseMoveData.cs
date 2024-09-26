using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.ColliderSystem.Scripts.Utility;
using VladislavTsurikov.Extensions.Scripts;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other
{
    public class MouseMoveData
    {
        public RayHit Raycast;
        public RayHit PrevRaycast;
        public float DragDistance;
        public Vector3 StrokeDirection;
        public Vector3 StrokeDirectionRefPoint;

        public void DragMouse(float spacing, Func<Vector3, bool> func)
        {
            Vector3 hitPoint = Raycast.Point;
            Vector3 lastHitPoint = PrevRaycast.Point;

            if(!CommonUtility.IsSameVector(hitPoint, lastHitPoint))
            { 
                Vector3 moveVector = (hitPoint - lastHitPoint);
                Vector3 moveDirection = moveVector.normalized;
                float moveLenght = moveVector.magnitude;

                StrokeDirection = (hitPoint - StrokeDirectionRefPoint).normalized;

                if (DragDistance + moveLenght >= spacing)
                {
                    float d = spacing - DragDistance;
                    Vector3 dragPoint = lastHitPoint + moveDirection * d;
                    DragDistance = 0;
                    moveLenght -= d;

                    func.Invoke(dragPoint);
                    StrokeDirectionRefPoint = Raycast.Point;

                    while (moveLenght >= spacing)
                    {
                        moveLenght -= spacing;
                        dragPoint += moveDirection * spacing;

                        func.Invoke(dragPoint);
                        StrokeDirectionRefPoint = Raycast.Point;
                    }
                }

                DragDistance += moveLenght;
            }
            
            PrevRaycast = Raycast;
        }

#if UNITY_EDITOR
        public bool UpdatePosition()
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            foreach (Group type in WindowDataPackage.Instance.SelectedVariables.SelectedGroupList)
            {
                LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;

                Raycast = RaycastUtility.Raycast(ray, layerSettings.GetCurrentPaintLayers(type.PrototypeType));

                if(Raycast != null)
                {
                    return true;
                }
            }

            return false;
        }

        public bool UpdatePosition(GameObject ignoreGameObjectRaycast)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;

            ObjectFilter objectRaycastFilter = new ObjectFilter();

            if(ignoreGameObjectRaycast != null)
            {
                List<object> gameObjects = new List<object>(ignoreGameObjectRaycast.GetAllChildrenAndSelf());
                objectRaycastFilter.SetIgnoreObjects(gameObjects);
            }
            
            objectRaycastFilter.LayerMask = layerSettings.PaintLayers;
            Raycast = ColliderUtility.Raycast(ray, objectRaycastFilter);

            if(Raycast != null)
            {
                return true;
            }

            return false;
        }
#endif
    }
}