using UnityEngine;

namespace VladislavTsurikov.ColliderSystem.Scripts.Shape
{
    public struct Sphere
    {
        public Vector3 Center;
        public float Radius;

        private bool _isCreated; 

        public Sphere(Vector3 sphereCenter, float sphereRadius)
        {
            Center = sphereCenter;
            Radius = sphereRadius;
			
            _isCreated = true;
        }

        public bool IsValid()
        {
            return _isCreated;
        }
        
        public void Сopy(Sphere sphere) 
        {
            Center = sphere.Center;
            Radius = sphere.Radius;
        }
		
        public bool Contains(Vector3 point)
        {
            return Vector3.Distance(point, Center) <= Radius;
        }
    }
}