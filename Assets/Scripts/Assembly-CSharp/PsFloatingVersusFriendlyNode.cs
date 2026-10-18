using UnityEngine;

public class PsFloatingVersusFriendlyNode : PsFloatingVersusNode
{
	public PsFloatingVersusFriendlyNode(PsTimedEventLoop _loop)
		: base(_loop)
	{
	}

	public override void SetPrefabName()
	{
		m_prefabName = "FloatingPlatformPrefab";
	}

	public override void AddGreyScale()
	{
		if (!m_grey && m_prefab != null)
		{
			m_prefab.p_gameObject.transform.Find("FloatingPlatform").transform.GetComponent<Renderer>().material.shader = Shader.Find("WOE/Units/Greyscale");
			m_grey = true;
		}
	}

	public override void RemoveGreyScale()
	{
		if (m_grey && m_prefab != null)
		{
			m_prefab.p_gameObject.transform.Find("FloatingPlatform").transform.GetComponent<Renderer>().material.shader = Shader.Find(m_domeShader);
			m_grey = false;
		}
	}

	public void SetOpponentImage(string _facebookId)
	{
		if (string.IsNullOrEmpty(_facebookId))
		{
			_facebookId = null;
		}
		TransformC transformC = TransformS.AddComponent(m_domeUITC.p_entity, "NodeProfileImage", Vector3.zero, Vector3.zero);
		TransformS.ParentComponent(transformC, m_domeUITC);
		TransformS.SetPosition(transformC, new Vector3(-0.05f, 0.11f, -0.43f));
		TransformS.SetRotation(transformC, new Vector3(-7.951386E-16f, 1.09106E-07f, 11.56f));
		PsGameLoopVersus psGameLoopVersus = m_loop as PsGameLoopVersus;
		m_opponentImage = new PsUIProfileImageSprite(_facebookId, psGameLoopVersus.m_planet.m_planetCamera, transformC);
	}

	public void GetOpponentFacebookId()
	{
		string id = string.Empty;
		PsGameLoopVersus versusloop = m_loop as PsGameLoopVersus;
		if (versusloop.m_versusData.GetOpponentInfo() != null)
		{
			id = versusloop.m_versusData.GetOpponentInfo().score.facebookId;
			SetOpponentImage(id);
		}
		else
		{
			if (!(versusloop.m_versusData.challengedPlayer != PlayerPrefsX.GetUserId()))
			{
				return;
			}
			PsMetagameManager.GetFriends(delegate(Friends _friendData)
			{
				if ((m_tc.p_entity != null || m_loop != null) && m_loop.m_planet.m_spaceEntity != null)
				{
					id = string.Empty;
					for (int i = 0; i < _friendData.friends.Count; i++)
					{
						if (versusloop.m_versusData.challengedPlayer == _friendData.friends[i].playerId)
						{
							id = _friendData.friends[i].facebookId;
							break;
						}
					}
					SetOpponentImage(id);
				}
			});
		}
	}

	private void SearchFacebookId(Friends _friendData)
	{
		if ((m_tc.p_entity == null && m_loop == null) || m_loop.m_planet.m_spaceEntity == null)
		{
			return;
		}
		PsGameLoopVersus psGameLoopVersus = m_loop as PsGameLoopVersus;
		string opponentImage = string.Empty;
		for (int i = 0; i < _friendData.friends.Count; i++)
		{
			if (psGameLoopVersus.m_versusData.challengedPlayer == _friendData.friends[i].playerId)
			{
				opponentImage = _friendData.friends[i].facebookId;
				break;
			}
		}
		SetOpponentImage(opponentImage);
	}

	public override void Create()
	{
		base.Create();
		GetOpponentFacebookId();
	}
}
