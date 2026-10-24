using System.Linq;
using UnityEngine;

public class UIEditorSelectorNavigator : UIHorizontalList
{
	public static string[] WIZARD_CATEGORIES = new string[3] { "Vehicle", "Game Mode", "Dome Size" };

	public Color[] m_buttonColors;

	public PsUIGenericButton m_backButton;

	public UICanvas[] m_categoryButtons;

	public UITextButton[] m_parentCategoryButtons;

	private string[] m_categories;

	private EditorSelectorContext m_context;

	private UIFittedText m_notificationAmount;

	public UIEditorSelectorNavigator(UIComponent _parent, string _title, string _description, string _tag, EditorSelectorContext _context, bool _createBackButton, UIFittedText _notificationAmount)
		: base(_parent, _tag)
	{
		m_context = _context;
		m_notificationAmount = _notificationAmount;
		switch (_context)
		{
		case EditorSelectorContext.ITEM_SELECT:
		{
			int num = ((1 == 0) ? (PsMetagameData.m_units.Count - 1) : PsMetagameData.m_units.Count);
			m_categories = new string[num];
			m_buttonColors = new Color[3]
			{
				PsColors.objectCatButtonBlue,
				PsColors.objectCatButtonGreen,
				PsColors.objectCatButtonRed
			};
			m_buttonColors = new Color[6]
			{
				PsColors.objectCatButtonBlue,
				PsColors.objectCatButtonGreen,
				new Color(1f, 0.62f, 0.12f),
				PsColors.objectCatButtonRed,
				new Color(0.26f, 0.31f, 0.4f),
				new Color(1f, 0.62f, 0.12f)
			};
			for (int i = 0; i < m_categories.Length; i++)
			{
				PsUnlockable psUnlockable = PsMetagameData.m_units[i];
				m_categories[i] = psUnlockable.m_name;
			}
			break;
		}
		case EditorSelectorContext.FREE_WIZARD:
			m_categories = WIZARD_CATEGORIES;
			m_buttonColors = new Color[1] { PsColors.objectCatButtonGrey };
			break;
		}
		SetAlign(0f, 1f);
		RemoveDrawHandler();
		UIVerticalList uIVerticalList = new UIVerticalList(this, "leftColumn");
		uIVerticalList.SetVerticalAlign(1f);
		uIVerticalList.SetMargins(0.05f, 0f, 0f, 0f);
		uIVerticalList.SetWidth(0.2f, RelativeTo.ScreenWidth);
		uIVerticalList.RemoveDrawHandler();
		if (_createBackButton)
		{
			m_backButton = new PsUIGenericButton(uIVerticalList);
			m_backButton.SetText("Back ");
			m_backButton.SetHeight(0.075f, RelativeTo.ScreenHeight);
		}
		UIVerticalList uIVerticalList2 = new UIVerticalList(this, "rightColumn");
		uIVerticalList2.SetMargins(0.1f, 0f, 0.1f, 0f);
		uIVerticalList2.RemoveDrawHandler();
		UIText uIText = new UIText(uIVerticalList2, false, "Title", "<color=#" + ToolBox.ColorToHex(PsColors.menuLimeGreen) + ">" + _title.ToUpper() + "</color>", "KGSecondChances_Font", 0.07f, RelativeTo.ScreenHeight);
		uIText.SetHorizontalAlign(0f);
		if (_description != string.Empty)
		{
			UIText uIText2 = new UIText(uIVerticalList2, false, "Description", _description, "HurmeSemiBold_Font", 0.03f, RelativeTo.ScreenHeight);
			uIText2.SetHorizontalAlign(0f);
		}
		if (m_categories == null)
		{
			return;
		}
		UIHorizontalList uIHorizontalList = new UIHorizontalList(uIVerticalList2, "buttonList");
		uIHorizontalList.SetHorizontalAlign(0f);
		uIHorizontalList.SetMargins(-0.025f, 0f, 0.035f, 0f);
		uIHorizontalList.SetSpacing(0.01f, RelativeTo.ParentHeight);
		uIHorizontalList.RemoveDrawHandler();
		m_categoryButtons = new UICanvas[m_categories.Length];
		for (int j = 0; j < m_categoryButtons.Length; j++)
		{
			Color color = new Color(0.26f, 0.31f, 0.4f);
			bool highlight = false;
			if (m_context == EditorSelectorContext.ITEM_SELECT)
			{
				if (PsState.m_editorLastSelectedItemCategory == m_categories[j])
				{
					highlight = true;
				}
				if (j < m_buttonColors.Length)
				{
					color = m_buttonColors[j];
				}
			}
			else if (m_context == EditorSelectorContext.FREE_WIZARD)
			{
				if (PsState.m_freeWizardLastSelectedItemCategory == m_categories[j])
				{
					highlight = true;
				}
				color = m_buttonColors[0];
			}
			m_categoryButtons[j] = AddCategoryButton(uIHorizontalList, m_categories[j], highlight, color);
		}
	}

