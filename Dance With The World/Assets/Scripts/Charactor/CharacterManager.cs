using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterManager : MonoBehaviour
{
    public CharacterController characterController;
    
    [Header("Flags")]
    public bool isJumping = false;
    public bool isGrounded = true;
    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
        
        characterController = GetComponent<CharacterController>();
    }

    protected virtual void  LateUpdate()
    {
        
    }
    
}
