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

    public void SetData(int newCardIndex, PlayerId newPlayerId, bool isTopCardValue)
    {
        cardIndex = newCardIndex;
        cardData = Database.Instance.improvCards[newCardIndex];
        playerId = newPlayerId;
        isTopCard = isTopCardValue;

        cardPlayerText.text = playerId == PlayerId.Player1 ? "Speler 1" : "Speler 2";
        cardTypeText.text = cardData.type.ToString();
        cardContentText.text = cardData.text;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isTopCard) return;
        pointerDragOffset = transform.position - Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isTopCard) return;

        List<RaycastResult> raycastResults = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, raycastResults);
        
        foreach (RaycastResult result in raycastResults)
        {
            if(result.gameObject.tag == "DropArea")
            {
                CardsManager.Instance.UseCard(this);
                isTopCard = false;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isTopCard) return;
        gameObject.transform.position = eventData.position + pointerDragOffset;
    }
}
