using UnityEngine;

public class PsUIVersusForfeitPopup : PsUIHeaderedCanvas
{
	private PsUIGenericButton m_cancel;

	private PsUIGenericButton m_forfeit;

	private VersusMetaData m_versusData;

	private string m_contentText;

	private string m_cancelText;

	private string m_forfeitText;

	private PsGameLoopVersus m_loop;

	private PsGameModeRaceVersus m_mode;

	public PsUIVersusForfeitPopup(UIComponent _parent)
		: base(_parent, string.Empty)
	{
		SetWidth(0.65f, RelativeTo.ScreenWidth);
		SetHeight(0.45f, RelativeTo.ScreenHeight);
		SetVerticalAlign(0.4f);
		SetMargins(0.0125f, 0.0125f, 0f, 0.0125f);
		SetDrawHandler(PsUIDrawHandlers.ScrollingUIBackground);
		m_header.SetDrawHandler(PsUIDrawHandlers.ScrollingUIHeader);
		m_header.SetMargins(0.0125f, 0.0125f, 0.0125f, 0f);
		CreateContent(this);
		CreateHeaderContent(m_header);
	}

	public void Initialize()
	{
		m_loop = PsState.m_activeGameLoop as PsGameLoopVersus;
		m_mode = m_loop.m_gameMode as PsGameModeRaceVersus;
		m_versusData = m_loop.m_versusData;
		m_contentText = "Are you 100 000% sure you want to give up? You could still upgrade your Vehicles or Boost your way to the trophy.";
		m_cancelText = "Cancel";
		m_forfeitText = "Give up";
	}

	public void CreateHeaderContent(UIComponent _parent)
	{
		UICanvas uICanvas = new UICanvas(_parent, false, string.Empty, null, string.Empty);
		uICanvas.SetHeight(1f, RelativeTo.ParentHeight);
		uICanvas.SetWidth(1f, RelativeTo.ParentHeight);
		uICanvas.SetHorizontalAlign(0f);
		uICanvas.SetDepthOffset(-10f);
		uICanvas.SetMargins(-0.075f, 0.05f, -0.025f, 0f);
		uICanvas.RemoveDrawHandler();
		UIFittedSprite uIFittedSprite = new UIFittedSprite(uICanvas, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("hud_icon_trophy_full"));
		uIFittedSprite.SetHeight(1.35f, RelativeTo.ParentHeight);
		TransformS.SetRotation(uIFittedSprite.m_TC, new Vector3(0f, 0f, 3.5f));
		UIHorizontalList uIHorizontalList = new UIHorizontalList(_parent, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0.025f, 0.025f, 0f, 0f);
		uIHorizontalList.SetHorizontalAlign(0.5f);
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, "Are you sure?!", "KGSecondChances_Font", 0.055f, RelativeTo.ScreenHeight, "#95e5ff");
	}

	public void CreateContent(UIComponent _parent)
	{
		Initialize();
		_parent.RemoveTouchAreas();
		UITextbox uITextbox = new UITextbox(_parent, false, string.Empty, m_contentText, "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, false, Align.Center, Align.Middle, "#aff6f");
		UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
		uIHorizontalList.RemoveDrawHandler();
		uIHorizontalList.SetAlign(0.5f, 0f);
		uIHorizontalList.SetSpacing(0.1f, RelativeTo.ScreenHeight);
		uIHorizontalList.SetMargins(0f, 0f, 0.075f, -0.075f);
		m_forfeit = new PsUIGenericButton(uIHorizontalList);
		m_forfeit.SetText(m_forfeitText);
		m_forfeit.SetRedColors();
		if (m_loop.m_tryCount < PsState.m_versusChallengeTryAmount)
		{
			m_cancel = new PsUIGenericButton(uIHorizontalList);
			m_cancel.SetOrangeColors();
			m_cancel.SetText(m_cancelText);
		}
	}

	public override void Step()
	{
		if (m_forfeit != null && m_forfeit.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Forfeit");
		}
		else if (m_cancel != null && m_cancel.m_hit)
		{
			(GetRoot() as PsUIBasePopup).CallAction("Cancel");
		}
		base.Step();
	}
}
