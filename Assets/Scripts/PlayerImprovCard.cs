using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerImprovCard : MonoBehaviour
{
    [SerializeField] private Text cardPlayerText;
    [SerializeField] private Text cardTypeText;
    [SerializeField] private Text cardContentText;

    private int cardIndex;
    private ImprovCardScriptable cardData;
    private PlayerId playerId;

    public void SetData(int newCardIndex)
    {
        cardIndex = newCardIndex;
        cardData = Database.Instance.improvCards[newCardIndex];

        cardPlayerText.text = playerId == PlayerId.Player1 ? "Speler 1" : "Speler 2";

        if (cardData.type == ImprovCardType.End)
        {
            cardPlayerText.text = "Beiden spelers";
        }

        cardTypeText.text = cardData.type.ToString();
        cardContentText.text = cardData.text;
    }
}
