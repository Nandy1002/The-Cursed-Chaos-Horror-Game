#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Noise;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.OverlapCheckSettings.OverlapChecks;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem.ScatterAlgorithms;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.TemplatesSystem;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Templates
{
	[Template("Rocks", new Type[]{typeof(AdvancedBrushTool.ScriptsEditor.AdvancedBrushTool), typeof(TerrainSpawner.Scripts.TerrainSpawner)},
		new []{typeof(PrototypeLargeObject), typeof(PrototypeGameObject)})]
	public class Rocks : Template
	{
		protected override void Apply(Group group)
    	{
	        FilterComponent filterComponent = (FilterComponent)group.GetSettings(typeof(FilterComponent));
			ScatterComponent scatterComponent = (ScatterComponent)group.GetSettings(typeof(ScatterComponent));

			#region Scatter Settings
			scatterComponent.Stack.Clear();

            RandomGrid randomGrid = (RandomGrid)scatterComponent.Stack.CreateIfMissing(typeof(RandomGrid));
            randomGrid.RandomisationType = RandomisationType.Square;
    		randomGrid.Vastness = 1;
    		randomGrid.GridStep = new Vector2(2.7f, 2.7f);
            randomGrid.FailureRate = 90;
            #endregion

    		#region Mask Filters
            filterComponent.FilterType = FilterType.MaskFilter;
            filterComponent.MaskFilterComponent.Stack.Clear();

    		NoiseFilter noiseFilter = (NoiseFilter)filterComponent.MaskFilterComponent.Stack.CreateIfMissing(typeof(NoiseFilter));
            noiseFilter.NoiseSettings = new NoiseSettings();
    		noiseFilter.NoiseSettings.TransformSettings = new NoiseSettings.NoiseTransformSettings();
    		noiseFilter.NoiseSettings.TransformSettings.Scale = new Vector3(37, 40, 37);

    		MaskOperationsFilter remapFilter = (MaskOperationsFilter)filterComponent.MaskFilterComponent.Stack.CreateIfMissing(typeof(MaskOperationsFilter));
			remapFilter.MaskOperations = MaskOperations.Remap;
    		remapFilter.RemapRange.x = 0.44f;
    		remapFilter.RemapRange.y = 0.47f;
    		#endregion
		}

		protected override void Apply(Prototype proto)
    	{
			TransformStackComponent transformStackComponent = (TransformStackComponent)proto.GetSettings(typeof(TransformStackComponent));
            OverlapCheckComponent overlapCheckComponent = (OverlapCheckComponent)proto.GetSettings(typeof(OverlapCheckComponent));

			#region Transform Components
    		transformStackComponent.Stack.Clear();

    		Rotation rotation = (Rotation)transformStackComponent.Stack.CreateIfMissing(typeof(Rotation));
    		rotation.RandomizeOrientationX = 100;
    		rotation.RandomizeOrientationY = 100;
    		rotation.RandomizeOrientationZ = 100;

    		Scale scale = (Scale)transformStackComponent.Stack.CreateIfMissing(typeof(Scale));
    		scale.MinScale = new Vector3(0.8f, 0.8f, 0.8f);
    		scale.MaxScale = new Vector3(1.2f, 1.2f, 1.2f);
    		#endregion

    		#region OverlapCheckSettings
    		overlapCheckComponent.OverlapShape = OverlapShape.Bounds;
    		overlapCheckComponent.BoundsCheck.BoundsType = BoundsCheckType.BoundsPrefab;
    		overlapCheckComponent.BoundsCheck.MultiplyBoundsSize = 1;
    		#endregion
		}
	}
}
#endif