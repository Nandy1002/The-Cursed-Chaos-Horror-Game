using UnityEditor;
using VladislavTsurikov.Scripts;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.PreferencesSettings
{
    [Location("MegaWorld/AdvancedSettings")]
    public class AdvancedSettings : SerializedScriptableObjectSingleton<AdvancedSettings>
    {
        public EditorSettings EditorSettings = new EditorSettings();
        public VisualisationSettings VisualisationSettings = new VisualisationSettings();

#if UNITY_EDITOR
        public void Save()
        {
            EditorUtility.SetDirty(this);
        }
#endif
    }
}