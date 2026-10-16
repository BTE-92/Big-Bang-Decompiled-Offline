using UnityEngine;

public class RagdollJointBootstrap : MonoBehaviour
{
	public RagdollNode m_ragdollNode;

	public TransformC m_mainTC;

	private void LateUpdate()
	{
		float num = ChipmunkProWrapper.ucpBodyGetAngle(m_ragdollNode.m_cmb.body) * 57.29578f;
		if (m_ragdollNode.m_parent != null)
		{
			num -= ChipmunkProWrapper.ucpBodyGetAngle(m_ragdollNode.m_parent.m_cmb.body) * 57.29578f;
		}
		float num2 = 1f;
		float y = 90f;
		if (m_ragdollNode.m_flipped)
		{
			num2 = -1f;
			y = 270f;
		}
		if (m_mainTC != null)
		{
			Quaternion localRotation = Quaternion.Euler(new Vector3((0f - num) * num2, y, 0f));
			m_mainTC.transform.position = m_ragdollNode.m_cmb.TC.transform.position;
			m_mainTC.transform.localRotation = localRotation;
			m_ragdollNode.m_transform.localPosition = new Vector3(0f, 0f - m_ragdollNode.m_centeroid.y, (0f - m_ragdollNode.m_centeroid.x) * num2);
			m_ragdollNode.m_transform.localRotation = Quaternion.identity;
		}
		else
		{
			Quaternion localRotation2 = Quaternion.Euler(m_ragdollNode.m_originalRot.eulerAngles + new Vector3((0f - num) * num2, 0f, 0f));
			m_ragdollNode.m_transform.localRotation = localRotation2;
		}
	}
}
