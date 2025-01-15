#if UNITY_EDITOR
using UnityEngine;

namespace VladislavTsurikov.EditorCoroutines.ScriptsEditor
{
	internal struct YieldWait : ICoroutineYield
	{
		public bool IsDone(float deltaTime)
		{
			return true;
		}
	}

	internal struct YieldDefault : ICoroutineYield
	{
		public bool IsDone(float deltaTime)
		{
			return true;
		}
	}

	internal struct YieldWaitForSeconds : ICoroutineYield
	{
		public float TimeLeft;

		public bool IsDone(float deltaTime)
		{
			TimeLeft -= deltaTime;
			return TimeLeft < 0;
		}
	}

	internal struct YieldCustomYieldInstruction : ICoroutineYield
	{
		public CustomYieldInstruction CustomYield;

		public bool IsDone(float deltaTime)
		{
			return !CustomYield.keepWaiting;
		}
	}

	internal struct YieldAsync : ICoroutineYield
	{
		public AsyncOperation AsyncOperation;

		public bool IsDone(float deltaTime)
		{
			return AsyncOperation.isDone;
		}
	}

	internal struct YieldNestedCoroutine : ICoroutineYield
	{
		public EditorCoroutine Coroutine;

		public bool IsDone(float deltaTime)
		{
			return Coroutine.finished;
		}
	}
}
#endif