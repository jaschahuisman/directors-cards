using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OnReceiveCardMock : MonoBehaviour
{
    private Database database;

    public ImprovCardScriptable _soPlayer;
    private AudioSource buzzer;
    public GameObject improvCard;


    //private float index;
    
    void Start()
    {
        database = Database.Instance;
        buzzer = GetComponent<AudioSource>();  
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Random card gets send from database to client (mock)
            OnReceiveCard(Random.Range(0, database.improvCards.Count)); 
        }
    }

    void OnReceiveCard(int cardId)
    {
        Debug.Log("### Receive Card: " + cardId);

        // Find card in database by index
        _soPlayer = database.improvCards[cardId];

        // Play buzzer sound
        buzzer.Play();

        // Haptic (trillen) feedback in left controller

        // Instantiate card prefab
        Instantiate(improvCard);

        // Fill in card prefab texts with ImrpovCard's data

        // Parent card prefab instance to world space UI on users wrist
    }
}
