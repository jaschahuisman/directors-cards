using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu(fileName = "type_00_kaart_name", menuName = "Theatersport/Kaart")]
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
    Actie,
    Restrictie,
}