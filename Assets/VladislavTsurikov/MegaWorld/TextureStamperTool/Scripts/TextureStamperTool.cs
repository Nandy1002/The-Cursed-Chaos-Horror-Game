using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Scripts.Scene;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Utility.Spawn;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using VladislavTsurikov.MegaWorld.TextureStamperTool.Scripts.AutoRespawn;
#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.MegaWorld.TextureStamperTool.ScriptsEditor;
#endif

namespace VladislavTsurikov.MegaWorld.TextureStamperTool.Scripts
{
    [ExecuteInEditMode]
    [Name("Stamper Tool")]
    [SupportMultipleSelectedGroups]
    [SupportedPrototypeTypes(new []{typeof(PrototypeTerrainTexture)})]
    [AddGeneralPrototypeComponents(typeof(PrototypeTerrainTexture), new []{typeof(MaskFilterComponent)})]
    public class TextureStamperTool : ToolMonoBehaviour
    {
        private IEnumerator _updateCoroutine;
        
        public Area Area = new Area();
        public TextureStamperToolControllerSettings textureStamperToolControllerSettings = new TextureStamperToolControllerSettings();
        
        public float SpawnProgress = 0f;
        public bool CancelSpawn = false;

#if UNITY_EDITOR
        public AutoRespawnController AutoRespawnController = new AutoRespawnController();
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

        public void StartEditorUpdates()
        {
#if UNITY_EDITOR
            EditorApplication.update += EditorUpdate;
#endif
        }

        public void StopEditorUpdates()
        {
  #if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
#endif
        }

        private void EditorUpdate()
        {
            if (_updateCoroutine == null)
            {
                StopEditorUpdates();
                return;
            }
            else
            {
                _updateCoroutine.MoveNext();
            }
        }
        
        public void Spawn()
        {
            _updateCoroutine = RunSpawnCoroutine();
            StartEditorUpdates();
        }

        public void SpawnWithCells(List<Bounds> cellList)
        {
            _updateCoroutine = RunSpawnCoroutineWithSpawnCells(cellList);
            StartEditorUpdates();
        }

        public void GenerateNewRandomSeed(Group group)
        {
            if (group.GenerateRandomSeed)
            {
                group.RandomSeed = Random.Range(0, int.MaxValue);
            }
        }

        public void Spawn(Group group, AreaVariables areaVariables)
        {            
            if(!IsReadyToSpawn(group))
            {
                return;
            }

            GenerateNewRandomSeed(group);

            Random.InitState(group.RandomSeed);
            
            if (group.PrototypeType == typeof(PrototypeTerrainTexture))
            {
                SpawnGroup.SpawnTerrainTexture(group, group.PrototypeList, areaVariables, 1);
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

                Spawn(group, areaVariables);
            }
        }

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
                EditorUtility.DisplayProgressBar("Running", "Running " + Data.GroupList[typeIndex].name, (float)completedTypes / (float)maxTypes);
#endif

                RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(transform.position), WindowDataPackage.Instance.layerSettings.GetCurrentPaintLayers(Data.GroupList[typeIndex].PrototypeType));
        
                if(rayHit == null)
                {
                    continue;
                }

                AreaVariables areaVariables = Area.GetAreaVariables(rayHit);
                
                Spawn(Data.GroupList[typeIndex], areaVariables);

                completedTypes++;
                SpawnProgress = (float)completedTypes / (float)maxTypes;
                yield return null;
            }

            SpawnProgress = (float)completedTypes / (float)maxTypes;
            yield return null;

            SpawnProgress = 0;

#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
            _updateCoroutine = null;
        }

        private IEnumerator RunSpawnCoroutineWithSpawnCells(List<Bounds> spawnCellList)
        {
            CancelSpawn = false;

            int maxTypes = Data.GroupList.Count;
            int completedTypes = 0;

            float oneStep = (float)1 / (float)spawnCellList.Count;

            for (int cellIndex = 0; cellIndex < spawnCellList.Count; cellIndex++)
            {
                float cellProgress = ((float)cellIndex / (float)spawnCellList.Count) * 100;

                for (int typeIndex = 0; typeIndex < Data.GroupList.Count; typeIndex++)
                {
                    if (CancelSpawn)
                    {
                        break;
                    }

                    Bounds bounds = spawnCellList[cellIndex];
                    
                    SpawnProgress = cellProgress / 100;

                    if(maxTypes != 1)
                    {
                        SpawnProgress = (cellProgress / 100) + Mathf.Lerp(0, oneStep, (float)completedTypes / (float)maxTypes);
                    }

#if UNITY_EDITOR
                    EditorUtility.DisplayProgressBar("Cell: " + cellProgress + "%" + " (" + cellIndex + "/" + spawnCellList.Count + ")", "Running " + Data.GroupList[typeIndex].name, SpawnProgress);
#endif

                    Group group = Data.GroupList[typeIndex];

                    RayHit rayHit = RaycastUtility.Raycast(RayUtility.GetRayDown(bounds.center), WindowDataPackage.Instance.layerSettings.GetCurrentPaintLayers(group.PrototypeType));

                    if(rayHit == null)
                    {
                        continue;
                    }

                    AreaVariables areaVariables = Area.GetAreaVariablesFromSpawnCell(rayHit, bounds);
                
                    Spawn(group, areaVariables);

                    completedTypes++;
                    yield return null;
                }
            }

            SpawnProgress = 1;
            yield return null;

            SpawnProgress = 0;

#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
            _updateCoroutine = null;
        }
    }
}