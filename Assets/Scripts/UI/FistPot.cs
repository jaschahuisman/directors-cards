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
            Debug.LogWarning("Pot Triggered", other);
            NetworkPlayer player = other.GetComponentInParent<NetworkPlayer>();

            if (player != null)
                OnFistEnterJoinEvent?.Invoke(team, player);
            else
                OnFistEnterDiscoverEvent?.Invoke();
        }
    }
}
