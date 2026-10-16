using UnityEngine;

public class PsUIGenericButton : UIHorizontalList
{
	public UIText m_UItext;

	public UIText m_priceText;

	public UIFittedSprite m_UIsprite;

	public UICanvas m_upgradeCanvas;

	public UICanvas m_skipCanvas;

	public UIVerticalList m_shopBase;

	private Color m_outlineColor;

	private Color m_fillTopColor;

	private Color m_fillBottomColor;

	private Color m_outlineTopColor;

	private Color m_outlineBottomColor;

	public string m_textColor;

	private Color m_iconColor;

	private float m_gradientSize;

	private float m_gradientPos;

	private float m_cornerSize;

	private bool m_customRelease;

	private bool m_customTweens;

	public TweenC m_touchScaleTween;

	public Vector3 m_currentScale;

	private Vector3 m_createScale;

	private bool m_scaleAtCreate;

	public bool m_lateDraw;

	public bool m_scaledUp;

	private UIVerticalList m_textArea;

	private string m_sound;

	private float m_glareYOffset;

	private float m_topShineXOffset;

	private float m_topShineYOffset;

	private float m_lowerShineXOffset;

	private float m_lowerShineYOffset;

	private SpeechBubbleHandlePosition m_speechBubbleHandlePosition;

	private SpeechBubbleHandleType m_speechBubbleHandleType;

	private bool m_speechBubbleHandler;

	public PsUIGenericButton(UIComponent _parent, float _gradientSize = 0.25f, float _gradientPos = 0.25f, float _cornerSize = 0.005f)
		: base(_parent, "Button")
	{
		SetMargins(0.02f, 0.02f, 0.02f, 0.02f);
		SetSpacing(0.01f, RelativeTo.ScreenHeight);
		CreateTouchAreas();
		m_TAC.m_letTouchesThrough = true;
		m_fillTopColor = DebugDraw.HexToColor("#0077d5");
		m_fillBottomColor = DebugDraw.HexToColor("#00b6ea");
		m_outlineTopColor = DebugDraw.HexToColor("#58cbe3");
		m_outlineBottomColor = DebugDraw.HexToColor("#0160bb");
		m_outlineColor = DebugDraw.HexToColor("#2a3f57");
		m_textColor = "#FFFFFF";
		m_iconColor = Color.gray;
		m_gradientSize = _gradientSize;
		m_gradientPos = _gradientPos;
		m_cornerSize = _cornerSize;
		m_customRelease = false;
		m_customTweens = false;
		m_currentScale = Vector3.one;
		m_lateDraw = false;
		m_scaledUp = false;
		m_scaleAtCreate = false;
		m_createScale = Vector3.zero;
		m_sound = "/UI/ButtonNormal";
	}

	public void SetSound(string _sound)
	{
		m_sound = _sound;
	}

	public void SetCustomRelease(bool _val)
	{
		m_customRelease = _val;
	}

	public void SetCustomTweens(bool _val)
	{
		m_customTweens = _val;
	}

	public void SetGreenColors(bool _enabled = true)
	{
		if (_enabled)
		{
			m_fillTopColor = DebugDraw.HexToColor("#47d30f");
			m_fillBottomColor = DebugDraw.HexToColor("#95ff37");
			m_outlineTopColor = DebugDraw.HexToColor("#a0ff19t");
			m_outlineBottomColor = DebugDraw.HexToColor("#45c200");
			m_outlineColor = DebugDraw.HexToColor("#1f6500");
			m_textColor = "#FFFFFF";
			m_iconColor = Color.gray;
		}
		else
		{
			m_fillTopColor = DebugDraw.HexToColor("#a3f255");
			m_fillBottomColor = DebugDraw.HexToColor("#a3f255");
			m_outlineTopColor = DebugDraw.HexToColor("#dcff92");
			m_outlineBottomColor = DebugDraw.HexToColor("#dcff92");
			m_outlineColor = DebugDraw.HexToColor("#FFFFFF");
			m_textColor = "#5bc70c";
			m_iconColor = Color.gray;
		}
	}

