using System.Collections.Generic;
using UnityEngine;

public class PsUICenterResearchUnlock : PsUICenterResearch
{
	public Queue<PsUIItemCardButton> m_itemCardUnlockQueue = new Queue<PsUIItemCardButton>();

	private int m_index;

	private int m_unlockSpaceIndex;

	private PsUIGenericButton m_proceed;

	private UIText m_title;

	private Camera m_dialogCamera;

	private PsUICrewMemberBase m_dialogCrew;

	private PsUIFixedDialogueSpeechBubble m_speechBubble;

	private int m_soundNumber;

	public PsUICenterResearchUnlock(UIComponent _parent)
		: base(_parent)
	{
		m_scrollArea.DisableTouchAreas();
		m_soundNumber = 1;
		List<PsUnlockable> list = null;
		if (PsState.m_activeGameLoop is PsGameLoopBlockSignal)
		{
			list = (PsState.m_activeGameLoop as PsGameLoopBlockSignal).m_unlockableList;
		}
		else if (PsState.m_activeGameLoop is PsGameLoopBlockSignalWithoutLevel)
		{
			list = (PsState.m_activeGameLoop as PsGameLoopBlockSignalWithoutLevel).m_unlockableList;
		}
		for (int i = 0; i < list.Count; i++)
		{
			PsUIItemCardButton itemCard = GetItemCard(list[i].m_identifier);
			itemCard.LockItemCard();
			itemCard.SetDepthOffset(-20f - (float)(i * 20));
			m_itemCardUnlockQueue.Enqueue(itemCard);
		}
		UIText uIText = new UIText(this, false, string.Empty, "New", "KGSecondChances_Font", 0.07f, RelativeTo.ScreenHeight, "#FFFFFF", "#000000");
		uIText.SetAlign(0.5f, 0.98f);
		uIText.SetShadowShift(new Vector2(0f, -1f), 0.035f);
		TransformS.SetRotation(uIText.m_TC, new Vector3(0f, 0f, 3f));
		string text = "items";
		if (list.Count < 2 && text.Length > 0 && text[text.Length - 1] == 's')
		{
			text = text.Substring(0, text.Length - 1);
		}
		m_title = new UIText(this, false, "ResearchUnlockTitle", text, "KGSecondChances_Font", 0.09f, RelativeTo.ScreenHeight, "#93fe36", "#5c3a0a");
		m_title.SetAlign(0.5f, 0.9f);
		m_title.SetShadowShift(new Vector2(0f, -1f), 0.035f);
		TransformS.SetRotation(m_title.m_TC, new Vector3(0f, 0f, 3f));
		SetScrollPositionToUnlockSpace();
		StartItemCardAnimations();
	}

	protected override void SetScrollPosition()
	{
	}

	protected override void SetHeader(UIComponent _parent)
	{
	}

	public void SetScrollPositionToUnlockSpace()
	{
		m_unlockSpaceIndex = GetItemCardIndex(m_itemCardUnlockQueue.Peek().m_unlockable.m_identifier);
		SetUnlockSpace(m_unlockSpaceIndex);
		m_scrollArea.SetScrollPositionToChild(m_hList.m_childs[m_unlockSpaceIndex]);
	}

	public void SetUnlockSpace(int _index)
	{
		UICanvas uICanvas = new UICanvas(m_hList, false, "Unlock Space", null, string.Empty);
		uICanvas.SetHeight(0.5f, RelativeTo.ScreenHeight);
		uICanvas.SetWidth(1f, RelativeTo.ScreenWidth);
		uICanvas.RemoveDrawHandler();
		uICanvas.MoveToIndexAtParentsChildList(_index);
	}

	protected override void CreateItemCard(UIComponent _parent, PsUnlockable _unlock, bool _isUnlocked = true)
	{
		PsUIItemCardButton psUIItemCardButton = new PsUIItemCardButton(_parent, _unlock);
		if (_isUnlocked)
		{
			psUIItemCardButton.HideDescription();
		}
		psUIItemCardButton.m_TAC.m_letTouchesThrough = true;
	}

	public void StartItemCardAnimations()
	{
		TimerS.AddComponent(m_TC.p_entity, "AnimationTimer", 0f, 0.125f, false, delegate(TimerC _c)
		{
			TimerS.RemoveComponent(_c);
			if (m_itemCardUnlockQueue.Count == 1)
			{
				OneCardAnimation();
			}
			else if (m_itemCardUnlockQueue.Count == 2)
			{
				TwoCardAnimation();
			}
			else if (m_itemCardUnlockQueue.Count > 2)
			{
				ThreeCardAnimation();
			}
		});
	}

	public void PlayCardAppearSound(TweenC _tween)
	{
		SoundS.PlaySingleShotWithParameter("/Ingame/Events/NewItemAppear", Vector2.zero, "ItemNumber", m_soundNumber);
		m_soundNumber++;
	}

