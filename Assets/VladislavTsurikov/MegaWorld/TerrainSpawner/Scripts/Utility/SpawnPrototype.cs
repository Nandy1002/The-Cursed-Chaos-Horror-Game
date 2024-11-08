using UnityEngine;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;

namespace VladislavTsurikov.MegaWorld.TerrainSpawner.Scripts.Utility
{
    public static class SpawnPrototype
    {
        public static void SpawnTerrainDetails(Group group, PrototypeTerrainDetail proto, AreaVariables areaVariables, Terrain terrain)
        {
            var terrainData = terrain.terrainData;
            var spawnSize = new Vector2Int(
                CommonUtility.WorldToDetail(areaVariables.Size * areaVariables.SizeMultiplier, terrainData.size.x, terrainData),
                CommonUtility.WorldToDetail(areaVariables.Size * areaVariables.SizeMultiplier, terrainData.size.z, terrainData));
            
            var halfBrushSize = spawnSize / 2;
            
            var center = new Vector2Int(
                CommonUtility.WorldToDetail(areaVariables.RayHit.Point.x - terrain.transform.position.x, terrain.terrainData),
                CommonUtility.WorldToDetail(areaVariables.RayHit.Point.z - terrain.transform.position.z, terrain.terrainData.size.z, terrain.terrainData));
            
            var position = center - halfBrushSize;
            var startPosition = Vector2Int.Max(position, Vector2Int.zero);
                
            Vector2Int offset = startPosition - position;

            float fitness = 1;
            float detailmapResolution = terrain.terrainData.detailResolution;

            int[,] localData = terrain.terrainData.GetDetailLayer(
                startPosition.x, startPosition.y,
                Mathf.Max(0, Mathf.Min(position.x + spawnSize.x, (int)detailmapResolution) - startPosition.x),
                Mathf.Max(0, Mathf.Min(position.y + spawnSize.y, (int)detailmapResolution) - startPosition.y), proto.TerrainProtoId);
        
            float widthY = localData.GetLength(1);
            float heightX = localData.GetLength(0);

            MaskFilterComponent maskFilterComponent = (MaskFilterComponent)proto.GetSettings(typeof(MaskFilterComponent));
            SpawnDetailComponent spawnDetailComponent = (SpawnDetailComponent)proto.GetSettings(typeof(SpawnDetailComponent));
                   
            for (int y = 0; y < widthY; y++)
            {
                for (int x = 0; x < heightX; x++)
                {
                    var current = new Vector2Int(y, x);
        
                    Vector2 normal = current + startPosition; 
                    normal /= detailmapResolution;
        
                    Vector2 worldPostion = CommonUtility.GetTerrainWorldPositionFromRange(normal, terrain);

                    fitness = GetFitness.Get(group, maskFilterComponent, terrain, new Vector3(worldPostion.x, 0, worldPostion.y));

                    int targetStrength;
        
                    if (spawnDetailComponent.UseRandomOpacity) 
                    {
                        float randomFitness = fitness;
                        randomFitness *= Random.value;
        
                        targetStrength = Mathf.RoundToInt(Mathf.Lerp(0, spawnDetailComponent.Density, randomFitness));
                    }
                    else
                    {
                        targetStrength = Mathf.RoundToInt(Mathf.Lerp(0, spawnDetailComponent.Density, fitness));
                    }
                    
                    if (Random.Range(0f, 1f) < (1 - fitness) || Random.Range(0f, 1f) < spawnDetailComponent.FailureRate / 100)
                    {
                        targetStrength = 0;
                    }

                    localData[x, y] = targetStrength;
                }
            }
        
            terrain.terrainData.SetDetailLayer(startPosition.x, startPosition.y, proto.TerrainProtoId, localData);
        }
    }
}
