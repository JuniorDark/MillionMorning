using System.Collections.Generic;
using System.Linq;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.ResourceSystem;

[ExecuteInEditMode]
public class AssetBundleLoader : MonoBehaviour
{
	private readonly Dictionary<string, MilMo_AssetBundle> _loadingAssetBundles = new Dictionary<string, MilMo_AssetBundle>();

	private void AddLoadingAssetBundle(MilMo_AssetBundle bundle)
	{
		_loadingAssetBundles.Add(bundle.Path, bundle);
	}

	private void RemoveLoadingAssetBundle(string path)
	{
		_loadingAssetBundles.Remove(path);
	}

	public void Load(string path, MilMo_ResourceManager.AssetBundleDoneCallback callback)
	{
		if (string.IsNullOrEmpty(path))
		{
			callback(null);
			return;
		}
		MilMo_AssetBundle loadedAssetBundle = MilMo_ResourceManager.Instance.GetLoadedAssetBundle(path);
		if (loadedAssetBundle != null)
		{
			callback(loadedAssetBundle);
			return;
		}
		if (_loadingAssetBundles.TryGetValue(path, out var value))
		{
			value.BundleDoneCallbacks.Add(callback);
			return;
		}
		MilMo_AssetBundle milMo_AssetBundle = new MilMo_AssetBundle(path, callback);
		AddLoadingAssetBundle(milMo_AssetBundle);
		if (MilMo_ResourceManager.Instance.profileStreaming)
		{
			MilMo_Timer.StartUnique("streaming bundle:" + path);
		}
		milMo_AssetBundle.StartStream();
	}

	public void Update()
	{
		if (_loadingAssetBundles.Count <= 0)
		{
			return;
		}
		foreach (KeyValuePair<string, MilMo_AssetBundle> item in _loadingAssetBundles.ToList())
		{
			string key = item.Key;
			MilMo_AssetBundle value = item.Value;
			if (value == null)
			{
				LoadingFinished(key);
			}
			else if (value.IsStreamDone())
			{
				LoadingFinished(key);
				MilMo_ResourceManager.Instance.AddLoadedAssetBundle(value);
				value.GetAssetBundle();
				value.Ready();
			}
		}
	}

	private void LoadingFinished(string path)
	{
		if (_loadingAssetBundles.ContainsKey(path))
		{
			RemoveLoadingAssetBundle(path);
		}
		if (MilMo_ResourceManager.Instance.profileStreaming)
		{
			MilMo_Timer.StopUnique("streaming bundle:" + path, path);
		}
	}
}
