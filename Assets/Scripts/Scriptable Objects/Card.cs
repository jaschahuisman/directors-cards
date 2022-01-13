using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType { Actie, Emotie, Restrictie, Einde };

[CreateAssetMenu(fileName = "c_type 1", menuName = "Theatersport/Card")]
public class Card : ScriptableObject
{
    public CardType type;
    public string content;
}
