using UnityEngine;

public class EditorSelectorBase : BasicState
{
	public EditorSelectorContext m_context;

	public UIModel m_model;

	public UIHorizontalList m_itemList;

	public UICanvas m_selector;

	public UIEditorSelectorNavigator m_navigator;

	public string m_headerName;

	public string m_description;

	protected UIFittedText m_notificationAmount;

	public EditorSelectorBase(string _header, string _description, EditorSelectorContext _context, UIFittedText _notificationAmount = null)
	{
		m_context = _context;
		m_headerName = _header;
		m_description = _description;
		m_notificationAmount = _notificationAmount;
	}

	public override void Enter(IStatedObject _parent)
	{
		m_model = new UIModel(this);
		EntityManager.SetActivityOfEntitiesWithTag("UIComponent", false);
		m_selector = new UICanvas(null, true, "SelectorContainer", null, string.Empty);
		m_selector.SetWidth(1f, RelativeTo.ScreenWidth);
		m_selector.SetHeight(1f, RelativeTo.ScreenHeight);
		m_selector.SetDepthOffset(100f);
		m_selector.SetDrawHandler(UIDrawHandlers.EditorPopupBackground);
		UIVerticalList uIVerticalList = new UIVerticalList(m_selector, "SelectorArea");
		uIVerticalList.RemoveDrawHandler();
		uIVerticalList.SetVerticalAlign(0.9f);
		uIVerticalList.RemoveTouchAreas();
		bool createBackButton = true;
		m_navigator = new UIEditorSelectorNavigator(uIVerticalList, m_headerName, m_description, "Navigation", m_context, createBackButton, m_notificationAmount);
		UIScrollableCanvas uIScrollableCanvas = new UIScrollableCanvas(uIVerticalList, "ItemArea");
		uIScrollableCanvas.SetWidth(1f, RelativeTo.ScreenWidth);
		uIScrollableCanvas.SetHeight(0.4f, RelativeTo.ScreenHeight);
		uIScrollableCanvas.SetMargins(0.35f, 0.3f, 0.02f, 0f, RelativeTo.ScreenShortest);
		uIScrollableCanvas.m_maxScrollInertialY = 0f;
		uIScrollableCanvas.m_maxScrollInertialX = 200f;
		uIScrollableCanvas.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uIScrollableCanvas, "ItemList");
		uIHorizontalList.SetSpacing(0.03f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetHorizontalAlign(0f);
		uIHorizontalList.RemoveDrawHandler();
		m_itemList = uIHorizontalList;
		m_selector.Update();
	}

