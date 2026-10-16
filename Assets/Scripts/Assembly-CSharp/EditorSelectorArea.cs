using UnityEngine;

public class EditorSelectorArea : EditorSelectorBase
{
	private class PsUIPopupChangeAreaPrompt : PsUIPopup
	{
		private PsUIGenericButton m_ok;

		public PsUIPopupChangeAreaPrompt()
		{
			UIVerticalList uIVerticalList = new UIVerticalList(this, string.Empty);
			uIVerticalList.SetDrawHandler(PsUIPopup.PopupDrawHandler);
			uIVerticalList.SetMargins(0.05f, 0.05f, 0.05f, 0.05f);
			new UITextbox(uIVerticalList, false, string.Empty, "Sorry!\nThe area can not be changed to a smaller size", "HurmeRegular_Font", 0.03f, RelativeTo.ScreenShortest);
			m_ok = new PsUIGenericButton(uIVerticalList);
			m_ok.SetText("Ok");
			m_ok.SetGreenColors();
			Update();
			UpdatePopupBG();
		}

		public override void Step()
		{
			if (m_ok.m_hit)
			{
				Destroy();
			}
			base.Step();
		}
	}

	public EditorSelectorArea(EditorSelectorContext _context)
		: base("dome size", "Choose dome size", _context)
	{
	}

	public override void Enter(IStatedObject _parent)
	{
		base.Enter(_parent);
		foreach (PsEditorItem item in PsMetagameData.m_gameAreas[0].m_items)
		{
			AddItemCard(item.m_name, item.m_description, PsColors.objectCatCardGrey, item.m_iconImage, item);
		}
		m_selector.Update();
	}

	public override void ItemTouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (_touchIsSecondary || _touchPhase != TouchAreaPhase.ReleaseIn || _touchArea.m_wasDragged)
		{
			return;
		}
		PsEditorItem psEditorItem = _touchArea.m_customObject as PsEditorItem;
		int num = 0;
		switch (psEditorItem.m_identifier)
		{
		default:
			num = 0;
			break;
		case "AreaMedium":
			num = 1;
			break;
		case "AreaLarge":
			num = 2;
			break;
		}
		if ((m_context == EditorSelectorContext.ITEM_SELECT || m_context == EditorSelectorContext.FREE_WIZARD) && num < PsState.m_wizardDomeIndex)
		{
			new PsUIPopupChangeAreaPrompt();
			return;
		}
		SoundS.PlaySingleShot("/UI/ItemCreate", Vector3.zero);
		if (m_context == EditorSelectorContext.ITEM_SELECT || m_context == EditorSelectorContext.FREE_WIZARD)
		{
			PsState.m_wizardDomeIndex = num;
			PsState.m_activeMinigame.UpdateAreaLimits(num);
			PsState.m_activeGameLoop.CreateEnvironment();
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
		}
		else
		{
			PsState.m_wizardDomeIndex = num;
			DefaultMinigame.ApplyWizard();
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
		}
	}

	public override void Execute()
	{
	}
}
