using UnityEngine;

public class UnityResource : IResource
{
	private static int _instanceCount;

	private IResourceGroup _resourceGroup;

	private string _identifier;

	private object _resourceObject;

	public string path;

	public IResourceGroup resourceGroup
	{
		get
		{
			return _resourceGroup;
		}
		set
		{
			_resourceGroup = value;
		}
	}

	public string identifier
	{
		get
		{
			return _identifier;
		}
		set
		{
			_identifier = value;
		}
	}

	public object ResourceObject
	{
		get
		{
			if (_resourceObject != null)
			{
				return _resourceObject;
			}
			return Load(path);
		}
		set
		{
			_resourceObject = value;
		}
	}

	public UnityResource(string _identifier, string _path, bool _forcePreload = false)
	{
		identifier = _identifier;
		path = _path;
		_resourceObject = null;
		if (ResourceManager.m_preloadResourcesByDefault || _forcePreload)
		{
			Load(_path);
		}
		_instanceCount++;
	}

	public UnityResource(Object _asset, string _bundleName)
	{
		identifier = _bundleName + "/" + _asset.name;
		_resourceObject = _asset;
		_instanceCount++;
	}

	private object Load(string _path)
	{
		if (_path != null)
		{
			_resourceObject = Resources.Load(_path);
			if (_resourceObject == null)
			{
				Debug.LogError("Cannot load resource: " + _path);
			}
			return _resourceObject;
		}
		Debug.LogError("Trying to load resource with null path!");
		return null;
	}

	public void Unload()
	{
		if (_resourceObject != null)
		{
			_resourceObject = null;
		}
	}

	~UnityResource()
	{
		_instanceCount--;
	}
}