	public void SetBlueColors(bool _enabled = true)
	{
		if (_enabled)
		{
			m_fillTopColor = DebugDraw.HexToColor("#0077d5");
			m_fillBottomColor = DebugDraw.HexToColor("#00b6ea");
			m_outlineTopColor = DebugDraw.HexToColor("#58cbe3");
			m_outlineBottomColor = DebugDraw.HexToColor("#0160bb");
			m_outlineColor = DebugDraw.HexToColor("#2a3f57");
			m_textColor = "#FFFFFF";
			m_iconColor = Color.gray;
		}
		else
		{
			m_fillTopColor = DebugDraw.HexToColor("#8ae2ff");
			m_fillBottomColor = DebugDraw.HexToColor("#8ae2ff");
			m_outlineTopColor = DebugDraw.HexToColor("#c5f4ff");
			m_outlineBottomColor = DebugDraw.HexToColor("#c5f4ff");
			m_outlineColor = DebugDraw.HexToColor("#FFFFFF");
			m_textColor = "#4fabe3";
			m_iconColor = Color.gray;
		}
	}

	public void SetOrangeColors(bool _enabled = true)
	{
		if (_enabled)
		{
			m_fillTopColor = DebugDraw.HexToColor("#ff9e0a");
			m_fillBottomColor = DebugDraw.HexToColor("#ffc321");
			m_outlineTopColor = DebugDraw.HexToColor("#ffd440");
			m_outlineBottomColor = DebugDraw.HexToColor("#ffa81f");
			m_outlineColor = DebugDraw.HexToColor("#ad4d00");
			m_textColor = "#FFFFFF";
			m_iconColor = Color.gray;
		}
		else
		{
			m_fillTopColor = DebugDraw.HexToColor("#ffd65b");
			m_fillBottomColor = DebugDraw.HexToColor("#ffd65b");
			m_outlineTopColor = DebugDraw.HexToColor("#ffe6a1");
			m_outlineBottomColor = DebugDraw.HexToColor("#ffe6a1");
			m_outlineColor = DebugDraw.HexToColor("#FFFFFF");
			m_textColor = "#ffa200";
			m_iconColor = Color.gray;
		}
	}

	public void SetTealColors()
	{
		m_fillTopColor = DebugDraw.HexToColor("#49b4ae");
		m_fillBottomColor = DebugDraw.HexToColor("#3ca19c");
		m_outlineTopColor = DebugDraw.HexToColor("#7ed0c5");
		m_outlineBottomColor = DebugDraw.HexToColor("#2f8587");
		m_outlineColor = DebugDraw.HexToColor("#083a3b");
		m_textColor = "#FFFFFF";
		m_iconColor = Color.gray;
	}

	public void SetRedColors()
	{
		m_fillTopColor = DebugDraw.HexToColor("#c90702");
		m_fillBottomColor = DebugDraw.HexToColor("#ee2219");
		m_outlineTopColor = DebugDraw.HexToColor("#fe5a5a");
		m_outlineBottomColor = DebugDraw.HexToColor("#c61706");
		m_outlineColor = DebugDraw.HexToColor("#850906");
		m_textColor = "#FFFFFF";
		m_iconColor = Color.gray;
	}

	public void SetGrayColors()
	{
		m_fillTopColor = DebugDraw.HexToColor("#8e8e8e");
		m_fillBottomColor = DebugDraw.HexToColor("#7D7D7D");
		m_outlineTopColor = DebugDraw.HexToColor("#B1B1B1");
		m_outlineBottomColor = DebugDraw.HexToColor("#696969");
		m_outlineColor = DebugDraw.HexToColor("#2A2A2A");
		m_textColor = "#cccccc";
		m_iconColor = DebugDraw.HexToColor("#cccccc") * Color.gray;
	}

