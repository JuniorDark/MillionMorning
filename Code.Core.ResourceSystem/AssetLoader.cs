using System.Collections.Generic;
using System.Linq;
using Code.Core.EventSystem;
using Code.Core.Utility;
using Code.Core.Utility.PriorityQueue;
using UnityEngine;

namespace Code.Core.ResourceSystem;

[ExecuteInEditMode]
public class AssetLoader : MonoBehaviour
{
	private readonly Dictionary<string, MilMo_Asset> _loadingAssets = new Dictionary<string, MilMo_Asset>();

	private readonly IPriorityQueue<MilMo_Asset, int> _queue = new PriorityQueue<MilMo_Asset, int>(0);

	private readonly List<MilMo_Asset> _pausedAssets = new List<MilMo_Asset>();

	private readonly List<MilMo_Asset> _stoppedAssets = new List<MilMo_Asset>();

	private bool _smoothLoadingActivated;

	private const int DEFAULT_MAX_NUMBER_OF_LOADING_ASSETS = 10;

	private const int SMOOTH_LOADING_MAX_NUMBER_OF_LOADING_ASSETS = 3;

	private int _maxNumberOfLoadingAssets = 10;

	private bool _stopModeActivated;

	private bool _pauseModeActivated;

	public int NumberOfLoadingAssets => _loadingAssets.Count;

	public bool SmoothLoading
	{
		get
		{
			return _smoothLoadingActivated;
		}
		set
		{
			_smoothLoadingActivated = value;
			_maxNumberOfLoadingAssets = (_smoothLoadingActivated ? 3 : 10);
		}
	}

	public bool Stopped
	{
		get
		{
			return _stopModeActivated;
		}
		set
		{
			_stopModeActivated = value;
			if (!_stopModeActivated)
			{
				ResumeAllStoppedAsset();
			}
		}
	}

	public bool Paused
	{
		get
		{
			return _pauseModeActivated;
		}
		set
		{
			_pauseModeActivated = value;
		}
	}

	private void AddLoadingAsset(MilMo_Asset asset)
	{
		_loadingAssets.Add(asset.Path, asset);
	}

	private void RemoveLoadingAsset(string path)
	{
		_loadingAssets.Remove(path);
	}

	private void ResumeAllStoppedAsset()
	{
		List<MilMo_Asset> list = _stoppedAssets.ToList();
		_stoppedAssets.Clear();
		foreach (MilMo_Asset item in list)
		{
			item.Ready();
		}
	}

	private void EnqueueAsset(MilMo_Asset loadingAsset)
	{
		_queue.Insert(loadingAsset, (int)loadingAsset.Priority);
	}

	public void Load(string path, string assetTag, MilMo_ResourceManager.Priority priority, bool pause, MilMo_ResourceManager.AssetDoneCallback callback)
	{
		if (_loadingAssets.TryGetValue(path, out var value))
		{
			value.AssetDoneCallbacks.Add(callback);
			return;
		}
		MilMo_Asset milMo_Asset = _pausedAssets.FirstOrDefault((MilMo_Asset pausedAsset) => pausedAsset.Path == path);
		if (milMo_Asset != null)
		{
			milMo_Asset.AssetDoneCallbacks.Add(callback);
			return;
		}
		MilMo_Asset milMo_Asset2 = _stoppedAssets.FirstOrDefault((MilMo_Asset stoppedAsset) => stoppedAsset.Path == path);
		if (milMo_Asset2 != null)
		{
			milMo_Asset2.AssetDoneCallbacks.Add(callback);
			return;
		}
		MilMo_Asset milMo_Asset3 = new MilMo_Asset(path, assetTag, priority, callback);
		if (Paused && pause)
		{
			_pausedAssets.Add(milMo_Asset3);
		}
		else if (_loadingAssets.Count >= _maxNumberOfLoadingAssets)
		{
			EnqueueAsset(milMo_Asset3);
		}
		else
		{
			StartLoading(milMo_Asset3);
		}
	}

	public void Update()
	{
		while (_loadingAssets.Count < _maxNumberOfLoadingAssets)
		{
			MilMo_Asset milMo_Asset = null;
			if (!Paused && _pausedAssets.Count > 0)
			{
				milMo_Asset = _pausedAssets.FirstOrDefault();
				_pausedAssets.Remove(milMo_Asset);
			}
			if (milMo_Asset == null)
			{
				if (_queue.IsEmpty())
				{
					break;
				}
				milMo_Asset = _queue.Pop();
			}
			MilMo_Asset loadedAsset = MilMo_ResourceManager.Instance.GetLoadedAsset(milMo_Asset.Path);
			if (loadedAsset != null)
			{
				loadedAsset.AssetDoneCallbacks.AddRange(milMo_Asset.AssetDoneCallbacks);
				loadedAsset.Ready();
				break;
			}
			if (_loadingAssets.TryGetValue(milMo_Asset.Path, out var value))
			{
				value.AssetDoneCallbacks.AddRange(milMo_Asset.AssetDoneCallbacks);
				break;
			}
			StartLoading(milMo_Asset);
		}
	}

	private void StartLoading(MilMo_Asset asset)
	{
		if (MilMo_ResourceManager.Instance.profileStreaming)
		{
			MilMo_Timer.StartUnique("loading asset: " + asset.Path);
		}
		AddLoadingAsset(asset);
		MilMo_AssetBundle loadedAssetBundle = MilMo_ResourceManager.Instance.GetLoadedAssetBundle(asset.BundlePath);
		if (loadedAssetBundle != null)
		{
			asset.SetAssetBundle(loadedAssetBundle);
			asset.GetAssetFromAssetBundle();
			LoadingFinished(asset);
			return;
		}
		MilMo_ResourceManager.Instance.LoadAssetBundle(asset.BundlePath, delegate(MilMo_AssetBundle bundle)
		{
			asset.SetAssetBundle(bundle);
			asset.GetAssetFromAssetBundle();
			LoadingFinished(asset);
		});
	}

	private void LoadingFinished(MilMo_Asset asset)
	{
		if (_loadingAssets.ContainsKey(asset.Path))
		{
			RemoveLoadingAsset(asset.Path);
		}
		MilMo_ResourceManager.Instance.AddLoadedAsset(asset);
		MilMo_ResourceManager.Instance.LoadedAssetsByTag.TryGetValue(asset.Tag, out var value);
		if (value != null)
		{
			value.Add(asset);
		}
		else
		{
			MilMo_ResourceManager.Instance.LoadedAssetsByTag.Add(asset.Tag, new List<MilMo_Asset> { asset });
		}
		if (Stopped)
		{
			_stoppedAssets.Add(asset);
			return;
		}
		if (SmoothLoading)
		{
			MilMo_EventSystem.UpdateOne(delegate
			{
				asset.Ready();
			});
		}
		else
		{
			asset.Ready();
		}
		if (MilMo_ResourceManager.Instance.profileStreaming)
		{
			MilMo_Timer.StopUnique("loading asset: " + asset.Path, asset.Path);
		}
	}
}
