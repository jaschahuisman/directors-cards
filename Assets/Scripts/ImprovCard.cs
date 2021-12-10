using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu(fileName = "type_00_kaart_name", menuName = "Theatersport/Kaart")]
public class ImprovCard : ScriptableObject
{
    public ImprovCardType type;
    public string text;
    public AudioClip audio;
}

public enum ImprovCardType
{
    End,
    Emotion,
    Action,
    Restriction,
}