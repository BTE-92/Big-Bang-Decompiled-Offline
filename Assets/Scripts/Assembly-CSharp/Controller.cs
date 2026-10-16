using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller
{
	private struct ControllerButton
	{
		public ControllerButtonType type;

		public UIComponent component;

		public Vector2 dir;

		public KeyCode keyCode;
	}

	private static Hashtable m_buttons = new Hashtable();

	public PsUIBoosterButton m_boostButton;

	protected bool m_open;

	public static bool m_controllerDisabled;

	public Controller()
	{
		m_controllerDisabled = false;
	}

	public void AddButton(string _name, UIComponent _component, ControllerButtonType _type, KeyCode _keyCode = KeyCode.None)
	{
		ControllerButton controllerButton = new ControllerButton
		{
			component = _component,
			type = _type,
			dir = Vector2.zero,
			keyCode = _keyCode
		};
		m_buttons.Add(_name, controllerButton);
	}

	public virtual void DisableController()
	{
		m_controllerDisabled = true;
	}

	public virtual void EnableController()
	{
		m_controllerDisabled = false;
	}

	public static void RemoveButton(string _name)
	{
		m_buttons.Remove(_name);
	}

	public static void RemoveAllButtons()
	{
		m_buttons.Clear();
	}

	public static bool GetAnyButton()
	{
		if (m_controllerDisabled)
		{
			return false;
		}
		foreach (DictionaryEntry button in m_buttons)
		{
			ControllerButton controllerButton = (ControllerButton)m_buttons[button.Key];
			if (controllerButton.component.m_isDown || Input.GetKey(controllerButton.keyCode))
			{
				return true;
			}
		}
		return false;
	}

	public static ControllerButtonState GetButtonState(string _name)
	{
		if (!m_buttons.ContainsKey(_name) || m_controllerDisabled)
		{
			return ControllerButtonState.OFF;
		}
		ControllerButton controllerButton = (ControllerButton)m_buttons[_name];
		if (controllerButton.component.m_isDown || Input.GetKey(controllerButton.keyCode))
		{
			return ControllerButtonState.ON;
		}
		return ControllerButtonState.OFF;
	}

	public static Vector2 GetControllerJoystickDir(string _name)
	{
		if (!m_buttons.ContainsKey(_name))
		{
			return Vector2.zero;
		}
		ControllerButton controllerButton = (ControllerButton)m_buttons[_name];
		if (controllerButton.type == ControllerButtonType.JOYSTICK)
		{
			return controllerButton.dir;
		}
		return Vector2.zero;
	}

	public virtual void Open()
	{
	}

	public virtual void Close()
	{
		foreach (DictionaryEntry button in m_buttons)
		{
			UIComponent component = ((ControllerButton)m_buttons[button.Key]).component;
			List<TLTouch> touchesOnTAC = TouchAreaS.GetTouchesOnTAC(component.m_TAC);
			for (int i = 0; i < touchesOnTAC.Count; i++)
			{
				touchesOnTAC[i].Reset();
			}
		}
	}

	public virtual void DisabletouchAreas()
	{
	}
}
