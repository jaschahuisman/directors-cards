using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImprovCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public int cardIndex;
    public ImprovCardScriptable cardData;
    public PlayerId playerId;
    public bool isTopCard;

    [SerializeField] private Text cardPlayerText;
    [SerializeField] private Text cardTypeText;
    [SerializeField] private Text cardContentText;

    private Vector2 pointerDragOffset;
    private Vector2 defaultPosition;

    public void SetData(int newCardIndex, PlayerId newPlayerId, bool isTopCardValue)
    {
        cardIndex = newCardIndex;
        cardData = Database.Instance.improvCards[newCardIndex];
        playerId = newPlayerId;
        isTopCard = isTopCardValue;

        cardPlayerText.text = playerId == PlayerId.Player1 ? "Speler 1" : "Speler 2";

        if(cardData.type == ImprovCardType.Einde)
        {
            cardPlayerText.text = "Beiden spelers";
        }

        cardTypeText.text = cardData.type.ToString();
        cardContentText.text = cardData.text;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isTopCard) return;

        pointerDragOffset = transform.position - Input.mousePosition;
        defaultPosition = transform.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isTopCard) return;

        List<RaycastResult> raycastResults = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, raycastResults);

        bool dropAreaFound = false;

        foreach (RaycastResult result in raycastResults)
        {
            if(result.gameObject.tag == "DropArea")
            {
                dropAreaFound = true;
                CardsManager.Instance.UseCard(this);
                isTopCard = false;
            }
        }

        if (!dropAreaFound)
        {
            StartCoroutine(MoveCard(eventData.position + pointerDragOffset, defaultPosition, 0.2f)); 
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isTopCard) return;
        gameObject.transform.position = eventData.position + pointerDragOffset;
    }

    public IEnumerator MoveCard(Vector2 source, Vector2 target, float overTime)
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
