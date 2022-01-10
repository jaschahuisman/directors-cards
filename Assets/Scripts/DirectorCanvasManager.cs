using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class DirectorCanvasManager : NetworkBehaviour
{
    [Header("Director canvasses")]
    [SerializeField] public GameObject waitingCanvas;
    [SerializeField] private GameObject preparationCanvas;
    [SerializeField] private GameObject gameCanvas;

    [Header("Director Preparation Components")]
    [SerializeField] private Button startButton;


    [Header("Director Waiting Components")]
    [SerializeField] private Text playerCountText;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        GameManager.OnGameStateChanged += OnGameStateUpdated;
        NetworkManagerExtended.OnConnectionEvent += OnPlayerCountChange;

        OnGameStateUpdated(gameManager.gameState);
        AddEventListeners();
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= OnGameStateUpdated;
        NetworkManagerExtended.OnConnectionEvent -= OnPlayerCountChange;
    }

    private void AddEventListeners()
    {
        startButton.onClick.AddListener(StartButtonOnClick);
    }

    public void OnGameStateUpdated(GameState newGameState)
    {
        waitingCanvas.SetActive(newGameState == GameState.Pending);
        preparationCanvas.SetActive(newGameState == GameState.WaitingForHost);
        gameCanvas.SetActive(newGameState == GameState.Playing);
    }

    public void OnPlayerCountChange(int playerCount)
    {
        if (playerCountText.isActiveAndEnabled)
        {
            playerCountText.text = $"{playerCount} / 2";
        }
    }

    private void StartButtonOnClick()
    {
        gameManager.StartBriefing();
    }

}
