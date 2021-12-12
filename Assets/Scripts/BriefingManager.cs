using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class BriefingManager : MonoBehaviour
{
    [SerializeField] public TheatersportBriefing scriptableObject;

    public Text rolPlayer1, rolPlayer2, scenarioPlayer1, scenarioPlayer2;
    public PlayableDirector playableDirector;
    public GameObject rolKaarten, scenarioKaarten;

    
    void Awake()
    {
        rolKaarten.SetActive(false);
        scenarioKaarten.SetActive(false);

        // geeft de waarde van de scriptable object door aan de rol en scenario kaarten van player 1
        rolPlayer1.text = scriptableObject.spelerRol1.rol;
        scenarioPlayer1.text = scriptableObject.spelerRol1.scenario;

        // geeft de waarde van de scriptable object door aan de rol en scenario kaarten van player 2
        rolPlayer2.text = scriptableObject.spelerRol2.rol;
        scenarioPlayer2.text = scriptableObject.spelerRol2.scenario;

        playableDirector.Play(scriptableObject.timeline);

      //  Debug.Log(testScriptableObject.spelerRol1.rol);
     //   Debug.Log(testScriptableObject.spelerRol1.scenario);
      //  Debug.Log(testScriptableObject.spelerRol2.rol);
     //   Debug.Log(testScriptableObject.spelerRol1.scenario);
    }


    public void ShowRol()
    {
        //Debug.Log("laat rol kaarten zien");
        rolKaarten.SetActive(true);
    }

    public void ShowScenario() 
    {
        //Debug.Log("laat scenario kaarten zien");
        scenarioKaarten.SetActive(true);
    }

   public void EndTimeline()
    {
        Debug.Log("End of timeline");
    }


}
