using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.HUD.Counters;

public class Counter : HudElement
{
	[Header("Elements")]
	[SerializeField]
	private TMP_Text text;

	[SerializeField]
	private Image icon;

	[Header("Events")]
	[SerializeField]
	private UnityEvent onIconChanged;

	protected void SetText(string newText)
	{
		text.text = newText;
	}

	protected void SetIcon(Texture2D newTexture)
	{
		if (icon.sprite.texture == newTexture)
		{
			return;
		}
		LeanTween.scale(icon.gameObject, Vector3.zero, 0.3f);
		LeanTween.delayedCall(0.3f, (Action)delegate
		{
			icon.enabled = false;
			if ((bool)newTexture)
			{
				Vector2 pivot = icon.rectTransform.pivot;
				Rect rect = new Rect(0f, 0f, newTexture.width, newTexture.height);
				icon.sprite = Sprite.Create(newTexture, rect, pivot);
				icon.enabled = true;
				onIconChanged?.Invoke();
			}
		});
	}

	public override void SetHudVisibility(bool shouldShow)
	{
		base.gameObject.SetActive(shouldShow);
	}
}
