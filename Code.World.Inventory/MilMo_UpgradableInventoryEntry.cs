using System;
using Code.Core.Items;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.Template;
using Core;

namespace Code.World.Inventory;

public class MilMo_UpgradableInventoryEntry : MilMo_InventoryEntry
{
	public Action<MilMo_UpgradableInventoryEntry> OnUpgraded;

	public string ItemTrackIcon { get; private set; }

	public int CurrentUpgradeLevel { get; private set; }

	public MilMo_UpgradableInventoryEntry()
	{
		ItemTrackIcon = "";
	}

	public override void Read(InventoryEntry entry, OnReadDoneCallback callback)
	{
		if (entry is UpgradableInventoryEntry upgradableInventoryEntry)
		{
			CurrentUpgradeLevel = upgradableInventoryEntry.GetCurrentLevel();
			ItemTrackIcon = upgradableInventoryEntry.GetItemTrackIcon();
		}
		base.Read(entry, callback);
	}

	public void Upgrade(ServerInventoryEntryUpgraded upgradeMessage, OnReadDoneCallback callback)
	{
		CurrentUpgradeLevel = upgradeMessage.getNewLevel();
		OnReadDone = callback;
		Modifiers = MilMo_Item.ReadModifiers(upgradeMessage.getNewItem().GetModifiers());
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(upgradeMessage.getNewItem().GetTemplate(), base.ReadFinished);
	}
}
