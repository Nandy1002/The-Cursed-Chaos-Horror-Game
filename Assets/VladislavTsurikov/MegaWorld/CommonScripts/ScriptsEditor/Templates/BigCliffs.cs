#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Noise;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem.ScatterAlgorithms;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.TransformComponentsSystem.Components;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.TemplatesSystem;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Templates
{
	[Template("Cliffs/Big Rocks", new Type[]{typeof(AdvancedBrushTool.ScriptsEditor.AdvancedBrushTool), typeof(TerrainSpawner.Scripts.TerrainSpawner)},
		new []{typeof(PrototypeLargeObject), typeof(PrototypeGameObject)})]
	public class BigCliffs : Template
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
    		randomGrid.GridStep = new Vector2(1.7f, 1.7f);
            randomGrid.FailureRate = 60;
            #endregion

			#region Mask Filters
			filterComponent.FilterType = FilterType.MaskFilter;
			filterComponent.MaskFilterComponent.Stack.Clear();

    		NoiseFilter noiseFilter = (NoiseFilter)filterComponent.MaskFilterComponent.Stack.CreateIfMissing(typeof(NoiseFilter));
            noiseFilter.NoiseSettings = new NoiseSettings();
    		noiseFilter.NoiseSettings.TransformSettings = new NoiseSettings.NoiseTransformSettings();
    		noiseFilter.NoiseSettings.TransformSettings.Scale = new Vector3(31, 40, 31);

    		MaskOperationsFilter remapFilter = (MaskOperationsFilter)filterComponent.MaskFilterComponent.Stack.CreateIfMissing(typeof(MaskOperationsFilter));
			remapFilter.MaskOperations = MaskOperations.Remap;
    		remapFilter.RemapRange.x = 0.44f;
    		remapFilter.RemapRange.y = 0.47f;

    		SlopeFilter slopeFilter = (SlopeFilter)filterComponent.MaskFilterComponent.Stack.CreateIfMissing(typeof(SlopeFilter));
    		slopeFilter.MinSlope = 48;
    		slopeFilter.MaxSlope = 90;
    		slopeFilter.AddSlopeFalloff = 17;
			#endregion
		}

		protected override void Apply(Prototype proto)
    	{
			TransformStackComponent transformStackComponent = (TransformStackComponent)proto.GetSettings(typeof(TransformStackComponent));
            OverlapCheckComponent overlapCheckComponent = (OverlapCheckComponent)proto.GetSettings(typeof(OverlapCheckComponent));

    		#region Transform Components
    		transformStackComponent.Stack.Clear();

			transformStackComponent.Stack.CreateIfMissing(typeof(CliffsAlign));
    		transformStackComponent.Stack.CreateIfMissing(typeof(SlopePosition));
    		Scale scale = (Scale)transformStackComponent.Stack.CreateIfMissing(typeof(Scale)); 
			scale.MaxScale = new Vector3(1.4f, 1.4f, 1.4f);
    		ScaleFitness scaleFitness = (ScaleFitness)transformStackComponent.Stack.CreateIfMissing(typeof(ScaleFitness));
			scaleFitness.OffsetScale = -1;
			transformStackComponent.Stack.CreateIfMissing(typeof(ScaleClamp));
    		#endregion

    		#region OverlapCheckSettings
    		overlapCheckComponent.OverlapShape = OverlapShape.Bounds;
			overlapCheckComponent.BoundsCheck.MultiplyBoundsSize = 0.4f;
    		#endregion
		}
	}
}
#endif