using UnityEngine;

public class EditorSelectorGameMode : EditorSelectorBase
{
	private Minigame m_minigame;

	public EditorSelectorGameMode(EditorSelectorContext _context)
		: base("game mode", "Choose game mode", _context)
	{
		m_minigame = LevelManager.m_currentLevel as Minigame;
	}

	public override void Enter(IStatedObject _parent)
	{
		base.Enter(_parent);
		foreach (PsEditorItem item in PsMetagameData.m_gameModes[0].m_items)
		{
			AddItemCard(item.m_name, item.m_description, PsColors.objectCatCardGrey, item.m_iconImage, item);
		}
		m_selector.Update();
	}

	public override void ItemTouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (!_touchIsSecondary && _touchPhase == TouchAreaPhase.ReleaseIn && !_touchArea.m_wasDragged)
		{
			PsEditorItem psEditorItem = _touchArea.m_customObject as PsEditorItem;
			string text = psEditorItem.m_identifier.Substring(8);
			if (m_context == EditorSelectorContext.ITEM_SELECT || m_context == EditorSelectorContext.FREE_WIZARD)
			{
				m_minigame.SetGameMode(text, CameraS.m_mainCamera.transform.position + new Vector3(-50f, 0f, 0f), false);
				Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
				Debug.Log("Setting game mode to: " + PsState.m_activeGameLoop.GetGameMode());
			}
			else
			{
				PsState.m_wizardGameMode = text;
				Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorSelectorArea(EditorSelectorContext.START_WIZARD));
			}
			SoundS.PlaySingleShot("/UI/ItemCreate", Vector3.zero);
		}
	}

	public override void Execute()
	{
	}
}
