#if UNITY_EDITOR
using System;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.ScriptsEditor.SelectionWindow;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.Window
{
    public class DrawBasicData 
    {
		public SelectionGroupWindow SelectionGroupWindow;
		public SelectionPrototypeWindow SelectionPrototypeWindow;

		public DrawBasicData(Type selectionGroupWindowType, Type selectionPrototypeWindowType, BasicData basicData, Type toolType)
		{
			if(selectionPrototypeWindowType != null)
			{
				SelectionPrototypeWindow = (SelectionPrototypeWindow)Activator.CreateInstance(selectionPrototypeWindowType, basicData, toolType);
			}
			else
			{
				SelectionPrototypeWindow = new GeneralSelectionPrototypeWindow(basicData, toolType);
			}

			if(selectionGroupWindowType != null)
			{
				SelectionGroupWindow = (SelectionGroupWindow)Activator.CreateInstance(selectionGroupWindowType, basicData, toolType);
			}
			else
			{
				SelectionGroupWindow = new GeneralSelectionGroupWindow(basicData, toolType);
			}
		}

		public virtual void OnGUI(BasicData basicData, Type toolType)
		{
			SelectionGroupWindow?.OnGUI();
			SelectionPrototypeWindow?.OnGUI();
		}
	}
}
#endif