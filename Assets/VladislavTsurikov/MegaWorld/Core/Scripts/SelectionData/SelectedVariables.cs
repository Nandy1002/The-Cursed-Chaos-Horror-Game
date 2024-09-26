using System;
using System.Collections.Generic;
using Prototype = VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes.Prototype;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData
{
	public class SelectedVariables
	{
		public List<Group> SelectedGroupList = new List<Group>();

		public List<SelectedPrototypeType> SelectedPrototypeTypes = new List<SelectedPrototypeType>();
		public List<Prototype> SelectedPrototypeList = new List<Prototype>();

		public Group SelectedGroup;
		public Prototype SelectedPrototype;

		public SelectedVariables()
		{
			foreach (var type in AllPrototypeTypes.TypeList)
			{
				SelectedPrototypeTypes.Add(new SelectedPrototypeType(type));
			}
		}

		public void SetAllSelectedParameters(List<Group> groupList)
		{
			ClearSelectedList();
			SetSelectedList(groupList);
			SetSelected();
		}

		public void ClearSelectedList()
		{
			SelectedGroupList.Clear();
			SelectedPrototypeList.Clear();

			foreach (var item in SelectedPrototypeTypes)
			{
				item.SelectedPrototypeList.Clear();
			}
		}

		public void SetSelected()
		{
			SetSelectedGroup();
			SetSelectedPrototype();
		}

		public void SetSelectedList(List<Group> groupList)
		{
		    for (int index = 0; index < groupList.Count; index++)
		    {
		    	if(groupList[index].Selected)
		    	{
					Group selectedGroup = groupList[index];
					SelectedGroupList.Add(selectedGroup);
					
			        SetSelectedPrototypeListFromAssets(selectedGroup.PrototypeList, SelectedPrototypeList);
			        
					foreach (var item in SelectedPrototypeTypes)
					{
						if(item.PrototypeType == selectedGroup.PrototypeType)
							SetSelectedPrototypeListFromAssets(selectedGroup.PrototypeList, item.SelectedPrototypeList);
					}
		        }
		    }
		}

		public void SetSelectedGroup()
		{
			if(SelectedGroupList.Count == 1)
			{
				SelectedGroup = SelectedGroupList[SelectedGroupList.Count - 1];
			}
			else
			{
				SelectedGroup = null;
			}
		}

		public void SetSelectedPrototype()
		{
			if(SelectedPrototypeList.Count == 1)
			{
				SelectedPrototype = SelectedPrototypeList[SelectedPrototypeList.Count - 1];
			}
			else
			{
				SelectedPrototype = null;
			}
		}

		public bool HasOneSelectedGroup()
		{
			if(SelectedGroup == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		
		public bool HasOneSelectedPrototype()
		{
			if(SelectedPrototype == null)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public bool HasOneSelectedData()
		{
			return SelectedGroup != null || SelectedPrototype != null;
		}
		
		public Prototype GetOneSelectedPrototype()
		{
			return SelectedPrototype;
		}

		public void DeleteNullValueIfNecessary(List<Group> groupList)
		{
			foreach (Group group in groupList)
			{
				if(group == null)
				{
					groupList.Remove(group);
					return;
				}
			}
		}

		public static void SetSelectedPrototypeListFromAssets<T>(List<T> baseList, List<T> setPrototypeList) where T : Prototype
        {
            foreach (T asset in baseList)
            {
	            if(asset.Selected)
	            {
		            setPrototypeList.Add((T)asset);
	            }
            }
        }

		public List<Prototype> GetSelectedPrototypes(Type prototypeType)
		{
			foreach (var item in SelectedPrototypeTypes)
			{
				if (item.PrototypeType == prototypeType)
				{
					return item.SelectedPrototypeList;
				}
			}

			return null;

		}
	}
}