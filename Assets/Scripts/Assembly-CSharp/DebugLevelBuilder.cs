using UnityEngine;

public class DebugLevelBuilder : MonoBehaviour
{
	public GameObject currentMap;

	private LevelBuilder levelBuilder;

	private void Start()
	{
		levelBuilder = currentMap.GetComponent("LevelBuilder") as LevelBuilder;
		levelBuilder.InitializeBackground(Random.Range(1, 1000), 1f);
	}
}
