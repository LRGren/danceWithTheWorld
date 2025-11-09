using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillItem : MonoBehaviour
{
    public string MoverName;
    public GameObject model;
    public bool trigger = false;
    public bool hasTriggered = false;
    public AudioDefination fx;

    private void Awake()
    {
        fx = GetComponent<AudioDefination>();
    }

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
