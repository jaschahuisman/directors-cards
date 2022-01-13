using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DirectorGameplayManager : MonoBehaviour
{
    [Header("Director Card prefab")]
    [SerializeField] private DirectorCard directorCardPrefab;

    [Header("UI Elements")]
    [SerializeField] private Transform deckTransformPlayer1;
    [SerializeField] private Transform deckTransformPlayer2;
    [SerializeField] private Transform cardHolderTransformPlayer1;
    [SerializeField] private Transform cardHolderTransformPlayer2;
    [SerializeField] private Transform cardHolderEndCard;
    [SerializeField] private Button endGameButton;
    [SerializeField] private TextMeshProUGUI endCardTitle;

    private NetworkManagerExtended network;
    private NetworkManagerExtended Network
    {
        get
        {
            if (network != null) { return network; }
            return network = NetworkManagerExtended.singleton as NetworkManagerExtended;
        }
    }

    private void Start()
    {
        endGameButton.gameObject.SetActive(false);
        endCardTitle.gameObject.SetActive(false);

        DrawCards();
    }

    private void DrawCards()
    {
        foreach(Transform child in deckTransformPlayer1) { Destroy(child.gameObject); }
        foreach(Transform child in deckTransformPlayer2) { Destroy(child.gameObject); }

        foreach (var card in Database.Instance.deckPlayer1)
        {
            DirectorCard cardObject = Instantiate(directorCardPrefab);
            cardObject.SetData(card, PlayerType.Player1, this);
            cardObject.transform.SetParent(deckTransformPlayer1);
        }

        foreach (var card in Database.Instance.deckPlayer2)
        {
            DirectorCard cardObject = Instantiate(directorCardPrefab);
            cardObject.SetData(card, PlayerType.Player2, this);
            cardObject.transform.SetParent(deckTransformPlayer2);
        }

        int remaindingCards = Database.Instance.deckPlayer1.Count + Database.Instance.deckPlayer2.Count;

        if (remaindingCards == 0)
        {
            DisplayEndCards();
        }
    }

    private void DisplayEndCards()
    {
        List<Card> endCards = Database.Instance.cards.Where(card => card.type == CardType.Einde).ToList();
        
        // assign 3 end cards in each player deck
    }

    public void UseCard(DirectorCard card, PlayerType team)
    {
        Transform destination = (team == PlayerType.Player1) 
            ? cardHolderTransformPlayer1 
            : cardHolderTransformPlayer2;

        if (card.isEndCard) 
        { 
            destination = cardHolderEndCard; 
            
            Destroy(cardHolderTransformPlayer1.gameObject);
            Destroy(cardHolderTransformPlayer2.gameObject);

            endGameButton.gameObject.SetActive(true);
        }

        card.transform.SetParent(destination, true);
        StartCoroutine(card.MoveCard(card.transform.position, destination.position, 0.4f));

        var databaseDeck = (team == PlayerType.Player1)
            ? Database.Instance.deckPlayer1
            : Database.Instance.deckPlayer2;

        databaseDeck.Remove(card.card);
        DrawCards();
    }
}
