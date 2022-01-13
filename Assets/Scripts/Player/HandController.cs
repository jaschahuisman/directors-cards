using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Mirror;

public enum HandType { Left, Right };

public class HandController : MonoBehaviour
{
    [SerializeField] private HandType handType;
    [SerializeField] private PlayerHandsController playerHandsController;
    [SerializeField] private float thumbMoveSpeed = 0.45f;

    [Header("Offline rig animator components")]
    [SerializeField] private bool offline = false;
    [SerializeField] private Animator offlineAnimator;


    private InputDevice inputDevice;

    private float indexValue, thumbValue, threeFingersValue;

    void Start()
    {
        GetInputDevice(out inputDevice);
    }

    void Update()
    {
        if (NetworkClient.ready || offline == true)
        {
            if (inputDevice != null) AnimateHand();
            else GetInputDevice(out inputDevice);
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

        if (inputDevices.Count > 0)
        {
            device = inputDevices[0];
            return true;
        }
        else
        {
            device = new InputDevice();
            return false;
        }
    }

    private void AnimateHand()
    {
        inputDevice.TryGetFeatureValue(CommonUsages.trigger, out indexValue);
        inputDevice.TryGetFeatureValue(CommonUsages.grip, out threeFingersValue);

        inputDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouched);
        inputDevice.TryGetFeatureValue(CommonUsages.secondaryTouch, out bool secondaryTouched);

        if (primaryTouched || secondaryTouched) { thumbValue += thumbMoveSpeed; }
        else { thumbValue -= thumbMoveSpeed; }

        thumbValue = Mathf.Clamp(thumbValue, 0, 1);

        if (offline == false && playerHandsController != null)
        {
            playerHandsController.HandleHandAnimation(handType, indexValue, threeFingersValue, thumbValue);
        }
        else
        {
            if (offlineAnimator == null) return;

            offlineAnimator.SetFloat("Index", indexValue);
            offlineAnimator.SetFloat("ThreeFingers", threeFingersValue);
            offlineAnimator.SetFloat("Thumb", thumbValue);
        }
    }
}
