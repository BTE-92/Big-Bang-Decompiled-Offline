using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
	public levelBuilderObject[] levelObjects;

	public List<GameObject> spawnedObjects;

	public void InitializeBackground(int _seed, float _domeWidth)
	{
		Debug.Log("LevelBuilder Dome width: " + _domeWidth);
		Random.seed = _seed;
		foreach (GameObject spawnedObject in spawnedObjects)
		{
			Object.Destroy(spawnedObject);
		}
		spawnedObjects.Clear();
		for (int i = 0; i < levelObjects.Length; i++)
		{
			levelBuilderObject levelBuilderObject2 = levelObjects[i];
			int num = Random.Range(levelBuilderObject2.minAmount, levelBuilderObject2.maxAmount);
			for (int j = 0; j < num; j++)
			{
				float x = Random.Range(levelBuilderObject2.minX, levelBuilderObject2.maxX);
				float z = Random.Range(levelBuilderObject2.minZ, levelBuilderObject2.maxZ);
				float y = Random.Range(levelBuilderObject2.minHeight, levelBuilderObject2.maxHeight);
				float num2 = Random.Range(levelBuilderObject2.minScale, levelBuilderObject2.maxScale);
				bool flag = true;
				GameObject gameObject = Object.Instantiate(levelBuilderObject2.levelObject);
				gameObject.transform.parent = base.transform;
				gameObject.transform.localPosition = new Vector3(x, y, z);
				gameObject.transform.localEulerAngles = new Vector3(gameObject.transform.localEulerAngles.x, gameObject.transform.localEulerAngles.y + Random.Range(levelBuilderObject2.rotationOffset, 0f - levelBuilderObject2.rotationOffset), gameObject.transform.localEulerAngles.z);
				if (levelBuilderObject2.allowMirror && Random.value > 0.5f)
				{
					gameObject.transform.localScale = new Vector3(num2 * -1f, num2, num2);
				}
				else
				{
					gameObject.transform.localScale = new Vector3(num2, num2, num2);
				}
				foreach (GameObject spawnedObject2 in spawnedObjects)
				{
					if (spawnedObject2.GetComponent<Collider>().bounds.Intersects(gameObject.GetComponent<Collider>().bounds) && levelObjects != null)
					{
						flag = false;
						Object.Destroy(gameObject);
					}
				}
				if (flag)
				{
					spawnedObjects.Add(gameObject);
				}
				foreach (GameObject spawnedObject3 in spawnedObjects)
				{
					Object.Destroy(spawnedObject3.GetComponent<Collider>());
				}
			}
		}
	}
}
