using UnityEngine;

public class UIBoolPropertyButton : UIVerticalListButton
{
	protected string[] m_valueLabels;

	public float m_minValue;

	public float m_maxValue;

	protected bool m_tempVal;

	protected float m_currentFloatValue;

	protected bool m_changeValue;

	protected bool m_changeValueByDragging;

	protected bool m_valueChangedByDragging;

	public UIText m_label;

	public UIText m_value;

	private int m_arrayIndex;

	public UIBoolPropertyButton(UIComponent _parent, string _tag, string _label, UIModel _model, string _BOOLfieldName, string[] _valueLabels, int _arrayIndex = -1)
		: base(_parent, null, null, _tag, _model, _BOOLfieldName)
	{
		m_minValue = 0f;
		m_maxValue = 1f;
		m_valueLabels = _valueLabels;
		m_arrayIndex = _arrayIndex;
		SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
		SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
		SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
		if (m_arrayIndex == -1)
		{
			m_currentFloatValue = ((!(bool)GetValue()) ? 0.4f : 0.6f);
		}
		else
		{
			m_currentFloatValue = ((!((bool[])GetValue())[m_arrayIndex]) ? 0.4f : 0.6f);
		}
		int num = Mathf.RoundToInt(m_currentFloatValue);
		string text = m_valueLabels[num];
		m_label = new UIText(this, false, _tag, _label, "HurmeRegular_Font", 0.02f, RelativeTo.ScreenHeight);
		m_label.SetAlign(0f, 1f);
		m_value = new UIText(this, false, _tag, text, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight);
		m_value.SetAlign(1f, 0f);
	}

	protected override void OnTouchBegan(TLTouch _touch)
	{
		base.OnTouchBegan(_touch);
		m_changeValue = true;
		m_changeValueByDragging = false;
		m_valueChangedByDragging = false;
	}

	protected override void OnTouchDragStart(TLTouch _touch)
	{
		base.OnTouchDragStart(_touch);
		Vector2 vector = _touch.m_currentPosition - _touch.m_startPosition;
		if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
		{
			FreezeVerticalScroll(false);
			m_changeValueByDragging = true;
			if (m_arrayIndex == -1)
			{
				m_tempVal = (bool)GetValue();
			}
			else
			{
				m_tempVal = ((bool[])GetValue())[m_arrayIndex];
			}
			m_currentFloatValue = ((!m_tempVal) ? 0.4f : 0.6f);
		}
		else
		{
			m_changeValue = false;
		}
	}

	protected override void OnTouchMove(TLTouch _touch, bool _inside)
	{
		base.OnTouchMove(_touch, _inside);
		if (m_changeValueByDragging)
		{
			bool flag = ((m_arrayIndex != -1) ? ((bool[])GetValue())[m_arrayIndex] : ((bool)GetValue()));
			float x = _touch.m_deltaPosition.x;
			float currentFloatValue = m_currentFloatValue;
			float num = Mathf.Max(m_minValue, Mathf.Min(m_maxValue, currentFloatValue + x / (Mathf.Max(m_actualWidth, m_maxValue - m_minValue) / (m_maxValue - m_minValue))));
			m_currentFloatValue = Mathf.Max(0.4f, Mathf.Min(0.6f, num));
			int num2 = Mathf.RoundToInt(num);
			m_tempVal = num2 == 1;
			m_value.SetText(m_valueLabels[num2]);
			if (flag != m_tempVal)
			{
				m_valueChangedByDragging = true;
			}
		}
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		base.OnTouchRelease(_touch, _inside);
		if (!_touch.m_dragged && m_changeValue && !m_valueChangedByDragging)
		{
			bool flag;
			if (m_arrayIndex == -1)
			{
				flag = (bool)GetValue();
				flag = !flag;
				SetValue(flag);
			}
			else
			{
				bool[] array = (bool[])GetValue();
				flag = array[m_arrayIndex];
				flag = !flag;
				array[m_arrayIndex] = flag;
				SetValue(array);
			}
			int num = (flag ? 1 : 0);
			m_value.SetText(m_valueLabels[num]);
		}
		else if (m_valueChangedByDragging)
		{
			if (m_arrayIndex == -1)
			{
				SetValue(m_tempVal);
			}
			else
			{
				bool[] array2 = (bool[])GetValue();
				array2[m_arrayIndex] = m_tempVal;
				SetValue(array2);
			}
		}
		UnfreezeVerticalScroll(false);
		m_changeValue = false;
		m_changeValueByDragging = false;
		m_valueChangedByDragging = false;
	}

	public override void OnValueChange(object _newValue, object _oldValue)
	{
		if (m_arrayIndex == -1)
		{
			int num = (bool.Parse(_newValue.ToString()) ? 1 : 0);
			m_value.SetText(m_valueLabels[num]);
		}
		else
		{
			int num2 = (((bool[])GetValue())[m_arrayIndex] ? 1 : 0);
			m_value.SetText(m_valueLabels[num2]);
		}
	}
}
