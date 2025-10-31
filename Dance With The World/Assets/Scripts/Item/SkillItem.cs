using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillItem : MonoBehaviour
{
    public string MoverName;
    public bool trigger = false;
    [SerializeField] protected bool hasTriggered = false;

    protected virtual void Update()
    {
        if (trigger && !hasTriggered)
        {
            SkillOn();
            trigger = false;
            hasTriggered = true;
        }
    }

    protected virtual void SkillOn() {}
    
}
