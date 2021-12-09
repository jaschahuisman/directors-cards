using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu(fileName = "Nieuwe Kaart", menuName = "Theatersport/Kaart")]
public class TheatersportKaart : ScriptableObject
{
    public TheatersportKaartType kaartType;
    public string kaartTekst;
    public AudioClip kaartAudio;
}

public enum TheatersportKaartType
{
    Einde,
    Emotie,
    Objectief,
    Restrictie,
}
