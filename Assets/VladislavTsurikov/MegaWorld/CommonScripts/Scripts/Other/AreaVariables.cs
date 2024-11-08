using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.Extensions.Scripts;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other
{
    public class AreaVariables
    {
        public float Size;
        public float SizeMultiplier = 1;
		public float Rotation = 0;
        public float CosAngle;
        public float SinAngle;
        public RayHit RayHit;
        public Bounds Bounds;
        public Terrain TerrainUnderCursor;
        public Texture2D Mask = Texture2D.whiteTexture;
        
        public float Radius => Size / 2;

        public AreaVariables() {}

        public AreaVariables(Bounds bounds, RayHit rayHit)
        {
            Bounds = bounds;
            RayHit = rayHit;
            Size = Bounds.size.x;
            
            TerrainUnderCursor = CommonUtility.GetTerrain(RayHit.Point);
        }
        
        public bool Contains(Vector2 point)
        {
            return RectExtension.CreateRectFromBounds(Bounds).Contains(point);
        }
    }
}