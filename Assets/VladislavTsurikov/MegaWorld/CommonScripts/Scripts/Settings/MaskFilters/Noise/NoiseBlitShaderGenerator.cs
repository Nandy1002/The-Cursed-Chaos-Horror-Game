using VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Noise.API;

namespace VladislavTsurikov.MegaWorld.CommonScripts.Scripts.Settings.MaskFilters.Noise
{
    internal class NoiseBlitShaderGenerator : NoiseShaderGenerator<NoiseBlitShaderGenerator>
    {
        private static ShaderGeneratorDescriptor m_desc = new ShaderGeneratorDescriptor()
        {
            name = "NoiseBlit",
            shaderCategory = "Hidden/TerrainTools/Noise/NoiseBlit",
            outputDir = "Assets/VladislavTsurikov/MegaWorld/CommonScripts/Scripts/Settings/MaskFilters/Shaders/Generated/",
            templatePath = "Assets/VladislavTsurikov/MegaWorld/CommonScripts/Scripts/Settings/MaskFilters/Shaders/NoiseLib/Templates/Blit.noisehlsltemplate"
        };

        public override ShaderGeneratorDescriptor GetDescription() => m_desc;   
    }
}