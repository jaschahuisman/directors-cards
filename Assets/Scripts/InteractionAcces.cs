using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionAcces : MonoBehaviour
{

        public Animator watchAnim;

    public void Update()
    {

         if (Input.GetKeyDown(KeyCode.Space))
         {

            //play feedback animation once
            watchAnim.SetTrigger("PlayAnimation");

         }
    }
}
