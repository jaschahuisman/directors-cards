using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DirectorLobbyCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image cardBackgroundImage;
    [SerializeField] private TextMeshProUGUI typeText1;
    [SerializeField] private TextMeshProUGUI typeText2;
    [SerializeField] private TextMeshProUGUI contentText;

    [HideInInspector] public Card card;
    [HideInInspector] public CardType type;
    // [HideInInspector]
    public PlayerType team;
    
    public DirectorLobbyManager directorLobbyManager;
    
    public bool selected = false;

    private void Start()
    {
        SetColor();
    }

    public void SetData(Card _card, PlayerType _team, DirectorLobbyManager gameplayManager)
    {
        card = _card;
        team = _team;
        directorLobbyManager = gameplayManager;

        type = card.type;

        typeText1.text = type.ToString();
        typeText2.text = type.ToString();
        contentText.text = card.content;
    }

    public void SetColor()
    {
        cardBackgroundImage.color = (selected == true)
            ? new Color(1, 0.9f, 0.75f, 1)
            : new Color(0.5f, 0.5f, 0.5f, 0.75f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        directorLobbyManager.HandleCardToggle(this);
    }
}
