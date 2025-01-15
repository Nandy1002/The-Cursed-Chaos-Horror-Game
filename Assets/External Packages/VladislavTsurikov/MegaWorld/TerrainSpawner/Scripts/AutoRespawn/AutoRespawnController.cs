#if UNITY_EDITOR
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData;
using VladislavTsurikov.MegaWorld.Core.Scripts.SelectionData.Prototypes;

namespace VladislavTsurikov.MegaWorld.TerrainSpawner.Scripts.AutoRespawn
{
    public class AutoRespawnController
    {
        private AutoRespawn _autoRespawn;

        private Timer.Scripts.Timer _timer;

        public void StartAutoRespawn(Group group, TerrainSpawner stamperTool)
        {
            _autoRespawn = new AutoRespawn(group, stamperTool);
            
            if (_timer == null || _timer.IsDone)
            {
                _timer = Timer.Scripts.Timer.Register(stamperTool.StamperToolControllerSettings.DelayAutoRespawn, () => _autoRespawn.TypeHasChanged());
            }
        }

        public void StartAutoRespawn(PrototypeTerrainDetail proto, TerrainSpawner stamperTool)
        {
            _autoRespawn = new AutoRespawn(proto, stamperTool);
            
            if (_timer == null || _timer.IsDone)
            {
                _timer = Timer.Scripts.Timer.Register(stamperTool.StamperToolControllerSettings.DelayAutoRespawn, () => _autoRespawn.TypeHasChanged());
            }
        }
    }
}
#endif