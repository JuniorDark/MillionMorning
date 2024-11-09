using System.Collections.Generic;
using Code.Core.Avatar;
using Code.World.Inventory;
using Code.World.Player;
using Core.GameEvent;
using UnityEngine;

namespace Code.Core.Items;

public class MilMo_Wieldable : MilMo_Wearable
{
	protected MilMo_Avatar Wielder;

	protected string RunAnimation = "RunWieldable";

	protected string WalkAnimation = "WalkWieldable";

	protected bool IsWielded;

	public new MilMo_WieldableTemplate Template => ((MilMo_Item)this).Template as MilMo_WieldableTemplate;

	public GameObject GameObject { get; protected set; }

	public virtual float Cooldown => Template.Cooldown;

	public MilMo_Wieldable(MilMo_WieldableTemplate template, Dictionary<string, string> modifiers)
		: base(template, modifiers)
	{
	}

	public override bool IsWieldable()
	{
		return true;
	}

	public virtual bool IsFood()
	{
		return false;
	}

	public virtual void Wield(MilMo_Avatar wielder, bool overrideIdleAnimation)
	{
		if (IsWielded)
		{
			return;
		}
		Wielder = wielder;
		if (Wielder != null)
		{
			if (base.BodyPack != null)
			{
				GameObject = base.BodyPack.GetFirstAddonGameObject(Wielder.Renderer);
			}
			Wielder.StackAnimation("Run", RunAnimation);
			Wielder.StackAnimation("Walk", WalkAnimation);
			IsWielded = true;
		}
	}

	public virtual void Unwield()
	{
		if (IsWielded && Wielder != null)
		{
			Wielder.UnstackAnimation("Run", RunAnimation);
			Wielder.UnstackAnimation("Walk", WalkAnimation);
			IsWielded = false;
		}
	}

	public string GetRandomWieldAnimation()
	{
		if (Template.WieldAnimations == null || Template.WieldAnimations.Count == 0)
		{
			Debug.LogWarning("Trying to get wield animation from an item that doesn't have any. Falling back to idle animation.");
			return "Idle";
		}
		int index = Random.Range(0, Template.WieldAnimations.Count);
		return Template.WieldAnimations[index];
	}

	public virtual bool CanUse()
	{
		if (Time.time - Wielder.ReadyAttackTime < 0f)
		{
			return false;
		}
		if (!GameObject && Wielder != null && base.BodyPack != null)
		{
			GameObject = base.BodyPack.GetFirstAddonGameObject(Wielder.Renderer);
		}
		Wielder.ReadyAttackTime = Time.time + Cooldown;
		return true;
	}

	public override bool Use(int entryId)
	{
		if (MilMo_Player.Instance == null || MilMo_Player.Instance.IsExhausted)
		{
			return false;
		}
		MilMo_Player instance = MilMo_Player.Instance;
		if (instance.EquipSlots.CurrentItem == null || !instance.EquipSlots.CurrentItem.Identifier.Equals(base.Identifier))
		{
			MilMo_InventoryEntry entry = instance.Inventory.GetEntry(entryId);
			if (entry != null && !entry.IsFavorite)
			{
				GameEvent.InventoryItemSetFavoriteEvent?.RaiseEvent(entry);
			}
			else
			{
				instance.EquipSlots.Wield(base.Identifier);
			}
		}
		else
		{
			instance.EquipSlots.UnwieldCurrent();
		}
		return true;
	}
}
