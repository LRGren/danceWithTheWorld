using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractureManager : SkillItem
{
    public ParticleSystem particle;
    public GameObject obj;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void SkillOn()
    {
        if(hasTriggered)return;
        
        base.SkillOn();
        
        obj.SetActive(false);
        particle.Play();
        
    }
}
