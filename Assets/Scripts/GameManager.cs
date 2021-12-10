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

    public List<NetworkConnection> gebriefteVerbindingen = new List<NetworkConnection>();

    // De static gamemanager instance
    public static GameManager Instance;

    private void Awake()
    {
        // Maak de static database instance
        Instance = this;
    }

    private void Start()
    {
        // Luister naar verandering in verbindingen
        NetwerkManager.OnVerbindingChange += OnAantalSpelersChange;

        // Zet de gamestatus standaard op incompleet
        if (isServer)
        {
            UpdateGameStatus(GameStatus.Incompleet);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartBriefing();
        }
    }

    [Server]
    public void UpdateGameStatus(GameStatus nieuweStatus)
    {
        if (isServerOnly)
        {
            Debug.Log(nieuweStatus);

            // Verander de gamestatus (server)
            gameStatus = nieuweStatus;
            OnGameStatusChanged?.Invoke(nieuweStatus);
        }
        if (isServer)
        {
            // Verander de gamestatus (client)
            UpdateClientGameStatus(nieuweStatus);
        }
    }

    [ClientRpc]
    private void UpdateClientGameStatus(GameStatus nieuweStatus)
    {
        if (isClient)
        {
            Debug.Log(nieuweStatus);

            // Verander de gamestatus (client)
            gameStatus = nieuweStatus;
            OnGameStatusChanged?.Invoke(nieuweStatus);
        }
    }

    [Server]
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

    [Server]
    public void StartBriefing()
    {
        // Verwijder de status van gebriefte spelers en
        // en zet de spelstatus op briefing
        gebriefteVerbindingen.Clear();
        UpdateGameStatus(GameStatus.Briefing);
    }

    [Command(requiresAuthority = false)]
    public void SpelerBriefingAfgelopen(NetworkConnectionToClient verbinding)
    {
        // Voeg speler toe aan gebriefte spelers
        gebriefteVerbindingen.Add(verbinding);

        if(gebriefteVerbindingen.Count == NetwerkManager.Instance.verbindingen.Count)
        {
            // Als alle spelers gebrieft zijn
            UpdateGameStatus(GameStatus.Spelen);
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