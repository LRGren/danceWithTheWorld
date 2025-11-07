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
    
    private LayerMask actorLayer;
    private LayerMask highlightedLayer;

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
        
        actorLayer = LayerMask.NameToLayer("Actor");
        highlightedLayer = LayerMask.NameToLayer("HighlightedActor");
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
                lastTarget.layer = actorLayer;
        }
    }

    private void HandleRayCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width, Screen.height) * .5f),
                out hit, interactionRange, LayerMask.GetMask("Actor","HighlightedActor")))
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
        }
        else
        {
            currentTarget = null;
            if (lastTarget)
                lastTarget.layer = actorLayer;
        }
    }
}
