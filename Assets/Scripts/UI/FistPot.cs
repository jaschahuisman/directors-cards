using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Mirror;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(ParticleSystem))]
public class FistPot : MonoBehaviour
{
    private AudioSource audioSource;
    private ParticleSystem particleSystem;
    private XRController currentController;

    public PlayerType team;
    public bool activated;

    public static event Action OnFistEnterDiscoverEvent;
    public static event Action<PlayerType, NetworkPlayer> OnFistEnterJoinEvent;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        particleSystem = GetComponent<ParticleSystem>();
        audioSource.playOnAwake = false;
    }

    public void Activate(bool value)
    {
        activated = value;

        if (value == true)
        {
            currentController.SendHapticImpulse(1, 0.4f);
            particleSystem.Play();
            audioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<XRController>() != null)
        {
            currentController = other.GetComponent<XRController>();
            NetworkPlayer player = other.GetComponentInParent<NetworkPlayer>();

            if (player != null)
                OnFistEnterJoinEvent?.Invoke(team, player);
            else
                OnFistEnterDiscoverEvent?.Invoke();
        }
    }
}
