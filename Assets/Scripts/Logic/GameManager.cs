using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    
}

public enum GameState
{
    Pending,            // Niet genoeg spelers om het spel te starten
    WaitingForHost,     // Wachten tot de host het spel start
    Briefing,           // Briefing van de spelers
    Playing,            // Spelen!
}