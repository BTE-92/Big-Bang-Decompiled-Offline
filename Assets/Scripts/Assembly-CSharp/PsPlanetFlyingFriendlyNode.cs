using UnityEngine;

public class PsPlanetFlyingFriendlyNode : PsPlanetFlyingNode
{
	private TransformC m_uiTC;

	private TransformC m_profileImageTC;

	public PsPlanetFlyingFriendlyNode(PsGameLoop _sourceLoop, PsGameLoop _targetLoop)
		: base(_sourceLoop, _targetLoop)
	{
		m_uiTC = TransformS.AddComponent(m_tc.p_entity, "flyingNodeUI", Vector3.zero, Vector3.zero);
		TransformS.ParentComponent(m_uiTC, m_tc);
		TransformS.SetPosition(m_uiTC, Vector3.zero);
		m_profileImageTC = TransformS.AddComponent(m_tc.p_entity, "NodeProfileImage", Vector3.zero, Vector3.zero);
		TransformS.ParentComponent(m_profileImageTC, m_uiTC);
		TransformS.SetPosition(m_profileImageTC, new Vector3(-0.05f, 0.11f, -0.43f));
		TransformS.SetRotation(m_profileImageTC, new Vector3(-7.951386E-16f, 1.09106E-07f, 11.56f));
		GetOpponentFacebookId();
	}

	public void SetOpponentImage(string _facebookId)
	{
		PsUIProfileImageSprite psUIProfileImageSprite = new PsUIProfileImageSprite(_facebookId, m_targetLoop.m_planet.m_planetCamera, m_profileImageTC);
	}

	public void GetOpponentFacebookId()
	{
		string id = string.Empty;
		if (m_targetLoop != null && m_targetLoop.m_versusData.GetOpponentInfo() != null)
		{
			id = m_targetLoop.m_versusData.GetOpponentInfo().score.facebookId;
			SetOpponentImage(id);
			return;
		}
		PsMetagameManager.GetFriends(delegate(Friends _friendData)
		{
			if ((m_tc.p_entity != null || m_targetLoop != null) && m_targetLoop.m_planet.m_spaceEntity != null)
			{
				id = string.Empty;
				for (int i = 0; i < _friendData.friends.Count; i++)
				{
					if (m_targetLoop.m_versusData.challengedPlayer == _friendData.friends[i].playerId)
					{
						id = _friendData.friends[i].facebookId;
						break;
					}
				}
				SetOpponentImage(id);
			}
		});
	}

	public override GameObject GetFlyerGameObject()
	{
		return ResourceManager.GetGameObject(RESOURCE.FloatingPlatformPrefab_GameObject);
	}

	public override void TakeOff()
	{
		Vector3 position = m_tc.transform.localPosition + m_tc.parent.transform.worldToLocalMatrix.MultiplyVector(m_tc.transform.up) * m_planet.m_planetRadius * 0.1f;
		TransformS.SetPosition(m_tc, position);
		TweenC tweenComponent = TweenS.AddTransformTween(m_tc, TweenedProperty.Scale, TweenStyle.ExpoOut, Vector3.zero, Vector3.one, 1f, 0.45f, true);
		TweenS.AddTweenEndEventListener(tweenComponent, base.TakeOffEnded);
		m_takeOffTime = -1f;
	}

	public override void Update()
	{
		if (m_targetLoop != null && m_sourceLoop != null && m_uiTC != null)
		{
			m_uiTC.transform.LookAt(m_targetLoop.m_planet.m_planetCamera.gameObject.transform, m_targetLoop.m_planet.m_planetCamera.gameObject.transform.up);
			m_uiTC.transform.localRotation *= Quaternion.Euler(Vector3.up * 180f);
		}
		base.Update();
	}
}
