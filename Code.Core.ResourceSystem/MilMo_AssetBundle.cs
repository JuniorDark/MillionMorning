using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Core.ResourceSystem;

public class MilMo_AssetBundle
{
	private AssetBundle _assetBundle;

	private AssetBundleCreateRequest _stream;

	public readonly List<MilMo_ResourceManager.AssetBundleDoneCallback> BundleDoneCallbacks = new List<MilMo_ResourceManager.AssetBundleDoneCallback>();

	private int _refCounter;

	public string Path { get; }

	public MilMo_AssetBundle(string path, MilMo_ResourceManager.AssetBundleDoneCallback callback)
	{
		Path = path;
		BundleDoneCallbacks.Add(callback);
	}

	public Object LoadAsset(string name)
	{
		if (!(_assetBundle != null))
		{
			return null;
		}
		return _assetBundle.LoadAsset(name);
	}

	public void IncRef()
	{
		_refCounter++;
	}

	public void DecrRef()
	{
		_refCounter--;
		if (_refCounter <= 0)
		{
			Unload();
		}
	}

	public void StartStream()
	{
		_stream = AssetBundle.LoadFromFileAsync(MilMo_ResourceManager.Instance.dataUrl + Path);
	}

	public bool IsStreamDone()
	{
		return _stream?.isDone ?? true;
	}

	public void GetAssetBundle()
	{
		_assetBundle = _stream?.assetBundle;
		_stream = null;
		if (!_assetBundle)
		{
			Debug.LogWarning("Asset bundle " + Path + " was null when loaded");
			Unload();
		}
	}

	public void Ready()
	{
		List<MilMo_ResourceManager.AssetBundleDoneCallback> list = BundleDoneCallbacks.ToList();
		BundleDoneCallbacks.Clear();
		if (list.Count != 0)
		{
			list.ForEach(delegate(MilMo_ResourceManager.AssetBundleDoneCallback callback)
			{
				callback(this);
			});
		}
	}

	private void Unload()
	{
		if (MilMo_ResourceManager.Instance.profileStreaming)
		{
			Debug.Log("Unloading bundle:  " + Path);
		}
		MilMo_ResourceManager.Instance.RemoveLoadedAssetBundle(Path);
		_refCounter = 0;
		if ((bool)_assetBundle)
		{
			_assetBundle.Unload(unloadAllLoadedObjects: true);
			_assetBundle = null;
		}
	}
}
