using Server;

public class PsUIComments : UIVerticalList
{
	public string m_gameId;

	public bool m_fetchingData;

	public PsUIComments(UIComponent _parent, bool _touchable, string _tag, string _gameId, bool _loadImmediately = true)
		: base(_parent, _tag)
	{
		SetSpacing(0.025f, RelativeTo.ScreenShortest);
		RemoveDrawHandler();
		RemoveTouchAreas();
		m_gameId = _gameId;
		m_fetchingData = false;
		if (_loadImmediately)
		{
			LoadContent();
		}
	}

	public void LoadContent()
	{
		if (!m_fetchingData)
		{
			HttpC c = Comment.Get(m_gameId, DataRequestSUCCEED, DataRequestFAILED);
			EntityManager.AddComponentToEntity(m_TC.p_entity, c);
			m_fetchingData = true;
		}
	}

	private void DataRequestSUCCEED(CommentData[] _commentData)
	{
		if (!m_hidden)
		{
			HandleData(_commentData);
		}
		m_fetchingData = false;
	}

	private void DataRequestFAILED(HttpC _request)
	{
		m_fetchingData = false;
		Debug.Log("Screenshot FAILED");
	}

	private void HandleData(CommentData[] _commentData)
	{
		for (int i = 0; i < _commentData.Length; i++)
		{
			CreateComment(_commentData[i]);
		}
		m_parent.m_parent.Update();
	}

	private void CreateComment(CommentData _commentData)
	{
		UIScaleToContentCanvas uIScaleToContentCanvas = new UIScaleToContentCanvas(this, "Row");
		uIScaleToContentCanvas.SetWidth(0.75f, RelativeTo.ParentWidth);
		uIScaleToContentCanvas.SetHeight(1f, RelativeTo.ScreenHeight);
		uIScaleToContentCanvas.RemoveTouchAreas();
		uIScaleToContentCanvas.RemoveDrawHandler();
		string text = "<color=#ffffff>";
		if (PlayerPrefsX.GetUserId().Equals(_commentData.playerId))
		{
			text = "<color=#80ff33>";
		}
		UIVerticalList uIVerticalList = new UIVerticalList(uIScaleToContentCanvas, string.Empty);
		uIVerticalList.SetAlign(0f, 1f);
		uIVerticalList.SetMargins(0f, 0f, 0.035f, 0f);
		uIVerticalList.RemoveDrawHandler();
		uIVerticalList.RemoveTouchAreas();
		PsUIProfileImage psUIProfileImage = new PsUIProfileImage(uIVerticalList, false, "profile", _commentData.facebookId, _commentData.gameCenterId);
		psUIProfileImage.SetSize(0.075f, 0.075f, RelativeTo.ScreenShortest);
		uIVerticalList = new UIVerticalList(uIScaleToContentCanvas, string.Empty);
		uIVerticalList.SetAlign(1f, 1f);
		uIVerticalList.SetMargins(0.1f, 0f, 0f, 0f, RelativeTo.ScreenShortest);
		uIVerticalList.RemoveTouchAreas();
		UIText uIText = new UIText(uIVerticalList, false, "User", text + PlayerPrefsX.GetUserName() + "</color>", "HurmeSemiBold_Font", 0.03f, RelativeTo.ScreenHeight);
		uIText.SetHorizontalAlign(0f);
		uIText.SetDrawHandler(UIDrawHandlers.DebugRect);
		UITextbox uITextbox = new UITextbox(uIVerticalList, false, "Comment", _commentData.comment, "HurmeRegular_Font", 0.035f, RelativeTo.ScreenHeight);
		uITextbox.SetWidth(1f, RelativeTo.ParentWidth);
		uITextbox.SetMargins(0.025f, 0.025f, 0.025f, 0.025f, RelativeTo.ScreenShortest);
		uITextbox.SetHorizontalAlign(0f);
		uITextbox.SetDrawHandler(UIDrawHandlers.DebugRect);
	}

	public void SendComment(string _comment)
	{
	}

	private void DataSendSUCCEED(HttpC _request)
	{
		Debug.Log("Comment send SUCCEED");
	}

	private void DataSendFAILED(HttpC _request)
	{
		Debug.Log("Comment send FAILED");
	}

	public override void Destroy()
	{
		base.Destroy();
	}
}
