using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public List<Briefing> briefings = new List<Briefing>();
    public List<ImprovCard> improvCards = new List<ImprovCard>();

    public List<int> deckPlayer1 = new List<int>();
    public List<int> deckPlayer2 = new List<int>();

    public static Database Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        fillPlayerDecks();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fillPlayerDecks();
        }
    }

    public void fillPlayerDecks() {
        List<ImprovCard> improvCardsCopy = improvCards.ToList();
        deckPlayer1 = createDeck(improvCardsCopy);
        deckPlayer2 = createDeck(improvCardsCopy);
    }

    public List<int> createDeck(List<ImprovCard> improvCardsCopy) {
        List<int> deck = new List<int>();

        foreach (ImprovCardType cardType in System.Enum.GetValues(typeof(ImprovCardType))) {
            List<ImprovCard> cardsOfType = improvCards.Where(improvCard => improvCard.type == cardType).ToList();
            if(cardType != ImprovCardType.End) {
                int randomIndex = Random.Range(0, cardsOfType.Count);
                ImprovCard card = cardsOfType[randomIndex];
                
                improvCardsCopy.Remove(card);
                deck.Add(improvCards.IndexOf(cardsOfType[randomIndex]));
            }
        }

        for (int i = 0; i < deck.Count; i++) {
            int randomIndex = Random.Range(0, deck.Count);
            int temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

        // Add a random end card at zero index
        List<ImprovCard> endCards = improvCards.Where(improvCard => improvCard.type == ImprovCardType.End).ToList();
        deck.Insert(0, improvCards.IndexOf(endCards[Random.Range(0, endCards.Count)]));

        return deck;
    }
}