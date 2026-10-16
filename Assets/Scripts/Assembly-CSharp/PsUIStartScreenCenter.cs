using UnityEngine;

public class PsUIStartScreenCenter : UICanvas
{
	private PrefabC m_logo;

	private int m_waitTicks = 210;

	private int m_fade = 30;

	private PrefabC m_bg;

	private PrefabC m_fadeObj;

	private Material m_fadeMaterial;

	private bool m_changeState;

	public PsUIStartScreenCenter(UIComponent _parent)
		: base(_parent, false, string.Empty, null, string.Empty)
	{
		float num = 0.0225f * (float)Screen.height;
		m_fadeMaterial = new Material(Shader.Find("WOE/Unlit/ColorUnlitTransparent"));
		m_changeState = false;
		SetWidth(1f, RelativeTo.ScreenWidth);
		SetHeight(1f, RelativeTo.ScreenHeight);
		RemoveDrawHandler();
		Vector2[] rect = DebugDraw.GetRect(Screen.width, Screen.height, Vector2.zero);
		Color black = Color.black;
		uint num2 = DebugDraw.ColorToUInt(black);
		m_bg = PrefabS.CreateFlatPrefabComponentsFromVectorArray(m_TC, Vector3.forward * 1f, rect, num2, num2, ResourceManager.GetMaterial("Framework/SolidMat"), m_camera, string.Empty);
		black.a = 0f;
		num2 = DebugDraw.ColorToUInt(black);
		m_fadeObj = PrefabS.CreateFlatPrefabComponentsFromVectorArray(m_TC, Vector3.forward * -1f, rect, num2, num2, m_fadeMaterial, m_camera, string.Empty);
		m_fadeObj.p_gameObject.GetComponent<Renderer>().material.color = black;
		m_logo = PrefabS.AddComponent(m_TC, Vector3.zero, Resources.Load("PlaySomething/FX/Prefabs/TraplightLogo") as GameObject);
		Vector3 euler = new Vector3(90f, 180f, 0f);
		m_logo.p_gameObject.transform.localRotation = Quaternion.Euler(euler);
		PrefabS.SetCamera(m_logo, m_camera);
		m_logo.p_gameObject.transform.localScale = new Vector3(num, num, num);
	}

	public override void Step()
	{
		if (m_changeState)
		{
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new PsUIBaseState(typeof(PsUIPreloaderCenter)));
		}
		m_waitTicks--;
		if (m_waitTicks <= 0)
		{
			m_waitTicks = 0;
			Color black = Color.black;
			black.a = (float)(30 - m_fade) / 30f;
			m_fadeObj.p_gameObject.GetComponent<Renderer>().material.color = black;
			m_fade--;
			if (m_fade <= 0)
			{
				m_changeState = true;
			}
		}
		base.Step();
	}

	public override void Destroy()
	{
		if (m_fadeMaterial != null)
		{
			Object.Destroy(m_fadeMaterial);
		}
		m_fadeMaterial = null;
		base.Destroy();
	}
}
