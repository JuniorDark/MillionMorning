using System;
using System.Collections.Generic;
using Code.Core.EventSystem;
using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.Core.Network.messages.server;
using Code.Core.ResourceSystem;
using Code.World.Player;
using Core;
using Localization;
using UI.HUD.Dialogues;
using UI.HUD.Dialogues.Modal;
using UI.Sprites;

namespace Code.Core.Items;

public class MilMo_NameChange : MilMo_Item, IUsable
{
	private enum NameChangeResult
	{
		Success,
		InvalidName,
		NameInUse,
		Error
	}

	private Action _onUsed;

	private Action _onFail;

	private int _entryId;

	private string _newName;

	private static MilMo_GenericReaction _nameChangeReaction;

	public MilMo_NameChange(MilMo_NameChangeTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public void RegisterOnUsed(Action onUsed)
	{
		_onUsed = (Action)Delegate.Combine(_onUsed, onUsed);
	}

	public void UnregisterOnUsed(Action onUsed)
	{
		_onUsed = (Action)Delegate.Remove(_onUsed, onUsed);
	}

	public void RegisterOnFailedToUse(Action onFail)
	{
		_onFail = (Action)Delegate.Combine(_onFail, onFail);
	}

	public void UnregisterOnFailedToUse(Action onFail)
	{
		_onFail = (Action)Delegate.Remove(_onFail, onFail);
	}

	public bool Use(int entryId)
	{
		_entryId = entryId;
		if (MilMo_Player.Instance == null || MilMo_Player.Instance.IsExhausted)
		{
			return false;
		}
		SpawnNameChangeDialogue();
		return true;
	}

	private void SpawnNameChangeDialogue()
	{
		DialogueButtonInfo confirm = new DialogueButtonInfo(Confirm, new LocalizedStringWithArgument("Generic_OK"));
		DialogueButtonInfo cancel = new DialogueButtonInfo(null, new LocalizedStringWithArgument("Generic_Cancel"));
		DialogueSpawner.SpawnInputDialogue(new InputModalMessageData(new LocalizedStringWithArgument("World_10089"), new LocalizedStringWithArgument("Generic_EnterText"), OnInputChange, confirm, cancel));
	}

	private void OnInputChange(string newValue)
	{
		_newName = newValue;
	}

	private void Confirm()
	{
		string @string = MilMo_Localization.GetLocString("World_10090").String;
		string string2 = MilMo_Localization.GetLocString("World_10091").String;
		DialogueSpawner.SpawnOkCancelModal(@string, string2, new AddressableSpriteLoader("WarningIcon"), SendNameChange, null);
	}

	private void SendNameChange()
	{
		if (!string.IsNullOrEmpty(_newName))
		{
			ListenForNameChangeResponse();
			Singleton<GameNetwork>.Instance.SendToGameServer(new ClientRequestChangeName(_newName, _entryId));
		}
	}

	private void ListenForNameChangeResponse()
	{
		StopListeningForNameChangeResponse();
		_nameChangeReaction = MilMo_EventSystem.Listen("name_change_result", NameChangeResponse);
	}

	private void StopListeningForNameChangeResponse()
	{
		if (_nameChangeReaction != null)
		{
			MilMo_EventSystem.RemoveReaction(_nameChangeReaction);
			_nameChangeReaction = null;
		}
	}

	private void NameChangeResponse(object msgAsObject)
	{
		StopListeningForNameChangeResponse();
		if (msgAsObject is ServerChangeNameResponse serverChangeNameResponse)
		{
			NameChangeResult result = (NameChangeResult)serverChangeNameResponse.getResult();
			string addressableKey = "ErrorIcon";
			string identifier = "Generic_ERROR";
			string identifier2;
			switch (result)
			{
			case NameChangeResult.Success:
				identifier2 = "World_10095";
				addressableKey = "ConfirmIcon";
				identifier = "Generic_4619";
				break;
			case NameChangeResult.InvalidName:
				identifier2 = "World_10092";
				break;
			case NameChangeResult.NameInUse:
				identifier2 = "World_10093";
				break;
			case NameChangeResult.Error:
				identifier2 = "World_10094";
				break;
			default:
				identifier2 = "World_10094";
				break;
			}
			DialogueSpawner.SpawnOkModal(new LocalizedStringWithArgument(identifier), new LocalizedStringWithArgument(identifier2), new AddressableSpriteLoader(addressableKey), null);
		}
	}

	public override bool IsWieldable()
	{
		return false;
	}

	public override bool IsWearable()
	{
		return false;
	}
}
