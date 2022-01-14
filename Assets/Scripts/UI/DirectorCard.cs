using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DirectorCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI typeText1;
    [SerializeField] private TextMeshProUGUI typeText2;
    [SerializeField] private TextMeshProUGUI contentText;

    public Card card;
    public CardType type;
    public PlayerType team;
    private string content;
    public DirectorGameplayManager directorGameplayManager;
    public bool isUsed = false;
    public bool isEndCard = false;


    public void SetData(Card _card, PlayerType _team, DirectorGameplayManager gameplayManager, bool _isEndCard = false)
    {
        card = _card;
        team = _team;
        directorGameplayManager = gameplayManager;
        isEndCard = _isEndCard;

        type = card.type;
        content = card.content;

        typeText1.text = type.ToString();
        typeText2.text = type.ToString();
        contentText.text = content;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isUsed == false)
        {
            directorGameplayManager.UseCard(this);
            isUsed = true;
        }
    }

    public IEnumerator MoveCard(Vector3 source, Vector3 target, float overTime)
    {
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            transform.position = Vector3.Lerp(source, target, (Time.time - startTime) / overTime);
            yield return null;
        }

        transform.position = target;
    }
}