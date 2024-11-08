using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.MegaWorld.TerrainSpawner.Scripts.AutoRespawn;
using VladislavTsurikov.MegaWorld.TerrainSpawner.Scripts.Utility;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.MegaWorld.TerrainSpawner.ScriptsEditor;
using VladislavTsurikov.EditorCoroutines.ScriptsEditor;
#endif

namespace VladislavTsurikov.MegaWorld.TerrainSpawner.Scripts
{
    [ExecuteInEditMode]
    [Name("Terrain Spawner")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new []{typeof(PrototypeLargeObject), typeof(PrototypeGameObject), typeof(PrototypeTerrainDetail), typeof(PrototypeTerrainTexture)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeLargeObject), new []{typeof(SuccessComponent), typeof(OverlapCheckComponent), typeof(TransformStackComponent)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeGameObject), new []{typeof(SuccessComponent), typeof(OverlapCheckComponent), typeof(TransformStackComponent)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainDetail), new []{typeof(SpawnDetailComponent), typeof(MaskFilterComponent)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainTexture), new []{typeof(MaskFilterComponent)})]
    [AddGeneralGroupComponents(new []{typeof(PrototypeLargeObject), typeof(PrototypeGameObject)}, new []{typeof(ScatterComponent), typeof(FilterComponent)})]
    public class TerrainSpawner : ToolMonoBehaviour
    {
        public Area Area = new Area();
        public TerrainSpawnerControllerSettings StamperToolControllerSettings = new TerrainSpawnerControllerSettings();
        
        public float SpawnProgress;
        public bool CancelSpawn;
		public bool SpawnComplete = true;

#if UNITY_EDITOR
        public AutoRespawnController AutoRespawnController = new AutoRespawnController();
        [NonSerialized]
        public StamperVisualisation StamperVisualisation = new StamperVisualisation();
#endif
        
        [OnDeserializing]
        private void Initialize()
        {
#if UNITY_EDITOR
            AutoRespawnController = new AutoRespawnController();
            StamperVisualisation = new StamperVisualisation();
#endif
        }

        private void OnEnable()
        {
            Area.SetAreaBoundsIfNecessary(this, true);
        }

        private void Update()
        {
            Data.SelectedVariables.DeleteNullValueIfNecessary(Data.GroupList);
            Data.SelectedVariables.SetAllSelectedParameters(Data.GroupList);

            Area.SetAreaBoundsIfNecessary(this);
        }

        public void Spawn()
        {
#if UNITY_EDITOR
            SpawnComplete = false;
            
            EditorCoroutines.ScriptsEditor.EditorCoroutines.StartCoroutine(RunSpawnCoroutine(), this);
#endif
        }

        public void GenerateNewRandomSeed(Group group)
        {
            if (group.GenerateRandomSeed)
            {
                group.RandomSeed = Random.Range(0, int.MaxValue);
            }
        }
        
#if UNITY_EDITOR
        public IEnumerator SpawnGroupCoroutine(Group group, AreaVariables areaVariables)
        {
            if(IsReadyToSpawn(group))
            {
                GenerateNewRandomSeed(group);

                Random.InitState(group.RandomSeed);
                
                if (group.PrototypeType == typeof(PrototypeGameObject))
                {
                    EditorCoroutine editorCoroutine = EditorCoroutines.ScriptsEditor.EditorCoroutines.StartCoroutine(Utility.SpawnGroupCoroutine.SpawnGameObject(group, areaVariables), this);
                        
                    yield return editorCoroutine;
                }
                else if (group.PrototypeType == typeof(PrototypeTerrainDetail))
                {
                    EditorCoroutine editorCoroutine = EditorCoroutines.ScriptsEditor.EditorCoroutines.StartCoroutine(Utility.SpawnGroupCoroutine.SpawnTerrainDetails(group, group.PrototypeList, areaVariables), this);

                    yield return editorCoroutine;
                }
                else if (group.PrototypeType == typeof(PrototypeLargeObject))
                {
                    EditorCoroutine editorCoroutine = EditorCoroutines.ScriptsEditor.EditorCoroutines.StartCoroutine(Utility.SpawnGroupCoroutine.SpawnInstantItem(group, areaVariables), this);
                        
                    yield return editorCoroutine;
                }
            }
            
            TerrainsMaskManager.Dispose();

            yield return null;
        }
#endif

        public void SpawnGroup(Group group, AreaVariables areaVariables)
        {            
            if(!IsReadyToSpawn(group))
            {
                return;
            }

            GenerateNewRandomSeed(group);

            Random.InitState(group.RandomSeed);
            
            if (group.PrototypeType == typeof(PrototypeGameObject))
            {
                CommonScripts.Scripts.Utility.Spawn.SpawnGroup.SpawnGameObject(group, areaVariables, false); 
            }
            else if (group.PrototypeType == typeof(PrototypeTerrainDetail))
            {
                CommonScripts.Scripts.Utility.Spawn.SpawnGroup.SpawnTerrainDetails(group, group.PrototypeList, areaVariables);
            }
            else if (group.PrototypeType == typeof(PrototypeLargeObject))
            {
                CommonScripts.Scripts.Utility.Spawn.SpawnGroup.SpawnInstantItem(group, areaVariables);
            }
        }

        public bool IsReadyToSpawn(Group group)
        {
            foreach (Prototype proto in group.PrototypeList)
            {
                if(proto.Active)
                {
                    return true;
                }
            }

            return false;
        }

        public void RunSpawn()
        {
            for (int typeIndex = 0; typeIndex < Data.GroupList.Count; typeIndex++)
            {
                Group group = Data.GroupList[typeIndex];

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(transform.position), WindowDataPackage.Instance.layerSettings.GetCurrentPaintLayers(group.PrototypeType));
        
                if(rayHit == null)
                {
                    return;
                }

                AreaVariables areaVariables = Area.GetAreaVariables(rayHit);

                SpawnGroup(group, areaVariables);
            }
        }
        
#if UNITY_EDITOR
        public IEnumerator RunSpawnCoroutine()
        {
            CancelSpawn = false;

            int maxTypes = Data.GroupList.Count;
            int completedTypes = 0;
            
            for (int typeIndex = 0; typeIndex < Data.GroupList.Count; typeIndex++)
            {
                if (CancelSpawn)
                {
                    break;
                }
#if UNITY_EDITOR
                EditorUtility.DisplayProgressBar("Running", "Running " + Data.GroupList[typeIndex].name, completedTypes / (float)maxTypes);
#endif

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(transform.position), WindowDataPackage.Instance.layerSettings.GetCurrentPaintLayers(Data.GroupList[typeIndex].PrototypeType));
        
                if(rayHit == null)
                {
                    continue;
                }

                AreaVariables areaVariables = Area.GetAreaVariables(rayHit);
                
                EditorCoroutine editorCoroutine = EditorCoroutines.ScriptsEditor.EditorCoroutines.StartCoroutine(SpawnGroupCoroutine(Data.GroupList[typeIndex], areaVariables), this);

                completedTypes++;
                SpawnProgress = completedTypes / (float)maxTypes;
                yield return editorCoroutine;
            }

            SpawnProgress = completedTypes / (float)maxTypes;
            SpawnProgress = 0;

#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
            SpawnComplete = true;
        }
#endif
    }
}