#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Scripts;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.Extensions.Scripts;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.ToolsSystem;
using VladislavTsurikov.MegaWorld.Core.Scripts.Utility;
using GUIUtility = VladislavTsurikov.Utility.GUIUtility;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.SelectionWindow
{
	public abstract class SelectionGroupWindow 
    {
	    protected List<ClipboardGroup> ClipboardGroups = new List<ClipboardGroup>();
	    protected readonly Type ToolType;
		protected readonly BasicData Data;
		
		public SelectionGroupWindow(BasicData data, Type toolType)
		{
			Data = data;
			ToolType = toolType;
			
			foreach (var type in AllPrototypeTypes.TypeList)
			{
				ClipboardGroups.Add(new ClipboardGroup(type));
			}
		}

		public abstract void OnGUI();

		protected void GroupWindowMenu()
        {
            GenericMenu menu = new GenericMenu();
            
            foreach (var type in ToolUtility.GetSupportedPrototypeTypes(ToolType))
            {
	            menu.AddItem(new GUIContent("Add Group/" + type.GetAttribute<NameAttribute>().Name), false, GUIUtility.ContextMenuCallback, new Action(() => AddGroup(Data.GroupList, type)));
            }

            List<Group> groups = AllAvailableGroups.GetAllGroups();

            if (groups.Count != 0)
            {
	            menu.AddSeparator ("");
	            menu.AddItem(new GUIContent("Show All Group"), false, GUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(groups[0])));
            }

            menu.ShowAsContext();
        }

		public GenericMenu GroupMenu(Icon icon)
        {
            GenericMenu menu = new GenericMenu();

			Group group = (Group)icon;

            menu.AddItem(new GUIContent("Reveal in Project"), false, GUIUtility.ContextMenuCallback, new Action(() => EditorGUIUtility.PingObject(group)));
            menu.AddSeparator ("");

            menu.AddItem(new GUIContent("Delete"), false, GUIUtility.ContextMenuCallback, new Action(() => DeleteSelectedGroups(Data.GroupList)));

            ClipboardGroup.GroupMenu(menu, Data.SelectedVariables, ClipboardGroups, ToolType);

            menu.AddSeparator ("");
			menu.AddItem(new GUIContent("Rename"), group.Renaming, GUIUtility.ContextMenuCallback, new Action(() => group.Renaming = !group.Renaming));

            return menu;
        }

		public static void AddGroup(List<Group> groupList, Type prototypeType)
        {
            Group newAsset = ProfileFactory.CreateGroup(prototypeType);
            groupList.Add(newAsset);       
        }

        public static void DeleteSelectedGroups(List<Group> groupList)
        {
            groupList.RemoveAll((group) => group.Selected);
        }
	}

	public class GeneralSelectionGroupWindow : SelectionGroupWindow
    {
		private bool _selectPrototypeFoldout = true;
		private IconStackEditor _iconStackEditor = new IconStackEditor(true);

		public GeneralSelectionGroupWindow(BasicData data, Type toolType) : base(data, toolType)
		{
			_iconStackEditor.AddIconCallback = AddGroup;
			_iconStackEditor.DrawIconRect = DrawIconRectForType;
			_iconStackEditor.DrawWindowMenu = GroupWindowMenu;
			_iconStackEditor.IconMenuCallback = GroupMenu;
			_iconStackEditor.IconColor = SetIconColor;
			_iconStackEditor.IconSelected = SetSelectedAllPrototypes;
			_iconStackEditor.ZeroIconsWarning = "Drag & Drop Type Here";
			_iconStackEditor.DraggedIconType = true;
		}

        public override void OnGUI()
        {
            _selectPrototypeFoldout = CustomEditorGUILayout.Foldout(_selectPrototypeFoldout, "Groups");
            
			if(_selectPrototypeFoldout)
			{
				++EditorGUI.indentLevel;
				_iconStackEditor.OnGUI(Data.GroupList, typeof(Group));
				--EditorGUI.indentLevel;
			}
        }

		public static void SetSelectedAllPrototypes(Icon icon)
        {
			Group group = (Group)icon;

            group.PrototypeList.ForEach(proto => proto.Selected = group.Selected);
        }

		public Icon AddGroup(UnityEngine.Object obj) 
		{
			Group group = (Group)obj;

			if(Data.GroupList.Contains(group) == false)
			{
				Data.GroupList.Add(group); 
				return group;
			}

			return null;
		}

		private void DrawIconRectForType(Event e, Icon icon, Rect iconRect, Color textColor, Color rectColor)
		{
			GUIStyle labelTextForIcon = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForIcon);

			Group group = (Group)icon;

			EditorGUI.DrawRect(iconRect, rectColor);
                
			// Prefab preview 
            if(e.type == EventType.Repaint)
            {
                Rect previewRect = new Rect(iconRect.x+2, iconRect.y+2, iconRect.width-4, iconRect.width-4);
                Color dimmedColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);

                Rect[] icons =
                {   new Rect(previewRect.x, previewRect.y, previewRect.width / 2 - 1, previewRect.height / 2 - 1),
                    new Rect(previewRect.x + previewRect.width / 2, previewRect.y, previewRect.width / 2, previewRect.height / 2 - 1),
                    new Rect(previewRect.x, previewRect.y + previewRect.height/2, previewRect.width / 2 - 1, previewRect.height / 2),
                    new Rect(previewRect.x + previewRect.width / 2, previewRect.y + previewRect.height / 2, previewRect.width / 2, previewRect.height / 2)
                };

				Texture2D[] textures = new Texture2D[4];
				
				for(int i = 0, j = 0; i < group.PrototypeList.Count && j < 4; i++)
				{
					if(group.PrototypeList[i].GetNecessaryData() != null)
					{
						textures[j] = group.PrototypeList[i].GetPreviewTexture();
						j++;
					}
				}

				for(int i = 0; i < 4; i++)
                {
                    if(textures[i] != null)
                    {
						EditorGUI.DrawPreviewTexture(icons[i], textures[i]);
                    }
                    else
					{
						EditorGUI.DrawRect(icons[i], dimmedColor);
					}
                }

				labelTextForIcon.normal.textColor = textColor;
				labelTextForIcon.Draw(iconRect, SelectedWindowUtility.GetShortNameForIcon(group.name, _iconStackEditor.IconWidth), false, false, false, false);
            }
		}

		private void SetIconColor(Icon icon, out Color textColor, out Color rectColor)
		{
			Group group = (Group)icon;
			textColor = EditorColors.Instance.LabelColor;

			if(group.Renaming)
			{
				rectColor = EditorColors.Instance.orangeNormal.WithAlpha(0.3f);
				
				if (EditorGUIUtility.isProSkin)
            	{
					textColor = EditorColors.Instance.orangeNormal; 
            	}
            	else
            	{
					textColor = EditorColors.Instance.orangeDark;
				}		
			}	

			else if(group.Selected)
			{
				rectColor = EditorColors.Instance.ToggleButtonActiveColor;
			}
			else
			{
				rectColor = EditorColors.Instance.ToggleButtonInactiveColor;
			}
		}
    }
}
#endif