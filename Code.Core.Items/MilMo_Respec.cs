using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Network;
using Code.Core.Network.messages.client;
using Code.World.Player;
using Core;
using UI.HUD.Dialogues;
using UI.Sprites;

namespace Code.Core.Items;

public class MilMo_Respec : MilMo_Item, IUsable
{
	private Action _onUsed;

	private Action _onFail;

	public MilMo_Respec(MilMo_RespecTemplate template, Dictionary<string, string> modifiers)
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
		if (MilMo_Player.Instance == null || MilMo_Player.Instance.IsExhausted)
		{
			return false;
		}
		DialogueSpawner.SpawnYesNoModal("CharBuilder_6054", "World_11374", new AddressableSpriteLoader(base.Template.IconPath.Split("/").Last()), Confirm, null);
		return true;
		void Confirm()
		{
			Singleton<GameNetwork>.Instance.SendToGameServer(new ClientRequestRespec(entryId));
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
