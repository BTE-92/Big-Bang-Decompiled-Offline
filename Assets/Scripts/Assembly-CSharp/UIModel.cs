using System;
using System.Collections.Generic;
using System.Reflection;

public class UIModel
{
	private object m_object;

	private Type m_objectType;

	private Dictionary<string, FieldInfo> m_objectFields;

	private Dictionary<string, List<UIComponent>> m_boundComponents;

	private Action<string, object, object> m_valueChangeDelegate;

	public UIModel(object _object, Action<string, object, object> _valueChangeDelegate = null)
	{
		m_object = _object;
		m_valueChangeDelegate = _valueChangeDelegate;
		m_objectType = _object.GetType();
		m_objectFields = new Dictionary<string, FieldInfo>();
		m_boundComponents = new Dictionary<string, List<UIComponent>>();
		BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
		FieldInfo[] fields = m_objectType.GetFields(bindingAttr);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!m_objectFields.ContainsKey(fieldInfo.Name))
			{
				m_objectFields.Add(fieldInfo.Name, fieldInfo);
			}
			else
			{
				Debug.LogError("UIModel trying to add an existing field, may cause crash or unwanted behavior. Check your variable names!");
			}
			if (!m_boundComponents.ContainsKey(fieldInfo.Name))
			{
				m_boundComponents.Add(fieldInfo.Name, new List<UIComponent>());
			}
			else
			{
				Debug.LogError("UIModel trying to add an existing field, may cause crash or unwanted behavior. Check your variable names!");
			}
		}
	}

	public FieldInfo GetFieldInfo(string _fieldName)
	{
		FieldInfo value;
		if (m_objectFields.TryGetValue(_fieldName, out value))
		{
			return value;
		}
		return null;
	}

	public object GetValue(UIComponent _uiComponent, bool _registerAsBound = true)
	{
		FieldInfo value;
		if (m_objectFields.TryGetValue(_uiComponent.m_fieldName, out value))
		{
			List<UIComponent> value2;
			m_boundComponents.TryGetValue(_uiComponent.m_fieldName, out value2);
			if (_registerAsBound && !value2.Contains(_uiComponent))
			{
				value2.Add(_uiComponent);
			}
			return value.GetValue(m_object);
		}
		Debug.LogError(string.Concat("No such property: ", m_objectType, ".", _uiComponent.m_fieldName));
		return null;
	}

	public object GetValue(UIComponent _uiComponent, out Type _fieldType, bool _registerAsBound = true)
	{
		FieldInfo value;
		if (m_objectFields.TryGetValue(_uiComponent.m_fieldName, out value))
		{
			List<UIComponent> value2;
			m_boundComponents.TryGetValue(_uiComponent.m_fieldName, out value2);
			if (_registerAsBound && !value2.Contains(_uiComponent))
			{
				value2.Add(_uiComponent);
			}
			_fieldType = value.FieldType;
			return value.GetValue(m_object);
		}
		_fieldType = null;
		Debug.LogError(string.Concat("No such property: ", m_objectType, ".", _uiComponent.m_fieldName));
		return null;
	}

	public object GetValue(string _fieldName, out Type _fieldType)
	{
		FieldInfo value;
		if (m_objectFields.TryGetValue(_fieldName, out value))
		{
			List<UIComponent> value2;
			m_boundComponents.TryGetValue(_fieldName, out value2);
			_fieldType = value.FieldType;
			return value.GetValue(m_object);
		}
		_fieldType = null;
		Debug.LogError(string.Concat("No such property: ", m_objectType, ".", _fieldName));
		return null;
	}

	public object GetValue(string _fieldName)
	{
		FieldInfo value;
		if (m_objectFields.TryGetValue(_fieldName, out value))
		{
			List<UIComponent> value2;
			m_boundComponents.TryGetValue(_fieldName, out value2);
			return value.GetValue(m_object);
		}
		Debug.LogError(string.Concat("No such property: ", m_objectType, ".", _fieldName));
		return null;
	}

	public void SetValue(object _newValue, UIComponent _uiComponent)
	{
		FieldInfo value;
		if (m_objectFields.TryGetValue(_uiComponent.m_fieldName, out value))
		{
			object value2 = GetValue(_uiComponent);
			value.SetValue(m_object, _newValue);
			List<UIComponent> value3;
			m_boundComponents.TryGetValue(_uiComponent.m_fieldName, out value3);
			for (int i = 0; i < value3.Count; i++)
			{
				UIComponent uIComponent = value3[i];
				if (uIComponent != _uiComponent)
				{
					uIComponent.OnValueChange(_newValue, value2);
				}
				if (m_valueChangeDelegate != null)
				{
					m_valueChangeDelegate(_uiComponent.m_fieldName, _newValue, value2);
				}
			}
		}
		else
		{
			Debug.LogError(string.Concat("No such property: ", m_objectType, ".", _uiComponent.m_fieldName));
		}
	}

	public void SetValue(object _newValue, string _fieldName)
	{
		FieldInfo value;
		if (m_objectFields.TryGetValue(_fieldName, out value))
		{
			object value2 = GetValue(_fieldName);
			value.SetValue(m_object, _newValue);
			List<UIComponent> value3;
			m_boundComponents.TryGetValue(_fieldName, out value3);
			for (int i = 0; i < value3.Count; i++)
			{
				UIComponent uIComponent = value3[i];
				uIComponent.OnValueChange(_newValue, value2);
				if (m_valueChangeDelegate != null)
				{
					m_valueChangeDelegate(_fieldName, _newValue, value2);
				}
			}
		}
		else
		{
			Debug.LogError(string.Concat("No such property: ", m_objectType, ".", _fieldName));
		}
	}

	public void RemoveBinding(UIComponent _uiComponent)
	{
		List<UIComponent> value;
		m_boundComponents.TryGetValue(_uiComponent.m_fieldName, out value);
		value.Remove(_uiComponent);
	}
}
