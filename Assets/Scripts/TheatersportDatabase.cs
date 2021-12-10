using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheatersportDatabase : MonoBehaviour
{
    // Briefing dataset
    public List<TheatersportBriefing> briefings = new List<TheatersportBriefing>();

    // Kaarten dataset
    public List<TheatersportKaart> kaarten = new List<TheatersportKaart>();

    // De static database instance
    public static TheatersportDatabase Instance;

    private void Awake()
    {
        if (Instance == null)
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

    private void Start()
    {
        Debug.Log(kaarten.Count);
    }
}