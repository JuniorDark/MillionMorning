using System;
using Core.Audio.AudioData;
using Localization;
using UI.HUD.Dialogues;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace UI.HUD.QuickInfo;

[CreateAssetMenu(menuName = "Notification/QuickInfoSO", fileName = "newQuickInfoSO")]
public class QuickInfoSO : ScriptableObject, IEquatable<QuickInfoSO>
{
	[SerializeField]
	private LocalizedStringWithArgument caption;

	[SerializeField]
	private LocalizedStringWithArgument message;

	[SerializeField]
	private string iconKey;

	[SerializeField]
	private string soundKey;

	[SerializeField]
	private int lifetime;

	[SerializeField]
	private UnityEvent callToActionEvent = new UnityEvent();

	[SerializeField]
	private LocalizedStringWithArgument callToActionLabel;

	public string GetAddressableKey()
	{
		return "QuickInfoWindow";
	}

	public void Init(QuickInfoMessageData quickInfoMessageData)
	{
		caption = quickInfoMessageData.GetCaption();
		message = quickInfoMessageData.GetMessage();
		iconKey = quickInfoMessageData.GetIconKey();
		lifetime = quickInfoMessageData.GetLifetime();
		soundKey = quickInfoMessageData.GetSoundKey();
		DialogueButtonInfo callToAction = quickInfoMessageData.GetCallToAction();
		if (callToAction != null)
		{
			callToActionEvent.AddListener(callToAction.GetAction());
			callToActionLabel = callToAction.GetLabel();
		}
	}

	public void CallToAction()
	{
		callToActionEvent?.Invoke();
	}

	public string GetCaption()
	{
		return caption?.GetMessage() ?? "";
	}

	public string GetMessage()
	{
		return message?.GetMessage() ?? "";
	}

	public string GetCallToLabel()
	{
		return callToActionLabel?.GetMessage() ?? "";
	}

	public int GetLifetime()
	{
		return lifetime;
	}

	public Sprite GetSprite()
	{
		if (!string.IsNullOrEmpty(iconKey))
		{
			return LoadSprite();
		}
		return null;
	}

	private Sprite LoadSprite()
	{
		return Addressables.LoadAssetAsync<Sprite>(iconKey).WaitForCompletion();
	}

	public UIAudioCueSO GetSound()
	{
		if (string.IsNullOrEmpty(soundKey))
		{
			return null;
		}
		return Addressables.LoadAssetAsync<UIAudioCueSO>(soundKey).WaitForCompletion();
	}

	public bool Equals(QuickInfoSO other)
	{
		if (other != null && other.caption == caption)
		{
			return other.message == message;
		}
		return false;
	}
}
