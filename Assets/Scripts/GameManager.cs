using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    // De huidige status van het spel
    public GameStatus gameStatus;

    // Event hook die luistert naar verandering in de gamestatus
    public static event Action<GameStatus> OnGameStatusChanged;

    // De static netwerk instance
    private NetwerkManager netwerk;

    private void Start()
    {
        // Sla de static netwerk instance op
        netwerk = NetwerkManager.Instance;

        // Luister naar veranderingen in verbindingen
        NetwerkManager.OnVerbindingenChange += OnVerbindingenChange;

        // Zet de gamestatus standaard op incompleet
        UpdateGameStatus(GameStatus.Incompleet);
    }

    public void UpdateGameStatus(GameStatus nieuweStatus)
    {
        // Verander de gamestatus (server)
        gameStatus = nieuweStatus;
        OnGameStatusChanged?.Invoke(nieuweStatus);
    }

    [ClientRpc]
    public void UpdateClientGameStatus(GameStatus nieuweStatus)
    {
        // Verander de gamestatus (vlient)
        gameStatus = nieuweStatus;
        OnGameStatusChanged?.Invoke(nieuweStatus);
    }

    public void OnVerbindingenChange(int aantalSpelers)
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
