using System.Collections;
using UnityEngine;

public class UnloadUnusedAssets : MonoBehaviour
{
	public bool isDone;

	private IEnumerator Start()
	{
		yield return Resources.UnloadUnusedAssets();
		Debug.Log("Unload assets complete.");
		isDone = true;
	}
}
