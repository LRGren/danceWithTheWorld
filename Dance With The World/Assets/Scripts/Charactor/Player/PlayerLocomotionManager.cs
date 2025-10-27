using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerLocomotionManager : CharactorLocomotionManager
{
    private PlayerManager player;
    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float moveAmount;
    
    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    
    [SerializeField] float walkingSpeed = 1.5f;
    [SerializeField] float runningSpeed = 3.5f;
    [SerializeField] float rotationSpeed = 15f;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] float gravity = -9.81f;
    
    [Header("RootMotion")]
    private Vector3 deltaPosition;
    private Quaternion deltaRotation;
    
    [Header("Ground Detection")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    
    [Header("Jump Settings")]
    public int maxJumps = 1;
    public float airControl = 0.5f;
    
    private int jumpsRemaining;
    private Vector3 velocity;
    private bool isGrounded;
    
    protected override void Awake()
    {
        base.Awake();
        
        player = GetComponent<PlayerManager>();
    }
    
    protected override void Start()
    {
        base.Start();
        
        jumpsRemaining = maxJumps;
    }

    protected override void Update()
    {
        base.Update();
        ApplyGravity();
        CheckGrounded();

        if (!player.playerAnimationManager.CheckState("land", "Locomotion"))
        {
            HandleGroundedMovement();
            HandleRotation();
            HandleExtralMovement();
        }

        HandleJump();
    }

    private void FixedUpdate()
    {
        FixedFallTime();
    }

    private void GetMovementValues()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount  = PlayerInputManager.instance.moveAmount;
    }

    private void HandleGroundedMovement()
    {
        if (player.isDancingMode)
            return;
        
        GetMovementValues();
        
        //our move direction is based on our cameras facing perspective & our movement input
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;
        
        if (PlayerInputManager.instance.moveAmount > 0.5f)
        {
            //running
            player.characterController.Move(moveDirection * (runningSpeed * Time.deltaTime));
        }
        else if (PlayerInputManager.instance.moveAmount <= 0.5f)
        {
            //walking
            player.characterController.Move(moveDirection * (walkingSpeed * Time.deltaTime));
        }
    }
    
    private void HandleRotation()
    {
        if(player.isDancingMode)
            return;
        
        targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;
        
        if (targetRotationDirection == Vector3.zero) 
        {
            targetRotationDirection = transform.forward;
        }
        
        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
        transform.rotation = targetRotation;
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && jumpsRemaining > 0 &&
            !player.playerAnimationManager.CheckState("land", "Locomotion")) 
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            player.playerAnimationManager.SetAnimTrigger("jump");
            player.playerAnimationManager.ResetAnimTrigger("ground");
            jumpsRemaining--;
        }
    }
    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        player.characterController.Move(velocity * Time.deltaTime);
    }
    
    public float FallTime = 0;
    public float MaxFallTime;
    void FixedFallTime()
    {
        if (velocity.y < 0)
            FallTime += Time.deltaTime;

        if (FallTime >= MaxFallTime)
            player.playerAnimationManager.SetAnimBool("longFall", true);
        else
            player.playerAnimationManager.SetAnimBool("longFall", false);

        if (player.playerAnimationManager.CheckStateWithTag("ground", "Locomotion"))
            FallTime = 0;
    }
    
    void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            player.playerAnimationManager.SetAnimTrigger("ground");
            jumpsRemaining = maxJumps;
        }
    }
    
    private void HandleExtralMovement()
    {
        if (player.playerAnimationManager.CheckStateWithTag("final", "Dance_W"))
        {
            player.characterController.Move(deltaPosition);
            transform.rotation *= deltaRotation;
        }

        deltaPosition = Vector3.zero;
        deltaRotation =  Quaternion.identity;
    }
    
    private void OnUpdateRootMotionPosition(object _deltaPosition)
    {
        deltaPosition +=  (Vector3)_deltaPosition;
    }

    private void OnUpdateRootMotionRotation(object _deltaRotation)
    {
        deltaRotation *= (Quaternion)_deltaRotation;
    }
}
