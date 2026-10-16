using UnityEngine;

public class CameraTargetC : BasicComponent
{
	public TransformC targetTC;

	public Vector3 lastTargetTCPos;

	public TransformC frameTC;

	public Vector3 lastFrameTCPos;

	public cpBB targetFrame;

	public Vector2 offset;

	public float secondaryTargetTransMult;

	public Vector2 framePeek;

	public Vector2 translatedCenter;

	public float interpolateSpeed;

	public float frameScaleVelocityMultiplier;

	public Vector2 frameGrowVelocityMultiplier;

	public Vector2 framePosVelocityMultiplier;

	public Vector2 frameSlopRadiusMinMax;

	public Vector2 velAngleChangeMult;

	public Vector2 angleLimits;

	public float activeRadius;

	public Vector2 framePeekShiftMax;

	public float framePeekShiftMultiplier;

	public cpBB frameWorldBounds;

	public PrefabC framePrefab;

	public CameraTargetC()
		: base(ComponentType.CameraTarget)
	{
	}

	public override void Reset()
	{
		base.Reset();
		interpolateSpeed = 1f / 30f;
		frameGrowVelocityMultiplier = Vector2.zero;
		framePosVelocityMultiplier = Vector2.zero;
		frameSlopRadiusMinMax = Vector2.zero;
		lastFrameTCPos = Vector3.zero;
		frameScaleVelocityMultiplier = 0f;
		offset = Vector2.zero;
		framePrefab = null;
		frameTC = null;
		velAngleChangeMult = Vector2.zero;
		angleLimits = new Vector2(90f, 90f);
		activeRadius = float.MaxValue;
		secondaryTargetTransMult = 1f;
		frameWorldBounds = new cpBB(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);
		framePeekShiftMax = Vector2.zero;
		framePeekShiftMultiplier = 0f;
		translatedCenter = Vector2.zero;
	}
}
