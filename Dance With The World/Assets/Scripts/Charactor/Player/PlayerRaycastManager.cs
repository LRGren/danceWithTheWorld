using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycastManager : MonoBehaviour
{
    public int interactionRange;
    public GameObject currentTarget;
    public GameObject lastTarget;
    
    public LayerMask actorLayer;
    public LayerMask highlightedLayer;

    private void Update()
    {
        HandleRayCast();
    }

    private void HandleRayCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width, Screen.height) * .5f),
                out hit, interactionRange, LayerMask.GetMask("Actor")))
        {
            currentTarget = hit.collider.gameObject;
            if (lastTarget != null)
            {
                lastTarget.layer = actorLayer;
                lastTarget = null;
            }

            if (currentTarget != null)
            {
                currentTarget.layer = highlightedLayer;
                lastTarget = currentTarget;
            }
            else
            {
                currentTarget = null;
                if (lastTarget != null)
                {
                    lastTarget.layer = actorLayer;
                }
            }
        }
    }
}
