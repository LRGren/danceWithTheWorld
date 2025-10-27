using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimationBack()
    {
        animator.SetTrigger("back");
    }
    
    public void PlayAnimation(string animationName)
    {
        animator.Play(animationName);
    }

    public void WhichToPlay(string stageName)
    {
        if (stageName == "null")
            return;
        
        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("final"))
            return;
        
        switch (stageName)
        {
            case "upper_hand":
                if(CheckStateWithTag("final","Dance_W"))
                    PlayAnimationBack();
                
                animator.Play("New State", 4);//Dance_W_L
                animator.Play("New State", 5);//Dance_W_L
                animator.Play("New State", 6);//Dance_W_L
                animator.Play("New State", 7);//Dance_W
                
                PlayAnimation("upper_hand_R");
                PlayAnimation("upper_hand_L");
                break;
            case "squat_down":
                if(CheckStateWithTag("final","Dance_W"))
                    PlayAnimationBack();
                
                animator.Play("New State", 7);//Dance_W
                
                animator.Play("New State", 2);//Dance_RH
                animator.Play("New State", 3);//Dance_LH
                
                if(!CheckState("crouch","Dance_WL"))
                    PlayAnimation("crouch");
                break;
            case "upper_hand_and_leg":
                if(CheckStateWithTag("final","Dance_W"))
                    PlayAnimationBack();
                
                animator.Play("New State", 7);//Dance_W
                
                animator.Play("New State", 6);//Dance_W_L
                
                PlayAnimation("upper_hand_R");
                PlayAnimation("upper_hand_L");
                PlayAnimation("upper_leg_R");
                break;
            case "spider_shake_punch":
                PlayAnimationBack();
                
                PlayAnimation("spider_shake_punch");
                break;
            case "hawaii_shake":
                PlayAnimationBack();

                PlayAnimation("hawaii_shake");
                break;
            case "subject_three":
                PlayAnimationBack();

                PlayAnimation("subject_three");
                break;
            case "thomas_spin":
                PlayAnimationBack();

                PlayAnimation("breakdance");
                break;
            case "sukarno":
                PlayAnimationBack();

                PlayAnimation("sukarno");
                break;
            case "rotate":
                PlayAnimationBack();

                PlayAnimation("rotate");
                break;
            case "jumping":
                PlayAnimationBack();

                PlayAnimation("jumping");
                break;
            default:
                return;
        }
    }
    
    public void SetAnimFloat(string variable, float value)
    {
        animator.SetFloat(variable, value);
    }

    public void SetAnimBool(string variable, bool value)
    {
        animator.SetBool(variable, value);
    }
    
    public void SetAnimTrigger(string variable)
    {
        animator.SetTrigger(variable);
    }
    public void ResetAnimTrigger(string variable)
    {
        animator.ResetTrigger(variable);
    }

    public void SetLayerWeight(int layerIndex, float value)
    {
        animator.SetLayerWeight(layerIndex, value);
    }

    public bool CheckState(string stateName, string layerName)
    {
        return animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(layerName)).IsName(stateName);
    }
    
    public bool CheckStateWithTag(string stateTag, string layerName)
    {
        return animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex(layerName)).IsTag(stateTag);
    }

    public void ResetAnimation()
    {
        animator.SetTrigger("break");
    }
    
}
