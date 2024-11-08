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
    [Name("Random Point")]  
    public class RandomPoint : Scatter
    {
        public int MinChecks = 15;
		public int MaxChecks = 15;
        
        public override IEnumerator SamplesCoroutine(AreaVariables areaVariables, List<Vector2> samples, Action<Vector2> onSpawn = null)
        {
            int numberOfChecks = UnityEngine.Random.Range(MinChecks, MaxChecks);
        
            for (int checks = 0; checks < numberOfChecks; checks++)
            {
                Vector2 point = GetRandomPoint(areaVariables);
                onSpawn?.Invoke(point);
                samples.Add(point);
                
                if(IsNeedCallNextFrame())
                    yield return null;
            }
        }

        public override void Samples(AreaVariables areaVariables, List<Vector2> samples, Action<Vector2> onSpawn = null)
        {
            int numberOfChecks = UnityEngine.Random.Range(MinChecks, MaxChecks);
        
            for (int checks = 0; checks < numberOfChecks; checks++)
            {
                Vector2 point = GetRandomPoint(areaVariables);
                onSpawn?.Invoke(point);
                samples.Add(point);
            }
        }

        public Vector2 GetRandomPoint(AreaVariables areaVariables)
        {
            Vector2 spawnOffset = new Vector3(UnityEngine.Random.Range(-areaVariables.Radius, areaVariables.Radius), UnityEngine.Random.Range(-areaVariables.Radius, areaVariables.Radius));
            return new Vector2(spawnOffset.x + areaVariables.RayHit.Point.x, spawnOffset.y + areaVariables.RayHit.Point.z);
        }
    }
}