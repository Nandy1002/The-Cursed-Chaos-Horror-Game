using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;

namespace VladislavTsurikov.MegaWorld.TextureStamperTool.Scripts.AutoRespawn
{
    public class AutoRespawn 
    {
        public Group _modifiedType = null;
        private TextureStamperTool _textureStamperTool;

        public AutoRespawn(Group group, TextureStamperTool textureStamperTool)
        {
            _textureStamperTool = textureStamperTool;
            _modifiedType = group;
        }

        public void TypeHasChanged()
        {
            if(_textureStamperTool.Area.UseSpawnCells == false)
			{
                if(_modifiedType != null)
			    {
                    _textureStamperTool.RunSpawn();
			    }
            }
        }
    }
}