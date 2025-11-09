using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTextManager : MonoBehaviour
{
    private Collider textCollider;

    private void Awake()
    {
        textCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //print(other.name);
        
        TextTrigger textTrigger = other.GetComponent<TextTrigger>();
        if (textTrigger != null)
        {
            if(!textTrigger.haveDone)
                TextManager.instance.textTriggers.Add(textTrigger);
        }
    }
    
    
}
