using UnityEngine;

public class EffectSmokeTrail : MonoBehaviour
{
	public LineRenderer line;

	private Transform tr;

	private Vector3[] positions;

	private Vector3[] directions;

	private int i;

	private float timeSinceUpdate;

	private Material lineMaterial;

	private float lineSegment;

	private int currentNumberOfPoints = 2;

	private bool allPointsAdded;

	public int numberOfPoints = 10;

	public float updateSpeed = 0.1f;

	public float riseSpeed = 150f;

	public float spread = 50f;

	private Vector3 tempVec;

	private void Start()
	{
		tr = base.transform;
		line = GetComponent<LineRenderer>();
		lineMaterial = line.material;
		lineSegment = 1f / (float)numberOfPoints;
		positions = new Vector3[numberOfPoints];
		directions = new Vector3[numberOfPoints];
		line.SetVertexCount(currentNumberOfPoints);
		for (i = 0; i < currentNumberOfPoints; i++)
		{
			tempVec = getSmokeVec();
			directions[i] = tempVec;
			positions[i] = tr.position;
			line.SetPosition(i, positions[i]);
		}
	}

	private void Update()
	{
		timeSinceUpdate += Time.deltaTime;
		if (timeSinceUpdate > updateSpeed)
		{
			timeSinceUpdate -= updateSpeed;
			if (!allPointsAdded)
			{
				currentNumberOfPoints++;
				line.SetVertexCount(currentNumberOfPoints);
				tempVec = getSmokeVec();
				directions[0] = tempVec;
				positions[0] = tr.position;
				line.SetPosition(0, positions[0]);
			}
			if (!allPointsAdded && currentNumberOfPoints == numberOfPoints)
			{
				allPointsAdded = true;
			}
			for (i = currentNumberOfPoints - 1; i > 0; i--)
			{
				tempVec = positions[i - 1];
				positions[i] = tempVec;
				tempVec = directions[i - 1];
				directions[i] = tempVec;
			}
			tempVec = getSmokeVec();
			directions[0] = tempVec;
		}
		for (i = 1; i < currentNumberOfPoints; i++)
		{
			tempVec = positions[i];
			tempVec += directions[i] * Time.deltaTime;
			positions[i] = tempVec;
			line.SetPosition(i, positions[i]);
		}
		positions[0] = tr.position;
		line.SetPosition(0, tr.position);
		if (allPointsAdded)
		{
			Vector2 mainTextureOffset = new Vector2(lineSegment * (timeSinceUpdate / updateSpeed), lineMaterial.mainTextureOffset.y);
			lineMaterial.mainTextureOffset = mainTextureOffset;
		}
	}

	private Vector3 getSmokeVec()
	{
		Vector3 result = default(Vector3);
		result.x = Random.Range(-1f, 1f);
		result.y = Random.Range(0f, 1f);
		result.z = Random.Range(-1f, 1f);
		result.Normalize();
		result *= spread;
		result.y += riseSpeed;
		return result;
	}
}
