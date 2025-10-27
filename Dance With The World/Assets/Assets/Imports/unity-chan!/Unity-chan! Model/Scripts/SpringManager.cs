//
//SpingManager.cs for unity-chan!
//
//Original Script is here:
//ricopin / SpingManager.cs
//Rocket Jump : http://rocketjump.skr.jp/unity3d/109/
//https://twitter.com/ricopin416
//
//Revised by N.Kobayashi 2014/06/24
//           Y.Ebata
//
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChan
{
	public class SpringManager : MonoBehaviour
	{
		//Kobayashi
		// DynamicRatio is paramater for activated level of dynamic animation 
		public float dynamicRatio = 1.0f;

		//Ebata
		public float			stiffnessForce;
		public AnimationCurve	stiffnessCurve;
		public float			dragForce;
		public AnimationCurve	dragCurve;
		public SpringBone[] springBones;

		void Start ()
		{
			UpdateParameters ();
		}
	
		void Update ()
		{
#if UNITY_EDITOR
		//Kobayashi
		if(dynamicRatio >= 1.0f)
			dynamicRatio = 1.0f;
		else if(dynamicRatio <= 0.0f)
			dynamicRatio = 0.0f;
		//Ebata
		UpdateParameters();
#endif
		}
	
		private void LateUpdate ()
		{
			//Kobayashi
			if (dynamicRatio != 0.0f) {
				for (int i = 0; i < springBones.Length; i++) {
					if (dynamicRatio > springBones [i].threshold) {
						springBones [i].UpdateSpring ();
					}
				}
			}
		}

		[ContextMenu("Find Colliders")]
		public void FindColliders()
		{
			SpringCollider[] springColliders = gameObject.GetComponentsInChildren<SpringCollider>();
			foreach (var sb in springBones)
			{
				sb.colliders = springColliders;
			}
		}
		
		[Header("Bind Bones Settings")]
		public string bindBoneName;
		public float setRadius = 0.05f;
		public float setSiffnessForce = 0.01f;
		public float setDragForce = 0.4f;
		public bool setIsUseEachBone = true;
		[ContextMenu("Bind Bones")]
		public void GetEveryBone()
		{
			List<SpringBone> spring = new List<SpringBone>();
			
			Transform[] transforms = GetComponentsInChildren<Transform>();
			foreach (Transform t in transforms)
			{
				if (t.name.StartsWith(bindBoneName)||t.name.StartsWith("HB"))
				{
					if (t.childCount > 0)
					{
						if(!t.GetComponent<SpringBone>())
							t.gameObject.AddComponent<SpringBone>();
						
						SpringBone sb = t.GetComponent<SpringBone>();
						
						sb.child = t.GetChild(0);
						sb.boneAxis = new Vector3(0, 1, 0);
						sb.radius = setRadius;
						sb.stiffnessForce = setSiffnessForce;
						sb.dragForce = setDragForce;
						sb.isUseEachBoneForceSettings = setIsUseEachBone;
						
						spring.Add(sb);
					}
				}
			}
			
			springBones = spring.ToArray();
		}

		[Header("Unbind Bones Settings")]
		public string unbindBoneName;
		[ContextMenu("Unbind Bones")]
		public void ClearBones()
		{
			Transform[] transforms = GetComponentsInChildren<Transform>();
			foreach (Transform t in transforms)
			{
				if (t.name.StartsWith(unbindBoneName)||t.name.StartsWith("HB"))
				{
					if (t.GetComponent<SpringBone>())
						DestroyImmediate(t.GetComponent<SpringBone>());
				}
			}
		}
		
		private void UpdateParameters ()
		{
			UpdateParameter ("stiffnessForce", stiffnessForce, stiffnessCurve);
			UpdateParameter ("dragForce", dragForce, dragCurve);
		}
	
		private void UpdateParameter (string fieldName, float baseValue, AnimationCurve curve)
		{
			var start = curve.keys [0].time;
			var end = curve.keys [curve.length - 1].time;
			//var step	= (end - start) / (springBones.Length - 1);
		
			var prop = springBones [0].GetType ().GetField (fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
		
			for (int i = 0; i < springBones.Length; i++) {
				//Kobayashi
				if (!springBones [i].isUseEachBoneForceSettings) {
					var scale = curve.Evaluate (start + (end - start) * i / (springBones.Length - 1));
					prop.SetValue (springBones [i], baseValue * scale);
				}
			}
		}
	}
}