using System;
using System.Collections.Generic;
using Code.Core.Network;
using Code.World.Inventory;
using Code.World.Player;
using Core;

namespace Code.Core.Items;

public class MilMo_LockBoxKey : MilMo_Item, IUsable
{
	private Action _onUsed;

	private Action _onFail;

	public new MilMo_LockBoxKeyTemplate Template => (MilMo_LockBoxKeyTemplate)base.Template;

	public MilMo_LockBoxKey(MilMo_LockBoxKeyTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public override bool IsWieldable()
	{
		return false;
	}

	public override bool IsWearable()
	{
		return false;
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
		MilMo_InventoryEntry lockBox = MilMo_Player.Instance.Inventory.GetLockBox(this);
		if (lockBox == null)
		{
			return false;
		}
		Singleton<GameNetwork>.Instance.RequestOpenBox(lockBox.Id);
		return true;
	}
}
