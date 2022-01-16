using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerJoinSystem : NetworkBehaviour
{
    [SerializeField] private FistPot fistPotPlayer1;
    [SerializeField] private FistPot fistPotPlayer2;

    private NetworkManagerExtended network;
    private NetworkManagerExtended Network
    {
        get
        {
            if (network != null) { return network; }
            return network = NetworkManagerExtended.singleton as NetworkManagerExtended;
        }
    }

    private void Start()
    {
        NetworkPlayer.OnReadyStateChanged += OnReadyStateChanged;        
        FistPot.OnFistEnterJoinEvent += JoinPlayer;        
    }

    private void OnReadyStateChanged(bool _isReady)
    {
        bool team1Ready = false;
        bool team2Ready = false;

        Network.LoopTroughPlayers((NetworkPlayer player) => {
            if (player.isReady && player.team == PlayerType.Player1) { team1Ready = true; }
            if (player.isReady && player.team == PlayerType.Player2) { team2Ready = true; }
        });

        fistPotPlayer1.Activate(team1Ready && fistPotPlayer1.activated == false); 
        fistPotPlayer2.Activate(team2Ready && fistPotPlayer2.activated == false); 
    }

    private void OnDestroy()
    {
        FistPot.OnFistEnterJoinEvent -= JoinPlayer;
    }

    private void JoinPlayer(PlayerType team, NetworkPlayer player)
    {
        if (player.isLocalPlayer)
        {
            player.CmdSetPlayerTeam(team);
            player.CmdSetReadyState(true);
        }
    }
}
