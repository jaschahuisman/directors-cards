using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectorMenu : MonoBehaviour
{
    [SerializeField] private GameObject waitingOnBriefingCanvas;
    [SerializeField] private GameObject cardsCanvas;

    private void Start()
    {
        NetworkManagerExt.GameplayStartedEvent += OnGamePlayStarted;
        OnGamePlayStarted(false);
    }

    private void OnGamePlayStarted(bool isStarted)
    {
        waitingOnBriefingCanvas.SetActive(!isStarted);
        cardsCanvas.SetActive(isStarted);
    }
}
