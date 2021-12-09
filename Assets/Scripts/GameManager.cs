using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    [Header("Spel status")]
    // De huidige status van het spel
    public GameStatus gameStatus;

    // Event hook die luistert naar verandering in de gamestatus
    public static event Action<GameStatus> OnGameStatusChanged;

    [Header("Spel managers")]
    public BriefingManager briefingManager;

    // De static gamemanager instance
    public static GameManager Instance;

    private void Start()
    {
        // Maak de static database instance
        Instance = this;

        // Verkrijg spelmanager componenten
        briefingManager = GetComponent<BriefingManager>();

        // Luister naar veranderingen in verbindingen
        NetwerkManager.OnVerbindingenChange += OnVerbindingenChange;

        // Zet de gamestatus standaard op incompleet
        UpdateGameStatus(GameStatus.Incompleet);
    }

    [Server]
    public void UpdateGameStatus(GameStatus nieuweStatus)
    {
        Debug.Log(nieuweStatus);

        if (isServerOnly)
        {
            // Verander de gamestatus (server)
            gameStatus = nieuweStatus;
            OnGameStatusChanged?.Invoke(nieuweStatus);

            // Verander de gamestatus (client)
            UpdateClientGameStatus(nieuweStatus);
        }
    }

    [ClientRpc]
    private void UpdateClientGameStatus(GameStatus nieuweStatus)
    {
        if (!isServer)
        {
            // Verander de gamestatus (client)
            gameStatus = nieuweStatus;
            OnGameStatusChanged?.Invoke(nieuweStatus);
        }
    }

    [Server]
    private void OnVerbindingenChange(int aantalSpelers)
    {
        // Hou in de gaten of het spel genoeg spelers heeft
        if (aantalSpelers < 2 && gameStatus != GameStatus.Incompleet)
        {
            // Spelers incompleet
            UpdateGameStatus(GameStatus.Incompleet);
        }
        else if (gameStatus != GameStatus.Wachten)
        {
            // Genoeg spelers
            UpdateGameStatus(GameStatus.Wachten);
        }
    }
}

public enum GameStatus
{
    Incompleet,     // Niet genoeg spelers om het spel te starten
    Wachten,        // Wachten tot de host het spel start
    Briefing,       // Briefing van de spelers
    Spelen,         // Spelen!
}
