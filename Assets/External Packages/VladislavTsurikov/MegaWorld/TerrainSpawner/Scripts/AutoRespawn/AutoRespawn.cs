using System.Collections.Generic;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility.Spawn;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;

namespace VladislavTsurikov.MegaWorld.TerrainSpawner.Scripts.AutoRespawn
{
    public class AutoRespawn 
    {
        public Group _modifiedType;
        private PrototypeTerrainDetail _modifiedTerrainDetailProto;
        private TerrainSpawner _stamperTool;

        public AutoRespawn(Group group, TerrainSpawner stamperTool)
        {
            _stamperTool = stamperTool;
            _modifiedType = group;
        }

        public AutoRespawn(PrototypeTerrainDetail proto, TerrainSpawner stamperTool)
        {
            _stamperTool = stamperTool;
            _modifiedTerrainDetailProto = proto;
        }

        public void TypeHasChanged()
        {
	        if(_stamperTool.StamperToolControllerSettings.AutoRespawn)
			{
                if(_modifiedType != null)
			    {
			    	UnspawnTypesForAutoRespawn(_stamperTool.Data);
                    _stamperTool.RunSpawn();
			    }
			    else if(_modifiedTerrainDetailProto != null)
			    {
			    	if(_stamperTool.Data.SelectedVariables.HasOneSelectedGroup())
			    	{
			    		List<Prototype> protoTerrainDetailList = new List<Prototype>();
			    		protoTerrainDetailList.Add(_modifiedTerrainDetailProto);
			    	    Unspawn.UnspawnTerrainDetail(protoTerrainDetailList, false);

			    		Group group = _stamperTool.Data.SelectedVariables.SelectedGroup;

                        LayerSettings layerSettings = WindowDataPackage.Instance.layerSettings;

			    		RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(_stamperTool.transform.position), 
                            layerSettings.GetCurrentPaintLayers(group.PrototypeType));
                    	
                		if(rayHit != null)
                		{
                		    AreaVariables areaVariables = _stamperTool.Area.GetAreaVariables(rayHit);

                            SpawnGroup.SpawnTerrainDetails(group, protoTerrainDetailList, areaVariables);
                		}
			    	}
			    }
            }
        }

        public void UnspawnTypesForAutoRespawn(BasicData data)
        {            
            foreach (Group group in data.SelectedVariables.SelectedGroupList)
            {
	            if (group.PrototypeType == typeof(PrototypeGameObject))
	            {
		            Unspawn.UnspawnGameObject(false);
	            }
	            else if (group.PrototypeType == typeof(PrototypeTerrainDetail))
	            {
		            Unspawn.UnspawnTerrainDetail(group.PrototypeList, false);
	            }
	            else if (group.PrototypeType == typeof(PrototypeLargeObject))
	            {
		            Unspawn.UnspawnInstantItem(group, false);
	            }
            }
        }
    }
}