	public UICanvas AddCategoryButton(UIComponent _parent, string _title, bool _highlight, Color _color)
	{
		UICanvas uICanvas = new UICanvas(_parent, true, string.Empty, null, string.Empty);
		uICanvas.SetWidth(0.2f, RelativeTo.ScreenHeight);
		uICanvas.SetHeight(0.12f, RelativeTo.ScreenHeight);
		uICanvas.RemoveDrawHandler();
		if (_highlight)
		{
			new UIFittedSprite(uICanvas, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("editor_item_category_button_highlight"));
		}
		UIFittedSprite uIFittedSprite = new UIFittedSprite(uICanvas, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("editor_item_category_button"));
		uIFittedSprite.SetColor(Color.Lerp(_color, Color.black, 0.5f));
		uIFittedSprite.SetWidth(0.9f, RelativeTo.ParentWidth);
		uIFittedSprite.SetDepthOffset(-5f);
		UIFittedText uIFittedText = new UIFittedText(uIFittedSprite, false, string.Empty, _title.ToUpper(), "KGSecondChances_Font", true);
		uIFittedText.SetWidth(0.8f, RelativeTo.ParentWidth);
		uIFittedText.m_TC.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 4f));
		if (PsState.m_newEditorItemCount > 0)
		{
			int num = 0;
			for (int i = 0; i < PsMetagameData.m_units.Count; i++)
			{
				PsUnlockableCategory psUnlockableCategory = PsMetagameData.m_units[i];
				if (!(psUnlockableCategory.m_name == _title))
				{
					continue;
				}
				for (int j = 0; j < psUnlockableCategory.m_items.Count; j++)
				{
					if (PsState.m_newEditorItems.Contains(psUnlockableCategory.m_items[j].m_identifier))
					{
						num++;
					}
				}
				break;
			}
			if (num > 0)
			{
				UICanvas uICanvas2 = new UICanvas(uICanvas, false, "notification", null, string.Empty);
				uICanvas2.SetSize(0.04f, 0.04f, RelativeTo.ScreenHeight);
				uICanvas2.SetMargins(-0.005f, 0.005f, -0.005f, 0.005f);
				uICanvas2.SetAlign(0f, 1f);
				uICanvas2.SetDepthOffset(-10f);
				uICanvas2.RemoveDrawHandler();
				UICanvas uICanvas3 = new UICanvas(uICanvas2, false, string.Empty, null, string.Empty);
				uICanvas3.SetSize(1f, 1f, RelativeTo.ParentHeight);
				uICanvas3.SetMargins(0.15f, RelativeTo.OwnHeight);
				uICanvas3.SetDrawHandler(PsUIDrawHandlers.NotificationDrawhandler);
				TweenC c = TweenS.AddTransformTween(uICanvas3.m_TC, TweenedProperty.Scale, TweenStyle.CubicInOut, new Vector3(1.1f, 1.1f, 1.1f), 0.5f, 0f, false);
				TweenS.SetAdditionalTweenProperties(c, -1, true, TweenStyle.CubicInOut);
				UIFittedText uIFittedText2 = new UIFittedText(uICanvas3, false, string.Empty, num.ToString(), "KGSecondChances_Font", true);
			}
		}
		return uICanvas;
	}

	public override void Step()
	{
		if (m_backButton != null && m_backButton.m_hit)
		{
			SoundS.PlaySingleShot("/UI/ButtonBack", Vector3.zero);
			if (m_context == EditorSelectorContext.START_WIZARD)
			{
				Main.m_currentGame.m_sceneManager.ChangeScene(new PsMenuScene("MenuScene"), new FadeLoadingScene(Color.black));
			}
			else
			{
				Main.m_currentGame.m_sceneManager.m_currentScene.m_stateMachine.ChangeState(new EditorBaseState());
			}
		}
		if (m_categoryButtons != null)
		{
			for (int i = 0; i < m_categoryButtons.Length; i++)
			{
				if (m_categoryButtons[i].m_hit)
				{
					if (m_context == EditorSelectorContext.ITEM_SELECT)
					{
						OpenItemCategory(m_categories[i]);
					}
					else if (m_context == EditorSelectorContext.FREE_WIZARD)
					{
						OpenWizardCategory(m_categories[i]);
					}
				}
			}
		}
		base.Step();
	}

	public void OpenItemCategory(string _category)
	{
		PsState.m_editorLastSelectedItemCategory = _category;
		Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorSelectorItem(_category, m_notificationAmount));
	}

	public void OpenWizardCategory(string _category)
	{
		PsState.m_freeWizardLastSelectedItemCategory = _category;
		if (_category == WIZARD_CATEGORIES[0])
		{
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorSelectorPlayer(m_context));
		}
		else if (_category == WIZARD_CATEGORIES[1])
		{
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorSelectorGameMode(m_context));
		}
		else if (_category == WIZARD_CATEGORIES[2])
		{
			Main.m_currentGame.m_currentScene.m_stateMachine.ChangeState(new EditorSelectorArea(m_context));
		}
	}
}
