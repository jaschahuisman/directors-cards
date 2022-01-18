using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI typeText1;
    [SerializeField] private TextMeshProUGUI typeText2;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private Color endCardColor;
    [SerializeField] private Image frontImage;
    [SerializeField] private Image backImage;

    public CardType type;
    private string content;

    public void SetData(Card card)
    {
        type = card.type;
        content = card.content;

        typeText1.text = type.ToString();
        typeText2.text = type.ToString();
        contentText.text = content;

        if (type == CardType.Einde)
        {
            frontImage.color = endCardColor;
            backImage.color = endCardColor;
        }
    }
}