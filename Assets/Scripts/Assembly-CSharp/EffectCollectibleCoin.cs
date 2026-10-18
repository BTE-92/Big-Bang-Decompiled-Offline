using System.Collections;
using UnityEngine;

public class EffectCollectibleCoin : MonoBehaviour
{
	public AnimationCurve idleHover = new AnimationCurve();

	public float idleRotation = 0.2f;

	public float idleLength = 1f;

	public AnimationCurve spinHover = new AnimationCurve();

	public AnimationCurve spinRotate = new AnimationCurve();

	public float spinLength = 1f;

	private Transform coin;

	private Transform coinHover;

	private Transform coinFx;

	private ParticleSystem pickUpEffect;

	private float curTime;

	private bool isCoroutineRunning;

	private bool isCoroutineBRunning;

	private Renderer coinRenderer;

	private void Start()
	{
		coin = base.transform.Find("CoinHover/Coin");
		coinFx = base.transform.Find("CoinHover/Coin/Fx");
		coinRenderer = coin.gameObject.GetComponent<Renderer>();
		coinHover = base.transform.Find("CoinHover");
		pickUpEffect = base.transform.Find("PickUpSparks").GetComponent<ParticleSystem>();
		coinHover.Rotate(0f, Random.Range(0f, 360f), 0f);
	}

	private void IdleCoin()
	{
		curTime += Main.m_gameDeltaTime;
		if (curTime > idleLength)
		{
			curTime -= idleLength;
		}
		Vector3 localPosition = coinHover.localPosition;
		localPosition.y = idleHover.Evaluate(curTime);
		coinHover.localPosition = localPosition;
		coinHover.Rotate(0f, idleRotation, 0f);
	}

	private IEnumerator SpinCoinRoutine()
	{
		isCoroutineRunning = true;
		float curSpinTime = 0f;
		while (curSpinTime <= spinLength)
		{
			curSpinTime += Main.m_gameDeltaTime;
			Vector3 localPos = coin.localPosition;
			localPos.y = spinHover.Evaluate(curSpinTime);
			coin.localPosition = localPos;
			coin.Rotate(0f, 0f, spinRotate.Evaluate(curSpinTime));
			yield return null;
		}
		isCoroutineRunning = false;
	}

	private IEnumerator PickUpCoinRoutine()
	{
		isCoroutineBRunning = true;
		coinHover.gameObject.SetActive(false);
		pickUpEffect.Play();
		while (pickUpEffect.IsAlive())
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}

	public void SpinCoin()
	{
		if (!isCoroutineRunning)
		{
			StartCoroutine("SpinCoinRoutine");
		}
	}

	public void PickUpCoin()
	{
		if (!isCoroutineBRunning)
		{
			StartCoroutine("PickUpCoinRoutine");
		}
	}

	private void Update()
	{
		if (!PsState.m_physicsPaused)
		{
			if (!coinRenderer.isVisible)
			{
				coinFx.gameObject.SetActive(false);
				return;
			}
			coinFx.gameObject.SetActive(true);
			IdleCoin();
		}
	}
}
