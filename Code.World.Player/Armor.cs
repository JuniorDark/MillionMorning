using System;
using Code.Core.EventSystem;
using Code.Core.Items;
using Code.Core.Network.messages.server;
using Code.World.Inventory;
using UnityEngine;

namespace Code.World.Player;

public class Armor
{
	private readonly MilMo_Player _milMoPlayer;

	private MilMo_Armor EquippedArmor { get; set; }

	public Armor(MilMo_Player milMoPlayer)
	{
		_milMoPlayer = milMoPlayer;
		MilMo_EventSystem.Listen("equip_armor", EquipArmor).Repeating = true;
	}

	public void UpdateArmor(float durability)
	{
		EquippedArmor?.SetDurability(durability);
	}

	public void DamageArmor(float armorDamage)
	{
		if ((double)Math.Abs(armorDamage) > 0.0 && EquippedArmor != null)
		{
			UpdateArmor(Mathf.Max(EquippedArmor.Durability - armorDamage, 0f));
		}
	}

	public void EquipArmor(object msgAsObject)
	{
		if (msgAsObject is ServerEquipArmor serverEquipArmor)
		{
			MilMo_InventoryEntry entry = _milMoPlayer.Inventory.GetEntry(serverEquipArmor.getInventoryId());
			EquipArmor(entry);
		}
	}

	private void EquipArmor(MilMo_InventoryEntry entry)
	{
		if (entry != null && entry.Item is MilMo_Armor equippedArmor)
		{
			if (EquippedArmor != null)
			{
				_milMoPlayer.Avatar.UnequipLocal(EquippedArmor);
			}
			EquippedArmor = equippedArmor;
			EquippedArmor?.Equip();
			MilMo_Player.Instance.Avatar.EquipLocal(EquippedArmor);
			MilMo_Player.Instance.Avatar.AsyncApply(delegate
			{
			}, "EquipArmor");
		}
	}
}
