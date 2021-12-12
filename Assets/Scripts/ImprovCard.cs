using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImprovCard : MonoBehaviour
{
    private ImprovCardScriptable cardData;
    private PlayerId playerId;
    public bool isTopCard;

    [SerializeField] private Text cardPlayerText;
    [SerializeField] private Text cardTypeText;
    [SerializeField] private Text cardContentText;


    public void SetData(int cardIndex, PlayerId newPlayerId, bool isTopCardValue)
    {
        playerId = newPlayerId;
        cardData = Database.Instance.improvCards[cardIndex];
        isTopCard = isTopCardValue;

        cardPlayerText.text = playerId == PlayerId.Player1 ? "Speler 1" : "Speler 2";
        cardTypeText.text = cardData.type.ToString();
        cardContentText.text = cardData.text;
    }

    public void Use()
    {
        Destroy(gameObject);
    }
}
