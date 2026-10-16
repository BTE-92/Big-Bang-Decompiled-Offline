public class UIHeaderedCanvas : UICanvas
{
	protected UICanvas m_container;

	protected UICanvas m_header;

	public UIHeaderedCanvas(UIComponent _parent, string _tag = "", float _headerHeight = 0.125f, RelativeTo _headerHeightRelativeTo = RelativeTo.ScreenHeight)
		: base(_parent, false, _tag, null, string.Empty)
	{
		m_container = new UICanvas(_parent, false, _tag + "Container", null, string.Empty);
		m_container.SetWidth(1f, RelativeTo.ParentWidth);
		m_container.RemoveDrawHandler();
		m_header = new UICanvas(m_container, false, string.Empty, null, string.Empty);
		m_header.SetWidth(1f, RelativeTo.ParentWidth);
		m_header.SetHeight(_headerHeight, _headerHeightRelativeTo);
		m_header.SetVerticalAlign(1f);
		m_header.RemoveDrawHandler();
		base.Parent(m_container);
		base.SetWidth(1f, RelativeTo.ParentWidth);
		base.SetHeight(1f, RelativeTo.ParentHeight);
		base.SetVerticalAlign(0f);
		base.RemoveDrawHandler();
	}

	public override void Update()
	{
		float num = m_container.m_actualHeight - m_container.m_actualMargins.t - m_container.m_actualMargins.b;
		float num2 = 0f;
		if (m_header != null)
		{
			num2 = m_header.m_actualHeight;
		}
		base.SetHeight(1f - num2 / num, RelativeTo.ParentHeight);
		base.Update();
	}

	public new virtual void Parent(UIComponent _parent)
	{
		m_container.Parent(_parent);
	}

	public new virtual void DetachFromParent()
	{
		m_container.DetachFromParent();
	}

	public new virtual void DetachFromParent(bool _addToManager)
	{
		m_container.DetachFromParent(_addToManager);
	}

	public new virtual void Destroy()
	{
		m_container.Destroy();
	}

	public new virtual void SetSize(float _widthRatio, float _heightRatio, RelativeTo _relativeTo)
	{
		m_container.SetSize(_widthRatio, _heightRatio, _relativeTo);
	}

	public new virtual void SetWidth(float _widthRatio, RelativeTo _relativeTo)
	{
		m_container.SetWidth(_widthRatio, _relativeTo);
	}

	public new virtual void SetHeight(float _heightRatio, RelativeTo _relativeTo)
	{
		m_container.SetHeight(_heightRatio, _relativeTo);
	}

	public new virtual void SetAlign(float _horizontal, float _vertical)
	{
		m_container.SetAlign(_horizontal, _vertical);
	}

	public new virtual void SetHorizontalAlign(float _horizontal)
	{
		m_container.SetHorizontalAlign(_horizontal);
	}

	public new virtual void SetVerticalAlign(float _vertical)
	{
		m_container.SetVerticalAlign(_vertical);
	}

	public new virtual void SetDrawHandler(UIDrawDelegate _handler)
	{
		m_container.SetDrawHandler(_handler);
	}

	public new virtual void RemoveDrawHandler()
	{
		m_container.RemoveDrawHandler();
	}

	public new virtual void RemoveTouchAreas()
	{
		m_container.RemoveTouchAreas();
	}

	public new virtual void SetTouchHandler(TouchEventDelegate _handler)
	{
		m_container.SetTouchHandler(_handler);
	}
}
