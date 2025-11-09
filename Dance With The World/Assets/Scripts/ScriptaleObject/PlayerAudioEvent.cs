using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/PlayerAudioSO")]
public class PlayerAudioEvent : ScriptableObject
{
    public UnityAction<AudioClip> OnEventRaised;

    public void RaisedEvent(AudioClip clip)
    {
        OnEventRaised?.Invoke(clip);
    }
}
