using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "b_title", menuName = "Theatersport/Briefing")]
public class Briefing : ScriptableObject
{
    public string title;
    public TimelineAsset timeline;
    public Role playerRole1;
    public Role playerRole2;
}
