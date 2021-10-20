using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnvilSounds : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _samples;
    private AudioSource _audio;

    private void Start()
    {
        _audio = GetComponent<AudioSource>();
    }
    public void Play()
    {
        _audio.clip = _samples[Random.Range(0, _samples.Count)];
        _audio.Play();
    }

}
