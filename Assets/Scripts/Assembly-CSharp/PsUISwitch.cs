using UnityEngine;

public class PsUISwitch : UIVerticalList
{
	public bool m_enabled;

	private PsUIGenericButton m_button;

	private float m_fontSize;

	private float m_textAreaWidth;

	private string m_enabledText;

	private string m_disabledText;

	private float m_dragStartXPosition;

	private bool m_dragChanged;

	public PsUISwitch(UIComponent _parent, bool _enabled = true, float _fontSize = 0.03f, string _enabledText = "On", string _disabledText = "Off", float _textAreaWidth = 0.05f)
		: base(_parent, string.Empty)
	{
		CreateTouchAreas();
		m_enabled = _enabled;
		m_fontSize = _fontSize;
		m_enabledText = _enabledText;
		m_disabledText = _disabledText;
		m_textAreaWidth = _textAreaWidth;
		m_button = new PsUIGenericButton(this);
		m_button.SetVerticalAlign(0.5f);
		m_button.m_TAC.m_active = false;
		m_button.SetText(string.Empty, _fontSize, _textAreaWidth);
		SetWidth(m_button.m_actualMargins.l + m_button.m_actualMargins.r + _textAreaWidth + 0.12f, RelativeTo.ScreenHeight);
		UpdateSwitchPosition();
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		if (_inside && !m_dragChanged)
		{
			m_enabled = !m_enabled;
			UpdateSwitchPosition();
			SoundS.PlaySingleShot("/UI/ButtonNormal", Vector3.zero);
		}
		base.OnTouchRelease(_touch, _inside);
	}

	protected override void OnTouchBegan(TLTouch _touch)
	{
		m_dragStartXPosition = _touch.m_currentPosition.x;
		m_dragChanged = false;
		base.OnTouchBegan(_touch);
	}

	protected override void OnTouchRollIn(TLTouch _touch, bool _secondery)
	{
		m_dragStartXPosition = _touch.m_currentPosition.x;
		m_dragChanged = false;
		base.OnTouchRollIn(_touch, _secondery);
	}

	protected override void OnTouchMove(TLTouch _touch, bool _inside)
	{
		if (_inside)
		{
			float num = _touch.m_currentPosition.x - m_dragStartXPosition;
			if (num > m_actualWidth / 3f && !m_enabled)
			{
				m_hit = true;
				m_enabled = true;
				UpdateSwitchPosition();
				m_dragStartXPosition = _touch.m_currentPosition.x;
				m_dragChanged = true;
			}
			else if (num < (0f - m_actualWidth) / 3f && m_enabled)
			{
				m_hit = true;
				m_enabled = false;
				UpdateSwitchPosition();
				m_dragStartXPosition = _touch.m_currentPosition.x;
				m_dragChanged = true;
			}
		}
		base.OnTouchMove(_touch, _inside);
	}

	private void UpdateSwitchPosition()
	{
		if (m_enabled)
		{
			m_button.SetText(m_enabledText, m_fontSize, m_textAreaWidth);
			m_button.SetHorizontalAlign(1f);
			m_button.SetGreenColors();
		}
		else
		{
			m_button.SetText(m_disabledText, m_fontSize, m_textAreaWidth);
			m_button.SetHorizontalAlign(0f);
			m_button.SetBlueColors(false);
		}
		m_button.Update();
	}

	public override void DrawHandler(UIComponent _c)
	{
		float num = 0.006f;
		PrefabS.RemoveComponentsByEntity(m_TC.p_entity);
		Vector2[] bezierRect = DebugDraw.GetBezierRect(_c.m_actualWidth - num * 3f * (float)Screen.height, _c.m_actualHeight - num * 3f * (float)Screen.height, num * (float)Screen.height, 10, Vector2.one * 0.9f);
		PrefabS.CreatePathPrefabComponentFromVectorArray(m_TC, Vector3.forward * 0.1f, bezierRect, (float)Screen.width * 0.005f, DebugDraw.HexToColor("#05192f"), DebugDraw.HexToColor("#05192f"), ResourceManager.GetMaterial("Framework/Line4Mat"), m_camera, Position.Center, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(m_TC, Vector3.forward * 0.2f, bezierRect, DebugDraw.HexToUint("#156591"), DebugDraw.HexToUint("#156591"), ResourceManager.GetMaterial("Framework/SolidMat"), m_camera, string.Empty);
		SpriteS.ConvertSpritesToPrefabComponent(m_TC, m_camera, true);
	}
}
