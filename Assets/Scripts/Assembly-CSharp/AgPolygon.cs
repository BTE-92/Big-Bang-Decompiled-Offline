using System.Collections.Generic;
using UnityEngine;

public class AgPolygon
{
	public bool isHole;

	public List<Vector2> vertices;

	public List<Vector3> extraData;

	public AgPolygon(int _capacity = 0)
	{
		vertices = new List<Vector2>();
		extraData = new List<Vector3>();
		if (_capacity > 0)
		{
			vertices.Capacity = _capacity;
			extraData.Capacity = _capacity;
		}
	}

	public void Clear()
	{
		vertices.Clear();
		extraData.Clear();
	}
}
