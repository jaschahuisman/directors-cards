using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Mirror.Discovery;

public class PlayerDiscoverySystem : MonoBehaviour
{
    [SerializeField] private NetworkDiscovery discovery;
    [SerializeField] private FistPot joinPot;

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
        discovery.OnServerFound.AddListener(OnServerFound);
        FistPot.OnFistEnterDiscoverEvent += StartDiscovery;
    }
    private void OnDestroy()
    {
        FistPot.OnFistEnterDiscoverEvent -= StartDiscovery;
    }

    private void StartDiscovery()
    {
        joinPot.Activate(true);
        discovery.StartDiscovery();
        Debug.LogWarning("Discovery Started");
    }

    private void OnServerFound(ServerResponse response)
    {
        Network.StartClient(response.uri);
    }
}
