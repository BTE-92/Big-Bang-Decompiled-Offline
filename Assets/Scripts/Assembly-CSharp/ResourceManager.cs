using System;
using System.Collections;
using System.IO;
using UnityEngine;

public static class ResourceManager
{
	public static bool m_preloadResourcesByDefault = false;

	private static Hashtable m_resourceGroups = new Hashtable();

	private static Hashtable m_resources = new Hashtable();

	public static void ListResources(string _groupIdentifier)
	{
		IResourceGroup resourceGroup = m_resourceGroups[_groupIdentifier] as IResourceGroup;
		string text = string.Empty;
		foreach (IResource resource in resourceGroup.resources)
		{
			text = text + resource.identifier + ",";
		}
	}

	public static int GetGroupCount()
	{
		return m_resourceGroups.Count;
	}

	public static int GetResourceCount()
	{
		return m_resources.Count;
	}

	public static void PreloadResourceGroupFromFolder(string _resourcesPath)
	{
		string[] array = _resourcesPath.Split(Path.DirectorySeparatorChar);
		string text = array[array.Length - 1];
		if (m_resourceGroups.ContainsKey(text))
		{
			Debug.LogError("Resource group with the name " + text + " already exists!");
			return;
		}
		AddResourceGroup(text);
		UnityEngine.Object[] array2 = Resources.LoadAll(_resourcesPath, typeof(UnityEngine.Object));
		for (int i = 0; i < array2.Length; i++)
		{
			if (array2[i] != null)
			{
				UnityResource resource = new UnityResource(array2[i], text);
				AddResourceToGroup(text, resource);
			}
		}
	}

	public static void AddResourceToGroup(string _resourcePath, string _group)
	{
		if (!m_resourceGroups.ContainsKey(_group))
		{
			AddResourceGroup(_group);
		}
		string[] array = _resourcePath.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		UnityResource resource = new UnityResource(_group + "/" + array[array.Length - 1], _resourcePath);
		AddResourceToGroup(_group, resource);
	}

	public static void AddResourceToGroup(string _identifier, string _resourcePath, string _group, bool _forcePreload = false)
	{
		if (!m_resourceGroups.ContainsKey(_group))
		{
			AddResourceGroup(_group);
		}
		UnityResource resource = new UnityResource(_identifier, _resourcePath, _forcePreload);
		AddResourceToGroup(_group, resource);
	}

	public static IResourceGroup AddResourceGroup(string _identifier)
	{
		Debug.LogInfo("NEW RESOURCE GROUP ADDED: " + _identifier);
		BasicResourceGroup basicResourceGroup = new BasicResourceGroup(_identifier);
		m_resourceGroups.Add(_identifier, basicResourceGroup);
		return basicResourceGroup;
	}

	public static IResource AddResourceToGroup(IResourceGroup _group, IResource _resource)
	{
		if (!m_resources.Contains(_resource.identifier))
		{
			m_resources.Add(_resource.identifier, _resource);
		}
		else
		{
			Debug.LogError("Duplicate resource names in group!! " + _resource.identifier);
			_resource = (IResource)m_resources[_resource.identifier];
		}
		if (!_group.resources.Contains(_resource))
		{
			_group.resources.Add(_resource);
		}
		return _resource;
	}

	public static IResource AddResourceToGroup(string _groupIdentifier, IResource _resource)
	{
		if (m_resourceGroups.Contains(_groupIdentifier))
		{
			return AddResourceToGroup(m_resourceGroups[_groupIdentifier] as IResourceGroup, _resource);
		}
		return null;
	}

	public static void UnloadResourceGroup(IResourceGroup _group)
	{
		for (int i = 0; i < _group.resources.Count; i++)
		{
			UnloadResource(_group.resources[i], false);
		}
		m_resourceGroups.Remove(_group.identifier);
		Debug.LogInfo("RESOURCE GROUP UNLOADED & REMOVED: " + _group.identifier);
	}

	public static void UnloadResourceGroup(string _identifier)
	{
		if (m_resourceGroups.Contains(_identifier))
		{
			UnloadResourceGroup((IResourceGroup)m_resourceGroups[_identifier]);
		}
		else
		{
			Debug.LogError("Resource identifier not found!");
		}
	}

	public static void UnloadResource(IResource _resource)
	{
		UnloadResource(_resource, true);
	}

	public static void UnloadResource(IResource _resource, bool _unloadAssets)
	{
		_resource.Unload();
		if (_resource.resourceGroup != null)
		{
			_resource.resourceGroup.resources.Remove(_resource);
		}
		m_resources.Remove(_resource.identifier);
		if (_unloadAssets)
		{
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}
	}

	public static void UnloadResource(string _identifier)
	{
		if (m_resources.Contains(_identifier))
		{
			UnloadResource((IResource)m_resources[_identifier]);
		}
		else
		{
			Debug.LogError("Trying to unload unknown resource!");
		}
	}

	public static IResource GetResourceClass(string _resourceIdentifier)
	{
		if (m_resources.Contains(_resourceIdentifier))
		{
			return (IResource)m_resources[_resourceIdentifier];
		}
		Debug.LogError("Cannot find resource " + _resourceIdentifier);
		return null;
	}

	public static AudioClip GetAudioClip(string _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier);
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as AudioClip;
		}
		return null;
	}

	public static Texture GetTexture(string _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier);
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as Texture;
		}
		return null;
	}

	public static Shader GetShader(string _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier);
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as Shader;
		}
		return null;
	}

	public static Material GetMaterial(string _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier);
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as Material;
		}
		return null;
	}

	public static GameObject GetGameObject(string _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier);
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as GameObject;
		}
		return null;
	}

	public static Level GetLevel(string _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier);
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as Level;
		}
		return null;
	}

	public static TextAsset GetTextAsset(string _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier);
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as TextAsset;
		}
		return null;
	}

	public static Font GetFont(string _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier);
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as Font;
		}
		return null;
	}

	public static AnimationClip GetAnimationClip(string _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier);
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as AnimationClip;
		}
		return null;
	}

	public static Texture GetTexture(RESOURCE _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier.ToString());
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as Texture;
		}
		return null;
	}

	public static Shader GetShader(RESOURCE _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier.ToString());
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as Shader;
		}
		return null;
	}

	public static Material GetMaterial(RESOURCE _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier.ToString());
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as Material;
		}
		return null;
	}

	public static GameObject GetGameObject(RESOURCE _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier.ToString());
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as GameObject;
		}
		return null;
	}

	public static Level GetLevel(RESOURCE _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier.ToString());
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as Level;
		}
		return null;
	}

	public static TextAsset GetTextAsset(RESOURCE _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier.ToString());
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as TextAsset;
		}
		return null;
	}

	public static Font GetFont(RESOURCE _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier.ToString());
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as Font;
		}
		return null;
	}

	public static AnimationClip GetAnimationClip(RESOURCE _resourceIdentifier)
	{
		IResource resourceClass = GetResourceClass(_resourceIdentifier.ToString());
		if (resourceClass != null && resourceClass.ResourceObject != null)
		{
			return resourceClass.ResourceObject as AnimationClip;
		}
		return null;
	}
}