	public void SetGlareOffset(float _y)
	{
		m_glareYOffset = _y;
	}

	public void SetUpperShineOffset(float _x, float _y)
	{
		m_topShineXOffset = _x;
		m_topShineYOffset = _y;
	}

	public void SetLowerShineOffset(float _x, float _y)
	{
		m_lowerShineXOffset = _x;
		m_lowerShineYOffset = _y;
	}

	public void SetText(string _text, float _fontSize = 0.03f, float _textAreaWidth = 0f, RelativeTo _textAreaRelativeTo = RelativeTo.ScreenHeight)
	{
		if (m_textArea == null)
		{
			m_textArea = new UIVerticalList(this, string.Empty);
			m_textArea.RemoveDrawHandler();
		}
		m_textArea.SetWidth(_textAreaWidth, _textAreaRelativeTo);
		if (m_UItext != null)
		{
			m_UItext.Destroy();
		}
		m_UItext = new UIText(m_textArea, false, string.Empty, _text, "KGSecondChances_Font", _fontSize, RelativeTo.ScreenShortest, m_textColor);
		if (_text.IndexOfAny(new char[1] { '\n' }) != -1)
		{
			SetHeight(0f, RelativeTo.ScreenShortest);
		}
	}

	public void SetIcon(string _iconFrame, float _iconHeight = 0.05f, string _iconColor = "#FFFFFF")
	{
		if (m_UIsprite != null)
		{
			m_UIsprite.Destroy();
		}
		m_UIsprite = new UIFittedSprite(this, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(_iconFrame));
		m_UIsprite.SetSize(_iconHeight, _iconHeight, RelativeTo.ScreenShortest);
		m_UIsprite.SetColor(m_iconColor);
	}

	public void RemoveIcon()
	{
		if (m_UIsprite != null)
		{
			m_UIsprite.Destroy();
		}
		m_UIsprite = null;
	}

	public void RemoveText()
	{
		if (m_textArea != null)
		{
			m_textArea.Destroy();
			m_textArea = null;
		}
		else if (m_UItext != null)
		{
			m_UItext.Destroy();
			m_UItext = null;
		}
	}

	public void SetCreateScale(Vector3 _scale)
	{
		m_scaleAtCreate = true;
		m_createScale = _scale;
	}

	public override void Update()
	{
		if (m_UItext != null)
		{
			m_UItext.SetColor(m_textColor);
		}
		if (m_UIsprite != null)
		{
			m_UIsprite.SetColor(m_iconColor);
		}
		if (m_hidden)
		{
			m_lateDraw = true;
		}
		base.Update();
		if (m_scaleAtCreate)
		{
			TransformS.SetScale(m_TC, m_createScale);
			m_scaleAtCreate = false;
		}
	}

