#if UNITY_EDITOR
using UnityEditor;
using VladislavTsurikov.CustomGUI.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.SelectionWindow;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.SelectionData
{
	[CustomEditor(typeof(Group))]
    public class GroupEditor : Editor
    {
        private Group group;

		public GeneralSelectionPrototypeWindow SelectionPrototypeWindow;
        public BasicData BasicData = new BasicData();

        private void OnEnable()
        {
            group = (Group)target;
        }

        public override void OnInspectorGUI()
        {
            CustomEditorGUILayout.IsInspector = true;
            OnGUI();
        }

		public void OnGUI()
		{
			BasicData = new BasicData();
            BasicData.GroupList.Add(group);
            BasicData.SelectedVariables.SelectedGroupList.Add(group);
            BasicData.SelectedVariables.SelectedGroup = group;

            if(SelectionPrototypeWindow == null)
            {
                SelectionPrototypeWindow = new GeneralSelectionPrototypeWindow(BasicData, null);
            }

			SelectionPrototypeWindow.OnGUI();
		}
	}
}
#endif
