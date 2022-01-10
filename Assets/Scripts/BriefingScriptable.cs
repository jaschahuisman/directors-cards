using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "00_briefing_name", menuName = "Theatersport/Briefing")]
public class BriefingScriptable : ScriptableObject
{
    public new string name;
    public TimelineAsset timeline;
    public PlayerRole playerRole1;
    public PlayerRole playerRole2;
}