using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerLobbyCanvas : NetworkBehaviour
{
    [SerializeField] private Button readyButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private PlayerTeam team;

    private NetworkManagerExt network;
    private NetworkManagerExt Network
    {
        get
        {
            if (network != null) { return network; }
            return network = NetworkManagerExt.singleton as NetworkManagerExt;
        }
    }

    private void Start()
    {
        readyButton.onClick.AddListener(OnPlayerReady);
        cancelButton.onClick.AddListener(OnPlayerUnready);
        
        cancelButton.gameObject.SetActive(false);

        foreach (var player in Network.NetworkPlayers)
        {
            if (player.isLocalPlayer)
            {
                player.OnReadyChanged += OnGameplayReadyEvent;
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var player in Network.NetworkPlayers)
        {
            if (player.isLocalPlayer)
            {
                player.OnReadyChanged -= OnGameplayReadyEvent;
            }
        }
    }

    private void OnGameplayReadyEvent(bool readyForGameplay) {
        readyButton.interactable = !readyForGameplay;
        cancelButton.gameObject.SetActive(readyForGameplay);
    }

    private void OnPlayerReady()
    {
        foreach(var player in Network.NetworkPlayers)
        {
            if (player.isLocalPlayer)
            {
                player.CmdSetPlayerTeam(team);
                player.CmdSetReadyState(true);
            }
        }
    }

    private void OnPlayerUnready()
    {
        foreach (var player in Network.NetworkPlayers)
        {
            if (player.isLocalPlayer)
            {
                player.CmdSetReadyState(false);
            }
        }
    }
}
