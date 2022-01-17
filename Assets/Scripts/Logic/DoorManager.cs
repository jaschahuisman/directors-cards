using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [SerializeField] private Vector3 openPosition;
    [SerializeField] private float openSpeed;
    [SerializeField] private AudioSource audioSource;

    private Vector3 defaultPosition;
    private Vector3 desiredPosition;

    public bool isOpen;

    private void Start()
    {
        defaultPosition = gameObject.transform.position;
    }

    private void Update()
    {
        desiredPosition = (isOpen) ? openPosition : defaultPosition;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, openSpeed * Time.deltaTime);
    }

    public void SetOpen(bool state)
    {
        isOpen = state;
        audioSource.Play();
    }
}
