using System.Collections.Generic;

public class GeMinigameMultiSelectionPopup : GeMinigameSelectionPopup
{
	public GeMinigameMultiSelectionPopup(UIModel _model, string _fieldName)
		: base(_model, _fieldName)
	{
	}

	protected override void LoadSUCCESS(PsMinigameMetaData[] _minigameMetaData)
	{
		m_verticalArea.DestroyChildren();
		for (int i = 0; i < _minigameMetaData.Length; i++)
		{
			new UIStringListSelectorButton(m_verticalArea, "ArraySelectorButton", m_model, m_fieldName, _minigameMetaData[i].id, _minigameMetaData[i].name);
		}
		m_verticalArea.m_parent.Update();
	}

	public override void OnValueChange(object _newValue, object _oldValue)
	{
		string text = "-";
		List<string> list = (List<string>)GetValue();
		for (int i = 0; i < list.Count; i++)
		{
			text = ((i != 0) ? (text + ", " + list[i]) : list[i]);
		}
		Debug.Log(text);
	}
}
