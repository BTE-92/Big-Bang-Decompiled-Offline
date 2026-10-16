using UnityEngine;

public class EffectDirectionBoost : MonoBehaviour, IMonoBehaviour
{
	private MonoBehaviourC _component;

	public float boostEffectLength = 0.5f;

	private float effectTimer;

	public AnimationCurve yCurve;

	public AnimationCurve xzCurve;

	public Transform circleTransform;

	public ParticleSystem particleCircles;

	public ParticleSystem particleSparks;

	public ParticleSystem particleBoostSparks;

	public MonoBehaviourC m_component
	{
		get
		{
			return _component;
		}
		set
		{
			_component = value;
		}
	}

	public void StartEffect()
	{
		effectTimer = boostEffectLength;
		particleBoostSparks.Play();
	}

	private void AnimateEffect()
	{
		effectTimer -= GetDeltaTime();
		if (effectTimer <= 0f)
		{
			circleTransform.localScale = new Vector3(1f, 1f, 1f);
			return;
		}
		float z = yCurve.Evaluate(1f - effectTimer / boostEffectLength);
		float num = xzCurve.Evaluate(1f - effectTimer / boostEffectLength);
		circleTransform.localScale = new Vector3(num, num, z);
	}

	private float GetDeltaTime()
	{
		return Main.m_gameDeltaTime;
	}

	public void TLUpdate()
	{
		if (effectTimer > 0f)
		{
			AnimateEffect();
		}
	}
}
