#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.Window
{
    public partial class MegaWorldWindow : BaseWindow<MegaWorldWindow>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            
            hideFlags = HideFlags.HideAndDontSave;

            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.modifierKeysChanged += Repaint;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            SceneView.duringSceneGui -= OnSceneGUI;
            EditorApplication.modifierKeysChanged -= Repaint;

            WindowDataPackage.Instance.ToolComponentsEditor.DisableAllTools();

            WindowDataPackage.Instance.Save();
        }

        private void OnSceneGUI(SceneView sceneView)
        { 
            WindowDataPackage.Instance.SelectedVariables.DeleteNullValueIfNecessary(WindowDataPackage.Instance.BasicData.GroupList);
            WindowDataPackage.Instance.SelectedVariables.SetAllSelectedParameters(WindowDataPackage.Instance.BasicData.GroupList);
            UpdateSceneViewEvent();

            WindowDataPackage.Instance.WindowToolStack.DoSelectedTool();
        }
    }
} 
#endif