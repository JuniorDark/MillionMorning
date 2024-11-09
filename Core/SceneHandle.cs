using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Core;

[Serializable]
public class SceneHandle
{
	public string name;

	public AssetReference assetReference;

	public bool ValidateAsset()
	{
		if (assetReference != null)
		{
			return assetReference.RuntimeKeyIsValid();
		}
		return false;
	}

	public async Task<SceneInstance?> LoadAdditiveAsync()
	{
		if (assetReference == null)
		{
			Debug.LogError("Scene " + name + " could not be loaded, asset reference is null");
			return null;
		}
		return await assetReference.LoadSceneAsync(LoadSceneMode.Additive).Task;
	}

	public async Task<SceneInstance?> UnloadAsync()
	{
		if (assetReference == null)
		{
			Debug.LogWarning("Failed to unload scene " + name + ", asset reference is null");
			return null;
		}
		return await assetReference.UnLoadScene().Task;
	}
}
