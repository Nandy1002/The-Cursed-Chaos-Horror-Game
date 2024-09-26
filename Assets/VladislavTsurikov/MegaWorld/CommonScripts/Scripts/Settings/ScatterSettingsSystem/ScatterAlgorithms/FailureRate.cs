using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem.ScatterAlgorithms
{
    [Serializable]
    [Name("Failure Rate")]  
    public class FailureRate : Scatter
    {
        public float Value = 70;
        
        public override IEnumerator SamplesCoroutine(AreaVariables areaVariables, List<Vector2> samples, Action<Vector2> onSpawn = null)
        {
            for (int i = samples.Count - 1; i >= 0 ; i--)
            {
                if(UnityEngine.Random.Range(0f, 100f) < Value)
                {
                    samples.RemoveAt(i);
                }
                else
                {
                    onSpawn?.Invoke(samples[i]);
                    if (IsNeedCallNextFrame())
                        yield return null;
                }
            }
        }
        
        public override void Samples(AreaVariables areaVariables, List<Vector2> samples, Action<Vector2> onSpawn = null)
        {
            for (int i = samples.Count - 1; i >= 0 ; i--)
            {
                if(UnityEngine.Random.Range(0f, 100f) < Value)
                {
                    samples.RemoveAt(i);
                }
                else
                {
                    onSpawn?.Invoke(samples[i]);
                }
            }
        }
    }
}