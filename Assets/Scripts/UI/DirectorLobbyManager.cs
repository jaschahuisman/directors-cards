using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DirectorLobbyManager : MonoBehaviour
{
    [SerializeField] private DirectorLobbyCard cardPrefab;
    [SerializeField] private int cardsPerPlayer = 3;
    [SerializeField] private int selectableCardsCount = 10;
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI startButtonText;
    [SerializeField] private Transform deckSelectionTransformPlayer1;
    [SerializeField] private Transform deckSelectionTransformPlayer2;

    public List<Card> deckPlayer1 = new List<Card>();
    public List<Card> deckPlayer2 = new List<Card>();

    private bool directorReady;

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
        NetworkManagerExtended.GameplayReadyEvent += GameplayReadyEvent;
        startButton.onClick.AddListener(StartShow);
        startButton.interactable = false;
        startButtonText.text = $"Selecteer {cardsPerPlayer} kaarten per speler...";

        DrawCards();
    }

    private void OnDestroy()
    {
        NetworkManagerExtended.GameplayReadyEvent -= GameplayReadyEvent;
    }

    public void DrawCards()
    {
        deckPlayer1.Clear();
        deckPlayer2.Clear();

        List<Card> cardsCopy = Database.Instance.cards;
        List<Card> cards = new List<Card>();

        foreach (var card in cardsCopy)
            if (card.type != CardType.Einde) cards.Add(card);

        foreach (PlayerType playerType in System.Enum.GetValues(typeof(PlayerType)))
        {
            for (int i = 0; i < selectableCardsCount; i++)
            {
                if (cards.Count > 0)
                {
                    Card card = cards[UnityEngine.Random.Range(0, cards.Count)];
                    cards.Remove(card);

                    DirectorLobbyCard cardObject = Instantiate(cardPrefab);
                    cardObject.SetData(card, playerType, this);

                    Transform parentTransform = (playerType == PlayerType.Player1)
                        ? deckSelectionTransformPlayer1
                        : deckSelectionTransformPlayer2;

                    cardObject.transform.SetParent(parentTransform, false);
                }
            }
        }
    }

    private void GameplayReadyEvent(bool enoughPlayers)
    {
        startButton.interactable = (enoughPlayers && directorReady);

        if (directorReady == false)
            startButtonText.text = $"Selecteer {cardsPerPlayer} kaarten per speler...";
        else
            startButtonText.text = (enoughPlayers == true)
                ? "Laat de show beginnen!"
                : "Wachten tot de spelers er ook klaar voor zijn...";
    }

    public void HandleCardToggle(DirectorLobbyCard card)
    {
        bool toggleValue = !card.selected;

        if (card.team == PlayerType.Player1)
        {
            if (toggleValue && deckPlayer1.Count < cardsPerPlayer)
            {
                card.selected = true;
                deckPlayer1.Add(card.card);
                card.SetColor();
            }
            else
            {
                card.selected = false;
                deckPlayer1.Remove(card.card);
                card.SetColor();
            }
        }
        
        if (card.team == PlayerType.Player2)
        {
            if (toggleValue && deckPlayer2.Count < cardsPerPlayer)
            {
                card.selected = true;
                deckPlayer2.Add(card.card);
                card.SetColor();
            }
            else
            { 
                card.selected = false;
                deckPlayer2.Remove(card.card);
                card.SetColor();
            }
        }

        directorReady = (deckPlayer1.Count + deckPlayer2.Count == cardsPerPlayer * 2);
        GameplayReadyEvent(Network.IsReadyToLoadGameplay());
    }

    private void StartShow()
    {
        Database.Instance.deckPlayer1 = deckPlayer1;
        Database.Instance.deckPlayer2 = deckPlayer2;
        Network.LoadGameplayScene();
    }
}
