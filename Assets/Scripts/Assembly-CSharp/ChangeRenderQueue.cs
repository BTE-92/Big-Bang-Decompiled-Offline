using UnityEngine;

public class ChangeRenderQueue : MonoBehaviour
{
	public int renderQueue;

	private Material material;

	private void Start()
	{
		material = base.transform.GetComponent<Renderer>().material;
		material.renderQueue = renderQueue;
	}

	private void OnDestroy()
	{
		Object.Destroy(material);
	}
}
