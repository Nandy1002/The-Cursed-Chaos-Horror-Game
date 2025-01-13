using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class SubtitleTrackMixer : PlayableBehaviour
{
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        TextMeshProUGUI text = playerData as TextMeshProUGUI;
    if (!text) { return; }

    string clipText = "";
    float clipTextAlpha = 0f;
    int inputCount = playable.GetInputCount();

    for (int i = 0; i < inputCount; i++)
    {
        float inputWeight = playable.GetInputWeight(i);
        ScriptPlayable<SubtitleBehaviour> inputPlayable = (ScriptPlayable<SubtitleBehaviour>)playable.GetInput(i);
        SubtitleBehaviour input = inputPlayable.GetBehaviour();

        if (inputWeight > 0)
        {
            clipText += input.subTitleText + " ";
            clipTextAlpha += inputWeight;
        }
    }

    text.text = clipText.Trim();
    text.color = new Color(1, 1, 1, Mathf.Clamp01(clipTextAlpha));
    }
}