	public void AddItemCard(string _name, string _description, Color _color, string _iconImage = null, PsEditorItem _itemMetaData = null)
	{
		_color = Color.Lerp(_color, Color.black, 0.5f);
		string empty = string.Empty;
		bool flag = false;
		if (_itemMetaData != null)
		{
			if (1 == 0)
			{
				flag = !_itemMetaData.m_unlocked;
			}
			else
			{
				empty = "Requirement: " + _itemMetaData.m_itemLevel;
			}
		}
		UICanvas uICanvas = new UICanvas(m_itemList, true, _name, null, string.Empty);
		uICanvas.SetHeight(1f, RelativeTo.ParentHeight);
		uICanvas.SetWidth(0.7f, RelativeTo.OwnHeight);
		uICanvas.RemoveDrawHandler();
		uICanvas.SetMargins(0f, 0f, Random.Range(-0.03f, 0.03f), 0f, RelativeTo.ParentHeight);
		uICanvas.m_TAC.m_letTouchesThrough = true;
		UIVerticalList uIVerticalList = new UIVerticalList(uICanvas, "vlist");
		uIVerticalList.RemoveDrawHandler();
		uIVerticalList.SetSpacing(-0.02f, RelativeTo.OwnHeight);
		UIFittedSprite uIFittedSprite = new UIFittedSprite(uIVerticalList, false, "top", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("editor_itemcard_frame_top"));
		uIFittedSprite.SetDepthOffset(-10f);
		uIFittedSprite.SetColor(_color);
		UIFittedSprite parent = new UIFittedSprite(uIVerticalList, false, "center", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("editor_itemcard_bg"));
		UIFittedSprite uIFittedSprite2 = new UIFittedSprite(parent, false, "left", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("editor_itemcard_frame_left"));
		uIFittedSprite2.m_rogue = true;
		uIFittedSprite2.SetDepthOffset(-8f);
		uIFittedSprite2.SetHorizontalAlign(0f);
		uIFittedSprite2.SetColor(_color);
		UIFittedSprite uIFittedSprite3 = new UIFittedSprite(parent, false, "right", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("editor_itemcard_frame_right"));
		uIFittedSprite3.m_rogue = true;
		uIFittedSprite3.SetDepthOffset(-8f);
		uIFittedSprite3.SetHorizontalAlign(1f);
		uIFittedSprite3.SetColor(_color);
		if (_itemMetaData != null && _itemMetaData.m_iconImage != null && !_itemMetaData.m_iconImage.Equals(string.Empty))
		{
			UIFittedSprite uIFittedSprite4 = new UIFittedSprite(parent, false, "Unit", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(_itemMetaData.m_iconImage));
			if (flag)
			{
				uIFittedSprite4.m_overrideShader = Shader.Find("WOE/Editor/ItemCardIconLocked");
			}
			uIFittedSprite4.m_rogue = true;
			uIFittedSprite4.SetDepthOffset(-15f);
			uIFittedSprite4.SetVerticalAlign(1.05f);
			if (_itemMetaData.m_iconImage == "_unknown")
			{
				uIFittedSprite4.SetVerticalAlign(0.86f);
			}
			uIFittedSprite4.SetWidth(0.6f, RelativeTo.ParentWidth);
		}
		UIVerticalList uIVerticalList2 = new UIVerticalList(parent, "textContainer");
		uIVerticalList2.RemoveDrawHandler();
		uIVerticalList2.SetVerticalAlign(1f);
		uIVerticalList2.SetMargins(0.05f, 0.05f, 0.55f, 0f, RelativeTo.ParentHeight);
		_name = (flag ? "LOCKED" : ("<color=#" + ToolBox.ColorToHex(PsColors.menuLimeGreen) + "> " + _name + " </color>"));
		UITextbox uITextbox = new UITextbox(uIVerticalList2, false, string.Empty, _name, "KGSecondChances_Font", 0.025f, RelativeTo.ScreenHeight, false, Align.Center);
		if (_description != null && !flag)
		{
			UITextbox uITextbox2 = new UITextbox(uIVerticalList2, false, string.Empty, _description, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight, false, Align.Center);
		}
		UIFittedSprite uIFittedSprite5 = new UIFittedSprite(uIVerticalList, false, "bottom", PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("editor_itemcard_frame_bottom"));
		uIFittedSprite5.SetDepthOffset(-10f);
		uIFittedSprite5.SetColor(_color);
		if (PsState.m_newEditorItemCount > 0)
		{
			bool flag2 = false;
			for (int i = 0; i < PsState.m_newEditorItems.Length; i++)
			{
				if (PsState.m_newEditorItems[i] == _itemMetaData.m_identifier)
				{
					flag2 = true;
					break;
				}
			}
			if (flag2)
			{
				UICanvas uICanvas2 = new UICanvas(uICanvas, false, "notification", null, string.Empty);
				uICanvas2.SetSize(0.05f, 0.05f, RelativeTo.ScreenHeight);
				uICanvas2.SetMargins(-0.005f, 0.005f, -0.005f, 0.005f);
				uICanvas2.SetAlign(0f, 1f);
				uICanvas2.SetDepthOffset(-25f);
				uICanvas2.RemoveDrawHandler();
				UICanvas uICanvas3 = new UICanvas(uICanvas2, false, string.Empty, null, string.Empty);
				uICanvas3.SetSize(1f, 1f, RelativeTo.ParentHeight);
				uICanvas3.SetMargins(0.15f, RelativeTo.OwnHeight);
				uICanvas3.SetDrawHandler(PsUIDrawHandlers.NotificationDrawhandler);
				TweenC c = TweenS.AddTransformTween(uICanvas3.m_TC, TweenedProperty.Scale, TweenStyle.CubicInOut, new Vector3(1.1f, 1.1f, 1.1f), 0.5f, 0f, false);
				TweenS.SetAdditionalTweenProperties(c, -1, true, TweenStyle.CubicInOut);
				UIFittedText uIFittedText = new UIFittedText(uICanvas3, false, string.Empty, "!", "KGSecondChances_Font", true);
			}
		}
		if (!flag)
		{
			uICanvas.SetTouchHandler(ItemTouchHandler);
			uICanvas.m_TAC.m_customObject = _itemMetaData;
		}
	}

	public virtual void ItemTouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
	}

	public override void Execute()
	{
	}

	public override void Exit()
	{
		if (m_selector != null)
		{
			m_selector.Destroy();
			m_selector = null;
		}
		EntityManager.SetActivityOfEntitiesWithTag("UIComponent", true);
	}
}
