using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractorMeshHider : MonoBehaviour
{
    public HandsController handsController;
    public HandType handType;

    public void HideMesh()
    {
        handsController.ToggleHand(false, handType);
    }

    public void ShowMesh()
    {
        handsController.ToggleHand(true, handType);
    }
}
