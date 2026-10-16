using System.Collections.Generic;
using UnityEngine;

public class BoneModifier : MonoBehaviour
{
	public class BoneModifierData
	{
		public float m_angleOffset;

		public Vector3 m_posOffset;

		public ChipmunkBodyC m_cmb;

		public Transform m_bone;

		public Transform m_parent;

		public float m_weight;
	}

	public float m_globalWeight = 1f;

	public List<BoneModifierData> m_bones = new List<BoneModifierData>();

	public BoneModifierData AddChipmunkModifier(ChipmunkBodyC _cmb, Transform _bone, float _weight = 1f, Transform _parent = null)
	{
		BoneModifierData boneModifierData = new BoneModifierData();
		boneModifierData.m_cmb = _cmb;
		boneModifierData.m_bone = _bone;
		boneModifierData.m_weight = _weight;
		boneModifierData.m_parent = _parent;
		boneModifierData.m_angleOffset = 0f;
		boneModifierData.m_posOffset = Vector3.zero;
		m_bones.Add(boneModifierData);
		return boneModifierData;
	}

	public BoneModifierData AddCustomModifier(Transform _bone, float _weight = 1f, Transform _parent = null)
	{
		BoneModifierData boneModifierData = new BoneModifierData();
		boneModifierData.m_cmb = null;
		boneModifierData.m_bone = _bone;
		boneModifierData.m_weight = _weight;
		boneModifierData.m_parent = _parent;
		boneModifierData.m_angleOffset = 0f;
		boneModifierData.m_posOffset = Vector3.zero;
		m_bones.Add(boneModifierData);
		return boneModifierData;
	}

	private void LateUpdate()
	{
		foreach (BoneModifierData bone in m_bones)
		{
			m_globalWeight = Mathf.Clamp(m_globalWeight, 0f, 1f);
			float num = 0f;
			if (bone.m_bone != null)
			{
				if (bone.m_cmb != null)
				{
					num = ToolBox.getRolledValue(ChipmunkProWrapper.ucpBodyGetAngle(bone.m_cmb.body) * 57.29578f, 0f, 360f);
					if (bone.m_parent != null)
					{
						num = Mathf.DeltaAngle(num, bone.m_parent.transform.localRotation.eulerAngles.z);
					}
					num *= bone.m_weight * m_globalWeight;
				}
				else
				{
					num = bone.m_angleOffset * bone.m_weight * m_globalWeight;
					Vector3 vector = bone.m_posOffset * bone.m_weight * m_globalWeight;
					bone.m_bone.localPosition = bone.m_bone.localPosition + vector;
				}
				bone.m_bone.localRotation = Quaternion.Euler(bone.m_bone.localRotation.eulerAngles + new Vector3(num, 0f, 0f));
			}
			else
			{
				Debug.LogError("Something went wrong!");
			}
		}
	}

	private void OnDestroy()
	{
		m_bones.Clear();
	}
}
