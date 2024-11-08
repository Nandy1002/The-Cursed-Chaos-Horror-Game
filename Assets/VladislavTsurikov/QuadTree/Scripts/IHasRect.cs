using UnityEngine;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.QuadTree.Scripts
{
    /// <summary>
    /// An interface that defines and object with a rectangle
    /// </summary>
    public interface IHasRect
    {
        [OdinSerialize]
        Rect Rectangle { get; }
    }
}
