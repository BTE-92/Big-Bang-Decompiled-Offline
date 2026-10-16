using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class PsUIPopup : UIScrollableCanvas
{
	private Action m_PROCEED;

	private Action m_CANCEL;

	private bool m_hideUI;

	private List<Entity> m_hiddenEntities;

	public PsUIPopup(Action _proceed = null, Action _cancel = null, bool _hideUI = false, string _tag = "Popup")
		: base(null, _tag)
	{
		m_hideUI = _hideUI;
		if (m_hideUI)
		{
			m_hiddenEntities = EntityManager.SetVisibilityOfEntitiesWithTag("UIComponent", false);
			PsState.m_UIComponentsHidden = true;
		}
		SetWidth(1f, RelativeTo.ScreenWidth);
		SetHeight(1f, RelativeTo.ScreenHeight);
		SetMargins(0.25f, 0.25f, 0.05f, 0.05f);
		SetDepthOffset(20f);
		m_PROCEED = _proceed;
		m_CANCEL = _cancel;
		SetDrawHandler(UIDrawHandlers.MenuPopupBackground);
		PsState.m_openPopups.Add(this);
	}

	public void UpdatePopupBG()
	{
	}

	public void Proceed()
	{
		if (m_PROCEED != null)
		{
			m_PROCEED();
		}
	}

	public void Cancel()
	{
		if (m_CANCEL != null)
		{
			m_CANCEL();
		}
	}

	public virtual void ToggleUI(bool _visible)
	{
		for (int i = 0; i < m_hiddenEntities.Count; i++)
		{
			if (m_hiddenEntities[i] != null)
			{
				EntityManager.SetVisibilityOfEntity(m_hiddenEntities[i], _visible);
			}
		}
		PsState.m_UIComponentsHidden = !_visible;
	}

	public override void Destroy()
	{
		PsState.m_openPopups.Remove(this);
		base.Destroy();
		if (m_hideUI)
		{
			ToggleUI(true);
		}
	}

	public static void PopupDrawHandler(UIComponent _c)
	{
		PrefabS.RemoveComponentsByEntity(_c.m_TC.p_entity);
		Material material = ResourceManager.GetMaterial(RESOURCE.MenuMetalMat_Material);
		string hex = "008ce5";
		string hex2 = "0061a3";
		Vector2[] roundedRect = DebugDraw.GetRoundedRect(_c.m_actualWidth, _c.m_actualHeight, (float)Screen.height * 0.025f, 2, Vector2.zero);
		PrefabS.CreateFlatPrefabComponentsFromVectorArray(_c.m_TC, Vector3.zero, roundedRect, DebugDraw.HexToUint(hex), DebugDraw.HexToUint(hex2), material, _c.m_camera, "BG");
	}
}
