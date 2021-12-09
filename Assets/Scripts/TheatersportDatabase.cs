using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheatersportDatabase : MonoBehaviour
{
    // De static database instance
    private static TheatersportDatabase Instance;

    // Briefing dataset
    public List<TheatersportBriefing> briefings;

    // Kaarten datasets
    public List<TheatersportKaart> emotieKaarten;
    public List<TheatersportKaart> restrictieKaarten;
    public List<TheatersportKaart> objectiefKaarten;
    public List<TheatersportKaart> eindeKaarten;

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
