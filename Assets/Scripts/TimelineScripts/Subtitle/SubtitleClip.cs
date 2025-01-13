using UnityEngine;
using UnityEngine.Playables;

public class SubtitleClip : PlayableAsset
{
    public string subTitleText;
    
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<SubtitleBehaviour>.Create(graph);
        var subtitleBehaviour = playable.GetBehaviour();
        subtitleBehaviour.subTitleText = subTitleText;
        return playable;
    }
}
