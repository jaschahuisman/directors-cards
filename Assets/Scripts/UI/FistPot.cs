using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Mirror;

public class FistPot : MonoBehaviour
{
    public PlayerType team;

    public static event Action OnFistEnterDiscoverEvent;
    public static event Action<PlayerType, NetworkPlayer> OnFistEnterJoinEvent;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.GetComponent<XRController>() != null)
        {
            NetworkPlayer player = other.GetComponentInParent<NetworkPlayer>();

            if (NetworkServer.localClientActive && player != null)
                OnFistEnterJoinEvent?.Invoke(team, player);
            else
                OnFistEnterDiscoverEvent?.Invoke();
        }
    }
}
