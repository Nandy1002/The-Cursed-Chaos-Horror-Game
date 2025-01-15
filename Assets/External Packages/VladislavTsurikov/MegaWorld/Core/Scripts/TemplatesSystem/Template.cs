#if UNITY_EDITOR
using System;
using System.Linq;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.TemplatesSystem
{
	public class Template 
    {
		public void Apply(Type[] supportedResourceTypes, Group group, Prototype proto)
		{
			if(supportedResourceTypes.Contains(group.PrototypeType))
			{
				Apply(group);
				Apply(proto);
			}
		}

		protected virtual void Apply(Group group){}
		protected virtual void Apply(Prototype proto){}
	}
}
#endif