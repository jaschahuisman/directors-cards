using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkSpectator : NetworkBehaviour
{
    [SerializeField] private float zoomFactor = 1.5f;
    [SerializeField] private float smoothTime = 0.8f;
    [SerializeField] private float yOffset = 0.8f;
    [SerializeField] private Vector3 defaultPosition = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 offlinePosition = new Vector3(0, 1, 0);
    private Vector3 velocity = Vector3.zero;


    private NetworkManagerExtended network;
    private NetworkManagerExtended Network
    {
        get
        {
            if (network != null) { return network; }
            return network = NetworkManagerExtended.singleton as NetworkManagerExtended;
        }
    }

    private void Awake()
    {
        transform.position = offlinePosition;
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (HasEnoughPLayers())
        {
            float xAxis = GetAveragePosition().x;
            float yAxis = GetAveragePosition().y + yOffset;
            float zAxis = CalculateDistance() * zoomFactor * -1;

            Vector3 newPosition = (new Vector3(xAxis, yAxis, zAxis) + defaultPosition) / 2;

            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, offlinePosition, ref velocity, smoothTime);
        }
    }

    private bool HasEnoughPLayers()
    {
        if (Network.networkPlayers.Count > 0)
            return true;
        else
            return false;
    }

    private Vector3 GetAveragePosition()
    {
        Vector3 average = Vector3.zero;

        foreach (var player in Network.networkPlayers)
        {
            average += player.playerGameplayManager.playerHeadTransform.position;
        }

        average /= Network.networkPlayers.Count;
        return average;
    }

    private float CalculateDistance()
    {
        float distance = 0;
        float largestX = 0;
        float smallestX = 0;

        foreach (var player in Network.networkPlayers)
        {
            if (player.playerGameplayManager.playerHeadTransform.position.x > largestX)
            {
                largestX = player.transform.position.x;
            }
            if (player.playerGameplayManager.playerHeadTransform.position.x < smallestX)
            {
                smallestX = player.transform.position.x;
            }
        }

        distance = largestX - smallestX;
        return distance;
    }
}