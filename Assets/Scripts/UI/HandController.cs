using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Mirror;

public class HandController : NetworkBehaviour
{
    [SerializeField] private HandType handType;
    [SerializeField] private Animator animator;
    
    private InputDevice inputDevice;

    private float thumbMoveSpeed;
    private float indexValue, thumbValue, threeFingersValue;

    void Start()
    { 
        inputDevice = GetInputDevice();
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            AnimateHand();
        }
    }

    InputDevice GetInputDevice()
    {
        InputDeviceCharacteristics controllerCharacteristic = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller;

        controllerCharacteristic = controllerCharacteristic | (
            handType == HandType.Left 
                ? InputDeviceCharacteristics.Left 
                : InputDeviceCharacteristics.Right
        );

        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristic, inputDevices);

        if (inputDevices.Count > 0) { return inputDevices[0]; } 
        else { return new InputDevice(); }
    }

    void AnimateHand()
    {
        inputDevice.TryGetFeatureValue(CommonUsages.trigger, out indexValue);
        inputDevice.TryGetFeatureValue(CommonUsages.grip, out threeFingersValue);

        inputDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouched);
        inputDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out bool secondaryTouched);

        if (primaryTouched || secondaryTouched) { thumbValue += thumbMoveSpeed; }
        else { thumbValue -= thumbMoveSpeed; }

        thumbValue = Mathf.Clamp(thumbValue, 0, 1);

        animator.SetFloat("Index", indexValue);
        animator.SetFloat("ThreeFingers", threeFingersValue);
        animator.SetFloat("Thumb", thumbValue);

        CmdAnimateHand(indexValue, threeFingersValue);
    }

    [Command]
    void CmdAnimateHand(float indexValue, float threeFingersValue)
    {
        animator.SetFloat("Index", indexValue);
        animator.SetFloat("ThreeFingers", threeFingersValue);
    }
}

public enum HandType { Left, Right }