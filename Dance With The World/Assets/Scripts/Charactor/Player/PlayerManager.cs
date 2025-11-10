using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : CharacterManager
{
    public static PlayerManager instance;
    
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerInputManager playerInputManager;
    [HideInInspector] public PlayerAnimationManager playerAnimationManager;
    
    public bool isDancingMode = false;
    
    public KeyCode changeKey = KeyCode.Tab;
    
    public GameObject playerModel;
    //public TextMeshProUGUI modeText;
    //public TextMeshProUGUI finalSkillText;
    
    protected override void Awake()
    {
        if(instance==null)
            instance = this;
        else
            Destroy(this.gameObject);
        
        base.Awake();
        
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerInputManager = PlayerInputManager.instance;
        playerAnimationManager = GetComponentInChildren<PlayerAnimationManager>();
    }

    private void DancingModeMotion()
    {
        PlayerSkillInputManager.instance.enabledSkillInput = true;
        
        //finalSkillText.text = PlayerSkillInputManager.instance.FINAL_SKILL_NAME;
            
        playerAnimationManager.SetAnimFloat("velocity", 0);

        playerAnimationManager.WhichToPlay(PlayerSkillInputManager.instance.FINAL_SKILL_NAME);
    }

    private void WalkingModeMotion()
    {
        PlayerSkillInputManager.instance.enabledSkillInput = false;
        
        playerAnimationManager.ResetAnimation();
        
        playerAnimationManager.SetAnimFloat("velocity",playerInputManager.moveAmount);
    }

    private void UpdateGameManager()
    {
        GameManager.instance.isDancingMode = isDancingMode;
    }
    
    private void Update()
    {
        if(GameManager.instance.isUI)
            return;
        
        UpdateGameManager();
        
        ChangeControlMode();
        
        if(!isDancingMode)
            WalkingModeMotion();
        else
            DancingModeMotion();
    }

    private void ChangeControlMode()
    {
        if (Input.GetKeyDown(changeKey))
        {
            PlayerSkillInputManager.instance.Reset();
            isDancingMode = !isDancingMode;
            
            //是否保留
            //playerAnimationManager.ResetAnimation();
            
            /*if (isDancingMode)
            {
                modeText.text = "Dancing mode";
            }
            else
            {
                modeText.text = "Walking mode";
            }*/
        }
    }
    
    protected override void LateUpdate()
    {
        base.LateUpdate();
        
        PlayerCamera.instance.HandleAllCameraActions();
    }
}
