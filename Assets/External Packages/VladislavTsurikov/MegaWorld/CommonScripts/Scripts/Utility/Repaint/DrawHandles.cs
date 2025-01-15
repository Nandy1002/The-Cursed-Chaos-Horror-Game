#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.Extensions.Scripts;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.PreferencesSettings;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility.Repaint
{
    public static class DrawHandles 
    {
        public static void DrawSpawnVisualizerPixel(SpawnVisualizerPixel spawnVisualizerPixel, float stepIncrement)
        {
            if(AdvancedSettings.Instance.VisualisationSettings.SimpleFilterSettings.ColorHandlesType == ColorHandlesType.Custom)
            {
                Handles.color = Color.Lerp(AdvancedSettings.Instance.VisualisationSettings.SimpleFilterSettings.InactiveColor, AdvancedSettings.Instance.VisualisationSettings.SimpleFilterSettings.ActiveColor, spawnVisualizerPixel.Fitness).WithAlpha(spawnVisualizerPixel.Alpha);
            }
            else
            {
                if(spawnVisualizerPixel.Fitness < 0.5)
                {
                    float difference = spawnVisualizerPixel.Fitness / 0.5f;
                    Handles.color = Color.Lerp(Color.red, Color.yellow, difference).WithAlpha((spawnVisualizerPixel.Alpha));
                }
                else
                {
                    float difference = (spawnVisualizerPixel.Fitness - 0.5f) / 0.5f;
                    Handles.color = Color.Lerp(Color.yellow, Color.green, difference).WithAlpha((spawnVisualizerPixel.Alpha));
                }
            }

            if(AdvancedSettings.Instance.VisualisationSettings.SimpleFilterSettings.HandlesType == HandlesType.DotCap)
            {
                if(AdvancedSettings.Instance.VisualisationSettings.SimpleFilterSettings.HandleResizingType == HandleResizingType.Resolution)
                {
                    DotCap(0, spawnVisualizerPixel.Position, Quaternion.identity, stepIncrement / 3);
                }
                else if(AdvancedSettings.Instance.VisualisationSettings.SimpleFilterSettings.HandleResizingType == HandleResizingType.Distance)
                {
                    DotCap(0, spawnVisualizerPixel.Position, Quaternion.identity, HandleUtility.GetHandleSize(spawnVisualizerPixel.Position) * 0.03f);
                }
                else
                {
                    DotCap(0, spawnVisualizerPixel.Position, Quaternion.identity, AdvancedSettings.Instance.VisualisationSettings.SimpleFilterSettings.CustomHandleSize);
                }
            }
            else
            {
                if(AdvancedSettings.Instance.VisualisationSettings.SimpleFilterSettings.HandleResizingType == HandleResizingType.Resolution)
                {
                    Handles.SphereHandleCap(0, new Vector3(spawnVisualizerPixel.Position.x, spawnVisualizerPixel.Position.y, spawnVisualizerPixel.Position.z), Quaternion.LookRotation(Vector3.up), 
                        stepIncrement / 2, EventType.Repaint);
                }
                else if(AdvancedSettings.Instance.VisualisationSettings.SimpleFilterSettings.HandleResizingType == HandleResizingType.Distance)
                {
                    Handles.SphereHandleCap(0, new Vector3(spawnVisualizerPixel.Position.x, spawnVisualizerPixel.Position.y, spawnVisualizerPixel.Position.z), Quaternion.LookRotation(Vector3.up), 
                        HandleUtility.GetHandleSize(spawnVisualizerPixel.Position) * 0.05f, EventType.Repaint);
                }
                else
                {
                    Handles.SphereHandleCap(0, new Vector3(spawnVisualizerPixel.Position.x, spawnVisualizerPixel.Position.y, spawnVisualizerPixel.Position.z), Quaternion.LookRotation(Vector3.up), 
                        AdvancedSettings.Instance.VisualisationSettings.SimpleFilterSettings.CustomHandleSize, EventType.Repaint);
                }
            }
        }

        public static void DrawXYZCross(RayHit hit, Vector3 upwards, Vector3 right, Vector3 forward)
		{
			float handleSize = HandleUtility.GetHandleSize (hit.Point) * 0.5f;

			Handles.color = Color.green;
            Handles.DrawAAPolyLine(3, hit.Point + upwards * handleSize, hit.Point + upwards * -handleSize * 0.2f);
			Handles.color = Color.red;
            Handles.DrawAAPolyLine(3, hit.Point + right * handleSize, hit.Point + right * -handleSize * 0.2f);
			Handles.color = Color.blue;
            Handles.DrawAAPolyLine(3, hit.Point + forward * handleSize, hit.Point + forward * -handleSize * 0.2f);
		}

        public static void CircleCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if(Event.current != null && (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint))
                Handles.CircleHandleCap(controlID, position, rotation, size, Event.current.type);
        }

        public static void DotCap(int controlID, Vector3 position, Quaternion rotation, float size)
        {
            if(Event.current != null && (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint))
            {
                Handles.DotHandleCap(controlID, position, rotation, size, Event.current.type);
            } 
        }
    }
}
#endif