using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Scripts;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters
{
    public enum BlendMode
    {
        Multiply,
        Add,
    }

    public enum ColorSpaceForBrushMaskFilter
    {
        СustomColor,
        Colorful,
        Heightmap
    }

    [Serializable]
    public class MaskFilterStack : ComponentStack<MaskFilter>
    {
        public void Eval(MaskFilterContext fc)
        {
            int count = ComponentList.Count;

            RenderTexture prev = RenderTexture.active;

            RenderTexture[] rts = new RenderTexture[2];

            int srcIndex = 0;
            int destIndex = 1;

            rts[0] = RenderTexture.GetTemporary(fc.DestinationRenderTexture.descriptor);
            rts[1] = RenderTexture.GetTemporary(fc.DestinationRenderTexture.descriptor);

            rts[0].enableRandomWrite = true;
            rts[1].enableRandomWrite = true;

            Graphics.Blit(Texture2D.whiteTexture, rts[0]);
            Graphics.Blit(Texture2D.blackTexture, rts[1]); //don't remove this! needed for compute shaders to work correctly.

            for( int i = 0; i < count; ++i )
            {
                if(ComponentList[i].Active)
                {
                    fc.SourceRenderTexture = rts[srcIndex];
                    fc.DestinationRenderTexture = rts[destIndex];

                    ComponentList[ i ].Eval(fc, i);

                    destIndex += srcIndex;
                    srcIndex = destIndex - srcIndex;
                    destIndex = destIndex - srcIndex;
                }
            }

            Graphics.Blit(rts[srcIndex], fc.Output);

            RenderTexture.ReleaseTemporary(rts[0]);
            RenderTexture.ReleaseTemporary(rts[1]);

            RenderTexture.active = prev;
        }
    }
}