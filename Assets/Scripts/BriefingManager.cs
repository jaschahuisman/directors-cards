using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BriefingManager : NetworkBehaviour
{
    private NetwerkManager netwerk;
    private TheatersportDatabase database;
    private GameManager gameManager;

    private void Start()
    {
        netwerk = NetwerkManager.Instance;
        database = TheatersportDatabase.Instance;
        gameManager = GetComponent<GameManager>();

        GameManager.OnGameStatusChanged += OnGameStatusChanged;
    }

    private void OnGameStatusChanged(GameStatus nieuweStatus)
    {
        if(nieuweStatus == GameStatus.Briefing)
        {
            StartBriefing();
        }
    }

    [Server]
    public void StartBriefing()
    {
        // Laad een random briefing op de server
        int randomBriefingIndex = Random.Range(0, database.briefings.Count);

        foreach (NetwerkSpeler speler in netwerk.spelers)
        {
            // Start de briefing op de spelers
            speler.StartBriefing(randomBriefingIndex);
        }
    }
}
