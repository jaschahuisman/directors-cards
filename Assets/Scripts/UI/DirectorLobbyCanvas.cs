using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DirectorLobbyCanvas : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private GameObject player1ReadyIcon, player2ReadyIcon;

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
        NetworkManagerExt.GameplayReadyEvent += OnGameplayReadyEvent;
        
        startGameButton.onClick.AddListener(StartGame);
        OnGameplayReadyEvent(Network.IsReadyToLoadGameplay());
    }

    private void OnDestroy()
    {
        NetworkManagerExt.GameplayReadyEvent -= OnGameplayReadyEvent;
    }

    private void OnGameplayReadyEvent(bool isReady)
    {
        startGameButton.gameObject.SetActive(isReady);
        statusText.text = isReady ? "Klaar om te spelen?" : "Wachten tot er genoeg spelers zijn...";

        bool team1Ready = false;
        bool team2Ready = false;

        foreach (var player in Network.NetworkPlayers)
        {
            if (player.IsReady && player.Team == PlayerTeam.P1) { team1Ready = true; }
            if (player.IsReady && player.Team == PlayerTeam.P2) { team2Ready = true; }
        }

        player1ReadyIcon.SetActive(team1Ready);
        player2ReadyIcon.SetActive(team2Ready);
    }

    private void StartGame()
    {
        Network.LoadGameplayScene();
    }
}
