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
    [SerializeField] private GameObject deckTitlePlayer1;
    [SerializeField] private GameObject deckTitlePlayer2;
    [SerializeField] private GameObject endCardTitle;
    [SerializeField] private GameObject waitingPanel;

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
        Network.IsFinishedBriefing();
        deckTitlePlayer1.SetActive(true);
        deckTitlePlayer2.SetActive(true);
        endCardTitle.SetActive(false);

        endGameButton.gameObject.SetActive(false);
        endGameButton.onClick.AddListener(Applause);

        DrawCards();

        NetworkManagerExtended.GameplayStartedEvent += OnGameplayStarted;
        OnGameplayStarted(false);
    }

    private void OnDestroy()
    {
        NetworkManagerExtended.GameplayStartedEvent -= OnGameplayStarted;
    }

    private void OnGameplayStarted(bool started)
    {
        waitingPanel.SetActive(!started);
        deckTransformPlayer1.gameObject.SetActive(started);
        deckTransformPlayer2.gameObject.SetActive(started);
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
        
        deckTitlePlayer1.SetActive(false);
        deckTitlePlayer2.SetActive(false);
        endCardTitle.SetActive(true);

        foreach (PlayerType playerType in System.Enum.GetValues(typeof(PlayerType)))
        {
            for (int i = 0; i < 4; i++)
            {
                if (endCards.Count == 0)
                {
                    Debug.LogWarning("Not enough endcards left");
                    break;
                }

                Card card = endCards[Random.Range(0, endCards.Count)];
                endCards.Remove(card);

                DirectorCard cardObject = Instantiate(directorCardPrefab);
                cardObject.SetData(card, playerType, this, true);

                Transform transformParent = (playerType == PlayerType.Player1)
                    ? deckTransformPlayer1
                    : deckTransformPlayer2;

                cardObject.transform.SetParent(transformParent);
            }
        }
    }

    public void UseCard(DirectorCard card)
    {
        Transform destination = (card.team == PlayerType.Player1) 
            ? cardHolderTransformPlayer1 
            : cardHolderTransformPlayer2;
        
        if (card.isEndCard) 
        { 
            destination = cardHolderEndCard; 
            
            Destroy(cardHolderTransformPlayer1.gameObject);
            Destroy(cardHolderTransformPlayer2.gameObject);

            deckTransformPlayer1.gameObject.SetActive(false);
            deckTransformPlayer2.gameObject.SetActive(false);

            endGameButton.gameObject.SetActive(true);
        }

        card.transform.SetParent(destination, true);
        StartCoroutine(card.MoveCard(card.transform.position, destination.position, 0.4f));

        var databaseDeck = (card.team == PlayerType.Player1)
            ? Database.Instance.deckPlayer1
            : Database.Instance.deckPlayer2;

        databaseDeck.Remove(card.card);

        Network.SendCardToPlayer(card.card, card.team);

        DrawCards();
    }

    public void Applause()
    {
        Network.FinishScene();
        endCardTitle.SetActive(false);
        endGameButton.gameObject.SetActive(false);
        cardHolderEndCard.gameObject.SetActive(false);
    }
}
