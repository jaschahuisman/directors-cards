using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkSpectator : NetworkBehaviour
{
    [SerializeField] private float zoomFactor = 1.5f;
    [SerializeField] private float followTimeDelta = 0.8f;

    private NetworkManagerExt network;
    private NetworkManagerExt Network
    {
        get
        {
            if (network != null) { return network; }
            return network = NetworkManagerExt.singleton as NetworkManagerExt;
        }
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        Vector3 positionsTeam1 = new Vector3();
        Vector3 positionsTeam2 = new Vector3();

        int team1Count = 0;
        int team2Count = 0;

        foreach (var player in Network.NetworkPlayers)
        {
            if (player.Team == PlayerTeam.P1)
            {
                team1Count++;
                positionsTeam1 += player.bodyTransform.position;
            }

            if (player.Team == PlayerTeam.P2)
            {
                team2Count++;
                positionsTeam2 += player.bodyTransform.position;
            }
        }

        Vector3 midpointTeam1 = positionsTeam1 / team1Count;
        Vector3 midpointTeam2 = positionsTeam2 / team2Count;
        
        Vector3 midpoint = (midpointTeam1 + midpointTeam2) / 2f;
        float distance = (midpointTeam1 - midpointTeam2).magnitude;

        Vector3 cameraDestination = midpoint - gameObject.transform.forward * distance * zoomFactor;

        if (team1Count + team2Count >= 2)
        {
            // Debug.Log(gameObject.transform.position);
            // Debug.Log(cameraDestination);
            // Debug.Log(followTimeDelta);
            // gameObject.transform.position = Vector3.Slerp(
            //     gameObject.transform.position, 
            //     cameraDestination, 
            //     followTimeDelta
            // );
        }

        if ((cameraDestination - gameObject.transform.position).magnitude <= 0.05f)
        {
            gameObject.transform.position = cameraDestination;
        }
    }
}
