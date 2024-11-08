#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.Extensions.Scripts;

namespace VladislavTsurikov.CustomGUI.ScriptsEditor
{
    public class InternalDragAndDrop
	{
		public enum State
		{
			None,
			Dragging,
			DragPerform
		}
        
        private static bool _repaint = false;
		
		private object s_dragData = null;
		private Vector2 s_mouseDownPosition;
		private State s_state = State.None;
		private const float s_kDragStartDistance = 14.0f;

        public void OnBeginGUI()
        {
            Event e = Event.current;

            if (e.type == EventType.MouseDown && e.button == 0)
            {
                s_mouseDownPosition = e.mousePosition;
            }

            if(s_state == State.Dragging)
            {
                if (e.type == EventType.MouseUp && e.button == 0)
                {                        
                    s_state = State.DragPerform;
                }  
            }
        }

        public void OnEndGUI()
        {
            Event e = Event.current;

            if(s_state == State.DragPerform)
            {
                s_dragData = null;
                s_state = State.None;
            }

            if(s_dragData != null)
            {
                if(s_state == State.None)
                {
                    if (e.type == EventType.MouseDrag &&
                        ((s_mouseDownPosition - e.mousePosition).magnitude > s_kDragStartDistance))
                    {
                        _repaint = true;
                        s_state = State.Dragging;
                    }
                }
            }
        }

        public void AddDragObject(object data)
        {
            if (data == null && s_dragData != null)
			{
				return;
			}

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                s_dragData = data;
            }
        }

        public bool IsDragging()
        {
            return s_state == State.Dragging;
        }

        public bool IsDragPerform()
        {
            return s_state == State.DragPerform;
        }

        public object GetData()
        {
            return s_dragData;
        }

        public void DrawDragRect(Rect iconRect, object icon)
        {
            if(s_dragData != icon)
            {
                EditorGUI.DrawRect(iconRect, Color.white.WithAlpha(0.3f));
            }
        }

        public static bool Repaint()
        {
            _repaint = false;
            return _repaint;
        }
    }
}
#endif