using System;
using System.Collections.Generic;

namespace VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData
{
    public class SelectedPrototypeType
    {
        public readonly Type PrototypeType;
        public readonly List<Prototypes.Prototype> SelectedPrototypeList = new List<Prototypes.Prototype>();

        public SelectedPrototypeType(Type prototypeType)
        {
            PrototypeType = prototypeType;
        }
    }
}