	public void SetUpgradePrice(int _coins, int _bolts, float _iconHeight)
	{
		if (m_upgradeCanvas != null)
		{
			m_upgradeCanvas.Destroy();
		}
		m_upgradeCanvas = new UICanvas(this, false, string.Empty, null, string.Empty);
		m_upgradeCanvas.SetSize(_iconHeight, _iconHeight, RelativeTo.ScreenHeight);
		m_upgradeCanvas.SetAlign(0.55f, 1f);
		m_upgradeCanvas.SetMargins(0f, 0f, -0.5f, 0.5f, RelativeTo.ParentHeight);
		m_upgradeCanvas.SetRogue();
		m_upgradeCanvas.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_upgradeCanvas, "upgradePrice");
		uIHorizontalList.SetHeight(_iconHeight, RelativeTo.ScreenHeight);
		uIHorizontalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetVerticalAlign(1f);
		uIHorizontalList.SetHorizontalAlign(1f);
		uIHorizontalList.RemoveDrawHandler();
		if (_bolts > 0)
		{
			UIHorizontalList uIHorizontalList2 = new UIHorizontalList(uIHorizontalList, string.Empty);
			uIHorizontalList2.SetHeight(1f, RelativeTo.ParentHeight);
			uIHorizontalList2.SetSpacing(-0.5f, RelativeTo.ParentHeight);
			uIHorizontalList2.SetVerticalAlign(1f);
			for (int i = 0; i < _bolts; i++)
			{
				UIFittedSprite uIFittedSprite = new UIFittedSprite(uIHorizontalList2, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_bolt_icon"));
				uIFittedSprite.SetHeight(_iconHeight, RelativeTo.ScreenHeight);
				uIFittedSprite.SetVerticalAlign(1f);
			}
		}
		UIHorizontalList uIHorizontalList3 = new UIHorizontalList(uIHorizontalList, string.Empty);
		uIHorizontalList3.SetHeight(1f, RelativeTo.ParentHeight);
		uIHorizontalList3.SetMargins(0.5f, 0.5f, 0f, 0f, RelativeTo.ParentHeight);
		uIHorizontalList3.SetVerticalAlign(1f);
		UIText uIText = new UIText(uIHorizontalList3, false, string.Empty, _coins.ToString(), "HurmeSemiBold_Font", 0.85f, RelativeTo.ParentHeight);
		uIText.SetVerticalAlign(1f);
		UIFittedSprite uIFittedSprite2 = new UIFittedSprite(uIHorizontalList3, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_coin_icon"));
		uIFittedSprite2.SetHeight(_iconHeight, RelativeTo.ScreenHeight);
		uIFittedSprite2.SetVerticalAlign(1f);
	}

	public void SetDiamondPrice(int _diamonds, float _iconHeight)
	{
		if (m_skipCanvas != null)
		{
			m_skipCanvas.Destroy();
		}
		m_skipCanvas = new UICanvas(this, false, string.Empty, null, string.Empty);
		m_skipCanvas.SetSize(_iconHeight, _iconHeight, RelativeTo.ScreenHeight);
		m_skipCanvas.SetAlign(1f, 1f);
		m_skipCanvas.SetMargins(0f, -0.045f, -0.045f, 0f);
		m_skipCanvas.SetDepthOffset(-10f);
		m_skipCanvas.SetRogue();
		m_skipCanvas.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_skipCanvas, "SkipPrice");
		uIHorizontalList.SetHeight(_iconHeight, RelativeTo.ScreenHeight);
		uIHorizontalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.015f, -0.01f, -0.01f, -0.01f);
		uIHorizontalList.SetVerticalAlign(1f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		uIHorizontalList.SetDepthOffset(-15f);
		uIHorizontalList.SetRogue();
		uIHorizontalList.SetDrawHandler(PsUIDrawHandlers.DiamondLabelBackground);
		if (_diamonds > 0)
		{
			UIText uIText = new UIText(uIHorizontalList, false, string.Empty, _diamonds.ToString(), "HurmeSemiBold_Font", 0.9f, RelativeTo.ParentHeight, "#3e3e3e");
			UIFittedSprite uIFittedSprite = new UIFittedSprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_diamond_icon"));
			uIFittedSprite.SetHeight(1f, RelativeTo.ParentHeight);
		}
	}

