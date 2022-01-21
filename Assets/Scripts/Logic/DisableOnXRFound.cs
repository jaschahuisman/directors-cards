using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnXRFound : MonoBehaviour
{
    List<UnityEngine.XR.InputDevice> inputDevices = new List<UnityEngine.XR.InputDevice>();

    private void Start()
    {
        UnityEngine.XR.InputDevices.GetDevices(inputDevices);
        if (inputDevices.Count > 0)
            gameObject.SetActive(false);
    }
}
