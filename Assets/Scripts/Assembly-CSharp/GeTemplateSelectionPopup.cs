using Server;

public class GeTemplateSelectionPopup : GeMinigameMultiSelectionPopup
{
	public GeTemplateSelectionPopup(UIModel _model, string _fieldName)
		: base(_model, _fieldName)
	{
	}

	protected override void SendRequest()
	{
		HttpC all = Template.GetAll(LoadSUCCESS, LoadFAILED, null, 200);
		EntityManager.AddComponentToEntity(m_TC.p_entity, all);
	}

	protected override void LoadFAILED(HttpC _c)
	{
		Debug.Log("SEND QUIT FAILED");
		string networkError = ServerErrors.GetNetworkError(_c.www.error);
		ServerManager.ThrowServerErrorException("Connection error", networkError, () => Template.GetAll(LoadSUCCESS, LoadFAILED, null, 200));
	}
}
