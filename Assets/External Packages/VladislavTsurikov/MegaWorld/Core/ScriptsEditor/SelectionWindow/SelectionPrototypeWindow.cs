#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;
using VladislavTsurikov.MegaWorld.Core.Scripts.TemplatesSystem;
using GUIUtility = VladislavTsurikov.Utility.GUIUtility;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.SelectionWindow
{
	public abstract class SelectionPrototypeWindow
	{
		protected readonly List<ClipboardPrototype> ClipboardPrototypes = new List<ClipboardPrototype>();
		protected readonly TemplateStackEditor TemplateStackEditor = new TemplateStackEditor();
		protected readonly Type ToolType;
		protected readonly BasicData Data;
		
		public SelectionPrototypeWindow(BasicData basicData, Type toolType)
		{
			Data = basicData;
			ToolType = toolType;

			foreach (var type in AllPrototypeTypes.TypeList)
			{
				ClipboardPrototypes.Add(new ClipboardPrototype(type));
			}
		}

		public abstract void OnGUI();

		protected GenericMenu PrototypeMenu(Icon icon)
        {
            GenericMenu menu = new GenericMenu();

            Prototype prototype = (Prototype)icon;
			
			UnityEngine.Object necessaryData = prototype.GetNecessaryData();

			if(necessaryData != null)
			{
				menu.AddItem(new GUIContent("Reveal in Project"), false, GUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(necessaryData)));
				menu.AddSeparator ("");
			}

			menu.AddItem(new GUIContent("Delete"), false, GUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.DeleteSelectedPrototype(Data.SelectedVariables.SelectedGroup)));

			ClipboardPrototype clipboardPrototype = ClipboardPrototype.GetCurrentClipboardPrototype(Data.SelectedVariables.SelectedGroup.PrototypeType, ClipboardPrototypes);

			clipboardPrototype?.PrototypeMenu(menu, Data.SelectedVariables, ToolType);
            TemplateStackEditor?.ShowMenu(menu, ToolType, Data.SelectedVariables.SelectedGroup, Data.SelectedVariables);
			
			menu.AddSeparator ("");
            menu.AddItem(new GUIContent("Select All"), false, GUIUtility.ContextMenuCallback, new Action(() => SelectionPrototypeUtility.SetSelectedAllPrototypes(Data.SelectedVariables.SelectedGroup, true)));
			menu.AddItem(new GUIContent("Active"), prototype.Active, GUIUtility.ContextMenuCallback, new Action(() => 
				Data.SelectedVariables.SelectedPrototypeList.ForEach ((proto) => { proto.Active = !proto.Active;})));


            return menu;
        }
    }

	public class GeneralSelectionPrototypeWindow : SelectionPrototypeWindow
    {
		private bool _selectPrototypeFoldout = true;
		private IconStackEditor _iconStackEditor = new IconStackEditor(true);

		public GeneralSelectionPrototypeWindow(BasicData basicData, Type toolType) : base(basicData, toolType)
		{
			
		}

        public override void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            
            if(Data.SelectedVariables.SelectedGroup != null)
            {
	            _selectPrototypeFoldout = CustomEditorGUILayout.Foldout(_selectPrototypeFoldout, "Prototypes " + "(" + Data.SelectedVariables.SelectedGroup.GetPrototypeTypeName() + ")");
            }
            else
            {
	            _selectPrototypeFoldout = CustomEditorGUILayout.Foldout(_selectPrototypeFoldout, "Prototypes ");
            }

            EditorGUILayout.EndHorizontal();

			if(_selectPrototypeFoldout)
			{
				++EditorGUI.indentLevel;

				if(Data.SelectedVariables.SelectedGroup == null)
				{
					_iconStackEditor.OnGUI("To Draw Prototype, you need to select one group");
				}
				else
				{
					_iconStackEditor.AddIconCallback = Data.SelectedVariables.SelectedGroup.AddMissingPrototype;
					_iconStackEditor.IconMenuCallback = PrototypeMenu;
					_iconStackEditor.OnGUI(Data.SelectedVariables.SelectedGroup.PrototypeList, Data.SelectedVariables.SelectedGroup.PrototypeType);
				}

				--EditorGUI.indentLevel;
			}
        }
    }
}
#endif