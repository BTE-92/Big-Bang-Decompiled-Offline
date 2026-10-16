using System.Collections.Generic;
using UnityEngine;

public class PsUIMysteryBox : UIScrollableCanvas
{
	private float m_duration;

	private float m_selectionDuration;

	private float m_currentTime;

	private List<TransformC> m_items;

	private List<Frame> m_frames;

	private SpriteSheet m_spriteSheet;

	private int m_lastRandomFrame;

	private int m_selectedFrame;

	private float m_speed;

	public PsUIMysteryBox()
		: base(null, "Gacha")
	{
		m_duration = 0f;
		m_selectionDuration = 1.5f;
		m_items = new List<TransformC>();
		m_spriteSheet = SpriteS.AddSpriteSheet(m_camera, ResourceManager.GetMaterial(RESOURCE.UiAtlasMat_Material), ResourceManager.GetTextAsset(RESOURCE.UiAtlas_TextAsset), 1f);
		m_frames = new List<Frame>();
		m_frames.Add(m_spriteSheet.m_atlas.GetFrame("menu_resources_key_icon"));
		m_frames.Add(m_spriteSheet.m_atlas.GetFrame("menu_resources_coin_icon"));
		m_frames.Add(m_spriteSheet.m_atlas.GetFrame("menu_resources_diamond_icon"));
		m_frames.Add(m_spriteSheet.m_atlas.GetFrame("menu_resources_bolt_icon"));
		m_lastRandomFrame = 0;
		m_selectedFrame = Random.Range(0, m_frames.Count);
		m_speed = -0.015f;
		SetSize(0.2f, 0.3f, RelativeTo.ScreenHeight);
		SetAlign(0.05f, 0.75f);
		RemoveTouchAreas();
		Update();
	}

	private Frame GetRandomFrame()
	{
		int num;
		for (num = Random.Range(0, m_frames.Count); num == m_lastRandomFrame; num = Random.Range(0, m_frames.Count))
		{
		}
		m_lastRandomFrame = num;
		return m_frames[num];
	}

	public override void Step()
	{
		m_currentTime += Main.m_dt;
		if (m_currentTime >= m_duration && m_speed == 0f)
		{
			Destroy();
		}
		else
		{
			if (m_speed == 0f)
			{
				return;
			}
			bool flag = false;
			if (m_currentTime >= m_selectionDuration)
			{
				flag = true;
			}
			while (m_items.Count < 5)
			{
				Entity entity = EntityManager.AddEntity();
				entity.m_persistent = true;
				TransformC transformC = TransformS.AddComponent(entity);
				TransformS.ParentComponent(transformC, m_TC);
				Frame randomFrame = GetRandomFrame();
				transformC.m_identifier = m_lastRandomFrame;
				SpriteC c = SpriteS.AddComponent(transformC, randomFrame, m_spriteSheet);
				SpriteS.SetDimensions(c, (float)Screen.height * 0.15f, (float)Screen.height * 0.15f * (randomFrame.height / randomFrame.width));
				if (m_items.Count == 0)
				{
					TransformS.SetPosition(transformC, Vector3.up * Screen.height * -0.02f);
				}
				else
				{
					TransformS.SetPosition(transformC, m_items[m_items.Count - 1].transform.localPosition + Vector3.up * Screen.height * 0.2f);
				}
				m_items.Add(transformC);
			}
			for (int num = m_items.Count - 1; num > -1; num--)
			{
				TransformS.Move(m_items[num], Vector3.up * Screen.height * m_speed);
				if (flag && m_items[num].m_identifier == m_selectedFrame && m_items[num].transform.localPosition.y < 0f && m_items[num].transform.localPosition.y > (float)Screen.height * -0.02f)
				{
					m_speed = 0f;
					m_duration = m_currentTime + 1f;
				}
				else if (m_items[num].transform.localPosition.y < (float)Screen.height * -0.3f)
				{
					EntityManager.RemoveEntity(m_items[num].p_entity);
					m_items.RemoveAt(num);
				}
			}
			base.Step();
		}
	}

	public override void Destroy()
	{
		base.Destroy();
		while (m_items.Count > 0)
		{
			EntityManager.RemoveEntity(m_items[0].p_entity);
			m_items.RemoveAt(0);
		}
	}
}
