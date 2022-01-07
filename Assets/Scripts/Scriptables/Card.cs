using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "c_type_name", menuName = "Theatersport/Card")]
public class Card : ScriptableObject
{
    public CardType type;
    public string content;
}

public enum CardType
{
    Actie,
    Emotie,
    Restrictie,
    Einde,
}
