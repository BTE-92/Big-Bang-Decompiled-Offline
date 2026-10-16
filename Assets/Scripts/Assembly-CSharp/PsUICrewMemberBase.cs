using System;
using System.Collections.Generic;
using UnityEngine;

public class PsUICrewMemberBase : UICanvas
{
	public UIHorizontalList m_contentList;

	public UIVerticalList m_speechContent;

	public UIHorizontalList m_buttonList;

	public List<KeyValuePair<PsUIGenericButton, Action>> m_buttons;

	public string m_text;

	public string m_label;

	public string m_characterFrameName;

	public UITextbox m_speechText;

	private PsDialogueCharacter m_lastCharacter;

	private UIFittedSprite m_characterSprite;

	public PsUIGenericButton m_ok;

	public PsUIGenericButton m_cancel;

	public SpeechBubbleHandlePosition m_bubbleHandlePos;

	public SpeechBubbleHandleType m_bubbleHandleType;

	public PsUICharacter m_character;

	public PsUICrewMemberBase(Camera _camera, string _tag)
		: base(null, false, _tag, null, string.Empty)
	{
		RemoveDrawHandler();
		SetHeight(1f, RelativeTo.ScreenHeight);
		SetWidth(1f, RelativeTo.ScreenWidth);
		SetMargins(0.02f, 0.02f, 0.02f, 0.04f);
		SetCamera(_camera, true, false);
	}

	public void CreateCharacter(PsDialogueCharacter _character, PsDialogueCharacterPosition _position, float _customScale = 1f)
	{
		if (m_character != null)
		{
			m_character.Destroy();
			m_character = null;
		}
		m_character = new PsUICharacter(this, _character, _position, _customScale);
		switch (_position)
		{
		case PsDialogueCharacterPosition.Right:
			m_character.SetAlign(1f, 0f);
			break;
		case PsDialogueCharacterPosition.Left:
			m_character.SetAlign(0f, 0f);
			break;
		}
		Update();
	}

	public void StartDialogue(PsDialogueStep _dialogue)
	{
		if (m_speechContent == null)
		{
			m_speechContent = new UIVerticalList(this, string.Empty);
			m_speechContent.SetMargins(0.05f, 0.05f, 0.05f, 0.05f);
			m_speechContent.SetWidth(0.5f, RelativeTo.ScreenWidth);
			m_speechContent.SetSpacing(0.05f, RelativeTo.ScreenHeight);
			m_speechContent.SetDrawHandler(SpeechBubbleDrawhandler);
			m_speechContent.SetDepthOffset(-20f);
		}
		if (_dialogue.m_characterPosition == PsDialogueCharacterPosition.Right)
		{
			m_speechContent.SetAlign(0.1f, 0.4f);
			m_bubbleHandlePos = SpeechBubbleHandlePosition.BottomRight;
			m_bubbleHandleType = SpeechBubbleHandleType.SmallToRight;
		}
		else
		{
			m_speechContent.SetAlign(0.9f, 0.4f);
			m_bubbleHandlePos = SpeechBubbleHandlePosition.BottomLeft;
			m_bubbleHandleType = SpeechBubbleHandleType.SmallToLeft;
		}
		if (!string.IsNullOrEmpty(_dialogue.m_charactertext))
		{
			if (m_speechText != null)
			{
				m_speechText.SetText(_dialogue.m_charactertext);
			}
			else
			{
				m_speechText = new UITextbox(m_speechContent, false, "text", _dialogue.m_charactertext, "HurmeSemiBold_Font", 0.03f, RelativeTo.ScreenHeight, true, Align.Center, Align.Top, "#4dcfff");
			}
		}
		if (m_buttonList == null)
		{
			m_buttonList = new UIHorizontalList(m_speechContent, string.Empty);
			m_buttonList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
			m_buttonList.RemoveDrawHandler();
		}
		m_buttonList.SetHorizontalAlign(0.5f);
		if (!string.IsNullOrEmpty(_dialogue.m_proceedText))
		{
			if (m_ok != null)
			{
				m_ok.SetText(_dialogue.m_proceedText, 0.04f);
			}
			else
			{
				m_ok = new PsUIGenericButton(m_buttonList);
				m_ok.SetText(_dialogue.m_proceedText, 0.04f);
				m_ok.SetHeight(0.075f, RelativeTo.ScreenHeight);
			}
			m_ok.SetGreenColors();
		}
		else if (m_ok != null)
		{
			m_ok.Destroy();
			m_ok = null;
		}
		if (!string.IsNullOrEmpty(_dialogue.m_cancelText))
		{
			if (m_cancel != null)
			{
				m_cancel.SetText(_dialogue.m_cancelText, 0.04f);
			}
			else
			{
				m_cancel = new PsUIGenericButton(m_buttonList);
				m_cancel.SetText(_dialogue.m_cancelText, 0.04f);
				m_cancel.SetHeight(0.075f, RelativeTo.ScreenHeight);
			}
		}
		else if (m_cancel != null)
		{
			m_cancel.Destroy();
			m_cancel = null;
		}
		Update();
	}

	public override void Update()
	{
		if (m_ok != null)
		{
			m_ok.SetDepthOffset(-10f);
		}
		if (m_cancel != null)
		{
			m_cancel.SetDepthOffset(-10f);
		}
		base.Update();
	}

	public void SpeechBubbleDrawhandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] bezierRect = DebugDraw.GetBezierRect(_c.m_actualWidth, _c.m_actualHeight, 0.025f * (float)Screen.height, 10, Vector2.zero);
		bezierRect = DebugDraw.AddSpeechHandleToVectorArray(bezierRect, m_bubbleHandlePos, m_bubbleHandleType);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -5f, bezierRect, (float)Screen.width * 0.005f, DebugDraw.HexToColor("#4dcfff"), ResourceManager.GetMaterial("Framework/Line8Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -3f, bezierRect, (float)Screen.width * 0.008f, new Color(1f, 1f, 1f, 0.5f), ResourceManager.GetMaterial("Framework/Line8Gradient2Mat"), m_camera, Position.Inside, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.forward * -2f, bezierRect, DebugDraw.HexToUint("#FFFFFF"), DebugDraw.HexToUint("#FFFFFF"), ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
	}

	public override void Step()
	{
		if (m_buttons != null)
		{
			for (int i = 0; i < m_buttons.Count; i++)
			{
				if (m_buttons[i].Key.m_hit)
				{
					m_buttons[i].Value();
					break;
				}
			}
		}
		base.Step();
	}

	public void Talk()
	{
		if (m_character != null)
		{
			m_character.Talk();
		}
	}

	public void LookAtCamera()
	{
		if (m_character != null)
		{
			m_character.LookAtCamera();
		}
	}

	public void LookAwayFromCamera()
	{
		if (m_character != null)
		{
			m_character.LookAwayFromCamera();
		}
	}
}
