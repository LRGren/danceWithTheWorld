using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public PlayerAudioEvent BGMEvent;
    public PlayerAudioEvent FXEvent;
    public PlayerAudioEvent ItemEvent;
    
    public AudioSource BGM;
    public AudioSource SFX;
    public AudioSource Item;

    private void OnEnable()
    {
        FXEvent.OnEventRaised += OnFXEvent;
        BGMEvent.OnEventRaised += OnBGMEvent;
        ItemEvent.OnEventRaised += OnItemEvent;
    }

    private void OnItemEvent(AudioClip arg0)
    {
        Item.clip = arg0;
        Item.Play();
    }

    private void OnBGMEvent(AudioClip clip)
    {
        BGM.clip = clip;
        BGM.Play();
    }

    private void OnFXEvent(AudioClip clip)
    {
        SFX.clip = clip;
        SFX.Play();
    }


    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnDisable()
    {
        BGMEvent.OnEventRaised -= OnBGMEvent;
        FXEvent.OnEventRaised -= OnFXEvent;
        ItemEvent.OnEventRaised -= OnItemEvent;
    }
}
