using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResourcePool : MonoBehaviour
{
	[Serializable]
	public class AssetListItem
	{
		public string name;

		public string path;

		public string GUID;

		public string suffix;
	}

	[Serializable]
	public class AssetGroup
	{
		public string name;

		public List<AssetListItem> items = new List<AssetListItem>();
	}

	private static ResourcePool m_pool;

	public List<AssetGroup> groups;

	public static void Initialize(string _resourcePoolPath)
	{
		GameObject gameObject = Resources.Load(_resourcePoolPath) as GameObject;
		m_pool = gameObject.GetComponent<ResourcePool>();
	}

	public static AssetListItem GetResourceAsset(RESOURCE _resource)
	{
		foreach (AssetGroup group in m_pool.groups)
		{
			foreach (AssetListItem item in group.items)
			{
				if (item.name.Equals(_resource.ToString()))
				{
					return item;
				}
			}
		}
		return null;
	}

	public static AssetGroup GetResourceGroup(RESOURCE_GROUP _group)
	{
		foreach (AssetGroup group in m_pool.groups)
		{
			if (group.name.Equals(_group.ToString()))
			{
				return group;
			}
		}
		return null;
	}

	public static void LoadResourceGroup(RESOURCE_GROUP _group, bool _forcePreload = false)
	{
		AssetGroup resourceGroup = GetResourceGroup(_group);
		foreach (AssetListItem item in resourceGroup.items)
		{
			ResourceManager.AddResourceToGroup(item.name, item.path, resourceGroup.name, _forcePreload);
		}
	}

	public static void UnloadResourceGroup(RESOURCE_GROUP _group)
	{
		AssetGroup resourceGroup = GetResourceGroup(_group);
		ResourceManager.UnloadResourceGroup(resourceGroup.name);
	}

	private void Reset()
	{
		groups = new List<AssetGroup>();
		for (int i = groups.Count; i < 3; i++)
		{
			AssetGroup assetGroup = new AssetGroup();
			assetGroup.name = ((RESOURCE_GROUP)i).ToString();
			groups.Add(assetGroup);
		}
	}
}
