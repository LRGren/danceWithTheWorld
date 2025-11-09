using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDefination : MonoBehaviour
{
    public PlayerAudioEvent playerAudioEvent;
    
    public AudioClip audioClip;
    public AudioClip[] audioClips;
    public bool playOnEnable;

    private void OnEnable()
    {
        if (playOnEnable)
            PlayAudioClip();
    }

    public void PlayAudioClip()
    {
        playerAudioEvent.RaisedEvent(audioClip);
    }
    
    public void PlayAudioClip(int index)
    {
        playerAudioEvent.RaisedEvent(audioClips[index]);
    }
}
