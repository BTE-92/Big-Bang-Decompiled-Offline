using System;
using System.Collections;
using UnityEngine;

public class UIHashtableArrayPropertyButton : UIVerticalListButton
{
	private class UIArrayLengthButton : UIVerticalListButton
	{
		protected int m_tempVal;

		public float m_minValue;

		public float m_maxValue;

		protected float m_currentFloatValue;

		protected bool m_changeValue;

		private string m_key;

		public UIText m_label;

		public UIText m_value;

		private int m_currentLength;

		public UIArrayLengthButton(UIComponent _parent, string _tag, string _label, UIModel _model, string _fieldName, string _key, int _minValue = 0, int _maxValue = 99999)
			: base(_parent, null, null, _tag, _model, _fieldName)
		{
			m_minValue = _minValue;
			m_maxValue = _maxValue;
			SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
			SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
			SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
			m_key = _key;
			Hashtable hashtable = (Hashtable)GetValue();
			object[] array = (object[])hashtable[m_key];
			m_currentLength = array.Length;
			string text = m_currentLength.ToString();
			m_label = new UIText(this, false, _tag, _label, "HurmeRegular_Font", 0.02f, RelativeTo.ScreenHeight);
			m_label.SetAlign(0f, 1f);
			m_value = new UIText(this, false, _tag, text, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight);
			m_value.SetAlign(1f, 0f);
		}

		protected override void OnTouchRelease(TLTouch _touch, bool _inside)
		{
			base.OnTouchRelease(_touch, _inside);
			if (!_touch.m_dragged)
			{
				UITextInput uITextInput = new UITextInput(m_label.m_text, DoneCallback, CancelCallback, KeyPressedCallback, TouchScreenKeyboardType.NumbersAndPunctuation, false, 64);
				uITextInput.SetText(m_currentLength.ToString());
				uITextInput.Update();
			}
			UnfreezeVerticalScroll(false);
			m_changeValue = false;
		}

		public override void OnValueChange(object _newValue, object _oldValue)
		{
			Hashtable hashtable = (Hashtable)GetValue();
			object[] array = (object[])hashtable[m_key];
			m_currentLength = array.Length;
			m_value.SetText(m_currentLength.ToString());
		}

		private void DoneCallback(string _value)
		{
			Debug.Log("DONE: " + _value);
			Hashtable hashtable = (Hashtable)GetValue();
			Hashtable hashtable2 = new Hashtable();
			foreach (string key in hashtable.Keys)
			{
				object[] array = (object[])hashtable[key];
				object[] array2 = new object[Convert.ToInt32(_value)];
				Array.Copy(array, array2, (array.Length <= array2.Length) ? array.Length : array2.Length);
				hashtable2.Add(key, array2);
			}
			SetValue(hashtable2);
			m_currentLength = Convert.ToInt32(_value);
			m_value.SetText(_value);
		}

		private void KeyPressedCallback(string _value)
		{
			Debug.Log("PRESSED: " + _value);
		}

		private void CancelCallback()
		{
			Debug.Log("CANCEL");
		}
	}

	private class UIStringArrayEditorButton : UIVerticalListButton
	{
		public UIText m_label;

		public UIText m_value;

		public UIScrollableCanvas scrollableCanvas;

		public PsUIGenericButton m_backButton;

		private string m_key;

		private object[] m_array;

		public UIStringArrayEditorButton(UIComponent _parent, UIScrollableCanvas _scrollableCanvas, UIPagedCanvas _pagedCanvas, string _tag, string _label, UIModel _model, string _fieldName, string _key)
			: base(_parent, _scrollableCanvas, _pagedCanvas, _tag, _model, _fieldName)
		{
			SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
			SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
			SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
			m_key = _key;
			string text = "0";
			Hashtable hashtable = (Hashtable)GetValue();
			m_array = (object[])hashtable[m_key];
			for (int i = 0; i < m_array.Length; i++)
			{
				text = ((i != 0) ? (text + ", " + (string)m_array.GetValue(i)) : ((string)m_array.GetValue(i)));
			}
			m_label = new UIText(this, false, _tag, _label, "HurmeRegular_Font", 0.02f, RelativeTo.ScreenHeight);
			m_label.SetAlign(0f, 1f);
			m_value = new UIText(this, false, _tag, text, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight);
			m_value.SetAlign(1f, 0f);
		}

