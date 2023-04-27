using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance { get; private set; }
    private AudioSource Source;

    void Awake() {
        Instance = this;
        Source = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip sound) { 
        Source.PlayOneShot(sound);
    }
}