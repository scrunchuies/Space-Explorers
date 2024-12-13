using System.Collections;
using System.Collections.Generic;
using DigitalRuby.SoundManagerNamespace;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController instance { get; private set; }


    private AudioSource source;

    private void Awake()
    {
        instance = this;
        source = GetComponent<AudioSource>();

    }

    public void PlaySound(AudioClip _sound)
    {

        source.PlayOneShot(_sound);

    }
}
