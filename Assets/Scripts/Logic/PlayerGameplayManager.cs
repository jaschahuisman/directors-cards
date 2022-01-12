using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class PlayerGameplayManager : MonoBehaviour
{
    [Header("Card related components")]
    [SerializeField] private GameObject playerCardPrefab;
    [SerializeField] private Transform playerWristTransform;
    [SerializeField] private XRController leftController;
    [SerializeField] private Animator notificationAnimator;
    [SerializeField] private AudioSource buzzerSound;

    [Header("Head related components")]
    [SerializeField] private Transform playerHeadTransform;

    public void DestroyCards()
    {
        foreach (Transform child in playerWristTransform) { Destroy(child.gameObject); }
    }
}
