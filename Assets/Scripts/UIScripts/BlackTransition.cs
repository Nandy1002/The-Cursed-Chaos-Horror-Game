using UnityEngine;

public class BlackTransition : MonoBehaviour
{
    public void FadeStart()
	{
		LeanTween.alpha (gameObject, 0f, 1f).setEase (LeanTweenType.linear).setOnComplete( FadeFinished );
	}

	public void FadeFinished()
	{
		LeanTween.alpha (gameObject, 1f, 1f).setEase (LeanTweenType.linear).setOnComplete( FadeStart );
	}
}
