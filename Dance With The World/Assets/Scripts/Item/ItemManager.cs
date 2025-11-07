using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public GameObject model;
    public SkillItem skillItem;

    private void Update()
    {
        if (skillItem == null)
        {
            skillItem = transform.parent.GetComponentInChildren<SkillItem>();
        }

        if (skillItem)
        {
            if (skillItem.hasTriggered)
            {
                model.SetActive(false);
            }
        }
    }
}
