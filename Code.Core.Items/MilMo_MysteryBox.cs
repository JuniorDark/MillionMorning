using System;
using System.Collections.Generic;
using Code.Core.Network;
using Code.World.Player;
using Core;

namespace Code.Core.Items;

public class MilMo_MysteryBox : MilMo_RandomBox, IUsable
{
	private Action _onUsed;

	private Action _onFail;

	public new MilMo_MysteryBoxTemplate Template => ((MilMo_Item)this).Template as MilMo_MysteryBoxTemplate;

	public MilMo_MysteryBox(MilMo_MysteryBoxTemplate template, Dictionary<string, string> modifiers)
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
		Singleton<GameNetwork>.Instance.RequestOpenBox(entryId);
		return true;
	}
}
