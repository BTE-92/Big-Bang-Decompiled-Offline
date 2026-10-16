using UnityEngine;

public class PsPlanetBlockNode : PsPlanetNode
{
	private SpriteC m_bgSpriteC;

	private PsPathBlockType m_blockType;

	protected int m_lastEarnedStars;

	private bool m_claimed;

	public PsPlanetBlockNode(PsGameLoop _loop, bool _tutorial = false)
		: base(_loop, _tutorial)
	{
	}

	public override void CreatePrefab()
	{
		if (m_prefab != null)
		{
			PrefabS.RemoveComponent(m_prefab);
			m_prefab = null;
		}
		GameObject gameObject = null;
		m_blockType = PsPathBlockType.Chest;
		if (m_loop is PsGameLoopBlockSignal)
		{
			m_blockType = (m_loop as PsGameLoopBlockSignal).m_blockType;
		}
		else if (m_loop is PsGameLoopBlockSignalWithoutLevel)
		{
			m_blockType = (m_loop as PsGameLoopBlockSignalWithoutLevel).m_blockType;
		}
		else
		{
			m_blockType = (m_loop as PsGameLoopBlockLevel).m_blockType;
		}
		m_prefab = PrefabS.AddComponent(_gameObject: (m_blockType == PsPathBlockType.Chest) ? ((m_loop.m_path == null || !(m_loop.m_path.m_name == "MainPath")) ? ResourceManager.GetGameObject(RESOURCE.PathButtonChestPrefab_GameObject) : ResourceManager.GetGameObject(RESOURCE.CheckpointChestPrefab_GameObject)) : ((m_blockType == PsPathBlockType.Antenna) ? ResourceManager.GetGameObject(RESOURCE.CheckpointAntennaPrefab_GameObject) : ((m_blockType != PsPathBlockType.Construction) ? ResourceManager.GetGameObject(RESOURCE.CheckpointGaragePrefab_GameObject) : ResourceManager.GetGameObject(RESOURCE.CheckpointCranePrefab_GameObject))), _parentTC: m_tc, _offset: Vector3.zero);
		PrefabS.SetCamera(m_prefab.p_gameObject, m_loop.m_planet.m_planetCamera);
		m_checkpointEffect = m_prefab.p_gameObject.GetComponent<EffectCheckpoint>();
	}

	public override void CreateUI()
	{
	}

	public override void RemoveUI()
	{
	}

	public override void Update()
	{
		base.Update();
		if (m_loop.m_nodeId < m_loop.m_path.m_currentNodeId || !m_hideUI || m_loop.m_planet.m_banners == null)
		{
			return;
		}
		for (int i = 0; i < m_loop.m_planet.m_banners.Count; i++)
		{
			if (m_loop.m_planet.m_banners[i] == m_loop)
			{
				m_loop.m_planet.m_banners[i].Hide();
			}
		}
		m_hideUI = false;
	}

	public override void Highlight()
	{
		if (m_highlightTC == null)
		{
			Entity entity = EntityManager.AddEntity();
			m_highlightTC = TransformS.AddComponent(entity);
			m_highlightTC.transform.localScale = new Vector3(1f, 0f, 1f) * 1.5f;
			TransformS.ParentComponent(m_highlightTC, m_tc, Vector3.zero);
			m_highlightTC.transform.localRotation = Quaternion.identity;
			GameObject gameObject = ResourceManager.GetGameObject(RESOURCE.PathButtonActiveEffectPrefab_GameObject);
			m_highlightPrefab = PrefabS.AddComponent(m_highlightTC, Vector3.zero, gameObject);
			m_highlightPrefab.p_gameObject.transform.localScale = Vector3.one * 1.5f;
			PrefabS.SetCamera(m_highlightPrefab.p_gameObject, m_loop.m_planet.m_planetCamera);
			m_highlightTween = TweenS.AddTransformTween(m_highlightTC, TweenedProperty.Scale, TweenStyle.CubicOut, new Vector3(1f, 0f, 1f) * 1.5f, Vector3.one * 1.5f, 1f, 0f, false);
		}
	}

	public override void RemoveHighlight()
	{
		if (m_highlightTC != null)
		{
			m_highlightTween = TweenS.AddTransformTween(m_highlightTC, TweenedProperty.Scale, TweenStyle.CubicOut, Vector3.one * 1.5f, new Vector3(1f, 0f, 1f) * 1.5f, 1f, 0f, false);
			TweenS.AddTweenEndEventListener(m_highlightTween, base.RemoveHighlightDelegate);
		}
	}

	public override void Activate2(TimerC _c)
	{
		if (!m_claimed)
		{
			base.Activate2(_c);
			if (m_blockType == PsPathBlockType.Chest)
			{
				SoundS.PlaySingleShot("/Metagame/ChestBounce", Vector3.zero);
			}
			SoundS.PlaySingleShot("/Metagame/CheckpointHighlight", Vector3.zero);
		}
	}

	public override void SetInactive()
	{
		base.SetInactive();
		if (!m_loop.m_keepNodeHidden)
		{
			CreateUI();
		}
	}

	public override void Reveal()
	{
		base.Reveal();
		SoundS.PlaySingleShot("/Metagame/CheckpointAppear", Vector3.zero);
	}

	public override void Claim()
	{
		m_claimed = true;
		base.Claim();
	}

	public override void SetClaimed()
	{
		m_claimed = true;
		base.SetClaimed();
	}
}
