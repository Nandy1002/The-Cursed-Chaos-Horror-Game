using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.Utility
{
    public static class AllAvailableGroups
	{
        private static List<Group> _groupList = new List<Group>();

        static AllAvailableGroups()
        {
            Group[] groups = Resources.FindObjectsOfTypeAll(typeof(Group)) as Group[];

            foreach (Group group in groups)
            {
                AddGroup(group);
            }
        } 

        public static List<Group> GetAllGroups()
        {
            return _groupList;
        }

        public static void AddGroup(Group group)
        {
            if(!_groupList.Contains(group))
            {
                _groupList.Add(group);
                CreateMegaWorldSettings.CreateGroupSettings(group);
            }
        }
	}
}