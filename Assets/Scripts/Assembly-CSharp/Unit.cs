using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Unit : BasicAssembledClass
{
	public UnitC m_unitC;

	public UnitType m_unitType;

	public string m_name;

	public Entity m_entity;

	private List<IComponent> m_bodyList;

	private int m_bodyListChecksum;

	public bool m_isDead;

	public bool m_checkForCrushing;

	public HitPointType m_hitPointType;

	public float m_hitPoints;

	public float m_maxHitPoints;

	public bool m_reactToBlastWaves;

	public float m_energy;

	public float m_maxEnergy;

	public float[] m_baseShield;

	public float[] m_currentShield;

	public List<StatModifierInfo> m_shieldModifiers;

	public float[] m_baseArmor;

	public float[] m_currentArmor;

	public List<StatModifierInfo> m_armorModifiers;

	public List<BuffInfo> m_buffs;

	public List<BuffInfo> m_debuffs;

	public bool m_isBurning;

	public bool m_canBurn;

	public bool m_isFrozen;

	public bool m_canFreeze;

	public bool m_isElectrified;

	public bool m_canElectrify;

	public int m_electricitySourceCount;

	public float m_lastElectrification;

	public float m_electrificationTime;

	public float m_temperature;

	public float m_burnTemperature;

	public float m_freezeTemperature;

	public List<ContactInfo> m_contacts;

	public float m_contactEndTreshold;

	public ContactState m_contactState;

	public float m_lastGroundContact;

	public bool m_isTeleporting;

	private int m_teleportingTicks;

	private int m_originalTeleTicks;

	private int m_teleportingState;

	public Vector3 m_teleportEndPos;

	public Vector3 m_teleportStartPos;

	private Vector3 m_centerMoveDelta;

	private bool m_teleportModifyAngle;

	private bool m_teleportFadeRotate;

	private bool m_goalTeleport;

	private float m_teleportOutAngle;

	private Vector2 m_teleportOutVelocity;

	private TweenStyle m_teleTweenStyle;

	private float m_teleTweenCurrentValue;

	private float m_teleTweenDuration;

	private float m_teleTweenEndValue;

	private float m_teleTweenStartValue;

	private float m_currentTeleTime;

	private TransformC m_teleportTransform;

	public bool m_teleported;

	public int m_crushTolerance;

	public int m_crushFramesCount;

	public Vector2 m_lastFrameVelocity;

	public int m_groundedChainIndex = -1;

	public bool m_immuneToGroundEffects;

	protected bool shieldModifiersChanged;

	protected bool armorModifiersChanged;

	public Unit(GraphElement _graphElement, UnitType _unitType)
		: base(_graphElement)
	{
		m_bodyListChecksum = 0;
		m_entity = EntityManager.AddEntity(new string[2] { "GTAG_UNIT", _graphElement.m_name });
		m_assembledEntities.Add(m_entity);
		m_unitC = PsS.AddUnit(m_entity, this);
		m_unitType = _unitType;
		m_bodyList = null;
		m_hitPointType = HitPointType.Health;
		m_hitPoints = 100f;
		m_maxHitPoints = 100f;
		m_reactToBlastWaves = true;
		m_energy = 100f;
		m_maxEnergy = 100f;
		m_baseShield = new float[6];
		m_currentShield = new float[6];
		m_baseArmor = new float[6];
		m_currentArmor = new float[6];
		m_shieldModifiers = new List<StatModifierInfo>();
		CalculateCurrentShield();
		m_armorModifiers = new List<StatModifierInfo>();
		SetAllBaseArmours();
		CalculateCurrentArmor();
		m_buffs = new List<BuffInfo>();
		m_debuffs = new List<BuffInfo>();
		m_temperature = 0f;
		m_freezeTemperature = -1f;
		m_burnTemperature = 1f;
		m_contacts = new List<ContactInfo>();
		m_contactEndTreshold = 0.1f;
		m_contactState = ContactState.OnAir;
		m_isTeleporting = false;
		m_teleportingTicks = (m_originalTeleTicks = 0);
		m_teleportingState = 0;
		m_teleportEndPos = Vector3.zero;
		m_centerMoveDelta = Vector3.zero;
		m_teleportModifyAngle = false;
		m_teleportFadeRotate = false;
		m_goalTeleport = false;
		m_teleportOutAngle = 0f;
		m_teleTweenCurrentValue = 0f;
		m_teleTweenDuration = 0f;
		m_teleTweenEndValue = 0f;
		m_teleTweenStartValue = 0f;
		m_currentTeleTime = 0f;
		m_teleported = false;
		m_checkForCrushing = false;
		m_crushFramesCount = 0;
		m_crushTolerance = 50000;
	}

	public virtual void CreateEditorTouchArea(float _width = 100f, float _height = 100f, TransformC _parentTC = null)
	{
		CreateGraphElementTouchArea(_width, _height, _parentTC);
	}

	public virtual void CreateEditorTouchArea(GameObject _collisionGO, TransformC _parentTC = null)
	{
		if (m_minigame.m_editing)
		{
			Mesh collisionMesh = ToolBox.DuplicateMesh((_collisionGO.GetComponent("MeshFilter") as MeshFilter).sharedMesh);
			CreateGraphElementTouchArea(collisionMesh, _parentTC);
		}
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public Vector3 GetZBufferBias()
	{
		UnityEngine.Random.seed = (int)m_graphElement.m_id;
		return new Vector3(0f, 0f, UnityEngine.Random.Range(-0.5f, 0.5f));
	}

	public virtual void SetAllBaseArmours()
	{
		m_baseArmor[1] = -1f;
		m_baseArmor[0] = -1f;
		m_baseArmor[2] = -1f;
	}

	public virtual void SetBaseArmor(DamageType _armorType, float _armorValue)
	{
		m_baseArmor[(int)_armorType] = _armorValue;
		CalculateCurrentArmor();
	}

	public virtual void SetBaseShield(DamageType _shieldType, float _shieldValue)
	{
		m_baseShield[(int)_shieldType] = _shieldValue;
		CalculateCurrentShield();
	}

	public void DestroyAllContacts()
	{
		for (int i = 0; i < m_contacts.Count; i++)
		{
			if (m_contacts[i].m_unit != null)
			{
				m_contacts[i].m_unit.RemoveUnitContactsImmediately(this);
			}
		}
		m_contacts.Clear();
	}

	public ContactState GetContactState(ChipmunkBodyC _body)
	{
		for (int num = m_contacts.Count - 1; num > -1; num--)
		{
			if (m_contacts[num].m_cmb.m_identifier != 1 && m_contacts[num].m_contactBody.body == _body.body)
			{
				return ContactState.OnContact;
			}
		}
		return ContactState.OnAir;
	}

	public bool Contact(CMBIdentifiers _identifier)
	{
		for (int num = m_contacts.Count - 1; num > -1; num--)
		{
			if (m_contacts[num].m_cmb.m_identifier == (int)_identifier)
			{
				return true;
			}
		}
		return false;
	}

	public bool AddGroundContact(ChipmunkBodyC _contactBody, ChipmunkBodyC _groundBody, Ground _ground, Vector2 _contactPos)
	{
		bool result = false;
		ContactInfo contactInfo = null;
		for (int num = m_contacts.Count - 1; num > -1; num--)
		{
			if (m_contacts[num].m_contactBody == _contactBody && m_contacts[num].m_ground == _ground)
			{
				contactInfo = m_contacts[num];
				break;
			}
		}
		if (contactInfo == null)
		{
			result = true;
			contactInfo = new ContactInfo(_contactBody, _groundBody, _ground, _contactPos);
			m_contacts.Add(contactInfo);
		}
		else
		{
			contactInfo.m_contactCount++;
			contactInfo.m_endTime = -1f;
			contactInfo.m_end = false;
			contactInfo.m_contactPoint = _contactPos;
		}
		if (_ground.m_isElectrified && m_canElectrify)
		{
			m_immuneToGroundEffects = IsImmuneToGroundEffects(contactInfo);
			if (!m_immuneToGroundEffects)
			{
				if (m_electricitySourceCount == 0)
				{
					SetElectrified(true, true, _contactPos);
				}
				m_electricitySourceCount++;
			}
		}
		return result;
	}

	public bool RemoveGroundContact(ChipmunkBodyC _contactBody, Ground _ground)
	{
		ContactInfo contactInfo = null;
		for (int num = m_contacts.Count - 1; num > -1; num--)
		{
			if (m_contacts[num].m_contactBody == _contactBody && m_contacts[num].m_ground == _ground)
			{
				contactInfo = m_contacts[num];
				break;
			}
		}
		if (contactInfo != null)
		{
			if (contactInfo.m_ground.m_isElectrified && m_canElectrify)
			{
				m_electricitySourceCount--;
				if (m_electricitySourceCount == 0)
				{
					SetElectrified(false);
				}
				if (m_electricitySourceCount < 0)
				{
					m_electricitySourceCount = 0;
				}
			}
			contactInfo.m_contactCount--;
			if (contactInfo.m_contactCount == 0)
			{
				contactInfo.m_end = true;
				contactInfo.m_endTime = Main.m_resettingGameTime;
			}
			return contactInfo.m_end;
		}
		return true;
	}

	public void AddUnitContact(ChipmunkBodyC _contactBody, ChipmunkBodyC _cmb, Unit _unit, Vector2 _contactPos)
	{
		ContactInfo contactInfo = null;
		for (int num = m_contacts.Count - 1; num > -1; num--)
		{
			if (m_contacts[num].m_cmb == _cmb && m_contacts[num].m_contactBody == _contactBody && m_contacts[num].m_unit == _unit)
			{
				contactInfo = m_contacts[num];
				break;
			}
		}
		if (contactInfo == null)
		{
			m_contacts.Add(new ContactInfo(_contactBody, _cmb, _unit, _contactPos));
			return;
		}
		contactInfo.m_contactCount++;
		contactInfo.m_endTime = -1f;
		contactInfo.m_end = false;
		contactInfo.m_contactPoint = _contactPos;
	}

	public void RemoveUnitContact(ChipmunkBodyC _contactBody, ChipmunkBodyC _cmb, Unit _unit)
	{
		ContactInfo contactInfo = null;
		for (int num = m_contacts.Count - 1; num > -1; num--)
		{
			if (m_contacts[num].m_contactBody == _contactBody && m_contacts[num].m_cmb == _cmb && m_contacts[num].m_unit == _unit)
			{
				contactInfo = m_contacts[num];
				break;
			}
		}
		if (contactInfo == null)
		{
			return;
		}
		contactInfo.m_contactCount--;
		if (contactInfo.m_contactCount == 0)
		{
			if (m_electricitySourceCount == 0 && contactInfo.m_unit.m_isElectrified && m_canElectrify && m_isElectrified)
			{
				SetElectrified(false);
			}
			contactInfo.m_end = true;
			contactInfo.m_endTime = Main.m_resettingGameTime;
		}
	}

	public void RemoveUnitContactsImmediately(Unit _unit)
	{
		List<ContactInfo> list = new List<ContactInfo>();
		for (int i = 0; i < m_contacts.Count; i++)
		{
			ContactInfo contactInfo = m_contacts[i];
			if (contactInfo.m_unit == _unit)
			{
				if (m_electricitySourceCount == 0 && _unit.m_isElectrified && m_canElectrify && m_isElectrified)
				{
					SetElectrified(false);
				}
				contactInfo.m_contactCount = 0;
				contactInfo.m_end = true;
				contactInfo.m_endTime = Main.m_resettingGameTime;
				HandleContactEnd(contactInfo);
				list.Add(contactInfo);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			m_contacts.Remove(list[j]);
		}
		if (m_contacts.Count == 0)
		{
			m_contactState = ContactState.OnAir;
		}
	}

	public virtual void KillingImpact(ChipmunkBodyC _collidingBody, Vector2 _point, Vector2 _normal, Vector2 _impulse)
	{
	}

	public List<IComponent> GetUnitBodyList()
	{
		if (m_bodyList == null || m_entity.m_componentsChecksum != m_bodyListChecksum)
		{
			m_bodyList = new List<IComponent>();
			m_bodyList = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, m_entity);
			m_bodyListChecksum = m_entity.m_componentsChecksum;
		}
		return m_bodyList;
	}

	public void AddSpeed(Vector2 _force, float _speedLimit)
	{
		List<IComponent> unitBodyList = GetUnitBodyList();
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < unitBodyList.Count; i++)
		{
			ChipmunkBodyC chipmunkBodyC = unitBodyList[i] as ChipmunkBodyC;
			zero += ChipmunkProWrapper.ucpBodyGetVel(chipmunkBodyC.body);
		}
		if (unitBodyList.Count <= 0)
		{
			return;
		}
		zero /= (float)unitBodyList.Count;
		if ((zero + _force).magnitude < _speedLimit)
		{
			for (int j = 0; j < unitBodyList.Count; j++)
			{
				ChipmunkBodyC chipmunkBodyC2 = unitBodyList[j] as ChipmunkBodyC;
				ChipmunkProWrapper.ucpBodyApplyImpulseAtWorldPoint(chipmunkBodyC2.body, _force * chipmunkBodyC2.m_mass, ChipmunkProWrapper.ucpBodyGetPos(chipmunkBodyC2.body));
			}
		}
	}

	public Vector3 GetUnitCenterPosition()
	{
		Vector2 zero = Vector2.zero;
		List<IComponent> unitBodyList = GetUnitBodyList();
		for (int i = 0; i < unitBodyList.Count; i++)
		{
			ChipmunkBodyC chipmunkBodyC = unitBodyList[i] as ChipmunkBodyC;
			zero += ChipmunkProWrapper.ucpBodyGetPos(chipmunkBodyC.body);
		}
		zero /= (float)unitBodyList.Count;
		return zero;
	}

	public bool isInsideGround()
	{
		bool result = false;
		List<IComponent> unitBodyList = GetUnitBodyList();
		for (int i = 0; i < unitBodyList.Count; i++)
		{
			ChipmunkBodyC chipmunkBodyC = unitBodyList[i] as ChipmunkBodyC;
			for (int j = 0; j < chipmunkBodyC.shapes.Count; j++)
			{
				IntPtr shapePtr = chipmunkBodyC.shapes[j].shapePtr;
				int layerAtWorldPos = AutoGeometryManager.GetLayerAtWorldPos(((!m_minigame.m_editing) ? ChipmunkProWrapper.ucpShapeGetBB(shapePtr) : ChipmunkProWrapper.ucpShapeCacheBB(shapePtr)).getCenter(), 64);
				if (layerAtWorldPos >= 0)
				{
					result = true;
				}
			}
		}
		return result;
	}

	public void MoveUnitRandomly(float _amount)
	{
		Vector2 vector = new Vector2(UnityEngine.Random.Range(0f - _amount, _amount), UnityEngine.Random.Range(0f - _amount, _amount));
		List<IComponent> unitBodyList = GetUnitBodyList();
		for (int i = 0; i < unitBodyList.Count; i++)
		{
			ChipmunkBodyC chipmunkBodyC = unitBodyList[i] as ChipmunkBodyC;
			Vector2 pos = ChipmunkProWrapper.ucpBodyGetPos(chipmunkBodyC.body) + vector;
			ChipmunkProWrapper.ucpBodySetPos(chipmunkBodyC.body, pos);
		}
	}

	public void SetAsTeleporting(int _ticks, Transform _from, Transform _to, bool _rotateInFade = true, bool _modifyOutAngle = false, bool _goalTeleport = false)
	{
		m_teleportStartPos = _from.position;
		m_teleportEndPos = _to.position;
		m_teleportEndPos.z = 0f;
		m_centerMoveDelta = Vector3.zero;
		float num = 0f;
		m_teleportOutAngle = 0f;
		Vector2 zero = Vector2.zero;
		SoundS.PlaySingleShot("/InGame/Units/PortalTeleport_In", _from.position);
		List<IComponent> unitBodyList = GetUnitBodyList();
		Vector2 zero2 = Vector2.zero;
		for (int i = 0; i < unitBodyList.Count; i++)
		{
			ChipmunkBodyC chipmunkBodyC = unitBodyList[i] as ChipmunkBodyC;
			zero += ChipmunkProWrapper.ucpBodyGetVel(chipmunkBodyC.body);
			zero2 += ChipmunkProWrapper.ucpBodyGetPos(chipmunkBodyC.body);
		}
		zero2 /= (float)unitBodyList.Count;
		m_centerMoveDelta = _from.position - (Vector3)zero2;
		m_centerMoveDelta /= (float)_ticks;
		num = zero.magnitude;
		zero = zero.normalized;
		num /= (float)unitBodyList.Count;
		m_teleportOutVelocity = zero * num;
		if (_modifyOutAngle)
		{
			float num2 = 1f;
			Vector3 vector = (((!(_to.rotation.eulerAngles.z < 180f) || !(_to.rotation.eulerAngles.z > 0f)) && (!(_to.rotation.eulerAngles.z < -180f) || !(_to.rotation.eulerAngles.z > -360f))) ? Vector3.right : (-Vector3.right));
			if (Vector3.Cross(vector, _to.up).z < 0f)
			{
				num2 = -1f;
			}
			Vector2 vector2 = num * _to.up;
			if ((Mathf.Abs(_to.rotation.eulerAngles.z) > 25f && Mathf.Abs(_to.rotation.z) < 155f) || (Mathf.Abs(_to.rotation.z) < 335f && Mathf.Abs(_to.rotation.eulerAngles.z) > 205f))
			{
				m_teleportEndPos += _to.right * vector.x * 40f;
				m_teleportOutAngle = Vector3.Angle(vector, _to.up) * num2 * ((float)Math.PI / 180f);
			}
			if (num <= 700f)
			{
				num += 300f;
			}
			m_teleportOutVelocity = (Vector2)_to.up * num;
		}
		m_teleportingTicks = (m_originalTeleTicks = _ticks);
		m_teleportingState = 0;
		m_teleportModifyAngle = _modifyOutAngle;
		m_teleportFadeRotate = _rotateInFade;
		m_goalTeleport = _goalTeleport;
		m_teleTweenStyle = TweenStyle.QuadIn;
		m_teleTweenStartValue = 0f;
		m_teleTweenEndValue = 360f;
		m_currentTeleTime = 0f;
		m_teleTweenDuration = (float)_ticks / 60f;
		EntityManager.SetActivityOfEntity(m_entity, false, false, true, false);
		List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.CameraTarget, m_entity);
		for (int j = 0; j < componentsByEntity.Count; j++)
		{
			CameraTargetC cameraTargetC = componentsByEntity[j] as CameraTargetC;
			cameraTargetC.m_active = true;
			cameraTargetC.frameTC.m_active = true;
		}
		m_isTeleporting = true;
		m_teleported = true;
		componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.Transform, m_entity);
		m_teleportTransform = TransformS.AddComponent(m_entity, zero2);
		foreach (TransformC item in componentsByEntity)
		{
			if (item.parent == null)
			{
				TransformS.ParentComponent(item, m_teleportTransform);
			}
		}
	}

	public void TeleportUpdate()
	{
		if (!m_isTeleporting)
		{
			return;
		}
		TransformC teleportTransform = m_teleportTransform;
		if (teleportTransform == null)
		{
			return;
		}
		m_currentTeleTime += Main.m_gameDeltaTime;
		m_teleTweenCurrentValue = TweenS.tween(m_teleTweenStyle, m_currentTeleTime, m_teleTweenDuration, m_teleTweenStartValue, m_teleTweenEndValue);
		switch (m_teleportingState)
		{
		case 0:
		{
			TransformS.SetGlobalPosition(teleportTransform, teleportTransform.transform.position + m_centerMoveDelta);
			if (m_teleportFadeRotate)
			{
				TransformS.SetGlobalRotation(teleportTransform, Vector3.forward * m_teleTweenCurrentValue);
			}
			float scale2 = 0.1f + (float)m_teleportingTicks / (float)m_originalTeleTicks * 0.9f;
			TransformS.SetScale(teleportTransform, scale2);
			m_teleportingTicks--;
			if (m_teleportingTicks <= 0)
			{
				m_teleportingState++;
				if (m_goalTeleport)
				{
					m_teleportingState = 3;
					EntityManager.SetActivityOfEntity(m_entity, false);
				}
				m_teleportingTicks = m_originalTeleTicks;
			}
			break;
		}
		case 1:
		{
			TransformS.SetGlobalPosition(teleportTransform, m_teleportEndPos);
			m_teleTweenStyle = TweenStyle.QuadOut;
			m_currentTeleTime = 0f;
			m_teleTweenStartValue = 0f;
			m_teleTweenEndValue = 360f + 57.29578f * m_teleportOutAngle;
			m_teleportingState++;
			List<IComponent> componentsByEntity2 = EntityManager.GetComponentsByEntity(ComponentType.CameraTarget, m_entity);
			for (int j = 0; j < componentsByEntity2.Count; j++)
			{
				CameraTargetC cameraTargetC = componentsByEntity2[j] as CameraTargetC;
				CameraS.ResetFramePosition(cameraTargetC);
				if (CameraS.isPrimaryTarget(cameraTargetC))
				{
					CameraS.SnapMainCameraPos(m_teleportEndPos);
				}
			}
			break;
		}
		case 2:
		{
			if (m_teleportFadeRotate)
			{
				TransformS.SetGlobalRotation(teleportTransform, Vector3.forward * m_teleTweenCurrentValue);
			}
			else
			{
				TransformS.SetGlobalRotation(teleportTransform, Vector3.forward * 57.29578f * m_teleportOutAngle);
			}
			float scale = 1f - (float)m_teleportingTicks / (float)m_originalTeleTicks * 0.9f;
			TransformS.SetScale(teleportTransform, scale);
			m_teleportingTicks--;
			if (m_teleportingTicks <= 0)
			{
				m_teleportingState++;
			}
			break;
		}
		case 3:
		{
			if (m_goalTeleport)
			{
				break;
			}
			TransformS.SetScale(teleportTransform, 1f);
			SoundS.PlaySingleShot("/InGame/Units/PortalTeleport_Out", m_teleportTransform.transform.position);
			TransformS.RemoveComponent(m_teleportTransform);
			m_teleportTransform = null;
			EntityManager.SetActivityOfEntity(m_entity, true, false, true, false);
			List<IComponent> componentsByEntity = EntityManager.GetComponentsByEntity(ComponentType.ChipmunkBody, m_entity);
			Vector3 vector = new Vector3(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f), 0f);
			for (int i = 0; i < componentsByEntity.Count; i++)
			{
				ChipmunkBodyC chipmunkBodyC = componentsByEntity[i] as ChipmunkBodyC;
				ChipmunkProWrapper.ucpBodySetPos(chipmunkBodyC.body, chipmunkBodyC.TC.transform.position + vector);
				if (m_teleportModifyAngle)
				{
					ChipmunkProWrapper.ucpBodySetAngle(chipmunkBodyC.body, m_teleportOutAngle);
				}
				ChipmunkProWrapper.ucpBodySetVel(chipmunkBodyC.body, m_teleportOutVelocity);
			}
			m_isTeleporting = false;
			break;
		}
		}
	}

	public virtual void InactiveUpdate()
	{
		if (!PsState.m_physicsPaused && m_entity != null)
		{
			TeleportUpdate();
		}
	}

	public virtual void Update()
	{
		if (!m_minigame.m_editing && !m_minigame.m_editing && m_entity != null)
		{
			shieldModifiersChanged = false;
			for (int num = m_shieldModifiers.Count - 1; num > -1; num--)
			{
				if (m_shieldModifiers[num].m_endTime != -1f && m_shieldModifiers[num].m_endTime < Main.m_resettingGameTime)
				{
					m_shieldModifiers.RemoveAt(num);
					shieldModifiersChanged = true;
				}
			}
			if (shieldModifiersChanged)
			{
				CalculateCurrentShield();
			}
			armorModifiersChanged = false;
			for (int num2 = m_armorModifiers.Count - 1; num2 > -1; num2--)
			{
				if (m_armorModifiers[num2].m_endTime != -1f && m_armorModifiers[num2].m_endTime < Main.m_resettingGameTime)
				{
					m_armorModifiers.RemoveAt(num2);
					armorModifiersChanged = true;
				}
			}
			if (armorModifiersChanged)
			{
				CalculateCurrentArmor();
			}
			for (int num3 = m_buffs.Count - 1; num3 > -1; num3--)
			{
				HandleBuff(m_buffs[num3]);
			}
			for (int num4 = m_debuffs.Count - 1; num4 > -1; num4--)
			{
				HandleBuff(m_debuffs[num4]);
			}
			int num5 = 0;
			int num6 = -1;
			bool flag = m_electricitySourceCount > 0;
			for (int num7 = m_contacts.Count - 1; num7 > -1; num7--)
			{
				ContactInfo contactInfo = m_contacts[num7];
				bool flag2 = contactInfo.m_cmb.m_identifier == 1;
				if (contactInfo.m_began)
				{
					HandleContactBegin(contactInfo);
				}
				else if (contactInfo.m_end && contactInfo.m_endTime < Main.m_resettingGameTime - m_contactEndTreshold)
				{
					HandleContactEnd(contactInfo);
					m_contacts.Remove(contactInfo);
					flag2 = false;
				}
				if (flag2)
				{
					num5++;
				}
				if (contactInfo.m_ground != null || (contactInfo.m_cmb != null && contactInfo.m_cmb.body != IntPtr.Zero && ChipmunkProWrapper.ucpBodyGetType(contactInfo.m_cmb.body) != ucpBodyType.DYNAMIC))
				{
					num6 = 0;
				}
				else if (contactInfo.m_unit != null && contactInfo.m_unit.m_groundedChainIndex > -1)
				{
					if (num6 == -1 && (contactInfo.m_unit.m_groundedChainIndex < m_groundedChainIndex || m_groundedChainIndex == -1))
					{
						num6 = contactInfo.m_unit.m_groundedChainIndex + 1;
					}
					else if (contactInfo.m_unit.m_groundedChainIndex < num6)
					{
						num6 = contactInfo.m_unit.m_groundedChainIndex + 1;
					}
				}
				bool flag3 = contactInfo.m_unit != null;
				bool flag4 = contactInfo.m_ground != null;
				if (flag3 && contactInfo.m_unit.m_isElectrified && !m_immuneToGroundEffects && !contactInfo.m_end)
				{
					m_immuneToGroundEffects = IsImmuneToGroundEffects(contactInfo);
					if (!m_immuneToGroundEffects)
					{
						flag = true;
					}
				}
				if (flag4 && contactInfo.m_ground.m_isElectrified && !m_immuneToGroundEffects && !contactInfo.m_end)
				{
					m_immuneToGroundEffects = IsImmuneToGroundEffects(contactInfo);
					if (!m_immuneToGroundEffects)
					{
						flag = true;
					}
				}
			}
			if (num6 < 20)
			{
				m_groundedChainIndex = num6;
			}
			else
			{
				m_groundedChainIndex = -1;
			}
			if (m_contacts.Count - num5 == 0)
			{
				m_contactState = ContactState.OnAir;
				m_lastGroundContact = Main.m_resettingGameTime;
			}
			else
			{
				m_contactState = ContactState.OnContact;
			}
			if (flag)
			{
				if (!m_isElectrified && m_canElectrify)
				{
					SetElectrified(true);
				}
			}
			else if (m_isElectrified && m_canElectrify)
			{
				SetElectrified(false);
			}
			if (m_isElectrified)
			{
				m_electrificationTime += Main.m_gameDeltaTime;
			}
			if (m_checkForCrushing)
			{
				CheckCrush();
			}
		}
		m_immuneToGroundEffects = false;
	}

	protected virtual bool IsImmuneToGroundEffects(ContactInfo _ci)
	{
		return false;
	}

	private void HandleBuff(BuffInfo _bi)
	{
		if (_bi.m_new)
		{
			if (_bi.m_buff.m_shieldModifier != null)
			{
				ModifyShield(_bi.m_buff.m_shieldModifier);
			}
			if (_bi.m_buff.m_armorModifier != null)
			{
				ModifyArmor(_bi.m_buff.m_armorModifier);
			}
			ApplyBuffBegan(_bi);
		}
		else if (_bi.m_buff.m_duration >= 0f && _bi.m_lastTick < Main.m_resettingGameTime)
		{
			RemoveBuff(_bi, true);
		}
		else if (_bi.m_buff.m_interval >= 0f && _bi.m_nextTick < Main.m_resettingGameTime)
		{
			ApplyBuffTick(_bi);
			_bi.m_nextTick += _bi.m_buff.m_interval;
		}
	}

	private void HandleContactBegin(ContactInfo ci)
	{
		ci.m_began = false;
		bool flag = ci.m_ground != null;
		bool flag2 = ci.m_unit != null;
		if (flag)
		{
			GroundContactStart(ci);
		}
		else if (flag2)
		{
			UnitContactStart(ci);
		}
	}

	private void HandleContactEnd(ContactInfo ci)
	{
		bool flag = ci.m_ground != null;
		bool flag2 = ci.m_unit != null;
		if (flag)
		{
			GroundContactEnd(ci);
		}
		else if (flag2)
		{
			UnitContactEnd(ci);
		}
	}

	private void CheckCrush()
	{
		if (m_contacts.Count < 2)
		{
			return;
		}
		List<IComponent> unitBodyList = GetUnitBodyList();
		for (int i = 0; i < unitBodyList.Count; i++)
		{
			ChipmunkBodyC chipmunkBodyC = unitBodyList[i] as ChipmunkBodyC;
			if (chipmunkBodyC.m_isSleeping)
			{
				continue;
			}
			float magnitude = ChipmunkProWrapper.ucpBodyGetForceVectorSum(chipmunkBodyC.body).magnitude;
			float num = ChipmunkProWrapper.ucpBodyGetForceMagnitudeSum(chipmunkBodyC.body);
			if (num > (float)m_crushTolerance)
			{
				float num2 = magnitude / num;
				if (!float.IsNaN(num2))
				{
					float num3 = 1f - num2;
					if (num3 > 0.8f)
					{
						m_crushFramesCount++;
					}
					if (m_crushFramesCount > 2)
					{
						EmergencyKill();
						break;
					}
				}
			}
			else if (num < (float)m_crushTolerance * 0.5f)
			{
				m_crushFramesCount = 0;
			}
		}
	}

	public Vector2 GetVelocityRelativeToContacts(ChipmunkBodyC _body)
	{
		Vector2 zero = Vector2.zero;
		int num = 0;
		Vector2 point = ChipmunkProWrapper.ucpBodyGetPos(_body.body);
		if (m_contactState == ContactState.OnContact)
		{
			for (int num2 = m_contacts.Count - 1; num2 > -1; num2--)
			{
				if (m_contacts[num2].m_contactBody == _body && m_contacts[num2].m_cmb != null)
				{
					Vector2 vector = ChipmunkProWrapper.ucpBodyGetVelAtWorldPoint(m_contacts[num2].m_cmb.body, point);
					zero += vector;
					num++;
				}
			}
		}
		if (num == 0)
		{
			return ChipmunkProWrapper.ucpBodyGetVel(_body.body);
		}
		zero /= (float)num;
		return ChipmunkProWrapper.ucpBodyGetVel(_body.body) - zero;
	}

	public int GetGroundContactsCount(ChipmunkBodyC _body)
	{
		int num = 0;
		for (int num2 = m_contacts.Count - 1; num2 > -1; num2--)
		{
			if (m_contacts[num2].m_ground != null && m_contacts[num2].m_contactBody == _body)
			{
				num++;
			}
		}
		return num;
	}

	public int GetDynamicContactsCount(ChipmunkBodyC _body, bool _discardEnded = false)
	{
		int num = 0;
		for (int num2 = m_contacts.Count - 1; num2 > -1; num2--)
		{
			if (m_contacts[num2].m_cmb != null && m_contacts[num2].m_contactBody == _body && ChipmunkProWrapper.ucpBodyGetType(m_contacts[num2].m_cmb.body) == ucpBodyType.DYNAMIC && (!_discardEnded || (_discardEnded && !m_contacts[num2].m_end)))
			{
				num++;
			}
		}
		return num;
	}

	public int GetRogueContactsCount(ChipmunkBodyC _body, bool _includeStatics)
	{
		int num = 0;
		for (int num2 = m_contacts.Count - 1; num2 > -1; num2--)
		{
			if (m_contacts[num2].m_cmb != null && m_contacts[num2].m_contactBody == _body && ChipmunkProWrapper.ucpBodyGetType(m_contacts[num2].m_cmb.body) == ucpBodyType.KINEMATIC)
			{
				bool flag = ChipmunkProWrapper.ucpBodyGetType(m_contacts[num2].m_cmb.body) == ucpBodyType.STATIC;
				if (_includeStatics || !flag)
				{
					num++;
				}
			}
		}
		return num;
	}

	public virtual void AddBuff(Buff _buff, IAssembledClass _source, Vector2 _contactPoint)
	{
		if (_buff == null || m_isDead)
		{
			return;
		}
		List<BuffInfo> list = ((!_buff.m_isDebuff) ? m_buffs : m_debuffs);
		BuffInfo buffInfo = null;
		bool flag = true;
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].m_buff == _buff)
			{
				buffInfo = list[i];
				flag = false;
				break;
			}
		}
		if (buffInfo == null)
		{
			buffInfo = new BuffInfo(_buff, _source, this, _contactPoint);
			list.Add(buffInfo);
		}
		else
		{
			buffInfo.m_contactPoint = _contactPoint;
		}
		if (flag)
		{
			if (_buff.m_duration >= 0f)
			{
				buffInfo.m_lastTick = Main.m_resettingGameTime + _buff.m_duration;
			}
			else
			{
				buffInfo.m_lastTick = -1f;
			}
			if (_buff.m_tickEffect != null && _buff.m_interval >= 0f)
			{
				buffInfo.m_nextTick = Main.m_resettingGameTime + _buff.m_interval;
			}
			else
			{
				buffInfo.m_nextTick = -1f;
			}
			HandleBuff(buffInfo);
		}
	}

	public virtual void ApplyBuffBegan(BuffInfo _bi)
	{
		_bi.m_new = false;
		if (!m_isDead && _bi.m_buff.m_beganEffect != null)
		{
			_bi.m_buff.PlayEffects(this, BuffState.Began, _bi.m_contactPoint);
			if (_bi.m_buff.m_isDebuff)
			{
				Damage(_bi.m_buff.m_beganEffect);
			}
			else
			{
				Heal(_bi.m_buff.m_beganEffect);
			}
		}
	}

	public virtual void ApplyBuffTick(BuffInfo _bi)
	{
		if (!m_isDead && _bi.m_buff.m_tickEffect != null)
		{
			_bi.m_buff.PlayEffects(this, BuffState.Tick, GetUnitCenterPosition());
			if (_bi.m_buff.m_isDebuff)
			{
				Damage(_bi.m_buff.m_tickEffect);
			}
			else
			{
				Heal(_bi.m_buff.m_tickEffect);
			}
		}
	}

	public virtual void ApplyBuffEnd(BuffInfo _bi)
	{
		if (!m_isDead && _bi.m_buff.m_endEffect != null)
		{
			if (_bi.m_buff.m_isDebuff)
			{
				Damage(_bi.m_buff.m_endEffect);
			}
			else
			{
				Heal(_bi.m_buff.m_endEffect);
			}
		}
	}

	public virtual void RemoveBuff(BuffInfo _buffInfo, bool _applyEndEffect)
	{
		List<BuffInfo> list = ((!_buffInfo.m_buff.m_isDebuff) ? m_buffs : m_debuffs);
		if (_buffInfo.m_buff.m_endEffect != null && _applyEndEffect)
		{
			ApplyBuffEnd(_buffInfo);
		}
		_buffInfo.m_buff.PlayEffects(this, BuffState.End, GetUnitCenterPosition());
		if (_buffInfo.m_buff.m_shieldModifier != null)
		{
			RemoveStatModifier(_buffInfo.m_buff.m_shieldModifier, m_shieldModifiers);
			CalculateCurrentShield();
		}
		if (_buffInfo.m_buff.m_armorModifier != null)
		{
			RemoveStatModifier(_buffInfo.m_buff.m_armorModifier, m_armorModifiers);
			CalculateCurrentArmor();
		}
		list.Remove(_buffInfo);
	}

	public virtual void RemoveBuff(Buff _buff)
	{
		Debug.LogWarning("remove: " + _buff.m_identifier);
		for (int num = m_buffs.Count; num > -1; num--)
		{
			if (m_buffs[num].m_buff == _buff)
			{
				RemoveBuff(m_buffs[num], false);
			}
		}
		for (int num2 = m_debuffs.Count; num2 > -1; num2--)
		{
			if (m_debuffs[num2].m_buff == _buff)
			{
				RemoveBuff(m_debuffs[num2], false);
			}
		}
	}

	public virtual void RemoveStatModifier(StatModifier _modifier, List<StatModifierInfo> _list)
	{
		for (int num = _list.Count - 1; num > -1; num--)
		{
			if (_list[num].m_modifier == _modifier)
			{
				_list.RemoveAt(num);
			}
		}
	}

	public virtual void ModifyShield(StatModifier _modifier)
	{
		if (_modifier == null || m_isDead)
		{
			return;
		}
		StatModifierInfo statModifierInfo = null;
		for (int i = 0; i < m_shieldModifiers.Count; i++)
		{
			if (m_shieldModifiers[i].m_modifier == _modifier)
			{
				statModifierInfo = m_shieldModifiers[i];
				break;
			}
		}
		if (statModifierInfo == null)
		{
			statModifierInfo = new StatModifierInfo(_modifier);
			statModifierInfo.m_stack = 1;
			m_shieldModifiers.Add(statModifierInfo);
		}
		else if (statModifierInfo.m_stack < statModifierInfo.m_maxStack)
		{
			statModifierInfo.m_stack++;
		}
		if (statModifierInfo.m_modifier.m_duration >= 0f)
		{
			statModifierInfo.m_endTime = Main.m_resettingGameTime + statModifierInfo.m_modifier.m_duration;
		}
		else
		{
			statModifierInfo.m_endTime = -1f;
		}
	}

	public virtual void ModifyArmor(StatModifier _modifier)
	{
		if (_modifier == null)
		{
			return;
		}
		StatModifierInfo statModifierInfo = null;
		for (int i = 0; i < m_armorModifiers.Count; i++)
		{
			if (m_armorModifiers[i].m_modifier == _modifier)
			{
				statModifierInfo = m_armorModifiers[i];
				break;
			}
		}
		if (statModifierInfo == null)
		{
			statModifierInfo = new StatModifierInfo(_modifier);
			statModifierInfo.m_stack = 1;
			m_armorModifiers.Add(statModifierInfo);
		}
		else if (statModifierInfo.m_stack < statModifierInfo.m_maxStack)
		{
			statModifierInfo.m_stack++;
		}
		if (statModifierInfo.m_modifier.m_duration >= 0f)
		{
			statModifierInfo.m_endTime = Main.m_resettingGameTime + statModifierInfo.m_modifier.m_duration;
		}
		else
		{
			statModifierInfo.m_endTime = -1f;
		}
	}

	public virtual void CalculateCurrentShield()
	{
		for (int i = 0; i < m_currentShield.Length; i++)
		{
			m_currentShield[i] = m_baseShield[i];
			if (m_currentShield[i] > -1f)
			{
				for (int j = 0; j < m_shieldModifiers.Count; j++)
				{
					m_currentShield[i] *= m_shieldModifiers[j].m_modifier.m_multipler;
				}
			}
		}
	}

	public virtual void CalculateCurrentArmor()
	{
		for (int i = 0; i < m_currentArmor.Length; i++)
		{
			m_currentArmor[i] = m_baseArmor[i];
			if (m_currentArmor[i] > -1f)
			{
				for (int j = 0; j < m_armorModifiers.Count; j++)
				{
					m_currentArmor[i] *= m_armorModifiers[j].m_modifier.m_multipler;
				}
			}
		}
	}

	public virtual void Heal(Damage _health, int _multipler = 1)
	{
		if (_health != null && !m_isDead)
		{
			float[] amount = _health.m_amount;
			float num = 0f;
			for (int i = 0; i < amount.Length; i++)
			{
				num += amount[i];
			}
			m_hitPoints += num * (float)_multipler;
			if (m_hitPoints > m_maxHitPoints)
			{
				m_hitPoints = m_maxHitPoints;
			}
			Nurture(num * (float)_multipler);
		}
	}

	public virtual void Damage(Damage _damage, float _multiplier = 1f, Unit _source = null)
	{
		Vehicle vehicle = this as Vehicle;
		if (_damage == null || m_isDead || (vehicle != null && PsState.m_activeMinigame.m_playerReachedGoal))
		{
			return;
		}
		float[] amount = _damage.m_amount;
		int damageType = 0;
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < amount.Length; i++)
		{
			if (m_currentArmor[i] == -1f || m_currentShield[i] == -1f || amount[i] == 0f)
			{
				continue;
			}
			float num3 = amount[i] * _multiplier;
			num3 -= m_currentShield[i];
			num3 -= m_currentArmor[i];
			if (num3 > 0f)
			{
				num2 += num3;
				if (num3 > num)
				{
					num = num3;
					damageType = i;
				}
			}
		}
		if (num2 > 0f)
		{
			if (m_hitPointType == HitPointType.Lives)
			{
				m_hitPoints -= 1f;
			}
			else
			{
				m_hitPoints -= num2;
			}
			if (m_hitPoints <= 0f)
			{
				Kill((DamageType)damageType, num2);
			}
			else
			{
				Wound((DamageType)damageType, num2);
			}
		}
	}

	public virtual void Wound(DamageType _damageType, float _totalDamage)
	{
		Debug.Log(m_name + " - Damage: " + _totalDamage + ", Hit Points: " + m_hitPoints + "/" + m_maxHitPoints);
	}

	public virtual void Nurture(float _totalHeal)
	{
		Debug.Log(m_name + " - Heal: " + _totalHeal + ", Hit Points: " + m_hitPoints + "/" + m_maxHitPoints);
	}

	public virtual void Kill(DamageType _damageType, float _totalDamage)
	{
		m_isDead = true;
	}

	public virtual void EmergencyKill()
	{
		m_isDead = true;
		Destroy();
	}

	public uint GetGroup()
	{
		return (uint)((m_entity.m_index + 1) * 100);
	}

	public virtual void GroundContactStart(ContactInfo _contactInfo)
	{
	}

	public virtual void GroundContactEnd(ContactInfo _contactInfo)
	{
	}

	public virtual void UnitContactStart(ContactInfo _contactInfo)
	{
	}

	public virtual void UnitContactEnd(ContactInfo _contactInfo)
	{
	}

	public virtual void SetBurning(bool _burn)
	{
	}

	public virtual void SetFrozen(bool _burn)
	{
	}

	public virtual void SetElectrified(bool _electrify, bool _affectContacts = true, Vector2 _contactPoint = default(Vector2))
	{
		if (_electrify)
		{
			Damage damage = new Damage(DamageType.Electric, float.MaxValue);
			Damage(damage);
			if (_contactPoint != Vector2.zero && m_lastElectrification + 0.25f < Main.m_resettingGameTime)
			{
				EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject(RESOURCE.ElectricBurst_GameObject), _contactPoint, Vector3.zero, 3f, "GTAG_AUTODESTROY");
				SoundS.PlaySingleShot("/Ingame/ElectricZap", _contactPoint);
			}
			m_isElectrified = true;
			m_lastElectrification = Main.m_resettingGameTime;
			return;
		}
		if (!_affectContacts)
		{
			m_isElectrified = false;
			return;
		}
		List<Unit> list = new List<Unit>();
		m_isElectrified = HasElectricSourceContact(this, list);
		if (m_isElectrified)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i].m_isElectrified)
			{
				list[i].SetElectrified(false, false);
			}
		}
	}

	public virtual bool HasElectricSourceContact(Unit _unit, List<Unit> _contactGroup)
	{
		bool flag = m_electricitySourceCount > 0;
		for (int i = 0; i < m_contacts.Count; i++)
		{
			ContactInfo contactInfo = m_contacts[i];
			if (contactInfo.m_unit != null && !contactInfo.m_end && !_contactGroup.Contains(contactInfo.m_unit))
			{
				_contactGroup.Add(contactInfo.m_unit);
				bool flag2 = HasElectricSourceContact(contactInfo.m_unit, _contactGroup);
				if (!flag)
				{
					flag = flag2;
				}
			}
		}
		return flag;
	}

	public virtual void CreateUpgradeFX()
	{
		EntityManager.AddTimedFXEntity(ResourceManager.GetGameObject(RESOURCE.GeneralUpgrade_GameObject), m_graphElement.m_position, Vector3.zero, 5f, "GTAG_AUTODESTROY");
	}

	public virtual List<int> ParseUpgradeValues(Hashtable _upgrades)
	{
		return null;
	}

	public virtual Hashtable GetUpgradeValues()
	{
		return null;
	}

	public virtual List<string> GetNodeUpgradeNames()
	{
		return null;
	}

	public virtual List<KeyValuePair<string, int>> GetUpgrades()
	{
		return null;
	}

	public virtual int GetTier(List<int> _upgradeValues)
	{
		int num = 99999999;
		for (int i = 0; i < _upgradeValues.Count; i++)
		{
			if (_upgradeValues[i] < num)
			{
				num = _upgradeValues[i];
			}
		}
		return Mathf.FloorToInt((float)num / 5f) + 1;
	}

	public virtual void AdjustTiers(ref List<int> _upgradeValues)
	{
		bool flag = false;
		int num = GetTier(_upgradeValues) - 1;
		int num2 = num;
		for (int i = 0; i < _upgradeValues.Count; i++)
		{
			int num3 = Mathf.FloorToInt((float)_upgradeValues[i] / 5f);
			int num4 = _upgradeValues[i] - num3 * 5;
			if (num4 > 0 && num3 > num2)
			{
				num2 = num3;
				flag = true;
			}
			else if (num4 == 0 && num3 - num > 1 && num3 > num2)
			{
				num2 = num3 - 1;
				flag = true;
			}
		}
		if (!flag)
		{
			return;
		}
		for (int j = 0; j < _upgradeValues.Count; j++)
		{
			int num5 = Mathf.FloorToInt((float)_upgradeValues[j] / 5f);
			if (num5 < num2)
			{
				_upgradeValues[j] = num2 * 5;
			}
		}
	}

	public int ToInt(object _value)
	{
		return Convert.ToInt32(_value);
	}

	public float ToFloat(object _value)
	{
		return Convert.ToSingle(_value);
	}

	public virtual float UpgradeLevelToValue(int _level, int[] _ranges, float[] _values)
	{
		if (_level <= _ranges[0])
		{
			return _values[0];
		}
		if (_level >= _ranges[_ranges.Length - 1])
		{
			return _values[_values.Length - 1];
		}
		for (int i = 1; i < _ranges.Length - 1; i++)
		{
			if (_level <= _ranges[i])
			{
				return (float)(_level - _ranges[i - 1]) / ((float)(_ranges[i] - _ranges[i - 1]) / (_values[i] - _values[i - 1])) + _values[i - 1];
			}
		}
		return (float)(_level - _ranges[_ranges.Length - 2]) / ((float)(_ranges[_ranges.Length - 1] - _ranges[_ranges.Length - 2]) / (_values[_values.Length - 1] - _values[_values.Length - 2])) + _values[_values.Length - 2];
	}

	public virtual int VehicleValueToUpgradeLevel(float _value, int[] _ranges, float[] _values)
	{
		if (_value <= _values[0])
		{
			return _ranges[0];
		}
		if (_value >= _values[_values.Length - 1])
		{
			return _ranges[_ranges.Length - 1];
		}
		for (int i = 1; i < _values.Length - 1; i++)
		{
			if (_value <= _values[i])
			{
				return GetRoundedValue((_value - _values[i - 1]) / ((_values[i] - _values[i - 1]) / (float)(_ranges[i] - _ranges[i - 1])) + (float)_ranges[i - 1]);
			}
		}
		return GetRoundedValue((_value - _values[_values.Length - 2]) / ((_values[_values.Length - 1] - _values[_values.Length - 2]) / (float)(_ranges[_ranges.Length - 1] - _ranges[_ranges.Length - 2])) + (float)_ranges[_ranges.Length - 2]);
	}

	public int GetRoundedValue(float _value)
	{
		float num = _value - (float)Mathf.FloorToInt(_value);
		if (num > 0.1f)
		{
			return Mathf.CeilToInt(_value);
		}
		return Mathf.FloorToInt(_value);
	}
}