		public override void OnValueChange(object _newValue, object _oldValue)
		{
			Debug.LogWarning("VALUE CHANGE ARRAY BUTTON");
			string text = "0";
			Hashtable hashtable = (Hashtable)GetValue();
			m_array = (object[])hashtable[m_key];
			for (int i = 0; i < m_array.Length; i++)
			{
				text = ((i != 0) ? (text + ", " + (string)m_array.GetValue(i)) : ((string)m_array.GetValue(i)));
			}
			m_value.SetText(text);
		}

		protected override void OnTouchRelease(TLTouch _touch, bool _inside)
		{
			if (m_pagedCanvas.IsChangingPage())
			{
				return;
			}
			base.OnTouchRelease(_touch, _inside);
			if (_inside)
			{
				scrollableCanvas = new UIScrollableCanvas(m_scrollableCanvas.m_parent, string.Empty);
				scrollableCanvas.SetWidth(1f, RelativeTo.ParentWidth);
				scrollableCanvas.SetHeight(1f, RelativeTo.ParentHeight);
				UIVerticalList uIVerticalList = new UIVerticalList(scrollableCanvas, "VerticalArea");
				uIVerticalList.SetVerticalAlign(1f);
				UIText uIText = new UIText(uIVerticalList, false, string.Empty, m_label.m_text, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight, "#000000");
				for (int i = 0; i < m_array.Length; i++)
				{
					new UIHashtableStringArrayEditButton(uIVerticalList, string.Empty, m_model, m_fieldName, m_key, i);
				}
				m_backButton = new PsUIGenericButton(uIVerticalList);
				m_backButton.SetText("Back");
				m_backButton.SetOrangeColors();
				m_pagedCanvas.NextPage();
				m_pagedCanvas.Update();
			}
		}

		public override void Step()
		{
			if (m_backButton != null && m_backButton.m_hit)
			{
				scrollableCanvas.Destroy();
				scrollableCanvas = null;
				m_pagedCanvas.PreviousPage();
				m_pagedCanvas.Update();
			}
			base.Step();
		}
	}

	private class UIHashtableStringArrayEditButton : UIVerticalListButton
	{
		public UIText m_label;

		public UIText m_value;

		public int m_arrayTargetIndex;

		private object[] m_array;

		private string m_key;

		public UIHashtableStringArrayEditButton(UIComponent _parent, string _tag, UIModel _model, string _fieldName, string _key, int _arrayTargetIndex)
			: base(_parent, null, null, _tag, _model, _fieldName)
		{
			SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
			SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
			SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
			m_arrayTargetIndex = _arrayTargetIndex;
			m_key = _key;
			string text = "0";
			Hashtable hashtable = (Hashtable)GetValue();
			m_array = (object[])hashtable[m_key];
			if (m_array[_arrayTargetIndex] != null)
			{
				text = m_array[_arrayTargetIndex].ToString();
			}
			m_label = new UIText(this, false, _tag, "[" + _arrayTargetIndex + "]", "HurmeRegular_Font", 0.02f, RelativeTo.ScreenHeight);
			m_label.SetAlign(0f, 1f);
			m_value = new UIText(this, false, _tag, text, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight);
			m_value.SetAlign(1f, 0f);
		}

		protected override void OnTouchRelease(TLTouch _touch, bool _inside)
		{
			base.OnTouchRelease(_touch, _inside);
			if (_inside)
			{
				UITextInput uITextInput = new UITextInput(m_label.m_text, DoneCallback, CancelCallback, KeyPressedCallback, TouchScreenKeyboardType.NumbersAndPunctuation, false, 64);
				string text = "0";
				Hashtable hashtable = (Hashtable)GetValue();
				m_array = (object[])hashtable[m_key];
				if (m_array[m_arrayTargetIndex] != null)
				{
					text = m_array[m_arrayTargetIndex].ToString();
				}
				uITextInput.SetText(text);
				uITextInput.Update();
			}
		}

		public override void OnValueChange(object _newValue, object _oldValue)
		{
			Debug.LogWarning("VALUE CHANGE EDIT BUTTON");
			string text = "0";
			Hashtable hashtable = (Hashtable)GetValue();
			m_array = (object[])hashtable[m_key];
			if (m_array[m_arrayTargetIndex] != null)
			{
				text = m_array[m_arrayTargetIndex].ToString();
			}
			m_value.SetText(text);
		}

