using System;
using UnityEngine;

[Serializable]
public struct levelBuilderObject
{
	public GameObject levelObject;

	public int minAmount;

	public int maxAmount;

	public float minScale;

	public float maxScale;

	public float rotationOffset;

	public bool allowMirror;

	public float minX;

	public float maxX;

	public float minZ;

	public float maxZ;

	public float minHeight;

	public float maxHeight;
}
