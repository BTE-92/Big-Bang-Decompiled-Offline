using System;
using UnityEngine;

public class EditorSelectorItem : EditorSelectorBase
{
	private static Color[] cardColors;

	private string m_categoryName;

	public EditorSelectorItem(string _categoryName, UIFittedText _notificationAmount)
		: base("insert item", "Select an item to place to your level.", EditorSelectorContext.ITEM_SELECT, _notificationAmount)
	{
		m_categoryName = _categoryName;
	}

	public override void Enter(IStatedObject _parent)
	{
		if (cardColors == null)
		{
			cardColors = new Color[3]
			{
				PsColors.objectCatCardBlue,
				PsColors.objectCatCardGreen,
				PsColors.objectCatCardRed
			};
		}
		if (m_categoryName == string.Empty)
		{
			m_categoryName = PsMetagameData.m_units[0].m_name;
			PsState.m_editorLastSelectedItemCategory = m_categoryName;
		}
		base.Enter(_parent);
		for (int i = 0; i < PsMetagameData.m_units.Count; i++)
		{
			PsUnlockableCategory psUnlockableCategory = PsMetagameData.m_units[i];
			if (psUnlockableCategory.m_name == m_categoryName)
			{
				for (int j = 0; j < psUnlockableCategory.m_items.Count; j++)
				{
					PsEditorItem psEditorItem = psUnlockableCategory.m_items[j] as PsEditorItem;
					AddItemCard(_color: (i >= cardColors.Length) ? PsColors.objectCatCardGrey : cardColors[i], _name: psEditorItem.m_name, _description: psEditorItem.m_description, _iconImage: psEditorItem.m_iconImage, _itemMetaData: psEditorItem);
				}
				break;
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
		Debug.Log(_touchArea.m_name);
		Vector3 position = CameraS.m_mainCamera.transform.position;
		position.z = 0f;
		Vector3 vector = new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f));
		position += vector;
		Vector3 zero = Vector3.zero;
		Vector3 one = Vector3.one;
		GraphElement graphElement = null;
		PsEditorItem psEditorItem = _touchArea.m_customObject as PsEditorItem;
		string graphNodeClassName = psEditorItem.m_graphNodeClassName;
		string identifier = psEditorItem.m_identifier;
		if (!PsMetagameData.CanPlaceUnit(identifier))
		{
			return;
		}
		object[] args = new object[6]
		{
			GraphNodeType.Normal,
			Type.GetType(identifier),
			identifier,
			position,
			zero,
			one
		};
		graphElement = (GraphElement)Activator.CreateInstance(Type.GetType(graphNodeClassName), args);
		LevelManager.m_currentLevel.m_currentLayer.AddElement(graphElement);
		graphElement.Assemble();
		if (graphElement != null)
		{
			EditorBaseState.CreateTransformGizmo(graphElement);
		}
		SoundS.PlaySingleShot("/UI/ItemCreate", Vector3.zero);
		LevelManager.m_currentLevel.ItemChanged();
		int newEditorItemCount = PsState.m_newEditorItemCount;
		PsMetagameManager.RemoveNewEditorItem(identifier);
		if (newEditorItemCount != PsState.m_newEditorItemCount && m_notificationAmount != null)
		{
			int num = Convert.ToInt32(m_notificationAmount.m_tmc.m_textMesh.text);
			num--;
			if (num > 0)
			{
				TextMeshS.SetText(m_notificationAmount.m_tmc, num.ToString());
			}
			else
			{
				UIComponent root = m_notificationAmount.GetRoot();
				m_notificationAmount.m_parent.m_parent.Destroy();
				root.Update();
			}
		}
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
	}

	public override void Execute()
	{
	}
}
