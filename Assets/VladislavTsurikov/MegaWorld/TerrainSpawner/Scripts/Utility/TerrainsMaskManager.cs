using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using Group = VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Group;

namespace VladislavTsurikov.MegaWorld.TerrainSpawner.Scripts.Utility
{
    public static class TerrainsMaskManager
    {
        private static readonly List<TerrainsMaskItem> _itemList = new List<TerrainsMaskItem>();

        public static float GetFitness(Group group, MaskFilterComponent maskFilterComponent, Vector3 point)
        {
            foreach (var item in _itemList)
            {
                if (item.MaskFilterComponent == maskFilterComponent)
                {
                    return item.GetFitness(group, point);
                }
            }
            
            TerrainsMaskItem localTerrainsMaskItem = new TerrainsMaskItem(maskFilterComponent);
            _itemList.Add(localTerrainsMaskItem);
            return localTerrainsMaskItem.GetFitness(group, point);
        }
        
        public static float GetFitness(Group group, MaskFilterComponent maskFilterComponent, Terrain terrain, Vector3 point)
        {
            foreach (var item in _itemList)
            {
                if (item.MaskFilterComponent == maskFilterComponent)
                {
                    return item.GetFitness(group, terrain, point);
                }
            }
            
            TerrainsMaskItem localTerrainsMaskItem = new TerrainsMaskItem(maskFilterComponent);
            _itemList.Add(localTerrainsMaskItem);
            return localTerrainsMaskItem.GetFitness(group, terrain, point);
        }
        
        public static void Dispose()
        {
            _itemList.Clear();
        }
    }
}