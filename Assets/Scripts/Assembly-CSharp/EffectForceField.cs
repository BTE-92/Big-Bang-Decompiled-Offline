using UnityEngine;

public class EffectForceField : MonoBehaviour
{
	public Transform leftObjTrans;

	public Transform rightObjTrans;

	public GameObject forceFieldObj;

	public GameObject forceFieldBar;

	public GameObject lightObj;

	public GameObject lightBackgroundObj;

	public GameObject lightButtonObj;

	public Material forceFieldBeamMat;

	private Material forceFieldTintedMat;

	private Material forceFieldBeamTintedMat;

	private Material lightButtonTintedMat;

	private Material lightTintedMat;

	public Color32 magenta;

	public Color32 purple;

	public Color32 blue;

	public Color32 teal;

	public Color32 green;

	public Color32 yellow;

	public Color32 orange;

	private Color32 curColor;

	private Color32 curFadedColor;

	private float beamDistance = 200f;

	private float sideLineOffset = 2.5f;

	private LineRenderer frontLine;

	private LineRenderer backLine;

	private float flashTimer;

	public void Initialize(ForceFieldColor effectColor, bool startEnabled)
	{
		curColor = EnumToColor(effectColor);
		curFadedColor = new Color32(curColor.r, curColor.g, curColor.b, 40);
		forceFieldTintedMat = new Material(forceFieldObj.GetComponent<Renderer>().material);
		forceFieldTintedMat.SetColor("_ColorTint", curColor);
		forceFieldObj.GetComponent<Renderer>().sharedMaterial = forceFieldTintedMat;
		lightButtonTintedMat = new Material(lightButtonObj.GetComponent<Renderer>().material);
		lightButtonTintedMat.SetColor("_Color", curColor);
		lightButtonObj.GetComponent<Renderer>().sharedMaterial = lightButtonTintedMat;
		lightTintedMat = new Material(lightObj.GetComponent<Renderer>().material);
		lightTintedMat.SetColor("_Color", curColor);
		lightObj.GetComponent<Renderer>().sharedMaterial = lightTintedMat;
		forceFieldBeamTintedMat = new Material(forceFieldBeamMat);
		forceFieldBeamTintedMat.SetColor("_ColorTint", curColor);
		GenerateForceField();
		AdjustHolderRotations();
		ToggleForceField(startEnabled);
	}

	public void ToggleForceField(bool _on)
	{
		if (!_on)
		{
			lightObj.SetActive(false);
			lightBackgroundObj.SetActive(true);
			forceFieldBeamTintedMat.SetColor("_ColorTint", curFadedColor);
			forceFieldTintedMat.SetColor("_ColorTint", curFadedColor);
		}
		else
		{
			lightObj.SetActive(true);
			lightBackgroundObj.SetActive(false);
			forceFieldBeamTintedMat.SetColor("_ColorTint", curColor);
			forceFieldTintedMat.SetColor("_ColorTint", curColor);
		}
	}

	private void AdjustForceField()
	{
		if (leftObjTrans != null && rightObjTrans != null)
		{
			float num = Vector3.Distance(leftObjTrans.position, rightObjTrans.position);
			forceFieldBar.transform.localScale = new Vector3(1f, 1f, num * 0.01f);
			UpdateLineRendererPositions();
		}
	}

	private void GenerateForceField()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "lineObjectFront";
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.layer = base.gameObject.layer;
		frontLine = gameObject.AddComponent<LineRenderer>();
		frontLine.SetVertexCount(2);
		frontLine.SetWidth(20f, 20f);
		frontLine.sharedMaterial = forceFieldBeamTintedMat;
		GameObject gameObject2 = new GameObject();
		gameObject2.name = "lineObjectBack";
		gameObject2.transform.parent = base.transform;
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.layer = base.gameObject.layer;
		backLine = gameObject2.AddComponent<LineRenderer>();
		backLine.SetVertexCount(2);
		backLine.SetWidth(20f, 20f);
		backLine.sharedMaterial = forceFieldBeamTintedMat;
		UpdateLineRendererPositions();
	}

	private void UpdateLineRendererPositions()
	{
		frontLine.SetPosition(0, new Vector3(leftObjTrans.position.x, leftObjTrans.position.y, leftObjTrans.position.z - beamDistance));
		frontLine.SetPosition(1, new Vector3(rightObjTrans.position.x, rightObjTrans.position.y, rightObjTrans.position.z - beamDistance));
		backLine.SetPosition(0, new Vector3(leftObjTrans.position.x, leftObjTrans.position.y, leftObjTrans.position.z - 10f));
		backLine.SetPosition(1, new Vector3(rightObjTrans.position.x, rightObjTrans.position.y, rightObjTrans.position.z - 10f));
	}

	private void AdjustHolderRotations()
	{
		Vector3 worldUp = Vector3.Cross(-Vector3.forward, leftObjTrans.position - rightObjTrans.position);
		leftObjTrans.LookAt(rightObjTrans, worldUp);
		rightObjTrans.LookAt(leftObjTrans, worldUp);
	}

	private void FlashEdges()
	{
		if (flashTimer <= 0f)
		{
			flashTimer = Random.Range(0.01f, 0.02f);
			float num = Random.Range(16f, 20f);
			float num2 = Random.Range(16f, 20f);
			frontLine.SetWidth(num, num2);
			backLine.SetWidth(num2, num);
		}
		else
		{
			flashTimer -= Time.deltaTime;
		}
	}

	private Color32 EnumToColor(ForceFieldColor color)
	{
		switch (color)
		{
		case ForceFieldColor.Magenta:
			return magenta;
		case ForceFieldColor.Purple:
			return purple;
		case ForceFieldColor.Blue:
			return blue;
		case ForceFieldColor.Teal:
			return teal;
		case ForceFieldColor.Green:
			return green;
		case ForceFieldColor.Yellow:
			return yellow;
		case ForceFieldColor.Orange:
			return orange;
		default:
			return teal;
		}
	}

	private void Update()
	{
		AdjustForceField();
		AdjustHolderRotations();
		FlashEdges();
	}

	private void OnDestroy()
	{
		Object.Destroy(forceFieldBeamTintedMat);
		Object.Destroy(forceFieldTintedMat);
		Object.Destroy(lightButtonTintedMat);
		Object.Destroy(lightTintedMat);
	}
}
