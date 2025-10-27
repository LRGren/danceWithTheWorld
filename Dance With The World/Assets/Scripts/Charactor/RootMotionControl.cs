using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotionControl : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorMove()
    {
        SendMessageUpwards("OnUpdateRootMotionPosition",(object)animator.deltaPosition);
        SendMessageUpwards("OnUpdateRootMotionRotation",(object)animator.deltaRotation);
    }
}
