using UnityEngine;

public class PsUIBoosterButton : UICanvas
{
	private UIFittedSprite m_boostIcon;

	public UISprite m_button;

	private UISprite m_icon;

	private PsUIGenericButton m_shopButton;

	private PsUIBasePopup m_shop;

	private UIText m_boosterCount;

	private bool m_createTutorialArrow;

	private PsUITutorialArrowUI m_tutorialArrow;

	private string m_defaultShaderName = "Framework/VertexColorUnlitDouble";

	private string m_greyScaleShaderName = "WOE/Fx/GreyscaleUnlitAlpha";

	public PsUIBoosterButton(UIComponent _parent)
		: base(_parent, false, "Booster Button Continer", null, string.Empty)
	{
		int boosterValue = PsMetagameManager.m_playerStats.GetBoosterValue();
		SetSize(0.18f, 0.18f, RelativeTo.ScreenHeight);
		RemoveDrawHandler();
		Frame frame = PsState.m_uiSheet.m_atlas.GetFrame("controller_boost_button");
		m_button = new UISprite(this, true, "BoosterButton", PsState.m_uiSheet, frame);
		m_button.SetWidth(0.9f, RelativeTo.ParentWidth);
		m_button.SetHeight(frame.height / frame.width, RelativeTo.OwnWidth);
		m_button.SetAlign(1f, 0f);
		m_button.m_TAC.m_cancelOtherTouches = false;
		Frame frame2 = PsState.m_uiSheet.m_atlas.GetFrame("menu_timer_background");
		UISprite uISprite = new UISprite(m_button, false, "BoosterButtonCountBackground", PsState.m_uiSheet, frame2);
		uISprite.SetWidth(0.7f, RelativeTo.ParentWidth);
		uISprite.SetHeight(0.25f, RelativeTo.ParentWidth);
		uISprite.SetMargins(0.3f, 0.06f, 0f, 0f, RelativeTo.ParentWidth);
		uISprite.SetDepthOffset(-1f);
		uISprite.SetAlign(1f, 0.3f);
		m_boosterCount = new UIText(uISprite, false, "BoosterCount", string.Empty + boosterValue, "HurmeSemiBold_Font", 0.3f, RelativeTo.ParentWidth);
		m_boosterCount.SetHorizontalAlign(1f);
		Frame frame3 = PsState.m_uiSheet.m_atlas.GetFrame("hud_boost_boost");
		m_icon = new UISprite(m_button, false, "BoosterButtonIcon", PsState.m_uiSheet, frame3);
		m_icon.SetWidth(1f, RelativeTo.ParentWidth);
		m_icon.SetHeight(frame3.height / frame3.width, RelativeTo.OwnWidth);
		m_icon.SetDepthOffset(-2f);
		if (!PsState.m_activeMinigame.m_gameStarted)
		{
			m_createTutorialArrow = PsState.m_activeGameLoop is PsGameLoopBlockLevel && (PsState.m_activeGameLoop as PsGameLoopBlockLevel).m_unlockedProgression.Equals("unlock_nitros") && boosterValue == 0 && PsMetagameManager.m_tutorialKey == "Booster";
			m_button.DisableTouchAreas();
			if (boosterValue == 0)
			{
				GreyScaleOn();
			}
			m_shopButton = new PsUIGenericButton(this);
			m_shopButton.SetIcon("hud_icon_plus", 0.04f);
			m_shopButton.SetMargins(0.01f);
			m_shopButton.SetTealColors();
			m_shopButton.SetHeight(1f, RelativeTo.OwnWidth);
			m_shopButton.SetAlign(0f, 1f);
			m_shopButton.SetDepthOffset(-3f);
		}
		else if (boosterValue == 0)
		{
			GreyScaleOn();
			m_button.DisableTouchAreas();
		}
		CreateBooster();
	}

	public virtual void CreateBooster()
	{
		Vehicle vehicle = PsState.m_activeMinigame.m_playerUnit as Vehicle;
		if (vehicle != null)
		{
			if (vehicle.m_booster == null)
			{
				bool freeBooster = PsState.m_activeGameLoop.m_freeConsumableUnlock == "Booster";
				CreateBooster(vehicle, freeBooster);
			}
			vehicle.m_booster.SetBoosterButton(this);
		}
	}

	public virtual void CreateBooster(Vehicle _vehicle, bool _freeBooster)
	{
		_vehicle.m_booster = new SpeedBooster(_freeBooster);
	}

	public override void Step()
	{
		if (m_createTutorialArrow)
		{
			if (m_tutorialArrow == null && m_shopButton != null)
			{
				m_tutorialArrow = new PsUITutorialArrowUI(m_shopButton);
				UnityUIManager.DisableEventSystem();
			}
			m_createTutorialArrow = false;
		}
		if (m_shopButton != null && m_shopButton.m_hit && m_shop == null)
		{
			LastResourceView lastView = PsMetagameManager.m_menuResourceView.SetLastView();
			m_shop = new PsUIBasePopup(typeof(PsUICenterShopBoosters));
			m_shop.SetAction("Exit", delegate
			{
				UnityUIManager.EnableEventSystem();
				m_shop.Destroy();
				m_shop = null;
				PsMetagameManager.m_menuResourceView.ShowLastView(lastView);
				int boosterValue = PsMetagameManager.m_playerStats.GetBoosterValue();
				m_boosterCount.SetText(string.Empty + boosterValue);
				m_boosterCount.Update();
				if (boosterValue > 0)
				{
					GreyScaleOff();
				}
				PsIngameMenu.OpenController(true);
				if (PsIngameMenu.m_controller.m_boostButton != null)
				{
					PsIngameMenu.m_controller.m_boostButton.m_createTutorialArrow = false;
				}
			});
		}
		base.Step();
	}

	public virtual void GreyScaleOn()
	{
		if (m_button.m_rect != null)
		{
			m_button.m_rect.p_gameObject.GetComponent<Renderer>().material.shader = Shader.Find(m_greyScaleShaderName);
		}
		if (m_icon.m_rect != null)
		{
			m_icon.m_rect.p_gameObject.GetComponent<Renderer>().material.shader = Shader.Find(m_greyScaleShaderName);
		}
	}

	public virtual void GreyScaleOff()
	{
		if (m_button.m_rect != null)
		{
			m_button.m_rect.p_gameObject.GetComponent<Renderer>().material.shader = Shader.Find(m_defaultShaderName);
		}
		if (m_icon.m_rect != null)
		{
			m_icon.m_rect.p_gameObject.GetComponent<Renderer>().material.shader = Shader.Find(m_defaultShaderName);
		}
	}

	public void UseBoost()
	{
		GreyScaleOn();
		RemoveBoost();
	}

	public void RemoveBoost()
	{
		TweenS.AddTransformTween(m_TC, TweenedProperty.Scale, TweenStyle.CubicIn, Vector3.zero, 0.2f, 0f, true);
	}

	public void Disable()
	{
		GreyScaleOn();
	}
}
