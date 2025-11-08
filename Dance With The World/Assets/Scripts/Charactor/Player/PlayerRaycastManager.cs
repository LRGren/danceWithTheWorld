using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycastManager : MonoBehaviour
{
    public static PlayerRaycastManager instance;
    
    public int interactionRange;
    public GameObject currentTarget;
    public GameObject lastTarget;
    
    public LayerMask actorLayer1, actorLayer2, actorLayer3;
    private LayerMask highlightedLayer;

    private LayerMask lastLayer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        actorLayer1 = LayerMask.NameToLayer("ActorRed");
        actorLayer2 = LayerMask.NameToLayer("ActorBlue");
        actorLayer3 = LayerMask.NameToLayer("ActorDark");
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (GameManager.instance.isDancingMode)
        {
            HandleRayCast();
        }
        else
        {
            currentTarget = null;
            if (lastTarget)
            {
                lastTarget.layer = lastLayer;
            }
        }
    }

    private void HandleRayCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width, Screen.height) * .5f),
                out hit, interactionRange, LayerMask.GetMask("ActorRed","ActorBlue","ActorDark","HighlightedActorRed","HighlightedActorBlue","HighlightedActorDark")))
        {
            currentTarget = hit.collider.gameObject;
            if (lastTarget != null)
            {
                lastTarget.layer = lastLayer;
                lastTarget = null;
            }

            if (currentTarget != null)
            {
                lastLayer = currentTarget.layer;
                lastTarget = currentTarget;

                if (currentTarget.layer == actorLayer1)
                {
                    currentTarget.layer = LayerMask.NameToLayer("HighlightedActorRed");
                }
                else if (currentTarget.layer == actorLayer2)
                {
                    currentTarget.layer = LayerMask.NameToLayer("HighlightedActorBlue");
                }
                else if (currentTarget.layer == actorLayer3)
                {
                    currentTarget.layer = LayerMask.NameToLayer("HighlightedActorDark");
                }
            }
        }
        else
        {
            currentTarget = null;
            if (lastTarget)
            {
                lastTarget.layer = lastLayer;
            }
        }
    }
}