	public void SetTicketPrice(int _tickets, float _iconHeight)
	{
		if (m_skipCanvas != null)
		{
			m_skipCanvas.Destroy();
		}
		m_skipCanvas = new UICanvas(this, false, string.Empty, null, string.Empty);
		m_skipCanvas.SetSize(_iconHeight, _iconHeight, RelativeTo.ScreenHeight);
		m_skipCanvas.SetAlign(1f, 1f);
		m_skipCanvas.SetMargins(0f, -0.045f, -0.045f, 0f);
		m_skipCanvas.SetDepthOffset(-10f);
		m_skipCanvas.SetRogue();
		m_skipCanvas.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_skipCanvas, "TicketPrice");
		uIHorizontalList.SetHeight(_iconHeight, RelativeTo.ScreenHeight);
		uIHorizontalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.015f, -0.01f, -0.01f, -0.01f);
		uIHorizontalList.SetVerticalAlign(1f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		uIHorizontalList.SetDepthOffset(-15f);
		uIHorizontalList.SetRogue();
		uIHorizontalList.SetDrawHandler(PsUIDrawHandlers.TicketLabelBackground);
	}

	public void SetSkipPrice(int _diamonds, float _iconHeight)
	{
		if (m_skipCanvas != null)
		{
			m_skipCanvas.Destroy();
		}
		m_skipCanvas = new UICanvas(this, false, string.Empty, null, string.Empty);
		m_skipCanvas.SetSize(_iconHeight * 1.2f, _iconHeight * 1.2f, RelativeTo.ScreenHeight);
		m_skipCanvas.SetMargins(0f, 0f, -0.03f, 0f);
		m_skipCanvas.SetHorizontalAlign(0.5f);
		m_skipCanvas.SetDepthOffset(-10f);
		m_skipCanvas.SetRogue();
		m_skipCanvas.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_skipCanvas, "SkipPrice");
		uIHorizontalList.SetHeight(_iconHeight, RelativeTo.ScreenHeight);
		uIHorizontalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.015f, -0.01f, -0.01f, -0.01f);
		uIHorizontalList.SetVerticalAlign(1f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		uIHorizontalList.SetDepthOffset(-15f);
		uIHorizontalList.SetDrawHandler(PsUIDrawHandlers.DiamondLabelBackground);
		if (_diamonds > 0)
		{
			m_priceText = new UIText(uIHorizontalList, false, string.Empty, _diamonds.ToString(), "HurmeSemiBold_Font", 0.9f, RelativeTo.ParentHeight, "#3e3e3e");
			UIFittedSprite uIFittedSprite = new UIFittedSprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_diamond_icon"));
			uIFittedSprite.SetHeight(1f, RelativeTo.ParentHeight);
		}
	}

	public void SetRentPrice(int _keys, float _iconHeight)
	{
		if (m_skipCanvas != null)
		{
			m_skipCanvas.Destroy();
		}
		m_skipCanvas = new UICanvas(this, false, string.Empty, null, string.Empty);
		m_skipCanvas.SetSize(_iconHeight, _iconHeight, RelativeTo.ScreenHeight);
		m_skipCanvas.SetMargins(2.8f, -2.8f, -1.15f, 1.15f, RelativeTo.OwnHeight);
		m_skipCanvas.SetDepthOffset(-10f);
		m_skipCanvas.SetRogue();
		m_skipCanvas.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(m_skipCanvas, "upgradePrice");
		uIHorizontalList.SetHeight(_iconHeight, RelativeTo.ScreenHeight);
		uIHorizontalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.005f);
		uIHorizontalList.SetVerticalAlign(1f);
		uIHorizontalList.SetHorizontalAlign(1f);
		uIHorizontalList.SetDrawHandler(PsUIDrawHandlers.KeyLabelBackground);
		if (_keys > 0)
		{
			UIText uIText = new UIText(uIHorizontalList, false, string.Empty, _keys.ToString(), "HurmeSemiBold_Font", 0.9f, RelativeTo.ParentHeight, "#3e3e3e");
			uIText.SetDepthOffset(-5f);
			UIFittedSprite uIFittedSprite = new UIFittedSprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_key_icon"));
			uIFittedSprite.SetHeight(_iconHeight, RelativeTo.ScreenHeight);
			uIFittedSprite.SetDepthOffset(-5f);
		}
		else
		{
			UIText uIText2 = new UIText(uIHorizontalList, false, string.Empty, "Free!", "HurmeSemiBold_Font", 0.9f, RelativeTo.ParentHeight, "#3e3e3e");
			uIText2.SetDepthOffset(-5f);
		}
	}

	public void SetRestartAmount(int _triesLeft, int _maxTries)
	{
		UICanvas uICanvas = new UICanvas(this, false, string.Empty, null, string.Empty);
		uICanvas.SetWidth(0.2f, RelativeTo.ScreenWidth);
		uICanvas.SetHeight(0.08f, RelativeTo.ScreenHeight);
		uICanvas.SetRogue();
		uICanvas.SetAlign(0f, 0.5f);
		uICanvas.SetMargins(-0.2f, 0.2f, 0f, 0f, RelativeTo.ScreenWidth);
		uICanvas.SetDepthOffset(12f);
		uICanvas.RemoveDrawHandler();
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uICanvas, string.Empty);
		uIHorizontalList.SetHeight(0.08f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.045f, 0f, 0f);
		uIHorizontalList.SetDrawHandler(PsUIDrawHandlers.ResourceLabelBackground);
		uIHorizontalList.SetHorizontalAlign(1f);
		uIHorizontalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		UIFittedSprite uIFittedSprite = new UIFittedSprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("item_mode_timeattack"));
		uIFittedSprite.SetHeight(1f, RelativeTo.ParentHeight);
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, _triesLeft + "/" + _maxTries, "HurmeSemiBoldMN_Font", 0.04f, RelativeTo.ScreenHeight);
		uIText.SetVerticalAlign(0.35f);
	}

	public void SetShopButton(string _priceText, string _label, string _iconName, bool _diamond = true, bool _coin = false, bool _bolt = false, bool _key = false)
	{
		if (m_shopBase != null)
		{
			m_shopBase.Destroy();
		}
		string text = string.Empty;
		UIDrawDelegate drawHandler = PsUIDrawHandlers.KeyLabelBackground;
		if (_coin)
		{
			text = "menu_resources_coin_price";
			drawHandler = PsUIDrawHandlers.CoinLabelBackground;
		}
		else if (_bolt)
		{
			text = "menu_resources_bolt_price";
			drawHandler = PsUIDrawHandlers.BoltLabelBackground;
		}
		else if (_key)
		{
			text = "menu_resources_key_price";
			drawHandler = PsUIDrawHandlers.KeyLabelBackground;
		}
		else if (_diamond)
		{
			text = "menu_resources_diamond_price";
			drawHandler = PsUIDrawHandlers.DiamondLabelBackground;
		}
		m_shopBase = new UIVerticalList(this, string.Empty);
		m_shopBase.SetWidth(0.3f, RelativeTo.ScreenHeight);
		m_shopBase.RemoveDrawHandler();
		m_shopBase.SetVerticalAlign(1f);
		UICanvas uICanvas = new UICanvas(m_shopBase, false, string.Empty, null, string.Empty);
		uICanvas.SetWidth(1f, RelativeTo.ParentWidth);
		uICanvas.SetHeight(0.325f, RelativeTo.ParentWidth);
		uICanvas.RemoveDrawHandler();
		UITextbox uITextbox = new UITextbox(uICanvas, false, string.Empty, _label, "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, false, Align.Center, Align.Middle);
		uITextbox.SetWidth(1f, RelativeTo.ParentWidth);
		UICanvas uICanvas2 = new UICanvas(m_shopBase, false, string.Empty, null, string.Empty);
		uICanvas2.SetWidth(1f, RelativeTo.ParentWidth);
		uICanvas2.SetHeight(0.625f, RelativeTo.ParentWidth);
		uICanvas2.SetMargins(0f, 0f, -0.3f, -0.3f, RelativeTo.ParentWidth);
		uICanvas2.RemoveDrawHandler();
		UIFittedSprite uIFittedSprite = new UIFittedSprite(uICanvas2, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(_iconName));
		uIFittedSprite.SetHeight(1f, RelativeTo.ParentHeight);
		UICanvas uICanvas3 = new UICanvas(m_shopBase, false, string.Empty, null, string.Empty);
		uICanvas3.SetWidth(1f, RelativeTo.ParentWidth);
		uICanvas3.SetHeight(0.325f, RelativeTo.ParentWidth);
		uICanvas3.RemoveDrawHandler();
		uICanvas3.SetDepthOffset(-20f);
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uICanvas3, string.Empty);
		uIHorizontalList.SetHeight(0.06f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetSpacing(0.01f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.02f, 0f, 0f, 0f);
		uIHorizontalList.SetDrawHandler(drawHandler);
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, _priceText, "HurmeSemiBold_Font", 1f, RelativeTo.ParentHeight, "#3e3e3e");
		if (text != string.Empty)
		{
			UIFittedSprite uIFittedSprite2 = new UIFittedSprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(text));
			uIFittedSprite2.SetHeight(1f, RelativeTo.ParentHeight);
		}
	}

	public void SetSpeechHandle(SpeechBubbleHandlePosition _position, SpeechBubbleHandleType _type = SpeechBubbleHandleType.SmallToCenter)
	{
		m_speechBubbleHandlePosition = _position;
		m_speechBubbleHandleType = _type;
		m_speechBubbleHandler = true;
	}

	public override void DrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(m_TC.p_entity);
		Vector2[] array = DebugDraw.GetBezierRect(_c.m_actualWidth - m_cornerSize * 3f * (float)Screen.height, _c.m_actualHeight - m_cornerSize * 3f * (float)Screen.height, m_cornerSize * (float)Screen.height, 10, Vector2.zero);
		if (m_speechBubbleHandler)
		{
			array = DebugDraw.AddSpeechHandleToVectorArray(array, m_speechBubbleHandlePosition, m_speechBubbleHandleType);
		}
		if (array != null)
		{
			float num = m_cornerSize * (float)Screen.height * 2f;
			PrefabS.CreatePathPrefabComponentFromVectorArray(m_TC, Vector3.forward * -1f, array, (float)Screen.width * 0.005f, m_outlineBottomColor, m_outlineTopColor, ResourceManager.GetMaterial("Framework/Line4Mat"), m_camera, Position.Center, true);
			GGData polygon = new GGData(array);
			PrefabS.CreateFlatPrefabComponentsFromPolygon(m_TC, Vector3.zero, polygon, m_fillBottomColor, m_fillTopColor, ResourceManager.GetMaterial("Framework/SolidMat"), m_camera);
			DebugDraw.ScaleVectorArray(array, new Vector2(0.98f, 1f));
			PrefabS.CreatePathPrefabComponentFromVectorArray(m_TC, Vector3.forward + Vector3.down * 0.001f * Screen.height, array, (float)Screen.height * 0.02f, m_outlineColor, ResourceManager.GetMaterial("Framework/Line8Mat"), m_camera, Position.Center, true);
			Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("menu_button_glare");
			SpriteC spriteC = SpriteS.AddComponent(m_TC, frame, PsState.m_uiSheet);
			SpriteS.SetOffset(spriteC, new Vector3(0f, m_actualHeight * 0.5f - (float)Screen.height * 0.015f + m_glareYOffset * (float)Screen.height, -5f), 0f);
			SpriteS.SetDimensions(spriteC, m_actualWidth - num, (float)Screen.height * 0.03f);
			Frame frame2 = PsState.m_uiSheet.m_atlas.GetFrame("menu_button_shine");
			SpriteC spriteC2 = SpriteS.AddComponent(m_TC, frame2, PsState.m_uiSheet);
			SpriteS.SetOffset(spriteC2, new Vector3(m_actualWidth * -0.5f + (float)Screen.height * 0.0175f + m_topShineXOffset * (float)Screen.height, m_actualHeight * 0.5f - (float)Screen.height * 0.015f + m_topShineYOffset * (float)Screen.height, -6f), 25f);
			SpriteS.SetDimensions(spriteC2, (float)Screen.height * 0.02f, (float)Screen.height * 0.01f);
			SpriteC spriteC3 = SpriteS.AddComponent(m_TC, frame2, PsState.m_uiSheet);
			SpriteS.SetOffset(spriteC3, new Vector3(m_actualWidth * 0.5f - (float)Screen.height * 0.0175f + m_lowerShineXOffset * (float)Screen.height, m_actualHeight * -0.5f + (float)Screen.height * 0.015f + m_lowerShineYOffset * (float)Screen.height, -6f), 205f);
			SpriteS.SetDimensions(spriteC3, (float)Screen.height * 0.015f, (float)Screen.height * 0.0085f);
			SpriteS.SetColor(spriteC3, new Color(1f, 1f, 1f, 0.6f));
			SpriteS.ConvertSpritesToPrefabComponent(m_TC, m_camera, true);
		}
	}

	protected override void OnTouchRollIn(TLTouch _touch, bool _secondary)
	{
		if (!m_customTweens && !m_scaledUp)
		{
			if (m_touchScaleTween != null)
			{
				TweenS.RemoveComponent(m_touchScaleTween);
				m_touchScaleTween = null;
			}
			Hide();
			m_touchScaleTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.CubicInOut, m_currentScale + new Vector3(0.05f, 0.05f, 0.05f), 0.1f, 0f, false);
			TweenS.AddTweenEndEventListener(m_touchScaleTween, EnableDraw);
			m_currentScale += new Vector3(0.05f, 0.05f, 0.05f);
			m_scaledUp = true;
		}
		base.OnTouchRollIn(_touch, _secondary);
	}

	protected override void OnTouchBegan(TLTouch _touch)
	{
		if (!m_customTweens && !m_scaledUp)
		{
			if (m_touchScaleTween != null)
			{
				TweenS.RemoveComponent(m_touchScaleTween);
				m_touchScaleTween = null;
			}
			Hide();
			m_touchScaleTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.CubicInOut, m_currentScale + new Vector3(0.05f, 0.05f, 0.05f), 0.1f, 0f, false);
			TweenS.AddTweenEndEventListener(m_touchScaleTween, EnableDraw);
			m_currentScale += new Vector3(0.05f, 0.05f, 0.05f);
			m_scaledUp = true;
		}
		base.OnTouchBegan(_touch);
	}

	protected override void OnTouchRollOut(TLTouch _touch, bool _secondary)
	{
		if (!m_customTweens && m_scaledUp)
		{
			if (m_touchScaleTween != null)
			{
				TweenS.RemoveComponent(m_touchScaleTween);
				m_touchScaleTween = null;
			}
			Hide();
			m_touchScaleTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.CubicInOut, m_currentScale - new Vector3(0.05f, 0.05f, 0.05f), 0.1f, 0f, false);
			TweenS.AddTweenEndEventListener(m_touchScaleTween, EnableDraw);
			m_currentScale -= new Vector3(0.05f, 0.05f, 0.05f);
			m_scaledUp = false;
		}
		base.OnTouchRollOut(_touch, _secondary);
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		if (_inside || m_scaledUp)
		{
			if (_inside && (!m_preventHit || (m_preventHit && !_touch.m_dragged)) && m_sound != null)
			{
				SoundS.PlaySingleShot(m_sound, Vector3.zero);
			}
			if (!m_customRelease && !m_customTweens && m_scaledUp)
			{
				if (m_touchScaleTween != null)
				{
					TweenS.RemoveComponent(m_touchScaleTween);
					m_touchScaleTween = null;
				}
				Hide();
				m_touchScaleTween = TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.CubicInOut, m_currentScale - new Vector3(0.05f, 0.05f, 0.05f), 0.1f, 0f, false);
				TweenS.AddTweenEndEventListener(m_touchScaleTween, EnableDraw);
				m_currentScale -= new Vector3(0.05f, 0.05f, 0.05f);
				m_scaledUp = false;
			}
		}
		base.OnTouchRelease(_touch, _inside);
	}

	public void EnableDraw(TweenC _c)
	{
		Show();
		if (m_lateDraw && m_currentScale == Vector3.one)
		{
			d_Draw(this);
			m_lateDraw = false;
		}
	}
}
