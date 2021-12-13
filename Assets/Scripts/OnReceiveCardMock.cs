using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnReceiveCardMock : MonoBehaviour
{
    private Database database;
    
    void Start()
    {
        database = Database.Instance;  
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

        // Play buzzer sound

        // Haptic feedback in left controller

        // Instantiate card prefab
        // Fill in card prefab texts with ImrpovCard's data

        // Parent card prefab instance to world space UI on users wrist
    }
}
