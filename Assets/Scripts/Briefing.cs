using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "00_briefing_name", menuName = "Theatersport/Briefing")]
public class Briefing : ScriptableObject
{
    public new string name;
    public PlayerRole spelerRol1;
    public PlayerRole spelerRol2;
}