	public void OneCardAnimation()
	{
		PsUIItemCardButton itemCard1 = m_itemCardUnlockQueue.Dequeue();
		float actualWidth = itemCard1.m_actualWidth;
		Vector3 localPosition = m_hList.m_childs[m_unlockSpaceIndex].m_TC.transform.localPosition;
		Vector3 endValue = new Vector3(localPosition.x, localPosition.y, itemCard1.m_TC.transform.localPosition.z);
		TweenC tweenComponent = TweenS.AddTransformTween(itemCard1.m_TC, TweenedProperty.Position, TweenStyle.CubicOut, endValue, 0.5f, 0.3f, true);
		TweenS.AddTweenStartEventListener(tweenComponent, PlayCardAppearSound);
		TweenC tweenComponent2 = TweenS.AddTransformTween(itemCard1.m_TC, TweenedProperty.Scale, TweenStyle.CubicOut, new Vector3(1f, 1f, 1f), new Vector3(1.2f, 1.2f, 1f), 0.25f, 0.6f, false);
		TweenS.AddTweenEndEventListener(tweenComponent2, delegate(TweenC _component)
		{
			TweenS.RemoveComponent(_component);
			itemCard1.UnlockItemCard();
			CreateParticleEffect(itemCard1.m_TC);
			CreateDialogue();
		});
	}

	public void TwoCardAnimation()
	{
		PsUIItemCardButton itemCard1 = m_itemCardUnlockQueue.Dequeue();
		PsUIItemCardButton itemCard2 = m_itemCardUnlockQueue.Dequeue();
		float actualWidth = itemCard1.m_actualWidth;
		Vector3 localPosition = m_hList.m_childs[m_unlockSpaceIndex].m_TC.transform.localPosition;
		Vector3 endValue = new Vector3(localPosition.x - actualWidth * 0.7f, localPosition.y, itemCard1.m_TC.transform.localPosition.z);
		itemCard1.m_TC.transform.Rotate(0f, 0f, 6f);
		TweenC tweenComponent = TweenS.AddTransformTween(itemCard1.m_TC, TweenedProperty.Position, TweenStyle.CubicOut, endValue, 0.5f, 0f, true);
		TweenS.AddTweenStartEventListener(tweenComponent, PlayCardAppearSound);
		Vector3 endValue2 = new Vector3(localPosition.x + actualWidth * 0.7f, localPosition.y, itemCard2.m_TC.transform.localPosition.z);
		itemCard2.m_TC.transform.Rotate(0f, 0f, -6f);
		TweenC tweenComponent2 = TweenS.AddTransformTween(itemCard2.m_TC, TweenedProperty.Position, TweenStyle.CubicOut, endValue2, 0.5f, 0.3f, true);
		TweenS.AddTweenStartEventListener(tweenComponent2, PlayCardAppearSound);
		TweenC tweenC = TweenS.AddTransformTween(itemCard2.m_TC, TweenedProperty.Scale, TweenStyle.CubicOut, new Vector3(1f, 1f, 1f), new Vector3(1.2f, 1.2f, 1f), 0.25f, 1.1f, true);
		TweenC tweenComponent3 = TweenS.AddTransformTween(itemCard1.m_TC, TweenedProperty.Scale, TweenStyle.CubicOut, new Vector3(1f, 1f, 1f), new Vector3(1.2f, 1.2f, 1f), 0.25f, 1.1f, false);
		TweenS.AddTweenEndEventListener(tweenComponent3, delegate(TweenC _component)
		{
			TweenS.RemoveComponent(_component);
			itemCard1.UnlockItemCard();
			itemCard2.UnlockItemCard();
			CreateParticleEffect(itemCard1.m_TC);
			CreateParticleEffect(itemCard2.m_TC);
			CreateDialogue();
		});
	}

