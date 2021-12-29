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
        if (isLocalPlayer && hasAuthority)
        {
            GetInputDevice(out inputDevice);
        }
    }

    void Update()
    {
        if (isLocalPlayer && hasAuthority)
        {
            if (inputDevice != null)
            {
                AnimateHand();   
            }
            else
            {
                GetInputDevice(out inputDevice);
            }
        }
    }

    private bool GetInputDevice(out InputDevice device)
    {
        InputDeviceCharacteristics controllerCharacteristic = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller;

        controllerCharacteristic = controllerCharacteristic | (
            handType == HandType.Left 
                ? InputDeviceCharacteristics.Left 
                : InputDeviceCharacteristics.Right
        );

        List<InputDevice> inputDevices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristic, inputDevices);

        if (inputDevices.Count > 0) { 
            device = inputDevices[0];
            return true;
        } 
        else {
            device = new InputDevice();
            return false;
        }
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