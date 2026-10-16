using UnityEngine;

public struct GhostPart
{
	public GameObject prefab;

	public Vector3 offset;

	public string nodeName;

	public bool flipX;

	public GhostPart(GameObject _prefab, Vector3 _offset, string _nodeName, bool _flipX = false)
	{
		prefab = _prefab;
		offset = _offset;
		nodeName = _nodeName;
		flipX = _flipX;
	}
}
