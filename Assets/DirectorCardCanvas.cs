using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectorCardCanvas : MonoBehaviour
{
    public int deckSize = 4;
    public List<int> deck1 = new List<int>();
    public List<int> deck2 = new List<int>();
    public List<DirectorCard> usedDeck = new List<DirectorCard>();

    [Header("UI")]
    [SerializeField] private GameObject directorCardPrefab;
    [SerializeField] private Transform deckTransform1;
    [SerializeField] private Transform deckTransform2;
    [SerializeField] private Transform usedDeckTransform;

    private NetworkManagerExt network;
    private NetworkManagerExt Network
    {
        get
        {
            if (network != null) { return network; }
            return network = NetworkManagerExt.singleton as NetworkManagerExt;
        }
    }

    private Database database;
    private Database Database
    {
        get
        {
            if (database != null) { return database; }
            return database = Database.Instance;
        }
    }

    private void Start()
    {
        ResetDecks();
    }

    public void ResetDecks()
    {
        List<Card> cardsCopy = Database.cards.ToList();
        
        foreach(Transform child in usedDeckTransform) { Destroy(child.gameObject); }
        
        deck1 = CreateDeck(cardsCopy);
        deck2 = CreateDeck(cardsCopy);

        DrawUI();
    }

    public List<int> CreateDeck(List<Card> cardsCopy)
    {
        List<int> deck = new List<int>(deckSize);

        // Base cards
        foreach (CardType cardType in System.Enum.GetValues(typeof(CardType)))
        {
            if (cardType != CardType.Einde)
            {
                List<Card> cardsOfType = cardsCopy.Where(card => card.type == cardType).ToList();
                
                if (cardsOfType.Count == 0) break;
                
                int randomIndex = Random.Range(0, cardsOfType.Count);
                Card card = cardsOfType[randomIndex];
                
                cardsCopy.Remove(card);
                deck.Add(Database.cards.IndexOf(card));
            }
        }

        // Additive cards
        for (int i = deck.Count; i < deck.Capacity - 1; i++)
        {
            if (cardsCopy.Count == 0) break;

            int randomIndex = Random.Range(0, cardsCopy.Count);
            Card card = cardsCopy[randomIndex];

            cardsCopy.Remove(card);
            deck.Add(Database.cards.IndexOf(card));
        }

        Shuffle(deck);

        // End card
        {
            List<Card> cards = cardsCopy.Where(card => card.type == CardType.Einde).ToList();
            
            int randomIndex = Random.Range(0, cards.Count);
            Card card = cards[randomIndex];

            cardsCopy.Remove(card);
            deck.Insert(0, Database.cards.IndexOf(card));
        }

        return deck;
    }

    private static System.Random rng = new System.Random();

    private void Shuffle(List<int> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void DrawUI()
    {
        foreach(Transform child in deckTransform1) { Destroy(child.gameObject); }
        foreach(Transform child in deckTransform2) { Destroy(child.gameObject); }
    
        foreach(int cardIndex in deck1) { DrawCardInDeck(cardIndex, PlayerTeam.P1, deck1); }
        foreach(int cardIndex in deck2) { DrawCardInDeck(cardIndex, PlayerTeam.P2, deck2); }
    }

    public void DrawCardInDeck(int cardIndex, PlayerTeam team, List<int> deck)
    {
        GameObject cardObject = Instantiate(directorCardPrefab);
        
        Card card = Database.cards[cardIndex];

        bool isTopCard = deck.IndexOf(cardIndex) == deck.Count - 1;
        
        cardObject.GetComponent<DirectorCard>().SetData(card, team, isTopCard);
        cardObject.transform.SetParent(team == PlayerTeam.P1 ? deckTransform1 : deckTransform2);
    }

    public void DrawUsedCard(DirectorCard card)
    {
        card.IsUsed = true;

        GameObject cardObject = card.gameObject;
        
        cardObject.transform.SetParent(usedDeckTransform);
        cardObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
    }

    public void UseCard(DirectorCard card)
    {
        DrawUsedCard(card);

        if(card.team == PlayerTeam.P1) deck1.Remove(card.index);
        if(card.team == PlayerTeam.P2) deck2.Remove(card.index);

        if(card.type == CardType.Einde)
        {
            Network.SendCardToClient(card.index, PlayerTeam.P1);
            Network.SendCardToClient(card.index, PlayerTeam.P2);
        }
        else
        {
            Network.SendCardToClient(card.index, card.team);
        }

        DrawUI();
    }
}
