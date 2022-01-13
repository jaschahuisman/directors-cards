using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;
using Mirror;

public class PlayerGameplayManager : NetworkBehaviour
{
    [Header("Network player components")]
    [SerializeField] private NetworkPlayer networkPlayer;
    [SerializeField] private PlayerHandsController playerHandsController;

    [Header("Card related components")]
    [SerializeField] private GameObject playerCardPrefab;
    [SerializeField] private Transform playerWristTransform;
    [SerializeField] private XRController leftController;
    [SerializeField] private Animator notificationAnimator;
    [SerializeField] private AudioSource buzzerSound;

    [Header("Head related components")]
    [SerializeField] public Transform playerHeadTransform;

    public void DestroyCards()
    {
        foreach (Transform child in playerWristTransform) { Destroy(child.gameObject); }
    }

    public void DestroyHead()
    {
        foreach (Transform child in playerHeadTransform) { NetworkServer.UnSpawn(child.gameObject); }
    }

    [Server]
    public void UpdateCharacter(int briefingIndex)
    {
        UpdateHead(briefingIndex);
        UpdateHandsColor(briefingIndex);
    }

    [Server]
    private void UpdateHead(int briefingIndex)
    {
        var briefing = Database.Instance.briefings[briefingIndex];

        DestroyHead();
        GameObject headObject = networkPlayer.team == PlayerType.Player1
                ? briefing.playerRole1.headPrefab
                : briefing.playerRole2.headPrefab;

        if (headObject != null)
        {
            GameObject head = Instantiate(headObject);

            head.transform.SetParent(playerHeadTransform, false);

            head.transform.position = playerHeadTransform.position;
            head.transform.rotation = playerHeadTransform.rotation;
            head.transform.localScale = playerHeadTransform.localScale;

            NetworkServer.Spawn(head, gameObject);
        }
    }

    [Server]
    private void UpdateHandsColor(int briefingIndex)
    {
        var briefing = Database.Instance.briefings[briefingIndex];

        Color handsColor = networkPlayer.team == PlayerType.Player1
           ? briefing.playerRole1.handsColor
           : briefing.playerRole2.handsColor;

        // playerHandsController.handleSetHandColor(handsColor.r, handsColor.g, handsColor.b); 
    }


    [ClientRpc]
    public void RpcReceiveCard(int cardIndex)
    {
        if (isLocalPlayer)
        {
            Card card = Database.Instance.cards[cardIndex];

            SendHaptics();
            buzzerSound.Play();
            notificationAnimator.SetTrigger("PlayAnimation");

            foreach (Transform child in playerWristTransform) { Destroy(child.gameObject); }

            GameObject playerCardObject = Instantiate(playerCardPrefab);
            PlayerCard playerCard = playerCardObject.GetComponent<PlayerCard>();

            playerCard.SetData(card);

            playerCardObject.transform.SetParent(playerWristTransform, false);
            playerCardObject.SetActive(true);

            Debug.LogWarning("Received card with index " + cardIndex);
        }
    }

    private void SendHaptics()
    {
        if (leftController != null) { leftController.SendHapticImpulse(1f, 0.6f); }
    }
}
