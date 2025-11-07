using System;
using System.Collections;
using System.Collections.Generic;
using Project.Scripts.Fractures;
using UnityEngine;

public class SelfBrokener : SkillItem
{
    [Header("力场设置")]
    public float radius = 5f;                   // 作用半径
    public ForceMode forceMode = ForceMode.VelocityChange; // 力的模式

    [Header("三次力的设置")]
    public float firstForceMagnitude = 1000000f;     // 第一次力的大小
    public float secondForceMagnitude = 15f;    // 第二次力的大小
    public float thirdForceMagnitude = 5f;      // 第三次力的大小
    public float forceInterval = 0.1f;          // 力之间的间隔时间

    [Header("调试显示")]
    public bool showGizmos = true;              // 是否显示辅助线

    private bool isApplyingForces = false;      // 防止重复触发

    IEnumerator ApplyRadialForcesSequence()
    {
        //print("oooooooo");
        
        isApplyingForces = true;

        // 第一次力
        ApplyRadialForce(firstForceMagnitude);
        yield return new WaitForSeconds(forceInterval);

        // 第二次力
        ApplyRadialForce(secondForceMagnitude);
        yield return new WaitForSeconds(forceInterval);

        // 第三次力
        ApplyRadialForce(thirdForceMagnitude);

        isApplyingForces = false;
    }

    void ApplyRadialForce(float forceMagnitude)
    {
        // 获取半径内的所有碰撞体，以自身位置为中心
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        
        foreach (var hitCollider in hitColliders)
        {
            Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
            if (rb != null && rb != GetComponent<Rigidbody>()) // 避免对自己施加力
            {
                //print(rb.name);
                // 计算从自身指向物体的方向
                Vector3 directionToTarget = hitCollider.transform.position - transform.position;
                
                // 标准化方向并应用力
                Vector3 forceDirection = directionToTarget.normalized;

                rb.AddForce(-forceDirection * forceMagnitude, forceMode);
            }
        }
    }

    // 在Scene视图中绘制辅助图形
    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        
        // 使用自身位置作为中心点
        Vector3 currentCenter = transform.position;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(currentCenter, radius);
        
        // 绘制中心点
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(currentCenter, 0.2f);
        
        // 绘制从中心向外的方向指示线
        Gizmos.color = Color.blue;
        for (int i = 0; i < 8; i++)
        {
            float angle = i * Mathf.PI * 2f / 8f;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Gizmos.DrawRay(currentCenter, dir);
        }
    }

    protected override void Update()
    {
        base.Update();

        /*if (Input.GetKeyDown(KeyCode.K)&& !isApplyingForces)
        {
            StartCoroutine(ApplyRadialForcesSequence());
        }*/

        if (model == null)
        {
            transform.parent.GetChild(0).gameObject.SetActive(true);
            model = transform.parent.GetComponentInChildren<ItemManager>().model;
            transform.parent.GetComponentInChildren<FractureThis>().enabled = false;
            MeshCollider mesh = model.AddComponent<MeshCollider>();
            mesh.convex = true;
        }
        
    }

    protected override void SkillOn()
    {
        if(hasTriggered)return;
        
        base.SkillOn();
        model.SetActive(false);
        StartCoroutine(ApplyRadialForcesSequence());
    }
}
