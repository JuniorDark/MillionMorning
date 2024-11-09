using System;
using System.Collections.Generic;
using Code.Core.Avatar;
using Code.Core.Network;
using Code.Core.Network.types;
using Code.World.Player;
using Core;
using UnityEngine;

namespace Code.Core.Items;

public class MilMo_Consumable : MilMo_Item, IUsable, IAssignable
{
	private Action _onUsed;

	private Action _onFail;

	public new MilMo_ConsumableTemplate Template => (MilMo_ConsumableTemplate)base.Template;

	public MilMo_Consumable(MilMo_ConsumableTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public string GetSaveString(int entryId)
	{
		return $"Item:{entryId}";
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
		MilMo_Avatar avatar = MilMo_Player.Instance.Avatar;
		Vector3 position = avatar.Position;
		Vector3 eulerAngles = avatar.GameObject.transform.eulerAngles;
		Singleton<GameNetwork>.Instance.RequestUseConsumable(entryId, new vector3(position.x, position.y, position.z), new vector3(eulerAngles.x, eulerAngles.y, eulerAngles.z));
		return true;
	}

	public override bool AutoPickup()
	{
		return Template.IsAutoPickup;
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
