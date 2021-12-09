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

    // De static gamemanager instance
    public static GameManager Instance;

    private void Start()
    {
        // Maak de static database instance
        Instance = this;

        //
        NetwerkManager.OnVerbindingChange += OnAantalSpelersChange;

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
        Debug.Log(nieuweStatus);

        if (!isServer)
        {
            // Verander de gamestatus (client)
            gameStatus = nieuweStatus;
            OnGameStatusChanged?.Invoke(nieuweStatus);
        }
    }

    private void OnAantalSpelersChange(int aantalSpelers)
    {
        if(aantalSpelers < 2 && gameStatus != GameStatus.Incompleet)
        {
            // Niet genoeg spelers
            UpdateGameStatus(GameStatus.Incompleet);
        }

        if(aantalSpelers >= 2 && gameStatus == GameStatus.Incompleet)
        {
            // Genoeg spelers
            UpdateGameStatus(GameStatus.Wachten);
        }
    }

    // Debugging functions
    public void DebugIncompleet()
    {
        UpdateGameStatus(GameStatus.Incompleet);
    }

    public void DebugWachten()
    {
        UpdateGameStatus(GameStatus.Wachten);
    }
}

public enum GameStatus
{
    Incompleet,     // Niet genoeg spelers om het spel te starten
    Wachten,        // Wachten tot de host het spel start
    Briefing,       // Briefing van de spelers
    Spelen,         // Spelen!
}