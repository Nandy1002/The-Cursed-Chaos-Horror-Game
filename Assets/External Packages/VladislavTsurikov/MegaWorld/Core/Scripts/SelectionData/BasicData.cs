using System;
using System.Collections.Generic;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.Window;
#if UNITY_EDITOR

#endif

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData
{
    [Serializable]
    public class BasicData
    {
        public List<Group> GroupList = new List<Group>();
        
        public readonly SelectedVariables SelectedVariables = new SelectedVariables();
        
#if UNITY_EDITOR
        public BasicDataEditor BasicDataEditor = new BasicDataEditor();

        public void OnGUI(DrawBasicData drawBasicData, Type toolType)
        {
            BasicDataEditor.OnGUI(this, drawBasicData, toolType);
        }

        public void SaveAllData()
        {
            GroupList.RemoveAll(group => group == null);
            
			foreach (Group group in GroupList)
			{
                group.Save();
            }
		}
#endif
    }
}