#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.OverlapCheckSettings;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.OverlapCheckSettings.OverlapChecks;

namespace VladislavTsurikov.MegaWorld.CommonScripts.ScriptsEditor.Settings.OverlapCheckSettings
{
	[SettingsEditor(typeof(OverlapCheckComponent))]
    public class OverlapCheckComponentEditor : ComponentEditor
    {
        private OverlapCheckComponent _component;
        
        public override void OnEnable()
        {
	        _component = (OverlapCheckComponent)Target;
        }

        public override void OnGUI()
        {
	        _component.OverlapShape = (OverlapShape)CustomEditorGUILayout.EnumPopup(overlapShape, _component.OverlapShape);

	        EditorGUI.indentLevel++;

	        switch (_component.OverlapShape)
	        {
		        case OverlapShape.Bounds:
		        {
			        BoundsCheck boundsCheck = _component.BoundsCheck;

			        boundsCheck.BoundsType = (BoundsCheckType)CustomEditorGUILayout.EnumPopup(boundsType, boundsCheck.BoundsType);

			        if(boundsCheck.BoundsType == BoundsCheckType.Custom)
			        {
				        boundsCheck.UniformBoundsSize = CustomEditorGUILayout.Toggle(uniformBoundsSize, boundsCheck.UniformBoundsSize);

				        if(boundsCheck.UniformBoundsSize)
				        {
					        boundsCheck.BoundsSize.x = CustomEditorGUILayout.FloatField(boundsSize, boundsCheck.BoundsSize.x);

					        boundsCheck.BoundsSize.z = boundsCheck.BoundsSize.x;
					        boundsCheck.BoundsSize.y = boundsCheck.BoundsSize.x;
				        }
				        else
				        {
					        boundsCheck.BoundsSize = CustomEditorGUILayout.Vector3Field(boundsSize, boundsCheck.BoundsSize);
				        }

				        boundsCheck.MultiplyBoundsSize = CustomEditorGUILayout.Slider(multiplyBoundsSize, boundsCheck.MultiplyBoundsSize, 0, 5);
			        }
			        else if(boundsCheck.BoundsType == BoundsCheckType.BoundsPrefab)
			        {
				        boundsCheck.MultiplyBoundsSize = CustomEditorGUILayout.Slider(multiplyBoundsSize, boundsCheck.MultiplyBoundsSize, 0, 5);
			        }
			        break;
		        }
		        case OverlapShape.Sphere:
		        {
			        SphereCheck sphereCheck = _component.SphereCheck;

			        sphereCheck.VegetationMode = CustomEditorGUILayout.Toggle(vegetationMode, sphereCheck.VegetationMode);

			        if(sphereCheck.VegetationMode)
			        {
				        sphereCheck.Priority = CustomEditorGUILayout.IntField(priority, sphereCheck.Priority);
				        sphereCheck.TrunkSize = CustomEditorGUILayout.Slider(trunkSize, sphereCheck.TrunkSize, 0, 10);
				        sphereCheck.ViabilitySize = CustomEditorGUILayout.FloatField(viabilitySize, sphereCheck.ViabilitySize);

				        if(sphereCheck.ViabilitySize < _component.SphereCheck.TrunkSize)
				        {
					        sphereCheck.ViabilitySize = _component.SphereCheck.TrunkSize;
				        }
			        }
			        else
			        {
				        sphereCheck.Size = CustomEditorGUILayout.FloatField(size, sphereCheck.Size);
			        }
			        break;
		        }
	        }

	        EditorGUI.indentLevel--;

	        CollisionCheck collisionCheck = _component.CollisionCheck;

	        collisionCheck.collisionCheckType =  CustomEditorGUILayout.Toggle(new GUIContent("Collision Check"), collisionCheck.collisionCheckType);
				
	        if(collisionCheck.collisionCheckType)
	        {
		        EditorGUI.indentLevel++;

		        collisionCheck.multiplyBoundsSize = CustomEditorGUILayout.Slider(multiplyBoundsSize, collisionCheck.multiplyBoundsSize, 0, 10);
		        collisionCheck.checkCollisionLayers = CustomEditorGUILayout.LayerField(new GUIContent("Check Collision Layers"), collisionCheck.checkCollisionLayers);

		        EditorGUI.indentLevel--;
	        }
        }

		[NonSerialized]
		public GUIContent overlapShape = new GUIContent("Overlap Shape", "What shape will be checked for intersection with other prototypes. Overlap Shape only works with added prototypes in MegaWorld. Overlap Chap can be Bounds and Sphere.");

		#region Bounds Check
		[NonSerialized]
		public GUIContent boundsType = new GUIContent("Bounds Type", "Which Bounds will be used.");
		[NonSerialized]
		public GUIContent uniformBoundsSize = new GUIContent("Uniform Bounds Size", "Each side of the Bounds has the same size value.");
		[NonSerialized]
		public GUIContent boundsSize = new GUIContent("Bounds Size", "Lets you choose the size of the vector for bounds size.");
		[NonSerialized]
		public GUIContent multiplyBoundsSize = new GUIContent("Multiply Bounds Size", "Allows you to resize the bounds.");
		#endregion

		#region Sphere Variables
		[NonSerialized]
		public GUIContent vegetationMode = new GUIContent("Vegetation Mode", "Allows you to use the priority system, which allows for example small trees to spawn under a large tree.");
		[NonSerialized]
		public GUIContent priority = new GUIContent("Priority", "Sets the ability of the object so that the object can spawn around the Viability Size of another object whose this value is less.");
		[NonSerialized]
		public GUIContent trunkSize = new GUIContent("Trunk Size", "Sets the size of the trunk. Other objects will never be spawn in this size.");
		[NonSerialized]
		public GUIContent viabilitySize = new GUIContent("Viability Size", " This is size in which other objects will not be spawned if Priority is less.");
		[NonSerialized]
		public GUIContent size = new GUIContent("Size", "The size of the sphere that will not spawn.");
		#endregion
    }
}
#endif