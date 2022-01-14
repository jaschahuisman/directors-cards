using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.Discovery;
using UnityEngine.SceneManagement;

public class NetworkManagerExtended : NetworkManager
{
    [Header("Gameplay scene")]
    [Scene] [SerializeField] private string gameplayScene = string.Empty;

    [Header("Playerlists")]
    public List<NetworkPlayer> networkPlayers = new List<NetworkPlayer>();
    public List<NetworkPlayer> briefedPlayers = new List<NetworkPlayer>();
    public List<NetworkPlayer> playingPlayers = new List<NetworkPlayer>();
    public List<NetworkPlayer> finishedPlayers = new List<NetworkPlayer>();

    public static event Action ConnectionEvent;
    public static event Action<bool> GameplayReadyEvent;
    public static event Action<bool> GameplayStartedEvent;

    public override void OnStartServer()
    {
        GetComponent<NetworkDiscovery>().AdvertiseServer();
        base.OnStartServer();
    }

    public override void OnStopServer()
    {
        networkPlayers.Clear();
        briefedPlayers.Clear();
        playingPlayers.Clear();

        base.OnStopServer();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
        ConnectionEvent?.Invoke();
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);

        if (!conn.identity) { return; }

        Scene activeScene = SceneManager.GetActiveScene();
        NetworkPlayer player = conn.identity.GetComponent<NetworkPlayer>();

        if (!player) { return; }

        if (gameplayScene.Contains(activeScene.name))
        {
            player.CmdReadyInGameplayScene();
        }
    }

    public void LoadGameplayScene()
    {
        ServerChangeScene(gameplayScene);
    }

    public void StartBriefing()
    {
        briefedPlayers.Clear();
        int briefingIndex = UnityEngine.Random.Range(0, Database.Instance.briefings.Count);
        
        LoopTroughPlayers((NetworkPlayer player) => {
            player.RpcStartBriefing(briefingIndex);
            player.UpdateCharacter(briefingIndex);
        });
    }

    public void StartGameplay()
    {
        GameplayStartedEvent?.Invoke(true);
        Debug.LogWarning("NetworkManager: Gameplay Started after briefing all players");
    }

    public void FinishScene()
    {
        LoopTroughPlayers((NetworkPlayer player) => {
            player.RpcFinishScene();
        });
    }

    public void StopGameplay()
    {
        GameplayStartedEvent?.Invoke(false);
        playingPlayers.Clear();
        finishedPlayers.Clear();

        LoopTroughPlayers((NetworkPlayer player) => {
            player.isReady = false;
        });

        ServerChangeScene(onlineScene);
    }

    public void SendCardToPlayer(Card card, PlayerType team)
    {
        LoopTroughPlayers((NetworkPlayer player) =>
        {
            if (player.team == team || card.type == CardType.Einde)
            {
                int cardIndex = Database.Instance.cards.IndexOf(card);
                player.playerGameplayManager.RpcReceiveCard(cardIndex);
            }
        });
    }

    public void HandlePlayerReadyStateChange()
    {
        Debug.LogWarning("NetworkManager: Player ready state change is handled");
        GameplayReadyEvent?.Invoke(IsReadyToLoadGameplay());
    }

    public void HandlePlayerGameplaySceneLoaded()
    {
        Debug.LogWarning("NetworkManager: Scene loaded on player");
        if (IsReadyToStartBriefing())
            StartBriefing();
    }

    public void HandlePlayerBriefingFinished()
    {
        Debug.LogWarning("NetworkManager: Scene loaded on player");
        if (IsFinishedBriefing())
            StartGameplay();
    }

    public void HandlePlayerFinishedScene()
    {
        Debug.LogWarning("NetworkManager: Scene finished on player");
        if (IsFinishedScene())
            StopGameplay();
    }

    public bool IsReadyToLoadGameplay()
    {
        bool team1Ready = false;
        bool team2Ready = false;

        LoopTroughPlayers((NetworkPlayer player) => {
            if (player.isReady && player.team == PlayerType.Player1) { team1Ready = true; }
            if (player.isReady && player.team == PlayerType.Player2) { team2Ready = true; }
        });

        return team1Ready && team2Ready;
    }

    public bool IsReadyToStartBriefing()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        return gameplayScene.Contains(activeScene.name) && playingPlayers.Count == networkPlayers.Count;
    }

    public bool IsFinishedBriefing()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        return gameplayScene.Contains(activeScene.name) && briefedPlayers.Count == networkPlayers.Count;
    }

    public bool IsFinishedScene()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        return gameplayScene.Contains(activeScene.name) && finishedPlayers.Count == networkPlayers.Count;
    }

    public void LoopTroughPlayers(Action<NetworkPlayer> function, List<NetworkPlayer> playerList = null)
    {
        if (playerList == null)
            playerList = networkPlayers;
        
        foreach (NetworkPlayer player in playerList)
            function(player);
    }

    public void ExecuteIfLocalPlayer(Action function)
    {
        LoopTroughPlayers(
           (NetworkPlayer player) => { if (player.isLocalPlayer) function(); }
        );
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadGameplayScene();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StopGameplay();
        }
    }
}
