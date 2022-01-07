using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractorMeshHider : MonoBehaviour
{
    public GameObject handMesh;

    public void HideMesh()
    {
        handMesh.SetActive(false);
    }

    public void ShowMesh()
    {
        handMesh.SetActive(true);
    }
}
