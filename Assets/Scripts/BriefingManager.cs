using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class BriefingManager : MonoBehaviour
{

    public List<TheatersportBriefing> briefings = new List<TheatersportBriefing>();
    int index;

    [SerializeField] public TheatersportBriefing _soPlayer;

    public Text rolPlayer1, rolPlayer2, scenarioPlayer1, scenarioPlayer2;
    public PlayableDirector playableDirector;
    public GameObject rolKaarten, scenarioKaarten;

    
    void Awake()
    {
        // Schakelt de kaarten uit
        rolKaarten.SetActive(false);
        scenarioKaarten.SetActive(false);

        GenerateNumber();

        _soPlayer = briefings[index];

        // geeft de waarde van de scriptable object door aan de rol en scenario kaarten van player 1
        rolPlayer1.text = _soPlayer.spelerRol1.rol;
        scenarioPlayer1.text = _soPlayer.spelerRol1.scenario;

        // geeft de waarde van de scriptable object door aan de rol en scenario kaarten van player 2
        rolPlayer2.text = _soPlayer.spelerRol2.rol;
        scenarioPlayer2.text = _soPlayer.spelerRol2.scenario;

        // Speelt de tijdlijn van de scriptable object af
        playableDirector.Play(_soPlayer.timeline);

    }


    public void ShowRol()
    {
        // Laat de rol kaarten zien van beiden spelers
        rolKaarten.SetActive(true);
    }

    public void ShowScenario() 
    {
        // Laat de Scenario kaarten zien van beiden spelers
        scenarioKaarten.SetActive(true);
    }

   public void EndTimeline()
    {
        // Weet niet of het nog nodig is, anders kan die weg. Misschien handig voor het einde.
        Debug.Log("End of timeline");
    }


    void GenerateNumber()
    {
        index = Random.Range(0, briefings.Count);
    }

}
