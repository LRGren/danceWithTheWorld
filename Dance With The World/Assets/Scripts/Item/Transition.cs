using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    public int type = 1;
    public int targetSceneIndex;
    public Transform targetPosition;
    private void OnTriggerStay(Collider other)
    {
        if (type == 1)
        {
            if (other.CompareTag("Player"))
            {
                if (Input.GetKey(KeyCode.E))
                {
                    GameManager.instance.LoadScene(targetSceneIndex);
                }
            }
        }
        else if (type == 2)
        {
            if (other.CompareTag("Player"))
            {
                if (Input.GetKey(KeyCode.E))
                {
                    GameManager.instance.TP(targetPosition.position);
                }
            }
        }
    }
}
