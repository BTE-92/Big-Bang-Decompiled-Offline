using UnityEngine;

public class GGVertex
{
	public Vector3 vertex;

	public Vector2 uv;

	public Color color = new Color(1f, 1f, 1f, 1f);

	public bool isDuplicate;

	public GGVertex Copy(bool _asDuplicate = false)
	{
		GGVertex gGVertex = new GGVertex();
		gGVertex.vertex = vertex;
		gGVertex.uv = uv;
		gGVertex.color = color;
		gGVertex.isDuplicate = _asDuplicate;
		return gGVertex;
	}
}
