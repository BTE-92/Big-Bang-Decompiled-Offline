using Server;

public class GeMinigameSelectionPopup : UIListPropertySelectionPopup
{
	protected UIText loadingText;

	public GeMinigameSelectionPopup(UIModel _model, string _fieldName)
		: base(_model, _fieldName)
	{
		CreateLoadingText();
		SendRequest();
	}

	protected virtual void SendRequest()
	{
		MinigameSearchParametres objectData = new MinigameSearchParametres();
		HttpC hidden = MiniGame.GetHidden(LoadSUCCESS, LoadFAILED, null, 500);
		hidden.objectData = objectData;
		EntityManager.AddComponentToEntity(m_TC.p_entity, hidden);
	}

	protected void CreateLoadingText()
	{
		loadingText = new UIText(m_verticalArea, false, string.Empty, "Loading...", "HurmeSemiBold_Font", 0.03f, RelativeTo.ScreenHeight);
		m_verticalArea.Update();
	}

	protected virtual void LoadSUCCESS(PsMinigameMetaData[] _minigameMetaData)
	{
		m_verticalArea.DestroyChildren();
		UIText uIText = new UIText(m_verticalArea, true, string.Empty, "Clear", "HurmeSemiBold_Font", 0.03f, RelativeTo.ScreenHeight);
		uIText.m_TAC.m_name = string.Empty;
		uIText.m_TAC.m_letTouchesThrough = true;
		uIText.SetDrawHandler(UIDrawHandlers.DebugRect);
		uIText.SetTouchHandler(ListItemTouchHandler);
		for (int i = 0; i < _minigameMetaData.Length; i++)
		{
			uIText = new UIText(m_verticalArea, true, string.Empty, _minigameMetaData[i].name, "HurmeSemiBold_Font", 0.03f, RelativeTo.ScreenHeight);
			uIText.m_TAC.m_name = _minigameMetaData[i].id;
			uIText.m_TAC.m_letTouchesThrough = true;
			uIText.m_TAC.m_customObject = _minigameMetaData[i];
			uIText.SetDrawHandler(UIDrawHandlers.DebugRect);
			uIText.SetTouchHandler(ListItemTouchHandler);
		}
		m_verticalArea.m_parent.Update();
	}

	public virtual void ListItemTouchHandler(TouchAreaC _touchArea, TouchAreaPhase _touchPhase, bool _touchIsSecondary, int _touchCount, TLTouch[] _touches)
	{
		if (_touchPhase == TouchAreaPhase.ReleaseIn && !_touchArea.m_wasDragged)
		{
			SetValue(_touchArea.m_customObject);
			Close();
		}
	}

	protected virtual void LoadFAILED(HttpC _c)
	{
		Debug.Log("SEND QUIT FAILED");
		string networkError = ServerErrors.GetNetworkError(_c.www.error);
		ServerManager.ThrowServerErrorException("Connection error", networkError, () => MiniGame.SearchMinigames(_c.objectData as MinigameSearchParametres, LoadSUCCESS, LoadFAILED));
	}
}
