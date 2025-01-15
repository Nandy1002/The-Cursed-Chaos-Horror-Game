#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.CustomGUI.ScriptsEditor;

namespace VladislavTsurikov.Scripts
{
    public abstract class BaseWindow<T> : EditorWindow where T : EditorWindow
    {
        private static T s_instance;
        
        private static T Window
        {
            get
            {
                T[] windows = Resources.FindObjectsOfTypeAll<T>();
                return windows.Length > 0 ? windows[0] : null;
            }
        }

        protected VisualElement root => rootVisualElement;
        protected VisualElement windowLayout { get; set; }
        
        protected double LastRepaintTime;
        
        protected virtual bool UseCustomRepaintInterval { get { return false; } }
        protected virtual double CustomRepaintIntervalDuringPlayMode { get { return 0.4f; } }
        protected virtual double CustomRepaintIntervalWhileIdle { get { return 0.6f; } }
        protected virtual bool RepaintOnInspectorUpdate { get { return true; } }

        public static bool IsOpen { get; private set; }

        public static T Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = Window;
                if (s_instance != null) return s_instance;
                s_instance = GetWindow<T>();
                return s_instance;
            }
        }
        
        public static void OpenWindow(string windowTitle)
        {
            Instance.Show();
            Instance.titleContent.text = windowTitle;
        }
        
        protected virtual void OnGUI()
        {
            CustomEditorGUILayout.ScreenRect = position;
            
            EditorGUI.indentLevel = 0;

            CustomEditorGUILayout.IsInspector = false;
            
            Event current = Event.current;

            if (current.type == EventType.Repaint)
            {
                LastRepaintTime = EditorApplication.timeSinceStartup;
            }
        }
        
        protected virtual void Update()
        {
            if (!UseCustomRepaintInterval) return;

            if (EditorApplication.isPlaying && EditorApplication.timeSinceStartup - LastRepaintTime < CustomRepaintIntervalDuringPlayMode ||
                !EditorApplication.isPlayingOrWillChangePlaymode && EditorApplication.timeSinceStartup - LastRepaintTime < CustomRepaintIntervalWhileIdle)
                Repaint();
        }
        
        /// <summary> Called 10 frames per second to give the inspector a chance to update </summary>
        protected virtual void OnInspectorUpdate()
        {
            if (RepaintOnInspectorUpdate) Repaint();
        }
        
        protected virtual void OnEnable()
        {
            IsOpen = true;
        }

        protected virtual void OnDisable()
        {
            IsOpen = false;
        }

        protected virtual void OnDestroy() {}
    }
}
#endif