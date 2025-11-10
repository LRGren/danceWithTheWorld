using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    
    public PlayerManager player;
    public Camera cameraObject;
    
    //cameraPivotTransform is used to control up and down rotation
    [SerializeField] private Transform cameraPivotTransform;
    
    //change these to tweak camera performance
    [Header("Camera Settings")] 
    [SerializeField] private float cameraSmoothSpeed = 1f;//the bigger this number, the longer for the camera to reach its position during movement
    [SerializeField] private float leftAndRightRotationSpeed = 220f;
    [SerializeField] private float upAndDownRotationSpeed = 220f;
    [SerializeField] private float minimumPivot = -30f;//lowest point
    [SerializeField] private float maximumPivot = 60f;//highest point
    [SerializeField] private float cameraCollisionRadius = 0.2f;
    [SerializeField] private LayerMask collideWithLayers;
    
    //just displays
    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPosition;//used for camera collisions
    [SerializeField] private float leftAndRightLookAngle;
    [SerializeField] private float upAndDownLookAngle;
    private float cameraZPosition;//values for camera collisions
    private float targetCameraZPosition;//values for camera collisions
    
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
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions()
    {
        if (player) 
        {
            //follow the player
            HandleFollowTarget();
            //rotate around the player
            HandleRotation();
            //collide the objects   
            HandleCollision();
        }
    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position,
            ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotation()
    {
        //if lock down, force rotation towards target
        //else rotate regularly
        
        //normal rotations
        //rotate left and right based on horizontal movement on the right joystick
        leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) *
                                 Time.deltaTime;
        //rotate up and down based on vertical movement on the right joystick
        upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) *
                                 Time.deltaTime;
        //clamp up and down look angle between a min and max value
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);
        
        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;
        //rotate this gameobject left and right
        cameraRotation.y = leftAndRightLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = targetRotation;
        
        //rotate this gameobject up and down
        cameraRotation = Vector3.zero;
        cameraRotation.x = upAndDownLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    private void HandleCollision()
    {
        //reset it every time it is called
        //we only change its z-value
        targetCameraZPosition = cameraZPosition;
        
        RaycastHit hit;
        //direction for collision check
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        //we check if there is an object in front of our desired direction( see above )
        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit,
                Mathf.Abs(targetCameraZPosition), collideWithLayers)) 
        {
            //if there is, we get out distance from it
            float distanceFormHitObject = Vector3.Distance(hit.point, cameraPivotTransform.position);
            //we then equate our target Z position to the following
            targetCameraZPosition = -(distanceFormHitObject - cameraCollisionRadius);
        }
        
        //if our target position is less than our collision radius, we subtract our collision radius (making it snap back)
        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }
        
        //we then apply our final position using a lerp over a time of 0.2f.
        //we only change its z-value
        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }
    
}
