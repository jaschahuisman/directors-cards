using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum PlayerType { Player1, Player2 }

public class NetworkPlayer : NetworkBehaviour
{
    [Header("Authority (make sure to disable on player)")]
    [SerializeField] private List<Behaviour> authoratativeBehaviours = new List<Behaviour>();
    [SerializeField] private List<GameObject> authoratativeGameObjects = new List<GameObject>();

    [Header("Card related components")]
    [SerializeField] private PlayerGameplayManager playerGameplayManager;

    [Header("Network Player Status")]
    [SyncVar]
    public PlayerType team = PlayerType.Player1;

    [SyncVar(hook = nameof(HandleReadyStateChanged))]
    public bool isReady = false;

    public event System.Action<bool> OnReadyStateChanged;

    private NetworkManagerExtended network;
    private NetworkManagerExtended Network
    {
        get
        {
            if (network != null) { return network; }
            return network = NetworkManagerExtended.singleton as NetworkManagerExtended;
        }
    }

    public override void OnStartAuthority()
    {
        foreach (Behaviour behaviour in authoratativeBehaviours)
            behaviour.enabled = true;
        
        foreach (GameObject gameObject in authoratativeGameObjects)
            gameObject.SetActive(true);

        base.OnStartAuthority();
    }

    public override void OnStartClient() => SetupPlayerInNetwork();
    public override void OnStartServer() => SetupPlayerInNetwork();
    public override void OnStopClient() => RemovePlayerFromNetwork();
    public override void OnStopServer() => RemovePlayerFromNetwork();

    private void SetupPlayerInNetwork()
    {
        Network.networkPlayers.Add(this);
        DontDestroyOnLoad(gameObject);
    }

    private void RemovePlayerFromNetwork()
    {
        Network.networkPlayers.Remove(this);
        Network.briefedPlayers.Remove(this);
        Network.playingPlayers.Remove(this);
    }

    [Command]
    public void CmdSetPlayerTeam(PlayerType newTeam)
    {
        team = newTeam;
    }

    [Command]
    public void CmdSetReadyState(bool newReadyState)
    {
        isReady = newReadyState;
        // Network.NotifyReadyToLoadGameplay();
    }

    public void HandleReadyStateChanged(bool oldVlaue, bool newReadyState)
    {
        OnReadyStateChanged?.Invoke(newReadyState);

        if (newReadyState == false)
            playerGameplayManager.DestroyCards();
    }

    [Command]
    public void CmdReadyInGameplayScene()
    {
        Network.playingPlayers.Add(this);
        // Network.NotifyReadyToStartBriefing();
    }

}

