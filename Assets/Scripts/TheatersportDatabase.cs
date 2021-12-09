using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheatersportDatabase : MonoBehaviour
{
    // Briefing dataset
    public List<TheatersportBriefing> briefings = new List<TheatersportBriefing>();

    // Kaarten datasets
    public List<TheatersportKaart> emotieKaarten = new List<TheatersportKaart>();
    public List<TheatersportKaart> restrictieKaarten = new List<TheatersportKaart>();
    public List<TheatersportKaart> objectiefKaarten = new List<TheatersportKaart>();
    public List<TheatersportKaart> eindeKaarten = new List<TheatersportKaart>();

    // De static database instance
    public static TheatersportDatabase Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            // Maak de static database instance
            Instance = this;
            // Zorg er voor dat de database altijd blijft bestaan
            DontDestroyOnLoad(gameObject);
        } 
        else
        {
            // Verwijder duplicates
            Destroy(gameObject);
        }
    }
}
