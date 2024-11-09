using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.ResourceSystem;
using Core.Audio.AudioData;
using Core.Utilities;
using Localization;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UI.HUD.Dialogues.NPC;

public abstract class NPCDialogueSO : DialogueSO
{
	[Header("Actor")]
	[SerializeField]
	private string actorName;

	[SerializeField]
	private Sprite actorPortrait;

	[SerializeField]
	private UIAudioCueSO actorVoice;

	private string _portraitKey;

	private string _voicePath;

	private int _npcId;

	[Header("Message")]
	[SerializeField]
	protected List<LocalizedStringWithArgument> messages = new List<LocalizedStringWithArgument>();

	[NonSerialized]
	private int _currentIndex;

	private DialogueSO _dialogueSOImplementation;

	public override int GetPriority()
	{
		return 5;
	}

	public virtual void Init(NPCMessageData npcMessageData)
	{
		actorName = npcMessageData.GetActorName();
		_portraitKey = npcMessageData.GetPortraitKey();
		actorPortrait = LoadActorPortrait();
		_voicePath = npcMessageData.GetVoicePath();
		actorVoice = LoadActorVoice();
		messages.Clear();
		messages.AddRange(npcMessageData.GetMessages());
		_currentIndex = 0;
		_npcId = npcMessageData.GetNpcId();
	}

	public int GetNPCId()
	{
		return _npcId;
	}

	public string GetActorName()
	{
		return actorName;
	}

	public Sprite GetActorPortrait()
	{
		return actorPortrait;
	}

	public UIAudioCueSO GetActorVoice()
	{
		return actorVoice;
	}

	private Sprite LoadActorPortrait(string appendix = "0")
	{
		return Addressables.LoadAssetAsync<Sprite>(_portraitKey + appendix).WaitForCompletion();
	}

	private UIAudioCueSO LoadActorVoice()
	{
		if (!string.IsNullOrEmpty(_voicePath))
		{
			return Addressables.LoadAssetAsync<UIAudioCueSO>(_voicePath).WaitForCompletion();
		}
		return null;
	}

	public override bool Equals(IHasPriorityKey otherSO)
	{
		if (!(otherSO is NPCDialogueSO nPCDialogueSO))
		{
			return false;
		}
		if (nPCDialogueSO.messages == null || messages == null)
		{
			return false;
		}
		if (messages?.Count != nPCDialogueSO.messages?.Count)
		{
			return false;
		}
		return !nPCDialogueSO.messages.Where((LocalizedStringWithArgument otherString, int i) => !string.Equals(messages[i]?.GetIdentifier(), otherString?.GetIdentifier())).Any();
	}

	public void ResetIndex()
	{
		_currentIndex = 0;
	}

	public bool HasMessage(string locIdentifier)
	{
		return messages.Any((LocalizedStringWithArgument loc) => loc.GetIdentifier() == locIdentifier);
	}

	public bool HasMoreMessages()
	{
		return _currentIndex < messages.Count;
	}

	public string GetNextMessage()
	{
		LocalizedStringWithArgument obj = ((_currentIndex < messages.Count) ? messages[_currentIndex] : null);
		_currentIndex++;
		return obj?.GetMessage();
	}

	public Sprite GetEmote(int position)
	{
		int num = _currentIndex - 1;
		if (num >= messages.Count)
		{
			return null;
		}
		List<MilMo_LocString.Tag> list = messages[num]?.GetTags();
		if (list == null)
		{
			return null;
		}
		foreach (MilMo_LocString.Tag item in list)
		{
			if (item.Index == position)
			{
				return LoadActorPortrait(item.Content);
			}
		}
		return null;
	}

	public Sprite GetLastEmote()
	{
		int num = _currentIndex - 1;
		if (num >= messages.Count)
		{
			return null;
		}
		MilMo_LocString.Tag tag = messages[num]?.GetTags()?.LastOrDefault();
		if (tag == null)
		{
			return null;
		}
		return LoadActorPortrait(tag.Content);
	}
}
