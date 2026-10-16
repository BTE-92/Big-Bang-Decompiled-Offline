using System.Collections;
using UnityEngine;

public class EffectCollectible : MonoBehaviour
{
	public AnimationCurve idleHover = new AnimationCurve();

	public float idleRotation = 0.2f;

	public float idleLength = 1f;

	public AnimationCurve spinHover = new AnimationCurve();

	public AnimationCurve spinRotate = new AnimationCurve();

	public float spinLength = 1f;

	public Transform collectible;

	public Transform collectibleParent;

	public Transform collectibleFx;

	private float curTime;

	private bool isCoroutineRunning;

	private bool isCoroutineBRunning;

	private Renderer collectibleRenderer;

	public bool hideFx;

	private void Start()
	{
		collectibleRenderer = collectible.GetComponent<Renderer>();
		collectibleParent.Rotate(0f, Random.Range(0f, 360f), 0f);
	}

	private void IdleCollectible()
	{
		curTime += Main.m_gameDeltaTime;
		if (curTime > idleLength)
		{
			curTime -= idleLength;
		}
		Vector3 localPosition = collectibleParent.localPosition;
		localPosition.y = idleHover.Evaluate(curTime);
		collectibleParent.localPosition = localPosition;
		collectibleParent.Rotate(0f, idleRotation, 0f);
	}

	private IEnumerator SpinCollectibleRoutine()
	{
		isCoroutineRunning = true;
		float curSpinTime = 0f;
		while (curSpinTime <= spinLength)
		{
			curSpinTime += Main.m_gameDeltaTime;
			Vector3 localPos = collectible.localPosition;
			localPos.y = spinHover.Evaluate(curSpinTime);
			collectible.localPosition = localPos;
			collectible.Rotate(0f, 0f, spinRotate.Evaluate(curSpinTime));
			yield return null;
		}
		isCoroutineRunning = false;
	}

	public void SpinCoin()
	{
		if (!isCoroutineRunning)
		{
			StartCoroutine("SpinCollectibleRoutine");
		}
	}

	private void Update()
	{
		if (PsState.m_activeMinigame.m_gamePaused)
		{
			return;
		}
		if (!collectibleRenderer.isVisible && collectibleFx != null)
		{
			if (collectibleFx.gameObject.activeSelf)
			{
				collectibleFx.gameObject.SetActive(false);
			}
			return;
		}
		if (collectibleFx != null)
		{
			if (hideFx)
			{
				if (collectibleFx.gameObject.activeSelf)
				{
					collectibleFx.gameObject.SetActive(false);
				}
			}
			else if (!collectibleFx.gameObject.activeSelf)
			{
				collectibleFx.gameObject.SetActive(true);
			}
		}
		IdleCollectible();
	}
}
