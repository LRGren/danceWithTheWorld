using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestory : MonoBehaviour
{
    public float setTime = 0.1f;
    private float time = 0;

    private void FixedUpdate()
    {
        time+= Time.fixedDeltaTime;
        if (time >= setTime)
        {
            Destroy(this.gameObject);
        }
    }
}
