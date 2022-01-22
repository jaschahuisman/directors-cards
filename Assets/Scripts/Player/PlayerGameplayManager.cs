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

    [Header("Camera related components")]
    [SerializeField] private Camera playerCamera;
    private int defaultCullingMask;

    [Header("Card related components")]
    [SerializeField] private GameObject playerCardPrefab;
    [SerializeField] private Transform playerWristTransform;
    [SerializeField] private XRController leftController;
    [SerializeField] private Animator notificationAnimator;
    [SerializeField] private AudioSource buzzerSound;

    [Header("Head related components")]
    [SerializeField] public Transform playerHeadTransform;

    public override void OnStartAuthority()
    {
        defaultCullingMask = playerCamera.cullingMask;
        base.OnStartAuthority();
    }

    public void DestroyCards()
    {
        foreach (Transform child in playerWristTransform) { Destroy(child.gameObject); }
    }

    public void DestroyHead()
    {
        foreach (Transform child in playerHeadTransform)
        {
            NetworkServer.Destroy(child.gameObject);
            NetworkServer.UnSpawn(child.gameObject);
        }
    }

    [Server]
    public void UpdateCharacter(int briefingIndex)
    {
        UpdateHead(briefingIndex);
        UpdateHandsColor(briefingIndex);
        
        RpcSetCameraCulling();
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
            RpcSetHeadTransform(head);
        }
    }

    [ClientRpc]
    private void RpcSetHeadTransform(GameObject head)
    {
        head.transform.SetParent(playerHeadTransform, false);

        head.transform.position = playerHeadTransform.position;
        head.transform.rotation = playerHeadTransform.rotation;
        head.transform.localScale = playerHeadTransform.localScale;
    }

    [Server]
    private void UpdateHandsColor(int briefingIndex)
    {
        var briefing = Database.Instance.briefings[briefingIndex];

        Color handsColor = networkPlayer.team == PlayerType.Player1
           ? briefing.playerRole1.handsColor
           : briefing.playerRole2.handsColor;

        RpcHandleSetHandColor(handsColor.r, handsColor.g, handsColor.b);
    }

    [ClientRpc]
    private void RpcHandleSetHandColor(float r, float g, float b)
    {
        playerHandsController.HandleSetHandColor(r,g,b); 
    }

    [ClientRpc]
    public void RpcSetCameraCulling()
    {
        if (isLocalPlayer)
        {
            string layerToHide = (networkPlayer.team == PlayerType.Player1)
                ? "Player2Only"
                : "Player1Only";

            string layerToShow = (networkPlayer.team == PlayerType.Player1)
                ? "Player1Only"
                : "Player2Only";

            HideLayer(layerToHide);
            ShowLayer(layerToShow);
        }
    }

    [ClientRpc]
    public void RpcResetCameraCulling()
    {
        playerCamera.cullingMask = defaultCullingMask;
    }

    private void ShowLayer(string layer)
    {
        playerCamera.cullingMask |= 1 << LayerMask.NameToLayer(layer);
    }

    private void HideLayer(string layer)
    {
        playerCamera.cullingMask &= ~(1 << LayerMask.NameToLayer(layer));
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
