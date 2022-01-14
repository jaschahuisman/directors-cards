using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Mirror.Discovery;

public class PlayerDiscoverySystem : MonoBehaviour
{
    [SerializeField] NetworkDiscovery discovery;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        XRController controller = other.GetComponent<XRController>();
        if (controller != null)
        {
            discovery.StartDiscovery();
            Debug.LogWarning(other);
        }
    }

    private void OnServerFound(ServerResponse response)
    {
        Network.StartClient(response.uri);
    }
}
