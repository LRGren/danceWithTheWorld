using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorLocomotionManager : MonoBehaviour
{
    private CharacterManager character;
    
    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }
}
