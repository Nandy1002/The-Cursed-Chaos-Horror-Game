#if UNITY_EDITOR
namespace VladislavTsurikov.EditorCoroutines.ScriptsEditor
{
    public interface ICoroutineYield
	{
		bool IsDone(float deltaTime);
	}
}
#endif