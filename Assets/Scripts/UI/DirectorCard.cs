using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DirectorCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI teamText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI contentText;

    public PlayerTeam team;
    public CardType type;
    private string content;

    public bool IsTopCard;
    public bool IsUsed = false;
    public int index;

    private Database database;
    private Database Database
    {
        get
        {
            if (database != null) { return database; }
            return database = Database.Instance;
        }
    }


    public void SetData(Card card, PlayerTeam cardTeam, bool isTopCard)
    {
        index = Database.cards.IndexOf(card);

        team = cardTeam;
        type = card.type;
        content = card.content;
        
        IsTopCard = isTopCard;

        teamText.text = team == PlayerTeam.P1 ? "Speler 1" : "Speler 2";
        typeText.text = type.ToString();
        contentText.text = content;
    }
}
