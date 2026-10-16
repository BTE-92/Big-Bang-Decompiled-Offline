using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PsUIMainMenuResources : UIVerticalList
{
	public bool m_listHorizontally;

	public bool m_showCoins;

	public bool m_showDiamonds;

	public bool m_showKeys;

	public bool m_showBolts;

	public bool m_showShopButtons;

	public bool m_showTickets;

	public UIText m_keys;

	public UIText m_keyTimer;

	public UIText m_coins;

	public UIText m_diamonds;

	public UIText m_bolts;

	public UIText m_tickets;

	public PsUIGenericButton m_shopButton;

	public PsUIGenericButton m_shopButtonCoins;

	public PsUIBasePopup m_shop;

	private UIHorizontalList m_keyRow;

	private UIHorizontalList m_coinRow;

	private UIHorizontalList m_diamondRow;

	private UIHorizontalList m_ticketRow;

	public UIHorizontalList m_boltRow;

	private UIFittedSprite m_keyIcon;

	private UIComponent m_shopParent;

	private UIFittedText m_rewind;

	private bool m_shopActive;

	public List<object> m_flyers = new List<object>();

	public List<TweenC> m_removeList = new List<TweenC>();

	public List<TimerC> m_indicatorCreations = new List<TimerC>();

	public Camera m_flyerCamera;

	public float m_topMargin;

	private int m_addCoinAmount;

	private int m_addDiamondAmount;

	private int m_addKeyAmount;

	private int m_addBoltAmount;

	private int m_coinAddTimer;

	private int m_diamondAddTimer;

	private int m_keyAddTimer;

	private int m_boltAddTimer;

	private bool m_cumulateCoins;

	private bool m_cumulateDiamonds;

	private bool m_cumulateKeys;

	private bool m_cumulateBolts;

	public UIFittedSprite m_icon;

	private LastResourceView m_lastView;

	public PsUIMainMenuResources(Camera _camera, bool _listHorizontally, bool _showCoins, bool _showDiamonds, bool _showKeys, bool _showBolts, float _topMargin, bool _showShopButtons, bool _showTickets = false)
		: base(null, "PlayerResources")
	{
		RemoveDrawHandler();
		m_topMargin = _topMargin;
		SetMargins(0f, 0.025f, m_topMargin, 0f, RelativeTo.ScreenShortest);
		SetSpacing(0.015f, RelativeTo.ScreenShortest);
		SetDepthOffset(-200f);
		if (_camera != null)
		{
			m_camera = _camera;
		}
		m_listHorizontally = _listHorizontally;
		m_showCoins = _showCoins;
		m_showDiamonds = _showDiamonds;
		m_showShopButtons = _showShopButtons;
		m_showTickets = _showTickets;
		CreateUI();
	}

	public void SetShopActivity(bool _active)
	{
	}

	public void CalculateControlMinMaxValues(float[] _control0, float[] _control1, float[] _height, ref float _min0, ref float _min1, ref float _max0, ref float _max1, ref float _minY, ref float _maxY)
	{
		float[] array = _control0;
		if (array == null)
		{
			array = new float[2] { -0.45f, 0.35f };
		}
		float[] array2 = _control1;
		if (array2 == null)
		{
			array2 = new float[2] { -0.95f, -0.75f };
		}
		float[] array3 = _height;
		if (array3 == null)
		{
			array3 = new float[2] { -0.25f, 0.25f };
		}
		if (array.Length == 1)
		{
			_min0 = (_max0 = array[0]);
		}
		else
		{
			_min0 = array[0];
			_max0 = array[1];
		}
		if (array2.Length == 1)
		{
			_min1 = (_max1 = array2[0]);
		}
		else
		{
			_min1 = array2[0];
			_max1 = array2[1];
		}
		if (array3.Length == 1)
		{
			_minY = (_maxY = array3[0]);
			return;
		}
		_minY = array3[0];
		_maxY = array3[1];
	}

	public void CreateFlyingResources(int _amount, Vector2 _startScreenPos, ResourceType _type, float _delay = 0f, TweenEventDelegate _tweenDelegate = null, float[] _control0XMinMax = null, float[] _control1XMinMax = null, float[] _heightMinMax = null, Vector2 _endScreenPos = default(Vector2))
	{
		if (_type != ResourceType.Coins && _type != ResourceType.Diamonds)
		{
			return;
		}
		TimerComponentDelegate _timerDelegate = null;
		string _iconName = string.Empty;
		UIComponent _target = m_coinRow;
		SetResourceProperties(_type, _amount, ref _iconName, ref _target, ref _timerDelegate);
		float _min = 0f;
		float _min2 = 0f;
		float _max = 0f;
		float _max2 = 0f;
		float _minY = 0f;
		float _maxY = 0f;
		CalculateControlMinMaxValues(_control0XMinMax, _control1XMinMax, _heightMinMax, ref _min, ref _min2, ref _max, ref _max2, ref _minY, ref _maxY);
		int num = _amount;
		if (_amount > 20)
		{
			num = 20;
		}
		if (_target != m_boltRow && _target != m_ticketRow)
		{
			for (int i = 0; i < num; i++)
			{
				FlyingResource flyingResource = new FlyingResource(_type, 0.06f, RelativeTo.ScreenHeight, _startScreenPos, m_flyerCamera);
				Vector3 zero = Vector3.zero;
				zero = ((_target == null) ? ((Vector3)_endScreenPos) : (_target.m_camera.WorldToScreenPoint(_target.m_TC.transform.position) - new Vector3(Screen.width, Screen.height, 0f) * 0.5f + new Vector3(_target.m_actualWidth * 0.5f - 0.05f * (float)Screen.height, 0f, 0f)));
				float num2 = UnityEngine.Random.Range(_min, _max);
				float num3 = UnityEngine.Random.Range(_min2, _max2);
				float num4 = UnityEngine.Random.Range(_minY, _maxY);
				Vector3 normalized = (zero - flyingResource.m_TC.transform.localPosition).normalized;
				Vector3 vector = Vector3.Cross(normalized, -Vector3.forward);
				Vector3 control = flyingResource.m_TC.transform.localPosition + normalized * Screen.height * num2 + vector * Screen.height * num4;
				Vector3 control2 = zero + normalized * Screen.height * num3 + vector * Screen.height * num4;
				float num5 = 1.5f + UnityEngine.Random.Range(-0.175f, 0.175f);
				if (i == 0)
				{
					TimerC timerC = TimerS.AddComponent(flyingResource.m_TC.p_entity, string.Empty, num5 - 0.45f, _delay, false, _timerDelegate);
					timerC.customObject = _type;
					if (_type != ResourceType.Tokens && _type != ResourceType.Templates)
					{
						timerC = TimerS.AddComponent(flyingResource.m_TC.p_entity, string.Empty, 0f, _delay + num5 / 1.5f, false, IndicatorCreationDelegate);
						timerC.customObject = new ResourceData
						{
							amount = _amount,
							delay = 0f,
							fadeOutDelay = 1f,
							iconName = _iconName,
							target = _target
						};
					}
				}
				TweenC c = TweenS.AddTransformTween(flyingResource.m_TC, TweenedProperty.Alpha, TweenStyle.CubicIn, Vector3.one, Vector3.zero, 0.3f, num5 / 1.5f + _delay, true);
				TweenS.SetTweenAlphaProperties(c, false, true, false);
				c = TweenS.AddCurvedTransformTween(flyingResource.m_TC, TweenedProperty.Position, TweenStyle.CubicOut, zero, control, control2, num5, _delay, false);
				TweenS.AddTweenEndEventListener(c, (_tweenDelegate == null) ? new TweenEventDelegate(TweenDelegate) : _tweenDelegate);
				m_flyers.Add(flyingResource);
			}
			return;
		}
		for (int j = 0; j < num; j++)
		{
			UIFittedSprite uIFittedSprite = new UIFittedSprite(null, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(_iconName));
			uIFittedSprite.SetSize(0.06f, 0.06f, RelativeTo.ScreenShortest);
			uIFittedSprite.SetCamera(m_flyerCamera, true, false);
			uIFittedSprite.Update();
			Vector3 position = _startScreenPos;
			TransformS.SetPosition(uIFittedSprite.m_TC, position);
			Vector3 zero2 = Vector3.zero;
			zero2 = ((_target == null) ? ((Vector3)_endScreenPos) : (_target.m_camera.WorldToScreenPoint(_target.m_TC.transform.position) - new Vector3(Screen.width, Screen.height, 0f) * 0.5f + new Vector3(_target.m_actualWidth * 0.5f - 0.05f * (float)Screen.height, 0f, 0f)));
			float num6 = UnityEngine.Random.Range(_min, _max);
			float num7 = UnityEngine.Random.Range(_min2, _max2);
			float num8 = UnityEngine.Random.Range(_minY, _maxY);
			Vector3 normalized2 = (zero2 - uIFittedSprite.m_TC.transform.localPosition).normalized;
			Vector3 vector2 = Vector3.Cross(normalized2, -Vector3.forward);
			Vector3 control3 = uIFittedSprite.m_TC.transform.localPosition + normalized2 * Screen.height * num6 + vector2 * Screen.height * num8;
			Vector3 control4 = zero2 + normalized2 * Screen.height * num7 + vector2 * Screen.height * num8;
			float num9 = 1.5f + UnityEngine.Random.Range(-0.175f, 0.175f);
			if (j == 0)
			{
				TimerC timerC2 = TimerS.AddComponent(uIFittedSprite.m_TC.p_entity, string.Empty, num9 - 0.45f, _delay, false, _timerDelegate);
				timerC2.customObject = _type;
				if (_type != ResourceType.Tokens && _type != ResourceType.Templates)
				{
					timerC2 = TimerS.AddComponent(uIFittedSprite.m_TC.p_entity, string.Empty, 0f, _delay + num9 / 1.5f, false, IndicatorCreationDelegate);
					timerC2.customObject = new ResourceData
					{
						amount = _amount,
						delay = 0f,
						fadeOutDelay = 1f,
						iconName = _iconName,
						target = _target
					};
				}
			}
			TweenC c2 = TweenS.AddTransformTween(uIFittedSprite.m_TC, TweenedProperty.Alpha, TweenStyle.CubicIn, Vector3.one, Vector3.zero, 0.3f, num9 / 1.5f + _delay, true);
			TweenS.SetTweenAlphaProperties(c2, false, true, false, Shader.Find("WOE/Unlit/ColorUnlitTransparent"));
			c2 = TweenS.AddCurvedTransformTween(uIFittedSprite.m_TC, TweenedProperty.Position, TweenStyle.CubicOut, zero2, control3, control4, num9, _delay, false);
			TweenS.AddTweenEndEventListener(c2, (_tweenDelegate == null) ? new TweenEventDelegate(TweenDelegate) : _tweenDelegate);
			m_flyers.Add(uIFittedSprite);
		}
	}

	public void CreateAddedResources(ResourceType _type, int _amount, float _delay)
	{
		if (_amount != 0 && (_type == ResourceType.Coins || _type == ResourceType.Diamonds))
		{
			TimerComponentDelegate _timerDelegate = null;
			string _iconName = string.Empty;
			UIComponent _target = m_coinRow;
			SetResourceProperties(_type, _amount, ref _iconName, ref _target, ref _timerDelegate);
			CreateResourceIndicator(_target, _amount, _iconName, _delay);
			StartCumulate(_type);
		}
	}

	private void CreateResourceIndicator(UIComponent _target, int _amount, string _iconName, float _delay, float _fadeOutDelay = 0f)
	{
		if (m_flyerCamera == null)
		{
			m_flyerCamera = CameraS.AddCamera("FlyerCamera", true);
		}
		float duration = 0.5f + _delay + _fadeOutDelay;
		string hex = "#81f02b";
		string text = "+" + _amount;
		if (_amount < 0)
		{
			hex = "#ff793f";
			text = _amount.ToString();
		}
		UIHorizontalList uIHorizontalList = new UIHorizontalList(null, string.Empty);
		uIHorizontalList.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		uIHorizontalList.RemoveDrawHandler();
		UIText uIText = new UIText(uIHorizontalList, false, string.Empty, text, "HurmeSemiBold_Font", 0.04f, RelativeTo.ScreenHeight);
		uIText.m_tmc.m_textMesh.GetComponent<Renderer>().material.color = DebugDraw.HexToColor(hex);
		UIFittedSprite uIFittedSprite = new UIFittedSprite(uIHorizontalList, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame(_iconName));
		uIFittedSprite.SetSize(0.06f, 0.06f, RelativeTo.ScreenShortest);
		uIHorizontalList.SetCamera(m_flyerCamera, true, false);
		Vector3 vector = _target.m_camera.WorldToScreenPoint(_target.m_TC.transform.position) - new Vector3(Screen.width, Screen.height, 0f) * 0.5f;
		Vector3 endValue = vector + new Vector3(0f, (float)Screen.height * 0.05f, 0f);
		if (_amount < 0)
		{
			endValue = vector - new Vector3(0f, (float)Screen.height * 0.05f, 0f);
		}
		TransformS.SetPosition(uIHorizontalList.m_TC, vector);
		uIHorizontalList.Update();
		for (int i = 0; i < uIHorizontalList.m_childs.Count; i++)
		{
			TweenC c = TweenS.AddTransformTween(uIHorizontalList.m_childs[i].m_TC, TweenedProperty.Alpha, TweenStyle.CubicOut, Vector3.one, Vector3.zero, 0.2f, _delay + 0.3f + _fadeOutDelay, true);
			if (i == 0)
			{
				TweenS.SetTweenAlphaProperties(c, false, false, true, Shader.Find("Framework/FontShader"));
			}
			else
			{
				TweenS.SetTweenAlphaProperties(c, false, true, false, Shader.Find("WOE/Unlit/ColorUnlitTransparent"));
			}
		}
		TweenC tweenC = TweenS.AddTransformTween(uIHorizontalList.m_TC, TweenedProperty.Position, TweenStyle.CubicOut, vector, endValue, 0.5f, _delay, false);
		TimerS.AddComponent(uIHorizontalList.m_TC.p_entity, string.Empty, duration, 0f, false, IndicatorTimerDelegate);
		m_flyers.Add(uIHorizontalList);
	}

	private void IndicatorCreationDelegate(TimerC _c)
	{
		m_indicatorCreations.Add(_c);
	}

	private void IndicatorTimerDelegate(TimerC _c)
	{
		IComponent componentByEntity = EntityManager.GetComponentByEntity(ComponentType.Tween, _c.p_entity);
		if (componentByEntity != null)
		{
			m_removeList.Add(componentByEntity as TweenC);
		}
		TimerS.RemoveComponent(_c);
	}

	public void SetResourceProperties(ResourceType _type, int _amount, ref string _iconName, ref UIComponent _target, ref TimerComponentDelegate _timerDelegate)
	{
		switch (_type)
		{
		case ResourceType.Coins:
			_iconName = "menu_resources_coin_icon";
			_target = m_coinRow;
			m_addCoinAmount += _amount;
			m_coinAddTimer += 45;
			break;
		case ResourceType.Diamonds:
			_iconName = "menu_resources_diamond_icon";
			_target = m_diamondRow;
			m_addDiamondAmount += _amount;
			m_diamondAddTimer += 45;
			break;
		case ResourceType.Keys:
			_iconName = "menu_resources_key_icon";
			_target = m_keyRow;
			m_addKeyAmount += _amount;
			m_keyAddTimer += 45;
			break;
		case ResourceType.Bolts:
			_iconName = "menu_resources_bolt_icon";
			_target = m_boltRow;
			m_addBoltAmount += _amount;
			m_boltAddTimer += 45;
			break;
		case ResourceType.Tokens:
			_target = null;
			break;
		case ResourceType.Templates:
			_iconName = "menu_dome_icon";
			_target = null;
			break;
		}
		_timerDelegate = TimerDelegate;
		if (m_flyerCamera == null)
		{
			m_flyerCamera = CameraS.AddCamera("FlyerCamera", true);
		}
	}

	private void TweenDelegate(TweenC _c)
	{
		m_removeList.Add(_c);
	}

	private void TimerDelegate(TimerC _c)
	{
		ResourceType resourceType = (ResourceType)(int)_c.customObject;
		if (resourceType != ResourceType.Tokens)
		{
			StartCumulate(resourceType);
		}
		TimerS.RemoveComponent(_c);
	}

	public void StartCumulate(ResourceType _type)
	{
		switch (_type)
		{
		case ResourceType.Coins:
			m_cumulateCoins = true;
			TweenResourceRow(m_coinRow, new Vector3(1.2f, 1.2f, 1.2f));
			break;
		case ResourceType.Diamonds:
			m_cumulateDiamonds = true;
			TweenResourceRow(m_diamondRow, new Vector3(1.2f, 1.2f, 1.2f));
			break;
		case ResourceType.Keys:
			m_cumulateKeys = true;
			TweenResourceRow(m_keyRow, new Vector3(1.2f, 1.2f, 1.2f));
			break;
		case ResourceType.Bolts:
			m_cumulateBolts = true;
			TweenResourceRow(m_boltRow, new Vector3(1.2f, 1.2f, 1.2f));
			break;
		}
	}

	private void CreateUI()
	{
		DestroyChildren();
		m_keyTimer = null;
		m_coins = null;
		m_diamonds = null;
		m_keys = null;
		m_bolts = null;
		m_shopParent = this;
		if (m_listHorizontally)
		{
			UIHorizontalList uIHorizontalList = new UIHorizontalList(this, string.Empty);
			uIHorizontalList.SetSpacing(0.015f, RelativeTo.ScreenShortest);
			uIHorizontalList.RemoveDrawHandler();
			uIHorizontalList.SetHorizontalAlign(1f);
			m_shopParent = uIHorizontalList;
		}
		if (m_showCoins)
		{
			m_coinRow = new UIHorizontalList(m_shopParent, string.Empty);
			m_coinRow.SetHorizontalAlign(1f);
			m_coinRow.SetHeight(0.08f, RelativeTo.ScreenShortest);
			m_coinRow.SetSpacing(0.025f, RelativeTo.ScreenHeight);
			m_coinRow.SetDrawHandler(PsUIDrawHandlers.ResourceLabelBackground);
			if (m_showShopButtons)
			{
				m_shopButtonCoins = new PsUIGenericButton(m_coinRow);
				m_shopButtonCoins.SetIcon("hud_icon_plus", 0.03f);
				m_shopButtonCoins.SetMargins(0.01f);
				m_shopButtonCoins.SetTealColors();
				m_shopButtonCoins.SetHeight(0.8f, RelativeTo.ParentHeight);
			}
			else
			{
				m_coinRow.SetMargins(0.025f, 0f, 0f, 0f, RelativeTo.ScreenShortest);
			}
			m_coins = new UIText(m_coinRow, false, string.Empty, PsMetagameManager.m_playerStats.coins.ToString(), "HurmeBold_Font", 0.04f, RelativeTo.ScreenShortest, "#52a7ad");
			m_icon = new UIFittedSprite(m_coinRow, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_coin_icon"));
			m_icon.SetHeight(1f, RelativeTo.ParentHeight);
			if (m_listHorizontally)
			{
				m_coinRow.MoveToIndexAtParentsChildList(0);
			}
		}
		if (m_showDiamonds)
		{
			m_diamondRow = new UIHorizontalList(m_shopParent, string.Empty);
			m_diamondRow.SetHorizontalAlign(1f);
			m_diamondRow.SetHeight(0.08f, RelativeTo.ScreenShortest);
			m_diamondRow.SetSpacing(0.025f, RelativeTo.ScreenHeight);
			m_diamondRow.SetDrawHandler(PsUIDrawHandlers.ResourceLabelBackground);
			m_diamondRow.SetMargins(0.025f, 0f, 0f, 0f, RelativeTo.ScreenShortest);
			m_diamonds = new UIText(m_diamondRow, false, string.Empty, PsMetagameManager.m_playerStats.diamonds.ToString(), "HurmeBold_Font", 0.04f, RelativeTo.ScreenShortest, "#52a7ad");
			UIFittedSprite uIFittedSprite = new UIFittedSprite(m_diamondRow, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_diamond_icon"));
			uIFittedSprite.SetHeight(1f, RelativeTo.ParentHeight);
			if (m_listHorizontally)
			{
				m_diamondRow.MoveToIndexAtParentsChildList(0);
			}
		}
	}

	public void CreateKeyTimer()
	{
		if (PsMetagameManager.m_playerStats.keys < PsMetagameManager.m_playerStats.maxKeys && m_keyTimer == null)
		{
			string timeStringFromSeconds = PsMetagameManager.GetTimeStringFromSeconds(PsMetagameManager.m_keyReloadTimeLeft, false);
			m_keyTimer = new UIText(m_keyIcon, false, string.Empty, timeStringFromSeconds, "HurmeBold_Font", 0.027f, RelativeTo.ScreenShortest, "#ffffff");
			m_keyTimer.SetShadowShift(new Vector2(2f, 1f), 0.035f);
			m_keyTimer.SetVerticalAlign(0f);
		}
	}

	public void CreateBoltRow(bool _update = false)
	{
		m_boltRow = new UIHorizontalList(m_shopParent, string.Empty);
		m_boltRow.SetHorizontalAlign(1f);
		m_boltRow.SetHeight(0.08f, RelativeTo.ScreenShortest);
		m_boltRow.SetSpacing(0.02f, RelativeTo.ScreenHeight);
		m_boltRow.SetMargins(0.025f, 0f, 0f, 0f, RelativeTo.ScreenShortest);
		m_boltRow.SetDrawHandler(PsUIDrawHandlers.ResourceLabelBackground);
		m_bolts = new UIText(m_boltRow, false, string.Empty, PsMetagameManager.m_playerStats.bolts.ToString(), "HurmeBold_Font", 0.04f, RelativeTo.ScreenShortest, "#52a7ad");
		UIFittedSprite uIFittedSprite = new UIFittedSprite(m_boltRow, false, string.Empty, PsState.m_uiSheet, PsState.m_uiSheet.m_atlas.GetFrame("menu_resources_bolt_icon"));
		uIFittedSprite.SetHeight(1f, RelativeTo.ParentHeight);
		if (m_listHorizontally)
		{
			m_boltRow.MoveToIndexAtParentsChildList(0);
		}
		if (_update)
		{
			Update();
		}
	}

	public void CumulateTimerAndAmount(ref int _amount, ref int _timer, ref UIText _text, ref UIHorizontalList _row, ref bool _updateAll, ref bool _cumulateBool, bool _keys = false)
	{
		if (_timer > 0 && _amount != 0 && _cumulateBool)
		{
			_updateAll = false;
			int num = 0;
			if (Mathf.Abs(_amount) > _timer)
			{
				num = _amount / _timer;
			}
			else if (_timer % _amount == 0)
			{
				num = ((_amount > 0) ? 1 : (-1));
			}
			int num2 = 0;
			if (!_keys)
			{
				num2 = Convert.ToInt32(_text.m_text);
			}
			else
			{
				string[] array = _text.m_text.Split('/');
				num2 = Convert.ToInt32(array[0]);
			}
			if (!_keys || (_keys && num2 + num <= PsMetagameManager.m_playerStats.maxKeys))
			{
				int num3 = Mathf.FloorToInt(Mathf.Log10(num2) + 1f);
				int num4 = Mathf.FloorToInt(Mathf.Log10(num2 + num) + 1f);
				if (!_keys)
				{
					_text.SetText((num2 + num).ToString());
				}
				else
				{
					_text.SetText((num2 + num).ToString() + "/" + PsMetagameManager.m_playerStats.maxKeys);
				}
				_amount -= num;
				_timer--;
				if (num3 != num4)
				{
					if (m_listHorizontally)
					{
						m_shopParent.Update();
						ArrangeContents();
					}
					else
					{
						_row.Update();
						m_shopParent.ArrangeContents();
					}
				}
			}
			else
			{
				_timer = 0;
				_amount = 0;
				TweenResourceRow(_row, Vector3.one, 0.15f);
				_cumulateBool = false;
			}
		}
		else
		{
			TweenResourceRow(_row, Vector3.one, 0.15f);
			_cumulateBool = false;
		}
	}

	public void TweenResourceRow(UIComponent _c, Vector3 _endValue, float _delay = 0f)
	{
		_c.Hide();
		for (int i = 0; i < _c.m_childs.Count; i++)
		{
			_c.m_childs[i].Hide();
		}
		TweenC tweenC = TweenS.AddTransformTween(_c.m_TC, TweenedProperty.Scale, TweenStyle.QuadInOut, _endValue, 0.1f, _delay, true);
		if (_endValue == Vector3.one)
		{
			TweenS.AddTweenEndEventListener(tweenC, TweenBackEventhandler);
			tweenC.customObject = _c;
		}
	}

	private void TweenBackEventhandler(TweenC _c)
	{
		UIComponent uIComponent = _c.customObject as UIComponent;
		if (uIComponent != null && uIComponent.m_TC.p_entity.m_visible && uIComponent.m_TC.transform.localScale == Vector3.one)
		{
			uIComponent.Show();
			for (int i = 0; i < uIComponent.m_childs.Count; i++)
			{
				uIComponent.m_childs[i].Show();
			}
			uIComponent.d_Draw(uIComponent);
		}
	}

	public void CumulateResources()
	{
		if (m_cumulateCoins || m_cumulateDiamonds || m_cumulateKeys || m_cumulateBolts)
		{
			bool _updateAll = true;
			if (m_cumulateCoins)
			{
				CumulateTimerAndAmount(ref m_addCoinAmount, ref m_coinAddTimer, ref m_coins, ref m_coinRow, ref _updateAll, ref m_cumulateCoins);
			}
			if (m_cumulateDiamonds)
			{
				CumulateTimerAndAmount(ref m_addDiamondAmount, ref m_diamondAddTimer, ref m_diamonds, ref m_diamondRow, ref _updateAll, ref m_cumulateDiamonds);
			}
			if (_updateAll && m_flyers.Count == 0)
			{
				ResetCumulateValues();
			}
		}
	}

	public override void Step()
	{
		if (m_camera == null)
		{
			SetCamera(CameraS.m_uiCamera, true, false);
		}
		if (m_indicatorCreations.Count > 0)
		{
			for (int i = 0; i < m_indicatorCreations.Count; i++)
			{
				ResourceData resourceData = (ResourceData)m_indicatorCreations[i].customObject;
				CreateResourceIndicator(resourceData.target, resourceData.amount, resourceData.iconName, resourceData.delay, resourceData.fadeOutDelay);
				TimerS.RemoveComponent(m_indicatorCreations[i]);
			}
			m_indicatorCreations.Clear();
		}
		if (m_removeList.Count > 0)
		{
			for (int num = m_removeList.Count - 1; num >= 0; num--)
			{
				for (int num2 = m_flyers.Count - 1; num2 >= 0; num2--)
				{
					FlyingResource flyingResource = m_flyers[num2] as FlyingResource;
					UIComponent uIComponent = m_flyers[num2] as UIComponent;
					if (flyingResource != null && m_removeList[num].p_entity == flyingResource.m_entity)
					{
						flyingResource.Destroy();
						m_flyers.RemoveAt(num2);
						m_removeList.RemoveAt(num);
						break;
					}
					if (uIComponent != null && m_removeList[num].p_entity == uIComponent.m_TC.p_entity)
					{
						uIComponent.Destroy();
						m_flyers.RemoveAt(num2);
						m_removeList.RemoveAt(num);
						break;
					}
				}
			}
		}
		CumulateResources();
		if (m_flyers.Count == 0 && m_flyerCamera != null)
		{
			CameraS.RemoveCamera(m_flyerCamera);
			m_flyerCamera = null;
		}
		if (m_shopButton != null && m_shopButton.m_hit && m_shop == null)
		{
			SetLastView();
			m_shop = new PsUIBasePopup(typeof(PsUICenterShopDiamonds));
			m_shop.SetAction("Exit", CloseShop);
			if (Main.m_currentGame.m_currentScene is PsMenuScene)
			{
				PsMenuScene.CreateBlur();
			}
		}
		if (m_shopButtonCoins != null && m_shopButtonCoins.m_hit && m_shop == null)
		{
			SetLastView();
			m_shop = new PsUIBasePopup(typeof(PsUICenterShopCoins));
			m_shop.SetAction("Exit", CloseShop);
			if (Main.m_currentGame.m_currentScene is PsMenuScene)
			{
				PsMenuScene.CreateBlur();
			}
		}
		base.Step();
		if (!PsMetagameManager.m_playerStats.updated)
		{
			return;
		}
		bool flag = false;
		List<UIComponent> _addList = new List<UIComponent>();
		if (m_showCoins && m_coins.m_TC.p_entity.m_visible)
		{
			CheckUpdateNeed(ref _addList, ref m_coinRow, m_coins.m_text, PsMetagameManager.m_playerStats.coins);
			m_coins.SetText(PsMetagameManager.m_playerStats.coins.ToString());
			flag = true;
		}
		if (m_showDiamonds && m_diamonds.m_TC.p_entity.m_visible)
		{
			CheckUpdateNeed(ref _addList, ref m_diamondRow, m_diamonds.m_text, PsMetagameManager.m_playerStats.diamonds);
			m_diamonds.SetText(PsMetagameManager.m_playerStats.diamonds.ToString());
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		if (_addList.Count != 0)
		{
			if (m_listHorizontally)
			{
				m_shopParent.Update();
				ArrangeContents();
			}
			else
			{
				for (int j = 0; j < _addList.Count; j++)
				{
					_addList[j].Update();
				}
				m_shopParent.ArrangeContents();
			}
		}
		ResetCumulateValues();
		PsMetagameManager.m_playerStats.updated = false;
	}

	public LastResourceView SetLastView()
	{
		m_lastView = default(LastResourceView);
		m_lastView.bolts = m_showBolts;
		m_lastView.coins = m_showCoins;
		m_lastView.diamonds = m_showDiamonds;
		m_lastView.keys = m_showKeys;
		m_lastView.tickets = m_showTickets;
		m_lastView.horizontal = m_listHorizontally;
		m_lastView.shopButtons = m_showShopButtons;
		m_lastView.margin = m_topMargin;
		m_lastView.camera = m_camera;
		return m_lastView;
	}

	public void CheckUpdateNeed(ref List<UIComponent> _addList, ref UIHorizontalList _add, string _oldValue, int _newValue, bool _key = false, bool _timerWasNull = false)
	{
		int num = 0;
		if (_key)
		{
			string[] array = _oldValue.Split('/');
			num = Convert.ToInt32(array[0]);
		}
		else
		{
			num = Convert.ToInt32(_oldValue);
		}
		int num2 = Mathf.FloorToInt(Mathf.Log10(Convert.ToSingle(num)) + 1f);
		int num3 = Mathf.FloorToInt(Mathf.Log10(_newValue) + 1f);
		if (num2 != num3 || _timerWasNull)
		{
			_addList.Add(_add);
		}
	}

	private void ResetCumulateValues()
	{
		m_cumulateBolts = (m_cumulateCoins = (m_cumulateKeys = (m_cumulateDiamonds = false)));
		m_addCoinAmount = (m_addDiamondAmount = (m_addBoltAmount = (m_addKeyAmount = 0)));
		m_keyAddTimer = (m_coinAddTimer = (m_diamondAddTimer = (m_boltAddTimer = 0)));
	}

	private void CloseShop()
	{
		m_shop.Destroy();
		m_shop = null;
		ShowLastView(m_lastView);
		if (Main.m_currentGame.m_currentScene is PsMenuScene)
		{
			PsMenuScene.RemoveBlur();
		}
	}

	public void ShowLastView(LastResourceView m_lastView)
	{
		PsMetagameManager.ShowResources(m_lastView.camera, m_lastView.horizontal, m_lastView.coins, m_lastView.diamonds, m_lastView.keys, m_lastView.bolts, m_lastView.margin, m_lastView.shopButtons, m_lastView.tickets);
	}

	public override void Destroy()
	{
		m_indicatorCreations.Clear();
		if (m_flyers.Count > 0)
		{
			for (int num = m_flyers.Count - 1; num >= 0; num--)
			{
				FlyingResource flyingResource = m_flyers[num] as FlyingResource;
				UIComponent uIComponent = m_flyers[num] as UIComponent;
				if (flyingResource != null)
				{
					flyingResource.Destroy();
				}
				else if (uIComponent != null)
				{
					uIComponent.Destroy();
				}
				m_flyers.RemoveAt(num);
			}
			m_removeList.Clear();
		}
		if (m_flyerCamera != null)
		{
			CameraS.RemoveCamera(m_flyerCamera);
			m_flyerCamera = null;
		}
		base.Destroy();
	}
}
