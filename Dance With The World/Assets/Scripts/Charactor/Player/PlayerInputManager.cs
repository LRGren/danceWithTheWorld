using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    
    public PlayerManager player;
    
    [Header("Camera Movement Input")]
    public float cameraHorizontalInput;
    public float cameraVerticalInput;
    
    [Header("Player Movement Input")]
    public float horizontalInput;
    public float verticalInput;
    public float moveAmount;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        HandlePlayerMovementInput();
        HandleCameraMovementInput();
    }
    
    private void HandlePlayerMovementInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        
        //returns the absolute number, (Meaning number without the negative sign, so it's always positive)
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        switch (moveAmount)
        {
            //clamp the value, so they are 0, 0.5 or 1(optional)
            case <= 0.5f and > 0:
                moveAmount = 0.5f;
                break;
            case > 0.5f and <= 1:
                moveAmount = 1f;
                break;
        }

    }
    
    private void HandleCameraMovementInput()
    {
        cameraVerticalInput = Input.GetAxis("Mouse Y");
        cameraHorizontalInput = Input.GetAxis("Mouse X");
    }
}
