using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsManager : MonoBehaviour
{
    private Database database;

    [Header("Deck card indexes")]
    public List<int> playerDeck1 = new List<int>();
    public List<int> playerDeck2 = new List<int>();
    
    [Header("Deck UI")]
    public GameObject cardPrefab;
    public Transform playerDeckTransform1;
    public Transform playerDeckTransform2;
    public Transform usedDeckTransform;

    public static CardsManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        database = Database.Instance;
        fillPlayerDecks();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            fillPlayerDecks();
        }
    }

    public void UseCard(ImprovCard improvCard)
    {
        DrawUsedCard(improvCard);

        if (improvCard.playerId == PlayerId.Player1)
        {
            playerDeck1.Remove(improvCard.cardIndex);
        }
        else
        {
            playerDeck2.Remove(improvCard.cardIndex);
        }

        DrawUI();
    }

    public void fillPlayerDecks()
    {
        List<ImprovCardScriptable> improvCardsCopy = database.improvCards.ToList();
        playerDeck1 = createDeck(improvCardsCopy);
        playerDeck2 = createDeck(improvCardsCopy);
        DrawUI();
    }

    public List<int> createDeck(List<ImprovCardScriptable> improvCardsCopy)
    {
        List<int> deck = new List<int>();

        foreach (ImprovCardType cardType in System.Enum.GetValues(typeof(ImprovCardType)))
        {
            List<ImprovCardScriptable> cardsOfType = database.improvCards.Where(improvCard => improvCard.type == cardType).ToList();
            if (cardType != ImprovCardType.End)
            {
                int randomIndex = Random.Range(0, cardsOfType.Count);
                ImprovCardScriptable card = cardsOfType[randomIndex];

                improvCardsCopy.Remove(card);
                deck.Add(database.improvCards.IndexOf(cardsOfType[randomIndex]));
            }
        }

        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(0, deck.Count);
            int temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

        List<ImprovCardScriptable> endCards = database.improvCards.Where(improvCard => improvCard.type == ImprovCardType.End).ToList();
        deck.Insert(0, database.improvCards.IndexOf(endCards[Random.Range(0, endCards.Count)]));

        return deck;
    }

    public void DrawUI()
    {
        foreach (Transform child in playerDeckTransform1){ Destroy(child.gameObject); }
        foreach (Transform child in playerDeckTransform2){ Destroy(child.gameObject); }
        foreach (int cardId in playerDeck1)
        {
            DrawCardInDeck(cardId, PlayerId.Player1, playerDeck1);
        }
        foreach (int cardId in playerDeck2)
        {
            DrawCardInDeck(cardId, PlayerId.Player2, playerDeck2);
        }
    }

    public void DrawCardInDeck(int cardId, PlayerId playerId, List<int> deck)
    {
        GameObject cardObject = Instantiate(cardPrefab);
        bool isTopCard = deck.IndexOf(cardId) == deck.Count - 1;
        cardObject.GetComponent<ImprovCard>().SetData(cardId, playerId, isTopCard);
        cardObject.transform.SetParent(playerId == PlayerId.Player1 ? playerDeckTransform1 : playerDeckTransform2);
    }

    public void DrawUsedCard(ImprovCard improvCard)
    {
        foreach (Transform child in usedDeckTransform) { Destroy(child.gameObject); }
        GameObject cardObject = improvCard.gameObject;
        cardObject.transform.SetParent(usedDeckTransform);
        cardObject.transform.position = usedDeckTransform.position;
        cardObject.GetComponent<Image>().color = new Color(1,1,1,0.4f);
    }
}