	public void ThreeCardAnimation()
	{
		PsUIItemCardButton itemCard1 = m_itemCardUnlockQueue.Dequeue();
		PsUIItemCardButton itemCard2 = m_itemCardUnlockQueue.Dequeue();
		PsUIItemCardButton itemCard3 = m_itemCardUnlockQueue.Dequeue();
		float actualWidth = itemCard1.m_actualWidth;
		Vector3 localPosition = m_hList.m_childs[m_unlockSpaceIndex].m_TC.transform.localPosition;
		Vector3 endValue = new Vector3(localPosition.x - actualWidth * 1.2f, localPosition.y, itemCard1.m_TC.transform.localPosition.z);
		itemCard1.m_TC.transform.Rotate(0f, 0f, 10f);
		TweenC tweenComponent = TweenS.AddTransformTween(itemCard1.m_TC, TweenedProperty.Position, TweenStyle.CubicOut, endValue, 0.5f, 0f, true);
		TweenS.AddTweenStartEventListener(tweenComponent, PlayCardAppearSound);
		Vector3 endValue2 = new Vector3(localPosition.x, localPosition.y + actualWidth * 0.125f, itemCard2.m_TC.transform.localPosition.z);
		TweenC tweenComponent2 = TweenS.AddTransformTween(itemCard2.m_TC, TweenedProperty.Position, TweenStyle.CubicOut, endValue2, 0.5f, 0.3f, true);
		TweenS.AddTweenStartEventListener(tweenComponent2, PlayCardAppearSound);
		Vector3 endValue3 = new Vector3(localPosition.x + actualWidth * 1.2f, localPosition.y, itemCard3.m_TC.transform.localPosition.z);
		itemCard3.m_TC.transform.Rotate(0f, 0f, -10f);
		TweenC tweenComponent3 = TweenS.AddTransformTween(itemCard3.m_TC, TweenedProperty.Position, TweenStyle.CubicOut, endValue3, 0.5f, 0.6f, false);
		TweenS.AddTweenStartEventListener(tweenComponent3, PlayCardAppearSound);
		TweenC tweenC = TweenS.AddTransformTween(itemCard1.m_TC, TweenedProperty.Scale, TweenStyle.CubicOut, new Vector3(1f, 1f, 1f), new Vector3(1.2f, 1.2f, 1f), 0.25f, 1.1f, true);
		TweenC tweenC2 = TweenS.AddTransformTween(itemCard2.m_TC, TweenedProperty.Scale, TweenStyle.CubicOut, new Vector3(1f, 1f, 1f), new Vector3(1.2f, 1.2f, 1f), 0.25f, 1.1f, true);
		TweenC tweenComponent4 = TweenS.AddTransformTween(itemCard3.m_TC, TweenedProperty.Scale, TweenStyle.CubicOut, new Vector3(1f, 1f, 1f), new Vector3(1.2f, 1.2f, 1f), 0.25f, 1.1f, false);
		TweenS.AddTweenEndEventListener(tweenComponent4, delegate(TweenC _component)
		{
			TweenS.RemoveComponent(_component);
			itemCard1.UnlockItemCard();
			itemCard2.UnlockItemCard();
			itemCard3.UnlockItemCard();
			CreateParticleEffect(itemCard1.m_TC);
			CreateParticleEffect(itemCard2.m_TC);
			CreateParticleEffect(itemCard3.m_TC);
			CreateDialogue();
		});
	}

	public virtual void CreateParticleEffect(TransformC _tc)
	{
		PrefabC prefabC = PrefabS.AddComponent(_tc, Vector3.zero, ResourceManager.GetGameObject(RESOURCE.ItemCardEffect_GameObject));
		prefabC.p_gameObject.transform.position -= new Vector3(0f, 0f, 250f);
		prefabC.p_gameObject.transform.Rotate(90f, 0f, 0f);
		PrefabS.SetCamera(prefabC, m_scrollArea.m_camera);
	}

	public void CreateDialogue()
	{
		SoundS.PlaySingleShot("/Ingame/Events/NewItemUnlocked", Vector2.zero);
		m_proceed = new AttentionButton(this);
		m_proceed.SetText("Collect!", 0.04f);
		m_proceed.SetAlign(0.5f, 0.15f);
		m_proceed.Update();
	}

	public void StartDialogue(TimerC _c)
	{
		TimerS.RemoveComponent(_c);
		string text = "Good news everyone! New thing!";
		string okText = "Yippii!";
		m_dialogCrew.CreateCharacter(PsDialogueCharacter.Scientist, PsDialogueCharacterPosition.Left);
		PsUIFixedDialogueSpeechBubble psUIFixedDialogueSpeechBubble = new PsUIFixedDialogueSpeechBubble(new Vector2((float)Screen.width * -0.2f, (float)Screen.height * -0.1f), text, okText, string.Empty, Leave, null, SpeechBubbleHandlePosition.Left, SpeechBubbleHandleType.SmallToLeft, m_dialogCamera);
	}

	public PsUIItemCardButton GetItemCard(string _unlockIdentifier)
	{
		for (int i = 0; i < m_hList.m_childs.Count; i++)
		{
			UIComponent uIComponent = m_hList.m_childs[i];
			if (uIComponent.GetType() == typeof(PsUIItemCardButton))
			{
				PsUIItemCardButton psUIItemCardButton = uIComponent as PsUIItemCardButton;
				if (psUIItemCardButton.m_unlockable.m_identifier == _unlockIdentifier)
				{
					return psUIItemCardButton;
				}
			}
		}
		return null;
	}

	public int GetItemCardIndex(string _unlockIdentifier)
	{
		for (int i = 0; i < m_hList.m_childs.Count; i++)
		{
			UIComponent uIComponent = m_hList.m_childs[i];
			if (uIComponent.GetType() == typeof(PsUIItemCardButton))
			{
				PsUIItemCardButton psUIItemCardButton = uIComponent as PsUIItemCardButton;
				if (psUIItemCardButton.m_unlockable.m_identifier == _unlockIdentifier)
				{
					return i;
				}
			}
		}
		return 0;
	}

	public void Leave()
	{
		TimerS.AddComponent(m_TC.p_entity, "DialogExitTimer", 0f, 0.4f, false, delegate(TimerC _c)
		{
			TimerS.RemoveComponent(_c);
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		});
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void Step()
	{
		if (m_proceed != null && m_proceed.m_hit)
		{
			SoundS.PlaySingleShot("/UI/CollectItem", Vector2.zero);
			(GetRoot() as PsUIBasePopup).CallAction("Exit");
		}
		base.Step();
	}
}
