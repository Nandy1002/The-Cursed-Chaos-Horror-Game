#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.ResourcesController;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ResourcesController 
{
    [Serializable]
    public class ResourcesControllerEditor
    {
        private bool _resourcesControllerFoldout = false;

        public void OnGUI(Group group, bool drawFoldout = true)
		{
			if(drawFoldout)
			{
				_resourcesControllerFoldout = CustomEditorGUILayout.Foldout(_resourcesControllerFoldout, "Resources Controller (" + group.GetPrototypeTypeName() + ")");

				if(_resourcesControllerFoldout)
				{
					EditorGUI.indentLevel++;

					DrawResourcesController(group);

					EditorGUI.indentLevel--;
				}
			}
			else
			{
				DrawResourcesController(group);
			}
		}

		public void DrawResourcesController(Group group)
		{
			if (group.PrototypeType == typeof(PrototypeLargeObject))
			{
#if INSTANT_RENDERER
				LargeObjectRendererControllerEditor.OnGUI(group);
#endif
			}
			else if (group.PrototypeType == typeof(PrototypeGameObject))
			{
				GameObjectControllerEditor.OnGUI();
			}
			else if (group.PrototypeType == typeof(PrototypeTerrainDetail) ||
			         group.PrototypeType == typeof(PrototypeTerrainTexture))
			{
				TerrainResourcesControllerEditor.OnGUI(group);
			}
		}

		public bool IsSyncError(Group group)
		{
			if (group.PrototypeType == typeof(PrototypeTerrainDetail) ||
			         group.PrototypeType == typeof(PrototypeTerrainTexture))
			{
				TerrainResourcesController.DetectSyncError(group, Terrain.activeTerrain);

				if(TerrainResourcesController.SyncError != TerrainResourcesController.TerrainResourcesSyncError.None)
				{
					return true;
				}
			}

			return false;
		}
    }
}
#endif
