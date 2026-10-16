using UnityEngine;

public class PsUIItemCardButton : PsUIGenericButton
{
	public PsUnlockable m_unlockable;

	public UITextbox m_descriptionTextBox;

	public UITextbox m_labelText;

	public PsUIItemCardButton(UIComponent _parent, PsUnlockable _unlockable)
		: base(_parent)
	{
		m_unlockable = _unlockable;
		SetItemCardButton(m_unlockable.m_unlocked);
	}

	public void SetItemCardButton(bool _unlocked)
	{
		m_shopBase = new UIVerticalList(this, "ItemCard");
		m_shopBase.SetWidth(0.3f, RelativeTo.ScreenHeight);
		m_shopBase.SetVerticalAlign(1f);
		m_shopBase.RemoveDrawHandler();
		UICanvas uICanvas = new UICanvas(m_shopBase, false, string.Empty, null, string.Empty);
		uICanvas.SetWidth(1f, RelativeTo.ParentWidth);
		uICanvas.SetHeight(0.3f, RelativeTo.ParentWidth);
		uICanvas.RemoveDrawHandler();
		string text = ((!_unlocked) ? "LOCKED" : ("<color=#" + ToolBox.ColorToHex(PsColors.menuLimeGreen) + "> " + m_unlockable.m_name + " </color>"));
		m_labelText = new UITextbox(uICanvas, false, string.Empty, text, "KGSecondChances_Font", 0.035f, RelativeTo.ScreenHeight, false, Align.Center, Align.Middle);
		m_labelText.SetWidth(1f, RelativeTo.ParentWidth);
		UICanvas uICanvas2 = new UICanvas(m_shopBase, false, string.Empty, null, string.Empty);
		uICanvas2.SetWidth(1f, RelativeTo.ParentWidth);
		uICanvas2.SetHeight(0.6f, RelativeTo.ParentWidth);
		uICanvas2.SetMargins(0f, 0f, 0f, 0f, RelativeTo.ParentWidth);
		uICanvas2.RemoveDrawHandler();
		m_UIsprite = new UIFittedSprite(uICanvas2, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(m_unlockable.m_iconImage));
		m_UIsprite.SetHeight(1f, RelativeTo.ParentHeight);
		if (!_unlocked)
		{
			m_UIsprite.m_overrideShader = Shader.Find("WOE/Editor/ItemCardIconLocked");
		}
		UICanvas uICanvas3 = new UICanvas(m_shopBase, false, string.Empty, null, string.Empty);
		uICanvas3.SetWidth(0.8f, RelativeTo.ParentWidth);
		uICanvas3.SetHeight(0.3f, RelativeTo.ParentWidth);
		uICanvas3.SetDepthOffset(-20f);
		uICanvas3.SetMargins(0f, 0f, 0f, 0f, RelativeTo.ParentWidth);
		uICanvas3.RemoveDrawHandler();
		string text2 = ((!_unlocked) ? string.Empty : m_unlockable.m_description);
		m_descriptionTextBox = new UITextbox(uICanvas3, false, string.Empty, text2, "HurmeSemiBold_Font", 0.025f, RelativeTo.ScreenHeight, false, Align.Center);
		m_descriptionTextBox.SetMaxRows(3);
	}

	public void LockItemCard()
	{
		HideDescription();
		LockImage();
		LockLabel();
	}

	public void UnlockItemCard()
	{
		ShowDescription();
		UnlockImage();
		UnlockLabel();
	}

	public void ShowDescription()
	{
		EntityManager.SetVisibilityOfEntity(m_descriptionTextBox.m_TC.p_entity, true);
	}

	public void HideDescription()
	{
		EntityManager.SetVisibilityOfEntity(m_descriptionTextBox.m_TC.p_entity, false);
	}

	public void LockImage()
	{
		m_UIsprite.m_overrideShader = Shader.Find("WOE/Editor/ItemCardIconLocked");
	}

	public void UnlockImage()
	{
		m_UIsprite.m_rect.p_gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Framework/VertexColorUnlitDouble");
	}

	public void LockLabel()
	{
		string text = "LOCKED";
		m_labelText.SetText(text);
	}

	public void UnlockLabel()
	{
		string text = "<color=#" + ToolBox.ColorToHex(PsColors.menuLimeGreen) + "> " + m_unlockable.m_name + " </color>";
		m_labelText.SetText(text);
	}
}
