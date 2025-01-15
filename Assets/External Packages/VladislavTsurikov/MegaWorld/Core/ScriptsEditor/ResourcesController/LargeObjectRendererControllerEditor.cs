#if UNITY_EDITOR
#if INSTANT_RENDERER
using UnityEngine;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.InstantRenderer.LargeObjectRenderer.Scripts;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ResourcesController 
{
    public static class LargeObjectRendererControllerEditor 
    {
        public static void OnGUI(Group group)
		{
			GUILayout.BeginHorizontal();
         	{
				GUILayout.Space(CustomEditorGUILayout.GetCurrentSpace());
				if(CustomEditorGUILayout.ClickButton("Get Resources", ButtonStyle.General, ButtonSize.ClickButton))
				{
					CreatePrototypeFromLargeObjectRenderer(group);
				}
				GUILayout.Space(5);
			}
			GUILayout.EndHorizontal();
		}
        
        public static void CreatePrototypeFromLargeObjectRenderer(Group group)
        {
	        foreach (PrototypeLargeObject prototype in LargeObjectRenderer.Instance.PrototypesPackage.PrototypeList)
	        {
		        group.AddMissingPrototype(prototype.Prefab);
	        }
        }
    }
}
#endif
#endif