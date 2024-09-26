using System;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;

namespace VladislavTsurikov.MegaWorld.Core.ScriptsEditor.ToolsSystem
{
    public interface IToolEditor
    {
	    BasicData BasicData { get; }
        Type TargetType { get; }
    }
}