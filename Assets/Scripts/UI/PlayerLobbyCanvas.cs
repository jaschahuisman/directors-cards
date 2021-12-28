using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLobbyCanvas : MonoBehaviour
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
        NetworkManagerExt.GameplayReadyEvent += OnGameplayReadyEvent;

        cancelButton.gameObject.SetActive(false);
        cancelButton.onClick.AddListener(OnPlayerUnready);
    }

    private void OnGameplayReadyEvent(bool readyForGameplay)
    {
        foreach (var player in Network.NetworkPlayers)
        {
            if (player.isLocalPlayer)
            {
                readyButton.interactable = !player.IsReady;
                cancelButton.gameObject.SetActive(player.IsReady);
            }
        }
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
