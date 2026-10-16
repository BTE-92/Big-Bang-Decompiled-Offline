using System;
using UnityEngine;

public class PsUIFreeDialogueSpeechBubble : UIVerticalList
{
	private PsUIGenericButton m_ok;

	private PsUIGenericButton m_cancel;

	private Action m_proceedOk;

	private Action m_proceedCancel;

	private string m_text;

	private string m_okText;

	private string m_okUNLOCK;

	private string m_okACTION;

	private string m_cancelText;

	private Vector2 m_rootPos;

	private TransformC m_rootTC;

	private bool m_exiting;

	private SpeechBubbleHandlePosition m_handlePosition;

	private SpeechBubbleHandleType m_handleType;

	public PsUIFreeDialogueSpeechBubble(Vector2 _rootPos, string _text, string _okText, string _cancelText, Action _proceedOk, Action _proceedCancel, Camera _camera = null)
		: base(null, string.Empty)
	{
		m_rootPos = _rootPos;
		m_text = _text;
		if (_camera != null)
		{
			SetCamera(_camera, true, false);
		}
		if (_okText.Contains(";"))
		{
			string[] array = _okText.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			m_okText = array[0];
			for (int i = 1; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[1] { ':' }, StringSplitOptions.RemoveEmptyEntries);
				if (array2[0] == "UNLOCK")
				{
					m_okUNLOCK = array2[1];
				}
				else if (array2[0] == "ACTION")
				{
					m_okACTION = array2[1];
				}
			}
		}
		else
		{
			m_okText = _okText;
			m_okUNLOCK = null;
			m_okACTION = null;
		}
		m_cancelText = _cancelText;
		m_proceedOk = _proceedOk;
		m_proceedCancel = _proceedCancel;
		if (m_rootPos.x > 0f)
		{
			if (m_rootPos.y > 0f)
			{
				m_handlePosition = SpeechBubbleHandlePosition.TopRight;
				m_handleType = SpeechBubbleHandleType.SmallToLeft;
			}
			else
			{
				m_handlePosition = SpeechBubbleHandlePosition.BottomRight;
				m_handleType = SpeechBubbleHandleType.SmallToRight;
			}
		}
		else if (m_rootPos.y > 0f)
		{
			m_handlePosition = SpeechBubbleHandlePosition.TopLeft;
			m_handleType = SpeechBubbleHandleType.SmallToRight;
		}
		else
		{
			m_handlePosition = SpeechBubbleHandlePosition.BottomLeft;
			m_handleType = SpeechBubbleHandleType.SmallToLeft;
		}
		SetWidth(0.7f, RelativeTo.ScreenHeight);
		SetSpacing(-0.05f, RelativeTo.ScreenHeight);
		SetDepthOffset(-20f);
		RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
		uIVerticalList.SetMargins(0.1f, 0.1f, 0.05f, 0.05f);
		uIVerticalList.SetDrawHandler(BubbleDrawHandler);
		UITextbox uITextbox = new UITextbox(uIVerticalList, false, string.Empty, m_text, "HurmeSemiBold_Font", 0.035f, RelativeTo.ScreenShortest);
		uITextbox.m_tmc.m_renderer.material.shader = Shader.Find("Framework/FontShader");
		uITextbox.m_tmc.m_textMesh.GetComponent<Renderer>().material.SetColor("_Color", DebugDraw.HexToColor("2aaded"));
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
		uIHorizontalList.SetSpacing(0.05f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.05f);
		if (m_handlePosition == SpeechBubbleHandlePosition.BottomRight || m_handlePosition == SpeechBubbleHandlePosition.TopRight)
		{
			uIHorizontalList.SetHorizontalAlign(0f);
		}
		else
		{
			uIHorizontalList.SetHorizontalAlign(1f);
		}
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetDepthOffset(-20f);
		if (m_okText != string.Empty)
		{
			m_ok = new PsUIGenericButton(uIHorizontalList);
			m_ok.SetText(m_okText, 0.04f);
			m_ok.SetHeight(0.075f, RelativeTo.ScreenHeight);
			m_ok.SetGreenColors();
		}
		if (m_cancelText != string.Empty)
		{
			m_cancel = new PsUIGenericButton(uIHorizontalList);
			m_cancel.SetText(m_cancelText, 0.04f);
			m_cancel.SetHeight(0.075f, RelativeTo.ScreenHeight);
		}
		Update();
		Entity entity = EntityManager.AddEntity();
		m_rootTC = TransformS.AddComponent(entity, m_rootPos);
		Vector2 vector = new Vector2(m_actualWidth * 0.5f, m_actualHeight * 0.5f + (float)Screen.height * 0.025f);
		if (m_rootPos.x > 0f)
		{
			vector.x = m_actualWidth * -0.5f;
			if (m_rootPos.y > 0f)
			{
				vector.y = m_actualHeight * -0.5f - (float)Screen.height * 0.025f;
			}
		}
		else if (m_rootPos.y > 0f)
		{
			vector.y = m_actualHeight * -0.5f - (float)Screen.height * 0.025f;
		}
		TransformS.ParentComponent(m_TC, m_rootTC, vector);
		TransformS.SetScale(m_rootTC, 0f);
		TweenC tweenC = TweenS.AddTransformTween(m_rootTC, TweenedProperty.Scale, TweenStyle.CubicOut, Vector3.zero, Vector3.one, 0.25f, 0f, true);
		SoundS.PlaySingleShot("/UI/SpeechBubbleAppear", Vector3.zero);
	}

	public override void Step()
	{
		if (m_exiting)
		{
			return;
		}
		if (m_ok != null && m_ok.m_hit)
		{
			m_exiting = true;
			if (m_proceedOk != null)
			{
				m_proceedOk();
			}
			TransformS.Move(m_rootTC, Vector3.forward * 50f);
			TweenC tweenComponent = TweenS.AddTransformTween(m_rootTC, TweenedProperty.Scale, TweenStyle.CubicIn, Vector3.zero, 0.25f, 0f, false);
			TweenS.AddTweenEndEventListener(tweenComponent, TweenEventHandler);
			if (m_okUNLOCK != null)
			{
				PsMetagameData.TemporarilyUnlockUnlockable(m_okUNLOCK);
			}
			if (m_okACTION != null)
			{
				if (m_okACTION.Contains("Tutorial>"))
				{
					TouchAreaS.Disable();
					string text = m_okACTION.Substring(9);
					Debug.Log("Tutorial key is: " + text);
					switch (text)
					{
					case "MechanicButton":
						Debug.LogWarning("setting mechianic");
						if (PsMainMenuState.m_versusButton != null)
						{
							PsMetagameManager.m_tutorialKey = "Versus";
							PsUITutorialArrowUI psUITutorialArrowUI = new PsUITutorialArrowUI(PsMainMenuState.m_versusButton);
						}
						break;
					case "PlanetNode2":
						if (PsPlanetManager.GetCurrentPlanet().m_mainPath.m_currentNodeId <= 2)
						{
							TimerS.AddComponent(PsPlanetManager.GetCurrentPlanet().m_spaceEntity, "Tutorial finger timer", 0f, 2f, false, delegate(TimerC _c)
							{
								TimerS.RemoveComponent(_c);
								PsUITutorialArrowNode psUITutorialArrowNode = new PsUITutorialArrowNode(2);
							});
						}
						else
						{
							TouchAreaS.Enable();
						}
						break;
					case "CurrentSidePathNode":
						new PsUITutorialArrowNodeSide();
						break;
					default:
						new PsUITutorialArrow("Tutorial", text);
						break;
					}
				}
				else if (m_okACTION == "Restart")
				{
					PsState.m_activeGameLoop.RestartMinigame();
				}
				else if (m_okACTION == "FreeBooster")
				{
					PsState.m_activeGameLoop.m_freeConsumableUnlock = "Booster";
				}
				else if (m_okACTION == "FreeBoosterAndRestart")
				{
					PsState.m_activeGameLoop.m_freeConsumableUnlock = "Booster";
					PsState.m_activeGameLoop.RestartMinigame();
				}
				else if (m_okACTION == "FreeRent")
				{
					PsState.m_activeGameLoop.m_freeConsumableUnlock = "Rent";
				}
				else if (m_okACTION == "SpawnDiamondNode")
				{
					PsPlanetManager.GetCurrentPlanet().m_floaterManager.CreateDiamondChallengePlanetForced();
				}
				else if (m_okACTION == "SpawnFreshNode")
				{
					PsPlanetManager.GetCurrentPlanet().m_floaterManager.CreateFreshAndFreePlanetForced();
				}
			}
		}
		else if (m_cancel != null && m_cancel.m_hit)
		{
			m_exiting = true;
			if (m_proceedCancel != null)
			{
				m_proceedCancel();
			}
			TransformS.Move(m_rootTC, Vector3.forward * 50f);
			TweenC tweenComponent2 = TweenS.AddTransformTween(m_rootTC, TweenedProperty.Scale, TweenStyle.CubicIn, Vector3.zero, 0.25f, 0f, false);
			TweenS.AddTweenEndEventListener(tweenComponent2, TweenEventHandler);
		}
		base.Step();
	}

	private void TweenEventHandler(TweenC _c)
	{
		Destroy();
	}

	public override void Destroy()
	{
		TransformS.UnparentComponent(m_TC);
		EntityManager.RemoveEntity(m_rootTC.p_entity);
		base.Destroy();
	}

	public void BubbleDrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Vector2[] bezierRect = DebugDraw.GetBezierRect(_c.m_actualWidth, _c.m_actualHeight, 0.025f * (float)Screen.height, 10, Vector2.zero);
		bezierRect = DebugDraw.AddSpeechHandleToVectorArray(bezierRect, m_handlePosition, m_handleType);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -5f, bezierRect, (float)Screen.width * 0.005f, DebugDraw.HexToColor("#4dcfff"), ResourceManager.GetMaterial("Framework/Line8Mat"), _c.m_camera, Position.Center, true);
		PrefabS.CreatePathPrefabComponentFromVectorArray(_c.m_TC, Vector3.forward * -3f, bezierRect, (float)Screen.width * 0.008f, new Color(1f, 1f, 1f, 0.5f), ResourceManager.GetMaterial("Framework/Line8Gradient2Mat"), m_camera, Position.Inside, true);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero, bezierRect, DebugDraw.HexToUint("#FFFFFF"), DebugDraw.HexToUint("#FFFFFF"), ResourceManager.GetMaterial("Framework/SolidMat"), _c.m_camera, string.Empty);
	}
}
