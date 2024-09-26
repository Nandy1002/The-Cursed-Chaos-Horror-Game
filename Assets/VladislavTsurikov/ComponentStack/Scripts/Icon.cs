using UnityEngine;

namespace VladislavTsurikov.ComponentStack.Scripts
{
    public interface Icon 
    {
        bool Selected
        {
            get; set;
        }

#if UNITY_EDITOR
        Texture2D GetPreviewTexture();
        bool IsRedIcon();
        string GetName();
#endif
    }
}