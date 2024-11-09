using System;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UI.HUD.Chat;

public class ChatMessage : MonoBehaviour
{
	public TMP_Text timestamp;

	public TMP_Text text;

	public Image icon;

	public void SetMessage(ChatMessageObject messageObject)
	{
		if (messageObject == null)
		{
			return;
		}
		if ((bool)timestamp)
		{
			timestamp.text = ((messageObject.time.Length > 0) ? messageObject.time : (DateTime.Now.ToShortTimeString() ?? ""));
		}
		if ((bool)text && !string.IsNullOrEmpty(messageObject.sender))
		{
			text.text = "[" + messageObject.sender + "] ";
		}
		else
		{
			text.text = "";
		}
		if ((bool)text && !string.IsNullOrEmpty(messageObject.message))
		{
			text.text += messageObject.message;
		}
		if ((bool)icon && !string.IsNullOrEmpty(messageObject.iconKey))
		{
			Texture2D texture2D = Addressables.LoadAssetAsync<Texture2D>(messageObject.iconKey).WaitForCompletion();
			if ((bool)texture2D)
			{
				Vector2 pivot = icon.rectTransform.pivot;
				Rect rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
				icon.sprite = Sprite.Create(texture2D, rect, pivot);
			}
		}
	}

	public void OverrideMaterial(Material material)
	{
		text.fontMaterial = material;
		timestamp.fontMaterial = material;
	}
}
