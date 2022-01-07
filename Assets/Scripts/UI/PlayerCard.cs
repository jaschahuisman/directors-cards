using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI teamText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI contentText;

    public PlayerTeam team;
    public CardType type;
    private string content;

    public void SetData(Card card, PlayerTeam cardTeam)
    {
        team = cardTeam;
        type = card.type;
        content = card.content;

        teamText.text = team == PlayerTeam.P1 ? "Speler 1" : "Speler 2";
        typeText.text = type.ToString();
        contentText.text = content;
    }
}
