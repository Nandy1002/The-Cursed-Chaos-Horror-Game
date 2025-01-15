using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;
using VladislavTsurikov.ComponentStack.ScriptsEditor;
using VladislavTsurikov.MegaWorld.Core.Scripts.SettingsSystem;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters
{
    [Serializable]
    [Name("Mask Filter Settings")]
    public class MaskFilterComponent : BaseComponent
    {
	    public MaskFilterStack Stack = new MaskFilterStack();
        public MaskFilterContext FilterContext;
		public Texture2D FilterMaskTexture2D;
    }
}