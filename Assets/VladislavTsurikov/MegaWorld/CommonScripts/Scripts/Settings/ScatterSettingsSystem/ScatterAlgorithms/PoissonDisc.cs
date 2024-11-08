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
    [Name("Poisson Disc")]  
    public class PoissonDisc : Scatter
    {
        /// Helper struct to calculate the x and y indices of a sample in the grid
        private struct GridPos
        {
            public int X;
            public int Y;

            public GridPos(Vector2 sample, float cellSize)
            {
                X = (int)(sample.x / cellSize);
                Y = (int)(sample.y / cellSize);
            }
        }

        public float PoissonDiscSize = 4;

        private const int K = 30;  // Maximum number of attempts before marking a sample as inactive.

        private Rect _rect;
        private float _radius2;  // radius squared
        private float _cellSize;
        private Vector2[,] _grid;
        private List<Vector2> _activeSamples = new List<Vector2>();

        /// Create a sampler with the following parameters:
        ///
        /// width:  each sample's x coordinate will be between [0, width]
        /// height: each sample's y coordinate will be between [0, height]
        /// radius: each sample will be at least `radius` units away from any other sample, and at most 2 * `radius`.
        
        public override IEnumerator SamplesCoroutine(AreaVariables areaVariables, List<Vector2> samples, Action<Vector2> onSpawn = null)
        {
            Init(areaVariables.Bounds.size.z, areaVariables.Bounds.size.x, PoissonDiscSize / 2);

            Vector2 point = AddSample(areaVariables, new Vector2(UnityEngine.Random.value * _rect.width, UnityEngine.Random.value * _rect.height));
            samples.Add(point);
            onSpawn?.Invoke(point);
            if (IsNeedCallNextFrame())
                yield return null;

            while (_activeSamples.Count > 0)
            {
                // Pick a random active sample
                int i = (int)UnityEngine.Random.value * _activeSamples.Count;
                Vector2 sample = _activeSamples[i];

                // Try `k` random candidates between [radius, 2 * radius] from that sample.
                bool found = false;
                for (int j = 0; j < K; ++j)
                {
                    float angle = 2 * Mathf.PI * UnityEngine.Random.value;
                    float r = Mathf.Sqrt(UnityEngine.Random.value * 3 * _radius2 + _radius2); // See: http://stackoverflow.com/questions/9048095/create-random-number-within-an-annulus/9048443#9048443
                    Vector2 candidate = sample + r * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                    // Accept candidates if it's inside the rect and farther than 2 * radius to any existing sample.
                    if (_rect.Contains(candidate) && IsFarEnough(candidate))
                    {
                        found = true;
                        
                        Vector2 localPoint = AddSample(areaVariables, candidate);

                        samples.Add(localPoint);
                        onSpawn?.Invoke(localPoint);
                        if (IsNeedCallNextFrame())
                            yield return null;
                        
                        break;
                    }
                }

                // If we couldn't find a valid candidate after k attempts, remove this sample from the active samples queue
                if (!found)
                {
                    _activeSamples[i] = _activeSamples[_activeSamples.Count - 1];
                    _activeSamples.RemoveAt(_activeSamples.Count - 1);
                }
            }
        }

        public override void Samples(AreaVariables areaVariables, List<Vector2> samples, Action<Vector2> onSpawn = null)
        {
            Init(areaVariables.Bounds.size.z, areaVariables.Bounds.size.x, PoissonDiscSize / 2);

            Vector2 point = AddSample(areaVariables, new Vector2(UnityEngine.Random.value * _rect.width, UnityEngine.Random.value * _rect.height));
            samples.Add(point);
            onSpawn?.Invoke(point);

            while (_activeSamples.Count > 0)
            {
                // Pick a random active sample
                int i = (int)UnityEngine.Random.value * _activeSamples.Count;
                Vector2 sample = _activeSamples[i];

                // Try `k` random candidates between [radius, 2 * radius] from that sample.
                bool found = false;
                for (int j = 0; j < K; ++j)
                {
                    float angle = 2 * Mathf.PI * UnityEngine.Random.value;
                    float r = Mathf.Sqrt(UnityEngine.Random.value * 3 * _radius2 + _radius2); // See: http://stackoverflow.com/questions/9048095/create-random-number-within-an-annulus/9048443#9048443
                    Vector2 candidate = sample + r * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                    // Accept candidates if it's inside the rect and farther than 2 * radius to any existing sample.
                    if (_rect.Contains(candidate) && IsFarEnough(candidate))
                    {
                        found = true;

                        Vector2 localPoint = AddSample(areaVariables, candidate);

                        samples.Add(localPoint);
                        onSpawn?.Invoke(localPoint);
                        break;
                    }
                }

                // If we couldn't find a valid candidate after k attempts, remove this sample from the active samples queue
                if (!found)
                {
                    _activeSamples[i] = _activeSamples[_activeSamples.Count - 1];
                    _activeSamples.RemoveAt(_activeSamples.Count - 1);
                }
            }
        }

        public void Init(float width, float height, float radius)
        {
            _rect = new Rect(0, 0, width, height);
            _radius2 = radius * radius;
            _cellSize = radius / Mathf.Sqrt(2);
            _grid = new Vector2[Mathf.CeilToInt(width / _cellSize),
                               Mathf.CeilToInt(height / _cellSize)];
        }

        private bool IsFarEnough(Vector2 sample)
        {
            GridPos pos = new GridPos(sample, _cellSize);

            int xmin = Mathf.Max(pos.X - 2, 0);
            int ymin = Mathf.Max(pos.Y - 2, 0);
            int xmax = Mathf.Min(pos.X + 2, _grid.GetLength(0) - 1);
            int ymax = Mathf.Min(pos.Y + 2, _grid.GetLength(1) - 1);

            for (int y = ymin; y <= ymax; y++)
            {
                for (int x = xmin; x <= xmax; x++)
                {
                    Vector2 s = _grid[x, y];
                    if (s != Vector2.zero)
                    {
                        Vector2 d = s - sample;
                        if (d.x * d.x + d.y * d.y < _radius2) return false;
                    }
                }
            }

            return true;

            // Note: we use the zero vector to denote an unfilled cell in the grid. This means that if we were
            // to randomly pick (0, 0) as a sample, it would be ignored for the purposes of proximity-testing
            // and we might end up with another sample too close from (0, 0). This is a very minor issue.
        }

        /// Adds the sample to the active samples queue and the grid before returning it
        private Vector2 AddSample(AreaVariables areaVariables, Vector2 sample)
        {
            float x = areaVariables.RayHit.Point.x + sample.x - areaVariables.Radius;
            float z = areaVariables.RayHit.Point.z + sample.y - areaVariables.Radius;
            
            _activeSamples.Add(sample);
            GridPos pos = new GridPos(sample, _cellSize);
            _grid[pos.X, pos.Y] = sample;
            return new Vector2(x, z);
        }
    }
}