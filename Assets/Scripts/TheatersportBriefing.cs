using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nieuwe Briefing", menuName = "Theatersport/Briefing")]
public class TheatersportBriefing : ScriptableObject
{
    public new string name;
    public TheatersportRol spelerRol1;
    public TheatersportRol spelerRol2;
}
