using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [Header("Game status")]
    public GameState gameState = GameState.Pending;
    public static event Action<GameState> OnGameStateChanged;
    
    public List<NetworkConnection> briefedConnections = new List<NetworkConnection>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        NetworkManagerExtended.OnConnectionEvent += OnConnectionCountChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // SendImprovCardToClient(UnityEngine.Random.Range(0, 10), PlayerId.Player1);
        }
    }

    [Server]
    public void UpdateGameState(GameState newGameState)
    {
        if (isServerOnly)
        {
            Debug.Log(newGameState);

            gameState = newGameState;
            OnGameStateChanged?.Invoke(newGameState);
        }
        if (isServer)
        {
            RpcUpdateClientGameState(newGameState);
        }
    }

    [ClientRpc]
    private void RpcUpdateClientGameState(GameState newGameState)
    {
        if (isClient)
        {
            Debug.Log(newGameState);

            gameState = newGameState;
            OnGameStateChanged?.Invoke(newGameState);
        }
    }

    [Server]
    private void OnConnectionCountChanged(int playerCount)
    {
        if(playerCount < 2 && gameState != GameState.Pending)
        {
            UpdateGameState(GameState.Pending);
        }

        if(playerCount >= 2 && gameState == GameState.Pending)
        {
            UpdateGameState(GameState.WaitingForHost);
        }
    }

    [Server]
    public void StartBriefing()
    {
        briefedConnections.Clear();
        Database database = Database.Instance;

        int briefingCount = database.briefings.Count;
        int randomBriefingIndex = UnityEngine.Random.Range(0, briefingCount);

        foreach (NetworkConnection connection in NetworkManagerExtended.Instance.connections)
        {
            NetworkPlayer networkPlayer = connection.identity.GetComponent<NetworkPlayer>();
            networkPlayer.RpcStartBriefing(randomBriefingIndex);
        }

        database.fillPlayerDecks();
        UpdateGameState(GameState.Briefing);
    }

    [Command(requiresAuthority = false)]
    public void CmdFinishBriefing(NetworkConnectionToClient connection)
    {
        if (gameState != GameState.Briefing)
        {
            return;
        }

        briefedConnections.Add(connection);

        if (briefedConnections.Count == NetworkManagerExtended.Instance.connections.Count)
        {
            UpdateGameState(GameState.Playing);
        }
    }

    [Server]
    public void SendImprovCardToClient(int cardIndex, PlayerId playerId) {
        foreach (NetworkConnection connection in NetworkManagerExtended.Instance.connections)
        {
            NetworkPlayer networkPlayer = connection.identity.GetComponent<NetworkPlayer>();
            if(networkPlayer.id == playerId)
            {
                Debug.Log(cardIndex + " sent to client with id: " + playerId);
                networkPlayer.RpcReceiveImprovCard(cardIndex);
            }
        }
    }
}

public enum GameState
{
    Pending,            // Niet genoeg spelers om het spel te starten
    WaitingForHost,     // Wachten tot de host het spel start
    Briefing,           // Briefing van de spelers
    Playing,            // Spelen!
}