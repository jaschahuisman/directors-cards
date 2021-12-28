using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DirectorCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private TextMeshProUGUI teamText;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI contentText;

    public PlayerTeam team;
    public CardType type;
    private string content;

    public bool IsTopCard;
    public bool IsUsed = false;
    public int index;

    private DirectorCardCanvas canvas;
    private Vector2 pointerDragOffset;
    private Vector2 defaultPosition;

    private Database database;
    private Database Database
    {
        get
        {
            if (database != null) { return database; }
            return database = Database.Instance;
        }
    }


    public void SetData(Card card, PlayerTeam cardTeam, bool isTopCard, DirectorCardCanvas cardsCanvas)
    {
        index = Database.cards.IndexOf(card);

        canvas = cardsCanvas;

        team = cardTeam;
        type = card.type;
        content = card.content;
        
        IsTopCard = isTopCard;

        teamText.text = team == PlayerTeam.P1 ? "Speler 1" : "Speler 2";
        typeText.text = type.ToString();
        contentText.text = content;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsTopCard) return;

        pointerDragOffset = transform.position - Input.mousePosition;
        defaultPosition = transform.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!IsTopCard) return;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        bool dropAreaFound = false;

        foreach(RaycastResult result in raycastResults)
        {
            if (result.gameObject.tag == "DropArea")
            {
                dropAreaFound = true;
                canvas.UseCard(this);
                IsTopCard = false;
            }
        }

        if (!dropAreaFound)
        {
            StartCoroutine(MoveCard(eventData.position + pointerDragOffset, defaultPosition, 0.2f));
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsTopCard) return;

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
