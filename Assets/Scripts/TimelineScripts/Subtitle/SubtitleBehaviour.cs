using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class SubtitleBehaviour : PlayableBehaviour
{
    public string subTitleText;

    // public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    // {
    //     TextMeshProUGUI text = playerData as TextMeshProUGUI;
    //     if (text == null)
    //         text.text = "";
    //     text.text = subTitleText;
    //     text.color = Color.white;
    // }
}
