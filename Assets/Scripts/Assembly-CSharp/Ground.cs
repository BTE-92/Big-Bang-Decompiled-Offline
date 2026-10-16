using UnityEngine;

public class Ground : BasicAssembledClass
{
	public string m_groundItemIdentifier;

	public string m_editorWheelFrameName;

	public float m_elasticity;

	public float m_friction;

	public Vector2 m_surfaceVelocity;

	public float m_smoothingAngle;

	public float m_depth;

	public float m_zOffset;

	public float m_segmentRadius;

	public string m_beltMaterialResourceName;

	public string m_frontMaterialResourceName;

	public string m_groundPropStorageName;

	public bool m_isBurning;

	public bool m_isFrozen;

	public bool m_isElectrified;

	public string m_tireRollSound;

	public string m_drawSound;

	public string m_driveFX;

	public string m_drawFx;

	public bool m_rectBrush;

	public bool m_marchHard;

	public bool m_destructible;

	public bool m_skiddable;

	public Ground(GraphElement _graphElement)
		: base(_graphElement)
	{
		m_elasticity = 0.5f;
		m_friction = 0.5f;
		m_surfaceVelocity = Vector2.zero;
		m_smoothingAngle = 90f;
		m_depth = 300f;
		m_zOffset = -10f;
		m_segmentRadius = 10f;
		m_beltMaterialResourceName = string.Empty;
		m_frontMaterialResourceName = string.Empty;
		m_tireRollSound = null;
		m_drawSound = null;
		m_driveFX = null;
		m_drawFx = null;
		m_rectBrush = false;
		m_marchHard = false;
		m_destructible = false;
		m_skiddable = false;
	}

	public virtual void Update()
	{
	}

	public virtual void StartedContactWithUnit(UnitC _unitC, ucpCollisionPair _pair, ucpCollisionPhase _phase)
	{
	}
}
