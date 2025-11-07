using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjPos : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector("_ObjPos",new Vector4(transform.position.x, transform.position.y, transform.position.z,transform.localScale.z));
    }
}
