using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerJoinSystem : NetworkBehaviour
{
    private void Start()
    {
        FistPot.OnFistEnterJoinEvent += JoinPlayer;        
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
