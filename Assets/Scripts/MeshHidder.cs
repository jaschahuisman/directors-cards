using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHidder : MonoBehaviour
{

public GameObject hand;


    public void Show()
    {
        hand.SetActive(true);
    }

    public void Hide() 
    {
        hand.SetActive(false);
    }

}
