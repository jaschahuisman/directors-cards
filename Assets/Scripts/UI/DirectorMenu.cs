using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectorMenu : MonoBehaviour
{
    [SerializeField] private GameObject waitingOnBriefingCanvas;
    [SerializeField] private GameObject cardsCanvas;
    [SerializeField] private GameObject gameEndCanvas;
    [SerializeField] private Button endGameButton;

    private void Start()
    {
        NetworkManagerExt.GameplayStartedEvent += OnGamePlayStarted;
        OnGamePlayStarted(false);

        endGameButton.onClick.AddListener(() =>
        {
            NetworkManagerExt network = NetworkManagerExt.singleton as NetworkManagerExt;
            network.StopGameplay();
        });
    }

    private void OnDestroy()
    {
        NetworkManagerExt.GameplayStartedEvent -= OnGamePlayStarted;
    }

    private void OnGamePlayStarted(bool isStarted)
    {
        waitingOnBriefingCanvas.SetActive(!isStarted);
        cardsCanvas.SetActive(isStarted);
        gameEndCanvas.SetActive(false);
    }

    public void OnGameEndCard()
    {
        gameEndCanvas.SetActive(true);
    }
}
