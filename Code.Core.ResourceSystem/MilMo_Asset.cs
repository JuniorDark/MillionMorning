using System.Collections.Generic;
using System.Linq;
using Code.Core.Utility;
using UnityEngine;

namespace Code.Core.ResourceSystem;

public class MilMo_Asset
{
	public readonly string Path;

	public readonly string Tag;

	public readonly MilMo_ResourceManager.Priority Priority;

	public readonly string BundlePath;

	public readonly List<MilMo_ResourceManager.AssetDoneCallback> AssetDoneCallbacks = new List<MilMo_ResourceManager.AssetDoneCallback>();

	public Object Asset;

	private MilMo_AssetBundle _assetBundle;

	private int _refCounter;

	public MilMo_Asset(string path, string tag, MilMo_ResourceManager.Priority priority, MilMo_ResourceManager.AssetDoneCallback callback)
	{
		Path = MilMo_Utility.ReplaceBackSlash(path);
		Tag = tag;
		Priority = priority;
		string text = MilMo_Utility.RemoveLastFolder(Path);
		string text2 = text.ToLowerInvariant().GetHashCode().ToString("X8");
		BundlePath = text + "_" + text2 + ".unity3d";
		AssetDoneCallbacks.Add(callback);
	}

	public void Ready()
	{
		List<MilMo_ResourceManager.AssetDoneCallback> list = AssetDoneCallbacks.ToList();
		AssetDoneCallbacks.Clear();
		if (list.Count > 0)
		{
			list.ForEach(delegate
			{
				IncRef();
			});
			list.ForEach(delegate(MilMo_ResourceManager.AssetDoneCallback callback)
			{
				callback(this);
			});
		}
	}

	public void SetAssetBundle(MilMo_AssetBundle assetBundle)
	{
		_assetBundle = assetBundle;
	}

	public void GetAssetFromAssetBundle()
	{
		Asset = _assetBundle.LoadAsset(Path);
	}

	private void IncRef()
	{
		_assetBundle?.IncRef();
		_refCounter++;
	}

	public void DecrRef()
	{
		_assetBundle?.DecrRef();
		_refCounter--;
		if (_refCounter <= 0)
		{
			Unload();
		}
	}

	private void Unload()
	{
		MilMo_ResourceManager.Instance.RemoveLoadedAsset(Path);
		_refCounter = 0;
		if (Tag != null)
		{
			MilMo_ResourceManager.Instance.LoadedAssetsByTag.TryGetValue(Tag, out var value);
			value?.Remove(this);
		}
	}
}
