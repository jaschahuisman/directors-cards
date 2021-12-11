using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public List<Briefing> briefings = new List<Briefing>();
    public List<ImprovCard> improvCards = new List<ImprovCard>();

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

    public CardDeck GetImprovCardDeck() {
        List<ImprovCard> emotionCards = improvCards.Where(improvCard => improvCard.type == ImprovCardType.Emotion).ToList();
        List<ImprovCard> actionCards = improvCards.Where(improvCard => improvCard.type == ImprovCardType.Action).ToList();
        List<ImprovCard> restrictionCards = improvCards.Where(improvCard => improvCard.type == ImprovCardType.Restriction).ToList();

        int emotionIndex = improvCards.IndexOf(emotionCards[Random.Range(0, emotionCards.Count)]);
        int actionIndex = improvCards.IndexOf(actionCards[Random.Range(0, emotionCards.Count)]);
        int restrictionIndex = improvCards.IndexOf(restrictionCards[Random.Range(0, emotionCards.Count)]);

        return new CardDeck(emotionIndex, actionIndex, restrictionIndex);
    }
}

public struct CardDeck
{
    public int emotionIndex;
    public int actionIndex;
    public int restrictionIndex;

    public CardDeck(int emotionIndex, int actionIndex, int restrictionIndex)
    {
        this.emotionIndex = emotionIndex;
        this.actionIndex = actionIndex;
        this.restrictionIndex = restrictionIndex;
    }
}