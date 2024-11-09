using System.Threading.Tasks;
using Code.Core.ResourceSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Core.Utilities;

public static class UI
{
	public static Canvas GetCanvas(GameObject g)
	{
		while (true)
		{
			if (g.GetComponent<Canvas>() != null)
			{
				return g.GetComponent<Canvas>();
			}
			if (!(g.transform.parent != null))
			{
				break;
			}
			g = g.transform.parent.gameObject;
		}
		Debug.LogWarning("Unable to find Canvas");
		return null;
	}

	public static void SetIcon(Image image, Texture2D newTexture)
	{
		if (image == null)
		{
			Debug.LogWarning("Unable to find image");
			return;
		}
		if (!newTexture)
		{
			Debug.LogWarning("Unable to find texture");
			return;
		}
		Vector2 pivot = image.rectTransform.pivot;
		Rect rect = new Rect(0f, 0f, newTexture.width, newTexture.height);
		Sprite sprite = Sprite.Create(newTexture, rect, pivot);
		image.enabled = true;
		image.sprite = sprite;
	}

	public static async Task<Texture2D> GetIcon(string iconPathOrKey)
	{
		if (string.IsNullOrEmpty(iconPathOrKey))
		{
			return null;
		}
		if (iconPathOrKey.Contains("Batch"))
		{
			return await MilMo_ResourceManager.Instance.LoadTextureAsync(iconPathOrKey);
		}
		Sprite sprite = Addressables.LoadAssetAsync<Sprite>(iconPathOrKey).WaitForCompletion();
		if (sprite == null)
		{
			return null;
		}
		return sprite.texture;
	}
}
