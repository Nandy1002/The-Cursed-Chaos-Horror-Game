using VladislavTsurikov.SceneDataSystem.Scripts.Utility;

namespace VladislavTsurikov.SceneDataSystem.Scripts
{
    public static class FindScene
    {
        private static SceneBVHTree _sceneObjectsCells = new SceneBVHTree();
        private static SceneBVHTree _streamingCells = new SceneBVHTree();

        public static SceneBVHTree SceneObjectsCells => _sceneObjectsCells;
        public static SceneBVHTree StreamingCells => _streamingCells;

        public static void ClearSceneCells()
        {
            SceneObjectsCells.Clear();
            StreamingCells.Clear();
        }

        public static void RemoveSceneCell(SceneDataManager sceneDataManager)
        {
            SceneObjectsCells.RemoveNodes(sceneDataManager);
            StreamingCells.RemoveNodes(sceneDataManager);
        }

        public static void AddSceneCell(SceneDataManager sceneDataManager)
        {
            SceneObjectsCells.RegisterSceneDataManager(sceneDataManager, SceneObjectsCellUtility.GetSceneObjectsCell(sceneDataManager));
            StreamingCells.RegisterSceneDataManager(sceneDataManager, sceneDataManager.StreamingCell);
        }
    }
}