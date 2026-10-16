using System;
using UnityEngine;

public class EditorSelectorPlayer : EditorSelectorBase
{
	public EditorSelectorPlayer(EditorSelectorContext _context)
		: base("player vehicle", "Choose vehicle", _context)
	{
	}

	public override void Enter(IStatedObject _parent)
	{
		base.Enter(_parent);
		for (int i = 0; i < PsMetagameData.m_playerUnits.Count; i++)
		{
			PsUnlockableCategory psUnlockableCategory = PsMetagameData.m_playerUnits[i];
			for (int j = 0; j < psUnlockableCategory.m_items.Count; j++)
			{
				if (j < psUnlockableCategory.m_items.Count || PsState.m_adminMode)
				{
					PsUpgradeableEditorItem psUpgradeableEditorItem = psUnlockableCategory.m_items[j] as PsUpgradeableEditorItem;
					AddItemCard(psUpgradeableEditorItem.m_name, psUpgradeableEditorItem.m_description, PsColors.objectCatCardGrey, psUpgradeableEditorItem.m_iconImage, psUpgradeableEditorItem);
				}
			}
		}
		m_selector.Update();
	}

	public override void ItemTouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (_touchIsSecondary || _touchPhase != TouchAreaPhase.ReleaseIn || _touchArea.m_wasDragged)
		{
			return;
		}
		PsUnlockable psUnlockable = _touchArea.m_customObject as PsUnlockable;
		if (m_context == EditorSelectorContext.ITEM_SELECT || m_context == EditorSelectorContext.FREE_WIZARD)
		{
			Vector3 vector = Vector3.zero;
			Vector3 rot = Vector3.zero;
			Vector3 sca = Vector3.one;
			LevelPlayerNode levelPlayerNode = LevelManager.m_currentLevel.m_currentLayer.GetElement("Player") as LevelPlayerNode;
			if (levelPlayerNode != null)
			{
				vector = levelPlayerNode.m_position;
				rot = levelPlayerNode.m_rotation;
				sca = levelPlayerNode.m_scale;
				levelPlayerNode.Dispose();
			}
			string identifier = psUnlockable.m_identifier;
			levelPlayerNode = new LevelPlayerNode(Type.GetType(identifier), "Player", vector, rot, sca);
			LevelManager.m_currentLevel.m_currentLayer.AddElement(levelPlayerNode);
			levelPlayerNode.Assemble();
			PsState.m_editorCameraPos = vector;
			CameraS.SnapMainCameraPos(PsState.m_editorCameraPos);
			SoundS.PlaySingleShot("/UI/ItemCreate", Vector3.zero);
			if (levelPlayerNode != null)
			{
				EditorBaseState.CreateTransformGizmo(levelPlayerNode);
			}
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
		}
		else
		{
			PsState.m_wizardPlayer = psUnlockable.m_identifier;
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorSelectorGameMode(EditorSelectorContext.START_WIZARD));
		}
	}

	public override void Execute()
	{
	}
}
