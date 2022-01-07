using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HandsController : NetworkBehaviour
{
    [SerializeField] private Animator leftHandAnimator;
    [SerializeField] private Animator rightHandAnimator;
    [SerializeField] private GameObject leftHandMesh;
    [SerializeField] private GameObject rightHandMesh;

    #region Animation
    public void HandleHandAnimation(HandType handType, float indexValue, float threeFingersValue, float thumbValue)
    {
        if (isLocalPlayer)
        {
            CmdSendAnimationDataToServer(handType, indexValue, threeFingersValue, thumbValue);
            AnimateHand(handType, indexValue, threeFingersValue, thumbValue);
        }
    }

    [Command]
    public void CmdSendAnimationDataToServer(HandType handType, float indexValue, float threeFingersValue, float thumbValue)
    {
        RpcReceiveAnimationDataOnClient(handType, indexValue, threeFingersValue, thumbValue);
        AnimateHand(handType, indexValue, threeFingersValue, thumbValue);
    }

    [ClientRpc]
    public void RpcReceiveAnimationDataOnClient(HandType handType, float indexValue, float threeFingersValue, float thumbValue)
    {
        if (!isLocalPlayer)
        {
            AnimateHand(handType, indexValue, threeFingersValue, thumbValue);
        }
    }

    public void AnimateHand(HandType handType, float indexValue, float threeFingersValue, float thumbValue)
    {
        Animator animator = handType == HandType.Left ? leftHandAnimator : rightHandAnimator;
        animator.SetFloat("Index", indexValue);
        animator.SetFloat("ThreeFingers", threeFingersValue);
        animator.SetFloat("Thumb", thumbValue);
    }
    #endregion

    #region Toggling
    public void HandleToggleHand(bool value, HandType handType)
    {
        if (isLocalPlayer)
        {
            CmdToggleHand(value, handType);
            ToggleHand(value, handType);
        }
    }

    [Command]
    public void CmdToggleHand(bool value, HandType handType)
    {
        RpcReceiveToggleHand(value, handType);
    }

    [ClientRpc]
    public void RpcReceiveToggleHand(bool value, HandType handType)
    {
        ToggleHand(value, handType);
    }

    public void ToggleHand(bool value, HandType handType)
    {
        if (handType == HandType.Left)
        {
            leftHandMesh.SetActive(value);
        }

        if (handType == HandType.Right)
        {
            rightHandMesh.SetActive(value);
        }
    }
    #endregion
}

