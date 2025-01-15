#if UNITY_EDITOR
#if INSTANT_RENDERER
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.BVH.Scripts.Math;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.ColliderSystem.Scripts.UnityIntegration;
using VladislavTsurikov.ColliderSystem.Scripts.UnityIntegration.Utility;
using VladislavTsurikov.ColliderSystem.Scripts.Utility;
using VladislavTsurikov.Extensions.Scripts;
using VladislavTsurikov.InstantRenderer.LargeObjectRenderer.Scripts.RendererData.ColliderSystem;
using VladislavTsurikov.SceneDataSystem.Scripts;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.ColliderSystem.Scripts.Plugin
{
    /// <summary>
    /// An enum which allows us to switch between different demos. Essentially,
    /// it will be used to control the type of test that we will perform in the
    /// scene view (raycast, overlap etc).
    /// </summary>
    public enum OverlapMode
    {
        BoxOverlap,
        SphereOverlap
    }

    public enum ShowMode
    {
        Object,
        Scene,
    }

    public enum DebugObjectMode
    {
        ShowAllCells,
        ShowHitRaycastCells,
        ShowMeshTree,
        ShowHitRaycast,
        ShowOverlapObjects,
    }

    public enum DebugSceneMode
    {
        ShowAllCells,
        ShowOverlapScenes,
    }

    /// <summary>
    /// A simple demo class which allows the user to perform different tests using the
    /// CFE API and show the results inside the scene view.
    /// </summary>
    [ExecuteInEditMode]
    public class ColliderDebug : MonoSingleton<ColliderDebug>
    {
        public LayerMask LayerMask = new LayerMask();
        public LayerMask OverlapObjectLayerMask = new LayerMask();
        public ShowMode ShowMode = ShowMode.Object;
        public DebugObjectMode DebugObjectMode = DebugObjectMode.ShowAllCells;
        public DebugSceneMode DebugSceneMode = DebugSceneMode.ShowAllCells;

        [SerializeField]
        private OverlapMode _demoMode = OverlapMode.SphereOverlap;

        private Vector3 _overlapBoxCenter;
        [SerializeField]
        private Vector3 _overlapBoxSize = Vector3.one;
        [SerializeField]
        private Vector3 _overlapBoxEuler = Vector3.zero;

        private Vector3 _overlapSphereCenter;
        [SerializeField]
        private float _overlapSphereRadius = 1.0f;

        [SerializeField]
        private Color _overlapShapeColor = Color.blue.WithAlpha(0.5f);
        [SerializeField]
        private Color _overlappedSolidColor = Color.green.WithAlpha(0.5f);
        [SerializeField]
        private Color _overlappedWireColor = Color.black;
        

        [SerializeField]
        private float _hitPointSize = 0.08f;
        [SerializeField]
        private Color _hitNormalColor = Color.green;
        [SerializeField]
        private Color _hitPointColor = Color.green;

        private RayHit _objectRayHit;
        private ColliderObject  _BVHObject;
        private List<ColliderObject > _overlappedObjects = new List<ColliderObject >();
        private List<SceneDataManager> _overlappedScenes = new List<SceneDataManager>();

        public Color HitTriangleColor = Color.red;
        public Color NodeColor = Color.white;
        public Vector3 OverlapBoxSize { get { return _overlapBoxSize; } set { _overlapBoxSize = Vector3.Max(value, Vector3.one * 1e-5f); } }
        public float OverlapSphereRadius { get { return _overlapSphereRadius; } set { _overlapSphereRadius = Mathf.Max(value, 1e-5f); } }
        public float HitPointSize { get { return _hitPointSize; } set { _hitPointSize = Mathf.Max(1e-5f, value); } }

        /// <summary>
        /// Called when the script is enabled.
        /// </summary>
        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        /// <summary>
        /// Called when the script is disabled.
        /// </summary>
        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnDrawGizmosSelected() 
        {
            switch (ShowMode)
            {
                case ShowMode.Object:
                {
                    switch (DebugObjectMode)
                    {
                        case DebugObjectMode.ShowAllCells:
                        {
                            DrawAllCells();

                            break;
                        }
                        case DebugObjectMode.ShowMeshTree:
                        {
                            if(_objectRayHit != null)
                            {
                                if (_objectRayHit.MeshRayHit != null)
                                {
                                    // Retrieve the editor mesh instance based on the Unity mesh which is attached to the hit object
                                    Mesh.Mesh editorMesh = _BVHObject.GetMesh();

                                    editorMesh.DrawAllCells(_BVHObject.GetMatrix(), NodeColor);
                                }
                            }

                            ShowHitRaycast();

                            break;
                        }
                        case DebugObjectMode.ShowHitRaycast:
                        {
                            ShowHitRaycast();

                            break;
                        }
                        case DebugObjectMode.ShowHitRaycastCells:
                        {
                            if(_objectRayHit != null)
                            {
                                if (_objectRayHit.MeshRayHit != null)
                                {
                                    // Retrieve the editor mesh instance based on the Unity mesh which is attached to the hit object
                                    Mesh.Mesh editorMesh = _BVHObject.GetMesh();

                                    editorMesh.DrawRaycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), _BVHObject.GetMatrix(), NodeColor);
                                }
                            }

                            DrawRaycast();

                            ShowHitRaycast();

                            break;
                        }
                        case DebugObjectMode.ShowOverlapObjects:
                        {
                            if (_demoMode == OverlapMode.BoxOverlap)
                            {
                                // Draw the box shape
                                GizmosEx.PushColor(_overlapShapeColor);
                                GizmosEx.PushMatrix(Matrix4x4.TRS(_overlapBoxCenter, Quaternion.Euler(_overlapBoxEuler), _overlapBoxSize));
                                Gizmos.DrawCube(Vector3.zero, Vector3.one);
                                GizmosEx.PopMatrix();
                                GizmosEx.PopColor();

                                // Draw the overlapped volumes
                                DrawOverlappedVolumesGizmos();
                            }
                            else if (_demoMode == OverlapMode.SphereOverlap)
                            {
                                // Draw the sphere shape
                                GizmosEx.PushColor(_overlapShapeColor);
                                GizmosEx.PushMatrix(Matrix4x4.TRS(_overlapSphereCenter, Quaternion.identity, Vector3.one * _overlapSphereRadius));
                                Gizmos.DrawSphere(Vector3.zero, 1.0f);
                                GizmosEx.PopMatrix();
                                GizmosEx.PopColor();

                                // Draw the overlapped volumes
                                DrawOverlappedVolumesGizmos();
                            }
                            break;
                        }
                    }

                    break;
                }
                case ShowMode.Scene:
                {
                    switch (DebugSceneMode)
                    {
                        case DebugSceneMode.ShowAllCells:
                        {
                            //FindScene.StreamingCells.DrawAllCells(NodeColor);
                            FindScene.SceneObjectsCells.DrawAllCells(NodeColor);

                            break;
                        }
                        case DebugSceneMode.ShowOverlapScenes:
                        {
                            if (_demoMode == OverlapMode.BoxOverlap)
                            {
                                // Draw the box shape
                                GizmosEx.PushColor(_overlapShapeColor);
                                GizmosEx.PushMatrix(Matrix4x4.TRS(_overlapBoxCenter, Quaternion.Euler(_overlapBoxEuler), _overlapBoxSize));
                                Gizmos.DrawCube(Vector3.zero, Vector3.one);
                                GizmosEx.PopMatrix();
                                GizmosEx.PopColor();

                                // Draw the overlapped volumes
                                DrawOverlappedScenesVolumesGizmos();
                            }
                            else if (_demoMode == OverlapMode.SphereOverlap)
                            {
                                // Draw the sphere shape
                                GizmosEx.PushColor(_overlapShapeColor);
                                GizmosEx.PushMatrix(Matrix4x4.TRS(_overlapSphereCenter, Quaternion.identity, Vector3.one * _overlapSphereRadius));
                                Gizmos.DrawSphere(Vector3.zero, 1.0f);
                                GizmosEx.PopMatrix();
                                GizmosEx.PopColor();

                                // Draw the overlapped volumes
                                DrawOverlappedScenesVolumesGizmos();
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }

        public void ShowHitRaycast()
        {
            if(_objectRayHit != null)
            {
                // Use a yellow color. Seems to work really well at least with the dev's workspace.
                GUIStyle style = new GUIStyle("label");
                style.normal.textColor = Color.yellow;

                // Build the label text. We will show the coordinates of the hit point and the hit point normal.
                var labelText = "Hit Point: " + _objectRayHit.Point.ToString() + "; \r\nHit Normal: " + _objectRayHit.Normal.ToString();
                Handles.Label(_objectRayHit.Point, new GUIContent(labelText), style);

                // Draw a sphere centered on the position of the hit point and a normal emenating from that point
                GizmosEx.PushColor(_hitPointColor);
                Gizmos.DrawSphere(_objectRayHit.Point, _hitPointSize * HandleUtility.GetHandleSize(_objectRayHit.Point));
                GizmosEx.PopColor();
                GizmosEx.PushColor(_hitNormalColor);
                Gizmos.DrawLine(_objectRayHit.Point, _objectRayHit.Point + _objectRayHit.Normal * HandleUtility.GetHandleSize(_objectRayHit.Point));
                GizmosEx.PopColor();

                if (_objectRayHit.MeshRayHit != null)
                {
                    // Retrieve the editor mesh instance based on the Unity mesh which is attached to the hit object
                    Mesh.Mesh editorMesh = _BVHObject.GetMesh();
                    if (editorMesh != null)
                    {
                        // Push the triangle forward a bit along its own normal so that the triangle
                        // line pixels won't fight with the mesh geometry.
                        Vector3 triangleOffset = _objectRayHit.Normal * 1e-3f;

                        // Activate the triangle color and retrieve the triangle vertices
                        GizmosEx.PushColor(HitTriangleColor);
                        var triangleVerts = editorMesh.GetTriangleVerts(_objectRayHit.MeshRayHit.TriangleIndex, _BVHObject.GetMatrix());
                        for(int vertIndex = 0; vertIndex < 3; ++vertIndex)
                        {
                            // Draw the current line
                            Gizmos.DrawLine(triangleVerts[vertIndex] + triangleOffset, triangleVerts[(vertIndex + 1) % 3] + triangleOffset);
                        }

                        // Restore the color
                        GizmosEx.PopColor();
                    }
                }
            }
        }

        /// <summary>
        /// Event handler for the 'SceneView.onSceneGUIDelegate' event.
        /// </summary>
        /// <remarks>
        /// You could also just have a custom editor (e.g. class MyCustomEditor : Editor {}) 
        /// for your own MonoBehaviour and implement the logic there inside the OnSceneGUI
        /// function. The advantage of using an event handler is that the object does not have
        /// to be selected for the logic to execute.
        /// </remarks>
        private void OnSceneGUI(SceneView sceneView)
        {
            // We only do anything if the current event is a mouse move event
            Event e = Event.current;
            if (e.type == EventType.MouseMove)
            {
                // Reset data
                _objectRayHit = null;
                _BVHObject = null;
                _overlappedObjects.Clear();
                _overlappedScenes.Clear();

                switch (ShowMode)
                {
                    case ShowMode.Object:
                    {
                        switch (DebugObjectMode)
                        {
                            case DebugObjectMode.ShowHitRaycast:
                            case DebugObjectMode.ShowHitRaycastCells:
                            case DebugObjectMode.ShowMeshTree:
                            {
                                ObjectFilter objectFilter = new ObjectFilter();
                                objectFilter.LayerMask = LayerMask;

                                _objectRayHit = ColliderUtility.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition), objectFilter);

                                if(_objectRayHit != null)
                                {
                                    if(_objectRayHit.Object is BVHGameObject)
                                    {
                                        _BVHObject = (BVHGameObject)_objectRayHit.Object;
                                    }
                                    else if(_objectRayHit.Object is LargeObjectCollider)
                                    {
                                        _BVHObject = (LargeObjectCollider)_objectRayHit.Object;
                                    }
                                }

                                break;
                            }
                            case DebugObjectMode.ShowOverlapObjects:
                            {
                                ObjectFilter objectFilter = new ObjectFilter();
                                objectFilter.LayerMask = LayerMask;

                                ObjectFilter overlapObjecFilter = new ObjectFilter();
                                overlapObjecFilter.LayerMask = OverlapObjectLayerMask;

                                if (_demoMode == OverlapMode.BoxOverlap)
                                {
                                    RayHit objectHit = ColliderUtility.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition), objectFilter);
                                    if (objectHit != null)
                                    {
                                        // Perform the overlap test
                                        _overlapBoxCenter = objectHit.Point;
                                        _overlappedObjects = GameObjectColliderUtility.OverlapBox(_overlapBoxCenter, _overlapBoxSize, Quaternion.Euler(_overlapBoxEuler), overlapObjecFilter);
                                    }
                                }
                                else if (_demoMode == OverlapMode.SphereOverlap)
                                {
                                    var objectHit = ColliderUtility.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition), objectFilter);
                                    if (objectHit != null)
                                    {
                                        _overlapSphereCenter = objectHit.Point;
                                        _overlappedObjects = GameObjectColliderUtility.OverlapSphere(_overlapSphereCenter, _overlapSphereRadius, overlapObjecFilter);
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
                    case ShowMode.Scene:
                    {
                        switch (DebugSceneMode)
                        {
                            case DebugSceneMode.ShowOverlapScenes:
                            {
                                ObjectFilter objectFilter = new ObjectFilter();
                                objectFilter.LayerMask = LayerMask;

                                if (_demoMode == OverlapMode.BoxOverlap)
                                {
                                    RayHit objectHit = ColliderUtility.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition), objectFilter);
                                    if(objectHit == null)
                                    {
                                        objectHit = ColliderUtility.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition), LayerMask);
                                    }
                                    if (objectHit != null)
                                    {
                                        // Perform the overlap test
                                        _overlapBoxCenter = objectHit.Point;
                                        _overlappedScenes = FindScene.StreamingCells.OverlapBox(_overlapBoxCenter, _overlapBoxSize, Quaternion.Euler(_overlapBoxEuler));
                                    }
                                }
                                else if (_demoMode == OverlapMode.SphereOverlap)
                                {
                                    var objectHit = ColliderUtility.Raycast(HandleUtility.GUIPointToWorldRay(e.mousePosition), objectFilter);
                                    if (objectHit != null)
                                    {
                                        _overlapSphereCenter = objectHit.Point;
                                        _overlappedScenes = FindScene.StreamingCells.OverlapSphere(_overlapSphereCenter, _overlapSphereRadius);
                                    }
                                }
                                break;
                            }
                        }
                        break;
                    }
                }

                sceneView.Repaint();
            }
        }
        
        private void DrawOverlappedVolumesGizmos()
        {
            // Set the color and then loop through each overlapped object and draw it
            GizmosEx.PushColor(_overlappedSolidColor);
            foreach (var gameObj in _overlappedObjects)
            {
                // Calculate the object's world OBB. If the OBB is valid, draw it.
                OBB worldOBB = gameObj.GetOBB();
                if (worldOBB.IsValid)
                {
                    // Inflate the OBB a bit to avoid any Z wars (e.g. cubes)
                    worldOBB.Inflate(1e-3f);

                    // Activate the trasnform matrix and then draw.
                    // Note: We use the OBB's transform information to build the matrix
                    GizmosEx.PushMatrix(Matrix4x4.TRS(worldOBB.Center, worldOBB.Rotation, worldOBB.Size));
                    Gizmos.DrawCube(Vector3.zero, Vector3.one);
                    GizmosEx.PushColor(_overlappedWireColor);
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                    GizmosEx.PopColor();
                    GizmosEx.PopMatrix();
                }
            }

            // Restore color
            GizmosEx.PopColor();
        }

        private void DrawOverlappedScenesVolumesGizmos()
        {
            // Set the color and then loop through each overlapped object and draw it
            GizmosEx.PushColor(_overlappedSolidColor);
            foreach (var sceneDataManager in _overlappedScenes)
            {
                AABB AABB = FindScene.StreamingCells.GetAABB(sceneDataManager);
                OBB worldOBB = new OBB(AABB.Center, AABB.Size);
            
                if (worldOBB.IsValid)
                {
                    // Inflate the OBB a bit to avoid any Z wars (e.g. cubes)
                    worldOBB.Inflate(1e-3f);

                    // Activate the trasnform matrix and then draw.
                    // Note: We use the OBB's transform information to build the matrix
                    GizmosEx.PushMatrix(Matrix4x4.TRS(worldOBB.Center, worldOBB.Rotation, worldOBB.Size));
                    Gizmos.DrawCube(Vector3.zero, Vector3.one);
                    GizmosEx.PushColor(_overlappedWireColor);
                    Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                    GizmosEx.PopColor();
                    GizmosEx.PopMatrix();
                }
            }

            // Restore color
            GizmosEx.PopColor();
        }

        public void DrawAllCells()
        {
            foreach (SceneDataManager sceneDataManager in SceneDataManagerStack.GetAllSceneDataManager())
            {
                GameObjectCollider gameObjectCollider = (GameObjectCollider)sceneDataManager.GetSceneData(typeof(GameObjectCollider));

                if(gameObjectCollider != null)
                {
                   gameObjectCollider.SceneObjectTree.DrawAllCells(NodeColor);  
                }
            }
        }

        public void DrawRaycast()
        {
            foreach (SceneDataManager sceneDataManager in SceneDataManagerStack.GetAllSceneDataManager())
            {
                GameObjectCollider gameObjectCollider = (GameObjectCollider)sceneDataManager.GetSceneData(typeof(GameObjectCollider));

                if(gameObjectCollider != null)
                {
                    gameObjectCollider.SceneObjectTree.DrawRaycast(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition), NodeColor);
                }
            }
        }
    }
}
#endif
#endif