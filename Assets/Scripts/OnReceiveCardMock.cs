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
    public GameObject wrist;

    public ImprovCardScriptable improvCardScriptable;
    

    public Camera headset;
    RaycastHit hit;
    public float distanceToSee = 0.2f;
    

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

        Debug.DrawRay(headset.transform.position, headset.transform.forward * distanceToSee, Color.red);

        if (Physics.Raycast(headset.transform.position, headset.transform.forward, out hit, distanceToSee))
        {


            if (hit.collider.gameObject.name == "LeftHand Controller")
            {
                Debug.Log("Hand Hit");
                StartCoroutine(ShowCard());
            }
   
        }
        else
        {
            wrist.SetActive(false);
            Debug.Log("Nothing");
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
        cardObject.SetActive(true);

        // Fill in card prefab texts with ImprovCard's data
        playerImprovCard.SetData(cardId);

        
    }

    void SendHaptics()
    {
        if (controller != null)
            controller.SendHapticImpulse(1f, 0.5f);
    }

    IEnumerator ShowCard() 
    {
        yield return new WaitForSeconds(0.3f);
        wrist.SetActive(true);
    }
}
