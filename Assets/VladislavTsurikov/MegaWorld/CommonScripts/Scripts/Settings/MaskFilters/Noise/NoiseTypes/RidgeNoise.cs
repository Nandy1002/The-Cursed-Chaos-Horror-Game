using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Noise.API;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Noise.NoiseTypes
{
    /// <summary>
    /// A NoiseType implementation for Ridge noise
    /// </summary>
    [System.Serializable]
    public class RidgeNoise : NoiseType<RidgeNoise>
    {
        private static NoiseTypeDescriptor desc = new NoiseTypeDescriptor()
        {
            name = "Ridge",
            outputDir = "Assets/VladislavTsurikov/MegaWorld/CommonScripts/Scripts/Settings/MaskFilters/Shaders/NoiseLib/",
            sourcePath = "Assets/VladislavTsurikov/MegaWorld/CommonScripts/Scripts/Settings/MaskFilters/Shaders/NoiseLib/Implementation/RidgeImpl.hlsl",
            supportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
            inputStructDefinition = null
        };

        public override NoiseTypeDescriptor GetDescription() => desc;
    }
}