		private void DoneCallback(string _value)
		{
			Debug.Log("DONE: " + _value);
			Hashtable hashtable = (Hashtable)GetValue();
			m_array = (object[])hashtable[m_key];
			m_array[m_arrayTargetIndex] = _value;
			hashtable[m_key] = m_array;
			SetValue(hashtable);
			m_value.SetText(_value);
		}

		private void KeyPressedCallback(string _value)
		{
			Debug.Log("PRESSED: " + _value);
		}

		private void CancelCallback()
		{
			Debug.Log("CANCEL");
		}
	}

	public UIText m_label;

	public UIText m_value;

	public UIScrollableCanvas scrollableCanvas;

	public PsUIGenericButton m_backButton;

	public int m_length;

	private string m_key;

	public UIHashtableArrayPropertyButton(UIComponent _parent, UIScrollableCanvas _scrollableCanvas, UIPagedCanvas _pagedCanvas, string _tag, string _label, UIModel _model, string _fieldName, string _key)
		: base(_parent, _scrollableCanvas, _pagedCanvas, _tag, _model, _fieldName)
	{
		SetWidth(UIVerticalListButton.m_defaultWidth, UIVerticalListButton.m_defaultWidthRelativeTo);
		SetHeight(UIVerticalListButton.m_defaultHeight, UIVerticalListButton.m_defaultHeightRelativeTo);
		SetMargins(UIVerticalListButton.m_defaultMargins, UIVerticalListButton.m_defaultMarginsRelativeTo);
		m_key = _key;
		Hashtable hashtable = (Hashtable)GetValue();
		Debug.LogWarning("UPGRADES: " + hashtable);
		Debug.LogWarning("ARRAY: " + hashtable[m_key]);
		string text = "-";
		int num = 0;
		object[] array = hashtable[m_key] as object[];
		m_length = array.Length;
		foreach (string key in hashtable.Keys)
		{
			text = ((num != 0) ? (text + ", " + key) : key);
			num++;
		}
		m_label = new UIText(this, false, _tag, _label, "HurmeRegular_Font", 0.02f, RelativeTo.ScreenHeight);
		m_label.SetAlign(0f, 1f);
		m_value = new UIText(this, false, _tag, text, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight);
		m_value.SetAlign(1f, 0f);
	}

	public override void OnValueChange(object _newValue, object _oldValue)
	{
		Debug.LogWarning("VALUE CHANGE HASHTABLE BUTTON");
	}

	protected override void OnTouchRelease(TLTouch _touch, bool _inside)
	{
		if (!m_pagedCanvas.IsChangingPage())
		{
			base.OnTouchRelease(_touch, _inside);
			if (_inside)
			{
				CreateArrays();
			}
		}
	}

	public void CreateArrays()
	{
		scrollableCanvas = new UIScrollableCanvas(m_scrollableCanvas.m_parent, string.Empty);
		scrollableCanvas.SetWidth(1f, RelativeTo.ParentWidth);
		scrollableCanvas.SetHeight(1f, RelativeTo.ParentHeight);
		UIVerticalList uIVerticalList = new UIVerticalList(scrollableCanvas, "VerticalArea");
		uIVerticalList.SetVerticalAlign(1f);
		UIText uIText = new UIText(uIVerticalList, false, string.Empty, m_label.m_text, "HurmeSemiBold_Font", 0.02f, RelativeTo.ScreenHeight, "#000000");
		new UIArrayLengthButton(uIVerticalList, string.Empty, "LENGTH", m_model, m_fieldName, m_key, 0, 20);
		Hashtable hashtable = (Hashtable)GetValue();
		foreach (string key in hashtable.Keys)
		{
			new UIStringArrayEditorButton(uIVerticalList, m_scrollableCanvas, m_pagedCanvas, string.Empty, key, m_model, m_fieldName, key);
		}
		m_backButton = new PsUIGenericButton(uIVerticalList);
		m_backButton.SetText("Back");
		m_backButton.SetOrangeColors();
		m_pagedCanvas.NextPage();
		m_pagedCanvas.Update();
	}

	public override void Step()
	{
		if (m_backButton != null && m_backButton.m_hit)
		{
			scrollableCanvas.Destroy();
			scrollableCanvas = null;
			m_pagedCanvas.PreviousPage();
			m_pagedCanvas.Update();
		}
		base.Step();
	}
}
