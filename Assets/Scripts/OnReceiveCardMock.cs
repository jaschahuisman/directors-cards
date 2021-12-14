using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.XR.Interaction.Toolkit;

public class OnReceiveCardMock : MonoBehaviour
{
    private Database database;
    private AudioSource buzzer;

    public XRBaseController controller;
    public GameObject improvCardPrefab;
    public Transform playerWrist;
    public ImprovCardScriptable improvCardScriptable;
    

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
        improvCardScriptable = database.improvCards[cardId];

        // Play buzzer sound
        buzzer.Play();

        // Haptic (trillen) feedback in left controller
        SendHaptics();

        // Remove existing card from users wrist
        foreach (Transform child in playerWrist) { Destroy(child.gameObject); }

        // Instantiate card prefab
        GameObject cardObject = Instantiate(improvCardPrefab);
        PlayerImprovCard playerImprovCard = cardObject.GetComponent<PlayerImprovCard>();


        // Parent card prefab instance to world space UI on users wrist
        cardObject.gameObject.transform.SetParent(playerWrist, false);

        // Fill in card prefab texts with ImprovCard's data
        playerImprovCard.SetData(cardId);
    }

    void SendHaptics()
    {
        if (controller != null)
            controller.SendHapticImpulse(0.9f, 0.25f);
    }
}
