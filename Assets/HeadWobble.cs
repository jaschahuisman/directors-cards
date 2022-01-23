using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadWobble : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(AnimateAfterRandomTime());
    }

    IEnumerator AnimateAfterRandomTime()
    {
        yield return new WaitForSeconds(Random.Range(0, 4));
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("StartAnimation");
    }
}
