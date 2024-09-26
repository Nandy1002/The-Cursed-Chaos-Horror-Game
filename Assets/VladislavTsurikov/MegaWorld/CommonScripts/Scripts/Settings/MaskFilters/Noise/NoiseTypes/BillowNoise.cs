using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Noise.API;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Noise.NoiseTypes
{
    /// <summary>
    /// A NoiseType implementation for Billow noise
    /// </summary>
    [System.Serializable]
    public class BillowNoise : NoiseType<BillowNoise>
    {
        private static NoiseTypeDescriptor desc =  new NoiseTypeDescriptor()
        {
            name = "Billow",
            outputDir = "Assets/VladislavTsurikov/MegaWorld/CommonScripts/Scripts/Settings/MaskFilters/Shaders/NoiseLib/",
            sourcePath = "Assets/VladislavTsurikov/MegaWorld/CommonScripts/Scripts/Settings/MaskFilters/Shaders/NoiseLib/Implementation/BillowImpl.hlsl",
            supportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
            inputStructDefinition = null
            // inputStructDefinition = new List<HlslInput>()
            // {
            //     new HlslInput() { name = "scale", valueType = HlslValueType.Float4, float4Value = new HlslFloat4(1,1,1,1) }
            // }
        };

        public override NoiseTypeDescriptor GetDescription() => desc;
    }
}