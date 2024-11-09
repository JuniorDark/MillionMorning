using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.Utilities;

public class Instantiator
{
	public static T Instantiate<T>(string address, Transform parent = null)
	{
		if (Addressables.LoadResourceLocationsAsync(address).WaitForCompletion().Count == 0)
		{
			Debug.LogWarning($"{typeof(T)}: Unable to find addressable");
			return default(T);
		}
		GameObject gameObject = Addressables.InstantiateAsync(address, parent).WaitForCompletion();
		if (gameObject == null)
		{
			Debug.LogWarning($"{typeof(T)}: Unable to instantiate object");
			return default(T);
		}
		return gameObject.GetComponent<T>();
	}

	public static T Instantiate<T>(AssetReference assetReference, Transform targetTransform)
	{
		if (targetTransform == null)
		{
			Debug.LogWarning($"{typeof(T)}: Invalid target transform");
			return default(T);
		}
		if (assetReference == null || !assetReference.RuntimeKeyIsValid())
		{
			Debug.LogWarning($"{typeof(T)}: Invalid assetReference");
			return default(T);
		}
		GameObject gameObject = (GameObject)assetReference.Asset;
		GameObject gameObject2 = (gameObject ? gameObject : assetReference.LoadAssetAsync<GameObject>().WaitForCompletion());
		if (gameObject2 == null)
		{
			Debug.LogWarning($"{typeof(T)}: Failed to load asset");
			return default(T);
		}
		GameObject gameObject3 = Object.Instantiate(gameObject2, targetTransform);
		if (gameObject3 == null)
		{
			Debug.LogWarning($"{typeof(T)}: Failed to instantiate asset");
			return default(T);
		}
		T component = gameObject3.GetComponent<T>();
		if (component == null)
		{
			Debug.LogWarning($"{typeof(T)}: Failed to get component");
			return default(T);
		}
		return component;
	}

	public static T Clone<T>(T original, Transform parent = null) where T : Component
	{
		return Object.Instantiate(original, parent);
	}
}
