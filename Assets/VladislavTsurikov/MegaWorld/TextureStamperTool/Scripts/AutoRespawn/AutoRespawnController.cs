using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;

namespace VladislavTsurikov.MegaWorld.TextureStamperTool.Scripts.AutoRespawn
{
    public class AutoRespawnController
    {
        private AutoRespawn _autoRespawn;

        private Timer.Scripts.Timer _timer;

        public void StartAutoRespawn(Group group, TextureStamperTool textureStamperTool)
        {
            if(!textureStamperTool.textureStamperToolControllerSettings.AutoRespawn)
                return;
            
            _autoRespawn = new AutoRespawn(group, textureStamperTool);

            if (_timer == null || _timer.IsDone)
            {
                _timer = Timer.Scripts.Timer.Register(textureStamperTool.textureStamperToolControllerSettings.DelayAutoRespawn, () => _autoRespawn.TypeHasChanged());
            }
        }
    }
}