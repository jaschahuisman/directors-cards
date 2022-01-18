using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Mirror.Discovery;
using TMPro;

public class PlayerDiscoverySystem : MonoBehaviour
{
    [SerializeField] private NetworkDiscovery discovery;
    [SerializeField] private FistPot joinPot;
    [SerializeField] private TextMeshProUGUI joinText;

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
        joinText.text = "Op zoek naar een regisseur...";
        joinPot.Activate(true);
        discovery.StartDiscovery();
        Debug.LogWarning("Discovery Started");
    }

    private void OnServerFound(ServerResponse response)
    {
        joinText.text = "Onderweg naar de set!";
        StartCoroutine(StartClientAfterTime(response.uri, 3.0f));
    }

    private IEnumerator StartClientAfterTime(System.Uri uri, float delay)
    {
        yield return new WaitForSeconds(delay);
        Network.StartClient(uri);
    }
}
