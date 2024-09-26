using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem.ScatterAlgorithms
{
    public enum RandomisationType 
    { 
        None, 
        Square,
        Sphere,
    }
    
    [Serializable]
    [Name("Random Grid")]  
    public class RandomGrid : Scatter
    {
	    private Vector3 _visualOrigin;
        
        public RandomisationType RandomisationType = RandomisationType.Square;
        [Range (0, 1)]
        public float Vastness = 1;
        public bool UniformGrid = true;
        public Vector2 GridStep = new Vector2(7, 7);
        public float GridAngle;
        
        public float FailureRate = 20;

        public override void Samples(AreaVariables areaVariables, List<Vector2> samples, Action<Vector2> onSpawn = null)
        {
	        UpdateGrid(areaVariables.RayHit.Point, areaVariables.Size);

            Vector3 gridOrigin = Vector3.zero;

            Vector3 position = Vector3.zero;
            float halfSpawnRange = areaVariables.Radius;

            Vector3 maxPosition = gridOrigin + (Vector3.one * (halfSpawnRange * 2));

            for (position.x = gridOrigin.x; position.x < maxPosition.x; position.x += GridStep.x)
            {
                for (position.z = gridOrigin.z; position.z < maxPosition.y; position.z += GridStep.y)
                {
                    Vector3 newLocalPosition = CommonUtility.RotatePointAroundPivot(position, new Vector3(maxPosition.x / 2, 0, maxPosition.z / 2), Quaternion.AngleAxis(GridAngle, Vector3.up));
                    Vector2 offsetLocalPosition = new Vector2(_visualOrigin.x + newLocalPosition.x, _visualOrigin.z + newLocalPosition.z);
                    offsetLocalPosition = GetCurrentRandomPosition(offsetLocalPosition);
                    
                    if(UnityEngine.Random.Range(0f, 100f) > FailureRate)
                    {
	                    if (areaVariables.Contains(offsetLocalPosition))
	                    {
		                    onSpawn?.Invoke(offsetLocalPosition);
		                    samples.Add(offsetLocalPosition);
	                    }
                    }
                }
            }
        }

        public override IEnumerator SamplesCoroutine(AreaVariables areaVariables, List<Vector2> samples, Action<Vector2> onSpawn = null)
        {
	        UpdateGrid(areaVariables.RayHit.Point, areaVariables.Size);

	        Vector3 gridOrigin = Vector3.zero;

	        Vector3 position = Vector3.zero;
	        float halfSpawnRange = areaVariables.Radius;

	        Vector3 maxPosition = gridOrigin + (Vector3.one * (halfSpawnRange * 2));

	        for (position.x = gridOrigin.x; position.x < maxPosition.x; position.x += GridStep.x)
	        {
		        for (position.z = gridOrigin.z; position.z < maxPosition.y; position.z += GridStep.y)
		        {
			        Vector3 newLocalPosition = CommonUtility.RotatePointAroundPivot(position, new Vector3(maxPosition.x / 2, 0, maxPosition.z / 2), Quaternion.AngleAxis(GridAngle, Vector3.up));
			        Vector2 offsetLocalPosition = new Vector2(_visualOrigin.x + newLocalPosition.x, _visualOrigin.z + newLocalPosition.z);
			        offsetLocalPosition = GetCurrentRandomPosition(offsetLocalPosition);
                    
			        if(UnityEngine.Random.Range(0f, 100f) > FailureRate)
			        {
				        if (areaVariables.Contains(offsetLocalPosition))
				        {
					        onSpawn?.Invoke(offsetLocalPosition);
					        samples.Add(offsetLocalPosition);
					        
					        if (IsNeedCallNextFrame())
						        yield return null;
				        }
			        }
		        }
	        }
        }

        public void UpdateGrid(Vector3 dragPoint, float size)
        {
            Vector3 gridOrigin = Vector3.zero;
            Vector3 localGridStep = new Vector3(GridStep.x, GridStep.y, 1);
            Vector3 gridNormal = Vector3.up;

            float halfSpawnRange = size / 2;  
            
            Vector3 point = new Vector3(dragPoint.x - halfSpawnRange, 0, dragPoint.z - halfSpawnRange);

            Matrix4x4 gridMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(GridAngle, gridNormal) * Quaternion.LookRotation(gridNormal), Vector3.one)
                                 * Matrix4x4.TRS(gridOrigin, Quaternion.identity, localGridStep);

            Vector3 gridSpacePoint = gridMatrix.inverse.MultiplyPoint(point);

            gridSpacePoint = new Vector3(Mathf.Round(gridSpacePoint.x), Mathf.Round(gridSpacePoint.y), gridSpacePoint.z);

            Vector3 snappedHitPoint = gridMatrix.MultiplyPoint(gridSpacePoint);
            _visualOrigin = snappedHitPoint;
        }

        public Vector2 GetRandomSquarePoint(Vector2 sample)
        {
            float halfDistanceX = GridStep.x / 2;
            float halfDistanceY = GridStep.y / 2;
            Vector2 distanceOffset = new Vector2(UnityEngine.Random.Range(-halfDistanceX, halfDistanceX), UnityEngine.Random.Range(-halfDistanceY, halfDistanceY));
            return sample + distanceOffset;
        }

        public Vector2 GetRandomSpherePoint(Vector2 sample)
        {
            float halfDistance = Mathf.Lerp(0, GridStep.x / 2, Vastness);
            Vector2 distanceOffset = new Vector2(UnityEngine.Random.Range(-halfDistance, halfDistance), UnityEngine.Random.Range(-halfDistance, halfDistance));
            return sample + distanceOffset;
        }

        public Vector2 GetCurrentRandomPosition(Vector2 sample)
        {
            switch (RandomisationType)
            {
                case RandomisationType.Square:
                {
                    return GetRandomSquarePoint(sample);
                }
                case RandomisationType.Sphere:
                {
                    return GetRandomSpherePoint(sample);
                }
            }

            return sample;
        }
    }
}