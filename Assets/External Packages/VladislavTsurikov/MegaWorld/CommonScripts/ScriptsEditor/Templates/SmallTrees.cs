#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Filters;
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
	[Template("Trees/Small Trees", new Type[]{typeof(AdvancedBrushTool.ScriptsEditor.AdvancedBrushTool), typeof(TerrainSpawner.Scripts.TerrainSpawner)}, 
		new []{typeof(PrototypeLargeObject), typeof(PrototypeGameObject)})]
	public class SmallTrees : Template
	{
		protected override void Apply(Group group)
    	{
			ScatterComponent scatterComponent = (ScatterComponent)group.GetSettings(typeof(ScatterComponent));
			FilterComponent filterComponent = (FilterComponent)group.GetSettings(typeof(FilterComponent));

			#region Scatter Settings
            scatterComponent.Stack.Clear();

            RandomGrid randomGrid = (RandomGrid)scatterComponent.Stack.CreateIfMissing(typeof(RandomGrid));
            randomGrid.RandomisationType = RandomisationType.Square;
    		randomGrid.Vastness = 1;
    		randomGrid.GridStep = new Vector2(5, 5);
            randomGrid.FailureRate = 65;
            #endregion

			#region Mask Filters
			filterComponent.FilterType = FilterType.MaskFilter;
			filterComponent.MaskFilterComponent.Stack.Clear();

    		HeightFilter heightFilter = (HeightFilter)filterComponent.MaskFilterComponent.Stack.CreateIfMissing(typeof(HeightFilter));
    		heightFilter.MinHeight = 0;
    		heightFilter.MaxHeight = 620;
    		heightFilter.AddHeightFalloff = 100;

            filterComponent.MaskFilterComponent.Stack.CreateIfMissing(typeof(SlopeFilter));
    		#endregion
		}

		protected override void Apply(Prototype proto)
    	{
			TransformStackComponent transformStackComponent = (TransformStackComponent)proto.GetSettings(typeof(TransformStackComponent));
            OverlapCheckComponent overlapCheckComponent = (OverlapCheckComponent)proto.GetSettings(typeof(OverlapCheckComponent));
			
    		#region Transform Components
    		transformStackComponent.Stack.Clear();

    		transformStackComponent.Stack.CreateIfMissing(typeof(TreeRotation));
    		transformStackComponent.Stack.CreateIfMissing(typeof(Align));
    		transformStackComponent.Stack.CreateIfMissing(typeof(PositionOffset));
    		transformStackComponent.Stack.CreateIfMissing(typeof(SlopePosition));
    		transformStackComponent.Stack.CreateIfMissing(typeof(Scale)); 
    		#endregion

    		#region OverlapCheckSettings
    		overlapCheckComponent.OverlapShape = OverlapShape.Sphere;
    		overlapCheckComponent.SphereCheck.VegetationMode = true;
    		overlapCheckComponent.SphereCheck.Priority = 1;
    		overlapCheckComponent.SphereCheck.TrunkSize = 0.4f;
    		overlapCheckComponent.SphereCheck.ViabilitySize = 2f;
    		#endregion
		}
	}
}
#endif