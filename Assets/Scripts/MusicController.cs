using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioSource _audioSource;

    public GameState gameState;

    public AudioClip music;

    public AudioClip bossMusic;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();    
    }

    void Update()
    {
        var audioClip = gameState.bossWave ? bossMusic : music;
        if (_audioSource.clip != audioClip)
        {
            _audioSource.clip = audioClip;
            _audioSource.Play();
        }
    }
}
