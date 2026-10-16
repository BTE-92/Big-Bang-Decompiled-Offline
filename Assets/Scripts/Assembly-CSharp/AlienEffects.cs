using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienEffects : MonoBehaviour
{
	private Animator alienAnimator;

	private SkinnedMeshRenderer[] bodyMeshes;

	private MeshRenderer[] eyeBalls;

	private List<GameObject> bones;

	private List<GameObject> eyeShadows;

	private float electroCuteLength = 1.25f;

	private ParticleSystem particleEffectA;

	private ParticleSystem particleEffectB;

	private ParticleSystem[] particleEffectStreaks;

	private ParticleSystem alienExplosion;

	private Material bodyMat;

	private Material eyeMat;

	private Material boneMat;

	private Color32 boneColorWhite;

	private Color32 boneColorBlack;

	private Color32 boneColorBurned;

	private float colorSwapSpeed = 0.1f;

	private bool isElectrocuting;

	private float flashTimer = 0.15f;

	private bool swapperToggle = true;

	private Transform headBone;

	private bool headWobbling;

	private float wobbleTime = 0.375f;

	private float wobbleTimer;

	public AnimationCurve wobbleCurve;

	private void Awake()
	{
		Initialize();
	}

	public void Initialize()
	{
		alienAnimator = GetComponent<Animator>();
		bodyMat = base.transform.Find("Body").GetComponent<SkinnedMeshRenderer>().material;
		base.transform.Find("Body").GetComponent<SkinnedMeshRenderer>().sharedMaterial = bodyMat;
		base.transform.Find("Hips/Spine1/Spine2/Neck/Head/EyeLids").GetComponent<SkinnedMeshRenderer>().sharedMaterial = bodyMat;
		base.transform.Find("Hips/Spine1/Spine2/Neck/Head/Mouth").GetComponent<SkinnedMeshRenderer>().sharedMaterial = bodyMat;
		bodyMeshes = new SkinnedMeshRenderer[3];
		bodyMeshes[0] = base.transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
		bodyMeshes[1] = base.transform.Find("Hips/Spine1/Spine2/Neck/Head/EyeLids").GetComponent<SkinnedMeshRenderer>();
		bodyMeshes[2] = base.transform.Find("Hips/Spine1/Spine2/Neck/Head/Mouth").GetComponent<SkinnedMeshRenderer>();
		eyeMat = base.transform.Find("Hips/Spine1/Spine2/Neck/Head/LeftEye/EyeLeft").GetComponent<MeshRenderer>().material;
		base.transform.Find("Hips/Spine1/Spine2/Neck/Head/LeftEye/EyeLeft").GetComponent<MeshRenderer>().sharedMaterial = eyeMat;
		base.transform.Find("Hips/Spine1/Spine2/Neck/Head/RightEye/EyeRight").GetComponent<MeshRenderer>().sharedMaterial = eyeMat;
		eyeBalls = new MeshRenderer[2];
		eyeBalls[0] = base.transform.Find("Hips/Spine1/Spine2/Neck/Head/LeftEye/EyeLeft").GetComponent<MeshRenderer>();
		eyeBalls[1] = base.transform.Find("Hips/Spine1/Spine2/Neck/Head/RightEye/EyeRight").GetComponent<MeshRenderer>();
		bones = new List<GameObject>();
		FindBones(base.gameObject);
		boneMat = bones[0].GetComponent<MeshRenderer>().material;
		foreach (GameObject bone in bones)
		{
			bone.GetComponent<MeshRenderer>().sharedMaterial = boneMat;
		}
		bones.Add(base.transform.Find("Hips/Spine1/Spine2/Neck/Head/AlienBones_Skull/EyeSocketLeft").gameObject);
		bones.Add(base.transform.Find("Hips/Spine1/Spine2/Neck/Head/AlienBones_Skull/EyeSocketRight").gameObject);
		boneColorWhite = new Color32(218, 208, 190, byte.MaxValue);
		boneColorBlack = new Color32(0, 0, 0, byte.MaxValue);
		boneColorBurned = new Color32(103, 99, 92, byte.MaxValue);
		particleEffectA = base.transform.Find("Hips/Spine1/Spine2/ElectricBurst").GetComponent<ParticleSystem>();
		particleEffectA.gameObject.SetActive(false);
		particleEffectB = base.transform.Find("Hips/Spine1/Spine2/EffectSmoke").GetComponent<ParticleSystem>();
		particleEffectB.gameObject.SetActive(false);
		alienExplosion = base.transform.Find("Hips/Spine1/Spine2/Neck/EffectAlienSplurt").GetComponent<ParticleSystem>();
		alienExplosion.gameObject.SetActive(false);
		particleEffectStreaks = new ParticleSystem[3];
		particleEffectStreaks[0] = base.transform.Find("Hips/Spine1/Spine2/Neck/Head/EffectElectrocutionStreaks").GetComponent<ParticleSystem>();
		particleEffectStreaks[1] = base.transform.Find("Hips/Spine1/Spine2/EffectElectrocutionStreaks").GetComponent<ParticleSystem>();
		particleEffectStreaks[2] = base.transform.Find("Hips/Spine1/Spine2/Neck/EffectElectrocutionStreaks").GetComponent<ParticleSystem>();
		headBone = base.transform.Find("Hips/Spine1/Spine2/Neck/Head");
		ParticleSystem[] array = particleEffectStreaks;
		foreach (ParticleSystem particleSystem in array)
		{
			particleSystem.GetComponent<Renderer>().sharedMaterial = bodyMat;
		}
	}

	private float GetDeltaTime()
	{
		if (Main.m_timeScale != 0f)
		{
			return Main.m_gameDeltaTime / ToolBox.limitBetween(Main.m_timeScale, 1E-09f, float.MaxValue);
		}
		return Main.m_gameDeltaTime;
	}

	public void FindBones(GameObject obj)
	{
		foreach (Transform item in obj.transform)
		{
			if (item.name.StartsWith("AlienBones_"))
			{
				bones.Add(item.gameObject);
			}
			FindBones(item.gameObject);
		}
	}

	public void Explode()
	{
		foreach (GameObject bone in bones)
		{
			bone.GetComponent<MeshRenderer>().enabled = true;
		}
		SkinnedMeshRenderer[] array = bodyMeshes;
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
		{
			skinnedMeshRenderer.enabled = false;
		}
		MeshRenderer[] array2 = eyeBalls;
		foreach (MeshRenderer meshRenderer in array2)
		{
			meshRenderer.enabled = false;
		}
		alienExplosion.gameObject.SetActive(true);
		alienExplosion.Play();
	}

	public void PlayElectrocute()
	{
		bodyMat.shader = Shader.Find("WOE/Units/UnlitBackface");
		bodyMat.SetColor("_Color", new Color(1f, 1f, 1f));
		eyeMat.shader = Shader.Find("WOE/Units/UnlitBackface");
		eyeMat.SetColor("_Color", new Color(0f, 0f, 0f));
		boneMat.SetColor("_Color", boneColorBlack);
		particleEffectA.gameObject.SetActive(true);
		particleEffectB.gameObject.SetActive(true);
		particleEffectA.Play();
		particleEffectB.Play();
		foreach (GameObject bone in bones)
		{
			bone.GetComponent<MeshRenderer>().enabled = true;
		}
		isElectrocuting = true;
	}

	public void StopElectrocute()
	{
		bodyMat.shader = Shader.Find("WOE/Units/ReflectiveSpecularRimUnit");
		bodyMat.SetColor("_Color", new Color(1f, 1f, 1f));
		eyeMat.shader = Shader.Find("WOE/Units/ReflectiveSpecularRimUnit");
		eyeMat.SetColor("_Color", new Color(1f, 1f, 1f));
		boneMat.SetColor("_Color", boneColorWhite);
		foreach (GameObject bone in bones)
		{
			bone.GetComponent<MeshRenderer>().enabled = false;
		}
		particleEffectA.Stop();
		particleEffectB.Stop();
		particleEffectA.gameObject.SetActive(false);
		particleEffectB.gameObject.SetActive(false);
		foreach (GameObject bone2 in bones)
		{
			bone2.GetComponent<MeshRenderer>().enabled = false;
		}
		isElectrocuting = false;
	}

	public void Electrocute()
	{
		flashTimer -= GetDeltaTime();
		if (flashTimer <= 0f)
		{
			if (swapperToggle)
			{
				boneMat.SetColor("_Color", boneColorBlack);
				bodyMat.SetColor("_Color", new Color(1f, 1f, 1f));
				eyeMat.SetColor("_Color", new Color(1f, 1f, 1f));
				swapperToggle = false;
			}
			else if (!swapperToggle)
			{
				boneMat.SetColor("_Color", boneColorWhite);
				bodyMat.SetColor("_Color", new Color(0f, 0f, 0f));
				eyeMat.SetColor("_Color", new Color(0f, 0f, 0f));
				swapperToggle = true;
			}
			flashTimer = Random.Range(0.01f, 0.1f);
		}
	}

	public void WobbleHead()
	{
		headWobbling = true;
		alienAnimator.SetTrigger("Blink");
		wobbleTimer = wobbleTime;
	}

	private void WobbleHeadUpdate()
	{
		if (wobbleTimer <= 0f)
		{
			headWobbling = false;
			headBone.localScale = new Vector3(1f, 1f, 1f);
			return;
		}
		wobbleTimer -= GetDeltaTime();
		float y = wobbleCurve.Evaluate(1f - wobbleTimer / wobbleTime);
		float num = 2f - wobbleCurve.Evaluate(1f - wobbleTimer / wobbleTime);
		headBone.localScale = new Vector3(num, y, num);
	}

	public IEnumerator ElectrocuteRoutine()
	{
		float effectTimer = electroCuteLength;
		float flashTimer = 0.15f;
		bool swapperToggle = true;
		bodyMat.shader = Shader.Find("WOE/Units/UnlitBackface");
		bodyMat.SetColor("_Color", new Color(1f, 1f, 1f));
		eyeMat.shader = Shader.Find("WOE/Units/UnlitBackface");
		eyeMat.SetColor("_Color", new Color(0f, 0f, 0f));
		boneMat.SetColor("_Color", boneColorBlack);
		particleEffectA.Play();
		foreach (GameObject i in bones)
		{
			i.GetComponent<MeshRenderer>().enabled = true;
		}
		particleEffectB.Play();
		while (effectTimer > 0f)
		{
			effectTimer -= GetDeltaTime();
			flashTimer -= GetDeltaTime();
			if (flashTimer <= 0f)
			{
				if (swapperToggle)
				{
					boneMat.SetColor("_Color", boneColorBlack);
					bodyMat.SetColor("_Color", new Color(1f, 1f, 1f));
					eyeMat.SetColor("_Color", new Color(1f, 1f, 1f));
					swapperToggle = false;
				}
				else if (!swapperToggle)
				{
					boneMat.SetColor("_Color", boneColorWhite);
					bodyMat.SetColor("_Color", new Color(0f, 0f, 0f));
					eyeMat.SetColor("_Color", new Color(0f, 0f, 0f));
					swapperToggle = true;
				}
				flashTimer = Random.Range(0.01f, 0.1f);
			}
			yield return null;
		}
		bodyMat.shader = Shader.Find("WOE/Units/ReflectiveSpecularRimUnit");
		bodyMat.SetColor("_Color", new Color(1f, 1f, 1f));
		eyeMat.shader = Shader.Find("WOE/Units/ReflectiveSpecularRimUnit");
		eyeMat.SetColor("_Color", new Color(1f, 1f, 1f));
		boneMat.SetColor("_Color", boneColorWhite);
		foreach (GameObject i2 in bones)
		{
			i2.GetComponent<MeshRenderer>().enabled = false;
		}
	}

	private void Update()
	{
		if (isElectrocuting)
		{
			Electrocute();
		}
	}

	private void LateUpdate()
	{
		if (headWobbling)
		{
			WobbleHeadUpdate();
		}
	}
}
