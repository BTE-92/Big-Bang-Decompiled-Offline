using System;
using UnityEngine;

public class GhostObject
{
	public bool m_destroyed;

	public string m_name;

	public int m_time;

	public string m_playerId;

	public Entity m_entity;

	public Ghost m_ghost;

	public TransformC[] m_parts;

	public GhostNode[] m_ghostNodes;

	public Material m_material;

	public TransformC m_nameTC;

	public TextMeshC m_nameTMC;

	public TransformC m_starsTC;

	public GhostObject(string _name, int _time, string _playerId, Ghost _ghost)
	{
		m_name = _name;
		m_time = _time;
		m_playerId = _playerId;
		m_ghost = _ghost;
		m_destroyed = false;
	}

	public void Destroy()
	{
		if (!m_destroyed)
		{
			DestroyVisuals();
			Debug.LogWarning(m_name + ": Destroy ghost data");
			m_ghost.Destroy();
			m_ghost = null;
			m_starsTC = null;
			m_nameTC = null;
			m_nameTMC = null;
			m_material = null;
			m_ghostNodes = null;
			m_parts = null;
			m_destroyed = true;
		}
	}

	public void CreateVisuals(GhostPart[] _parts)
	{
		if (m_entity != null)
		{
			DestroyVisuals();
		}
		m_entity = EntityManager.AddEntity();
		m_material = _parts[0].prefab.transform.gameObject.GetComponent<Renderer>().sharedMaterial;
		int num = _parts.Length;
		m_parts = new TransformC[num];
		m_ghostNodes = new GhostNode[num];
		for (int i = 0; i < num; i++)
		{
			TransformC transformC = TransformS.AddComponent(m_entity, _parts[i].nodeName);
			if (i == 0)
			{
				CameraTargetC cameraTargetC = CameraS.AddTargetComponent(transformC, 300f, 300f, new Vector2(0f, 0f));
				cameraTargetC.activeRadius = 500f;
			}
			PrefabS.AddComponent(transformC, _parts[i].offset, _parts[i].prefab);
			if (_parts[i].flipX)
			{
				TransformS.SetScale(transformC, new Vector3(-1f, 1f, 1f));
			}
			m_parts[i] = transformC;
			m_ghostNodes[i] = m_ghost.m_nodes[_parts[i].nodeName] as GhostNode;
		}
		m_nameTC = TransformS.AddComponent(m_entity, "GhostNameTC");
		m_nameTMC = TextMeshS.AddComponent(m_nameTC, Vector3.zero, "KGSecondChances_Font", 0f, 200f, 25.6f, Align.Center, Align.Top, CameraS.m_mainCamera, string.Empty);
		TextMeshS.SetText(m_nameTMC, m_name);
		Update();
	}

	public void DestroyVisuals()
	{
		if (m_entity != null)
		{
			Debug.LogWarning(m_name + ": Destroy visuals");
			EntityManager.RemoveEntity(m_entity);
			m_entity = null;
		}
	}

	public void Update()
	{
		if (m_entity == null || m_ghost == null)
		{
			return;
		}
		m_ghost.Update();
		if (m_ghost.m_keyframeCount > 0)
		{
			Vector3 vector = new Vector3(0f, 0f, 80f);
			for (int i = 0; i < m_parts.Length; i++)
			{
				GhostNode node = m_ghostNodes[i];
				TransformS.SetPosition(m_parts[i], m_ghost.GetCurrentPosition(node) + vector);
				TransformS.SetRotation(m_parts[i], new Vector3(0f, 0f, m_ghost.GetCurrentRotation(node)));
			}
			TransformS.SetPosition(m_nameTC, m_ghost.GetCurrentPosition(m_ghostNodes[0]) + vector);
			float magnitude = (PsState.m_activeMinigame.m_playerTC.transform.position - m_parts[0].transform.position).magnitude;
			float positionBetween = ToolBox.getPositionBetween(magnitude, 10f, 130f);
			m_material.SetFloat("_Brightness", positionBetween * 1.5f);
			positionBetween = ToolBox.getPositionBetween(magnitude, 80f, 260f);
			m_nameTMC.m_textMesh.color = new Color32(178, 252, 234, Convert.ToByte(positionBetween * 255f));
			if (m_ghost.PlaybackEnded())
			{
				DestroyVisuals();
			}
		}
	}
}
