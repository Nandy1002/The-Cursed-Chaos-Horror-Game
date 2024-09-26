using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Other;
#if UNITY_EDITOR
using VladislavTsurikov.EditorCoroutines.ScriptsEditor;
#endif

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.ScatterSettingsSystem
{
    [Serializable]
    public class ScatterStack : ComponentStack<Scatter>
    {
        public void Samples(AreaVariables areaVariables, Action<Vector2> onAddSample = null)
        {
            List<Scatter> enabledScatter = new List<Scatter>(ComponentList);
            enabledScatter.RemoveAll(scatter => !scatter.Active);

            List<Vector2> samples = new List<Vector2>();

            for (int i = 0; i < enabledScatter.Count; i++)
            {
                enabledScatter[i].Samples(areaVariables, samples, i == enabledScatter.Count - 1 ? onAddSample : null);
            }
        }
        
#if UNITY_EDITOR
        public IEnumerator SamplesCoroutine(AreaVariables areaVariables, Action<Vector2> onSpawn)
        {
            List<Scatter> enabledScatter = new List<Scatter>(ComponentList);
            enabledScatter.RemoveAll(scatter => !scatter.Active);
            
            List<Vector2> samples = new List<Vector2>();
            
            for (int i = 0; i < enabledScatter.Count; i++)
            {
                EditorCoroutine editorCoroutine = EditorCoroutines.ScriptsEditor.EditorCoroutines.StartCoroutine(enabledScatter[i].SamplesCoroutine(areaVariables, samples, i == enabledScatter.Count - 1 ? onSpawn : null), this);
                yield return editorCoroutine;
            }
        }
#endif
    }
}