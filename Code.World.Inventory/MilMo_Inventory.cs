using System.Collections.Generic;
using System.Linq;
using Code.Core.Avatar;
using Code.Core.EventSystem;
using Code.Core.Items;
using Code.Core.Network;
using Code.Core.Network.messages.server;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.World.Achievements;
using Code.World.Player;
using Core;
using Core.GameEvent;
using UI.HUD.Dialogues;
using UnityEngine;

namespace Code.World.Inventory;

public class MilMo_Inventory
{
	private readonly MilMo_Player _player;

	private readonly MilMo_InventoryItems _items = new MilMo_InventoryItems();

	private readonly List<MilMo_InventoryEntry> _wearableItems = new List<MilMo_InventoryEntry>();

	private readonly Dictionary<string, float> _giftsReceived = new Dictionary<string, float>();

	private MilMo_Avatar _avatar;

	private readonly MilMo_GenericReaction _inventoryAdd;

	private readonly MilMo_GenericReaction _inventoryAmount;

	private readonly MilMo_GenericReaction _inventoryEquip;

	private readonly MilMo_GenericReaction _inventoryUnEquip;

	private readonly MilMo_GenericReaction _inventoryRemove;

	private readonly MilMo_GenericReaction _inventoryModifierChanged;

	private readonly MilMo_GenericReaction _inventoryEntryUpgraded;

	private readonly Dictionary<string, List<MilMo_AchievementObjectiveListener>> _itemAddedListeners = new Dictionary<string, List<MilMo_AchievementObjectiveListener>>();

	public MilMo_Inventory(MilMo_Player player, MilMo_Avatar avatar)
	{
		_player = player;
		_avatar = avatar;
		_inventoryAdd = MilMo_EventSystem.Listen("inventory_add", ItemsAdded);
		_inventoryAmount = MilMo_EventSystem.Listen("inventory_amount", ItemAmount);
		_inventoryModifierChanged = MilMo_EventSystem.Listen("inventory_modifier_changed", ItemModifierChanged);
		_inventoryEquip = MilMo_EventSystem.Listen("inventory_equip", ItemEquip);
		_inventoryUnEquip = MilMo_EventSystem.Listen("inventory_unequip", ItemUnEquip);
		_inventoryRemove = MilMo_EventSystem.Listen("inventory_remove", ItemRemove);
		_inventoryEntryUpgraded = MilMo_EventSystem.Listen("inventory_entry_upgraded", ItemUpgraded);
		_inventoryAdd.Repeating = true;
		_inventoryAmount.Repeating = true;
		_inventoryEquip.Repeating = true;
		_inventoryUnEquip.Repeating = true;
		_inventoryRemove.Repeating = true;
		_inventoryModifierChanged.Repeating = true;
		_inventoryEntryUpgraded.Repeating = true;
	}

	public void AddItems(IEnumerable<InventoryEntry> items)
	{
		foreach (InventoryEntry item in items)
		{
			ItemAdded(item, isNewEntry: false, showGameDialog: false, isGift: false);
		}
	}

	public void Destroy()
	{
		Clear();
		MilMo_EventSystem.RemoveReaction(_inventoryAdd);
		MilMo_EventSystem.RemoveReaction(_inventoryAmount);
		MilMo_EventSystem.RemoveReaction(_inventoryEquip);
		MilMo_EventSystem.RemoveReaction(_inventoryUnEquip);
		MilMo_EventSystem.RemoveReaction(_inventoryRemove);
		MilMo_EventSystem.RemoveReaction(_inventoryModifierChanged);
		MilMo_EventSystem.RemoveReaction(_inventoryEntryUpgraded);
		_avatar = null;
	}

	public void Clear()
	{
		_items.Clear();
		_wearableItems.Clear();
		_itemAddedListeners.Clear();
	}

	public void GiftReceived(string giftItemIdentifier)
	{
		_giftsReceived[giftItemIdentifier] = Time.time;
	}

	public bool HaveItem(MilMo_Item item)
	{
		return _items.Contains(item);
	}

	public bool HaveItemTemplate(MilMo_Item item)
	{
		return _items.ContainsTemplate(item);
	}

	public bool HaveItem(string templateIdentifier)
	{
		return _items.Contains(templateIdentifier);
	}

	public MilMo_InventoryEntry GetEntry(int id)
	{
		return _items.Get(id);
	}

	public MilMo_InventoryEntry GetEntry(string templateIdentifier)
	{
		return _items.GetByTemplateIdentifier(templateIdentifier);
	}

	public MilMo_InventoryEntry GetEntryByIdentifier(string identifier)
	{
		return _items.Get(identifier);
	}

	public MilMo_InventoryEntry GetLockBox(MilMo_LockBoxKey key)
	{
		return _items.GetAllByType(typeof(MilMo_LockBox)).FirstOrDefault((MilMo_InventoryEntry lockBox) => ((MilMo_LockBoxTemplate)lockBox.Item.Template).KeyTemplateIdentifier == key.Template.Identifier);
	}

	public void DoForeachItem(MilMo_InventoryItems.ForEachItem callback)
	{
		_items.DoForEach(callback);
	}

	public MilMo_InventoryEntry GetEquippedEntryByBodyPackCategory(MilMo_Wearable wearable)
	{
		foreach (MilMo_InventoryEntry wearableItem in _wearableItems)
		{
			if (wearableItem.IsEquipped)
			{
				if (!(wearableItem.Item is MilMo_Wearable milMo_Wearable))
				{
					Debug.LogWarning("Got non wearable item as equipped item in inventory (" + wearableItem.Item.Template.Identifier + ").");
				}
				else if (wearable.BodyPack.IsCategory(milMo_Wearable.BodyPack))
				{
					return wearableItem;
				}
			}
		}
		return null;
	}

	private void AddItem(MilMo_InventoryEntry item, bool isNewEntry, bool showGameDialog, bool isGift)
	{
		if (item == null)
		{
			Debug.Log("Item is null");
			return;
		}
		_items.Add(item);
		if (item.Item.IsWearable())
		{
			_wearableItems.Add(item);
		}
		if (!_player.IsDone)
		{
			return;
		}
		MilMo_ItemTemplate template = item.Item.Template;
		if (showGameDialog && isNewEntry && template.FeedMode != 0 && !_player.InShop)
		{
			DialogueSpawner.SpawnImportantItemAcquired(template);
		}
		if (isNewEntry)
		{
			CreatePickupThought(item.Item, item.Amount);
			MilMo_EventSystem.Instance.PostEvent("tutorial_ReceiveItem", template.Name);
			MilMo_EventSystem.Instance.PostEvent("tutorial_ReceiveItemType", template.Type);
		}
		if (item.Item.IsWieldable())
		{
			bool forceWield = isNewEntry && !isGift;
			if (item is MilMo_UpgradableInventoryEntry newItem)
			{
				forceWield = _player.EquipSlots != null && _player.EquipSlots.ShouldWieldNewUpgradable(newItem);
				_player.EquipSlots?.AddToSlot(item, forceWield);
			}
			else if (item.IsFavorite)
			{
				_player.EquipSlots?.AddToSlot(item, forceWield);
			}
		}
		MilMo_Item item2 = item.Item;
		if (item2 is MilMo_Consumable || item2 is MilMo_Ability || item2 is MilMo_SkillItem)
		{
			MilMo_EventSystem.Instance.PostEvent("consumable_or_ability_added", item);
		}
		item.IsFullyAdded = true;
	}

	private void CreatePickupThought(MilMo_Item item, int amountAdded)
	{
		if (item?.Template == null || amountAdded <= 0)
		{
			return;
		}
		MilMo_LocString thought = ((amountAdded == 1) ? item.Template.PickupMessageSingle : item.Template.PickupMessageSeveral);
		if (thought.IsEmpty)
		{
			return;
		}
		if (_player.InDialogue)
		{
			_player.OnStopTalkingWithNPC += delegate
			{
				GameEvent.ThinkEvent?.RaiseEvent(thought.String);
			};
		}
		else
		{
			GameEvent.ThinkEvent?.RaiseEvent(thought.String);
		}
	}

	public void HandleNotFullyAdded()
	{
		if (!_player.IsDone)
		{
			return;
		}
		_items.DoForEach(delegate(MilMo_InventoryEntry entry)
		{
			if (!entry.IsFullyAdded)
			{
				if (entry.Item.IsWieldable() && entry.IsFavorite)
				{
					_player.EquipSlots?.AddToSlot(entry, forceWield: true);
				}
				entry.IsFullyAdded = true;
			}
		});
	}

	public void AddItemAddedListener(MilMo_AchievementObjectiveListener listener)
	{
		if (!_itemAddedListeners.ContainsKey(listener.Object))
		{
			_itemAddedListeners.Add(listener.Object, new List<MilMo_AchievementObjectiveListener>());
		}
		_itemAddedListeners[listener.Object].Add(listener);
	}

	public void RemoveItemAddedListener(MilMo_AchievementObjectiveListener listener)
	{
		if (_itemAddedListeners.ContainsKey(listener.Object))
		{
			if (_itemAddedListeners[listener.Object].Count == 1)
			{
				_itemAddedListeners.Remove(listener.Object);
			}
			else
			{
				_itemAddedListeners[listener.Object].Remove(listener);
			}
		}
	}

	public void EquipAllOnAvatar(MilMo_Avatar avatar)
	{
		_items.DoForEach(delegate(MilMo_InventoryEntry entry)
		{
			if (entry.IsEquipped && entry.Item.IsWearable() && !entry.Item.IsWieldable() && entry.Item.IsUseableByGender(avatar.Gender == 0))
			{
				avatar.EquipLocal((MilMo_Wearable)entry.Item);
			}
		});
	}

	public void EquipAll()
	{
		if (_player.AnyShopState)
		{
			return;
		}
		foreach (MilMo_InventoryEntry item in _wearableItems.Where((MilMo_InventoryEntry entry) => !entry.IsEquipped))
		{
			_avatar.UnequipLocal(item.Item as MilMo_Wearable);
		}
		foreach (MilMo_InventoryEntry item2 in _wearableItems.Where((MilMo_InventoryEntry entry) => entry.IsEquipped))
		{
			if (!item2.Item.IsUseableByGender(_player.Avatar.IsBoy))
			{
				Singleton<GameNetwork>.Instance.SendUnequipUpdate(item2.Id);
			}
			else if (item2.Item.IsWieldable() && _player.EquipSlots != null && !_player.EquipSlots.IsEnabled(item2.Item as MilMo_Wieldable))
			{
				MilMo_EventSystem.Instance.PostEvent("toggle_wearable", item2.Id);
			}
			else
			{
				_avatar.EquipLocal(item2.Item as MilMo_Wearable);
			}
		}
		_avatar.AsyncApply();
	}

	private void ItemsAdded(object msgObject)
	{
		if (!(msgObject is ServerInventoryAdd serverInventoryAdd))
		{
			return;
		}
		bool isNewEntry = serverInventoryAdd.getIsNewEntry() != 0;
		bool showGameDialog = serverInventoryAdd.getShowGameDialog() != 0;
		foreach (InventoryEntry entry in serverInventoryAdd.getEntries())
		{
			string key = entry.GetItem().GetTemplate().GetCategory() + ":" + entry.GetItem().GetTemplate().GetPath();
			bool isGift = _giftsReceived.ContainsKey(key) && Time.time - _giftsReceived[key] < 60f;
			ItemAdded(entry, isNewEntry, showGameDialog, isGift);
		}
	}

    private void ItemAdded(object msgObject, bool isNewEntry, bool showGameDialog, bool isGift)
    {
        InventoryEntry inventoryEntry = (InventoryEntry)msgObject;
        ((inventoryEntry is UpgradableInventoryEntry) ? new MilMo_UpgradableInventoryEntry() : new MilMo_InventoryEntry()).Read(inventoryEntry, Callback);

        void Callback(bool success, MilMo_InventoryEntry item)
        {
            if (success)
            {
                AddItem(item, isNewEntry, showGameDialog, isGift);
                GameEvent.InventoryItemAddedEvent?.RaiseEvent(item);
                MilMo_EventSystem.Instance.PostEvent("inventory_changed", this);
                MilMo_EventSystem.Instance.PostEvent("inventory_bag_add", item);

                if (_itemAddedListeners.ContainsKey(item.Item.Template.Identifier))
                {
                    foreach (MilMo_AchievementObjectiveListener listener in _itemAddedListeners[item.Item.Template.Identifier])
                    {
                        listener.Notify();
                    }
                    _itemAddedListeners.Remove(item.Item.Template.Identifier);
                }
            }
        }
    }


    private void ItemAmount(object msgObject)
	{
		ServerInventoryAmount obj = (ServerInventoryAmount)msgObject;
		int itemID = obj.getItemID();
		short amount = obj.getAmount();
		MilMo_InventoryEntry milMo_InventoryEntry = _items.Get(itemID);
		if (milMo_InventoryEntry == null)
		{
			Debug.LogWarning("Got increase amount message to inventory for unknown inventory item");
			return;
		}
		CreatePickupThought(milMo_InventoryEntry.Item, amount - milMo_InventoryEntry.Amount);
		milMo_InventoryEntry.Amount = amount;
		MilMo_EventSystem.Instance.PostEvent("inventory_changed", this);
		MilMo_EventSystem.Instance.PostEvent("inventory_bag_amount", milMo_InventoryEntry);
	}

	private void ItemModifierChanged(object msgAsObj)
	{
		ServerInventoryModifierChanged msg = msgAsObj as ServerInventoryModifierChanged;
		if (msg == null)
		{
			return;
		}
		MilMo_InventoryEntry item = _items.Get(msg.getItemId());
		if (item?.Item == null)
		{
			Debug.LogWarning("Got change modifier message to inventory for unknown inventory item");
			return;
		}
		if (item.Item is MilMo_WieldableFood milMo_WieldableFood)
		{
			MilMo_Avatar avatar = _player.Avatar;
			if (avatar != null && avatar.EmoteManager != null && msg.getModifierKey() == "UsesLeft" && sbyte.Parse(msg.getModifierValue()) == milMo_WieldableFood.Template.Uses)
			{
				milMo_WieldableFood.ChangeModifier(msg.getModifierKey(), 0.ToString());
				float delay = 0f;
				if (!_player.Avatar.EmoteManager.IsPlaying && milMo_WieldableFood.TimeSinceLastEat >= 0.5f)
				{
					delay = 0.3f;
				}
				MilMo_EventSystem.At(delay, delegate
				{
					_player.Avatar.EmoteManager.RegisterEmoteDoneCallback(delegate
					{
						item.Item.ChangeModifier(msg.getModifierKey(), msg.getModifierValue());
						if (_player.EquipSlots?.CurrentItemInventoryId == msg.getItemId())
						{
							_player.EquipSlots?.UnwieldCurrent();
						}
					});
				});
				return;
			}
		}
		item.Item.ChangeModifier(msg.getModifierKey(), msg.getModifierValue());
	}

	private void ItemUpgraded(object msgAsObject)
	{
		if (!(msgAsObject is ServerInventoryEntryUpgraded serverInventoryEntryUpgraded))
		{
			return;
		}
		MilMo_InventoryEntry milMo_InventoryEntry = _items.Get(serverInventoryEntryUpgraded.getId());
		if (milMo_InventoryEntry?.Item == null)
		{
			return;
		}
		MilMo_UpgradableInventoryEntry upgradable = milMo_InventoryEntry as MilMo_UpgradableInventoryEntry;
		if (upgradable != null)
		{
			MilMo_Wearable oldWearable = upgradable.Item as MilMo_Wearable;
			upgradable.Upgrade(serverInventoryEntryUpgraded, delegate(bool success, MilMo_InventoryEntry entry)
			{
				if (entry.IsEquipped)
				{
					if (oldWearable != null)
					{
						_avatar.UnequipLocal(oldWearable);
					}
					if (entry.Item.IsWearable() && !entry.Item.IsWieldable())
					{
						_avatar.EquipLocal(entry.Item as MilMo_Wearable);
					}
				}
				_player.EquipSlots?.WieldableUpgraded(entry.Id);
				upgradable.OnUpgraded?.Invoke(upgradable);
			});
		}
		else
		{
			Debug.LogWarning("Got item upgrade for non upgradable inventory entry " + milMo_InventoryEntry.Item.Identifier);
		}
	}

	private void ItemEquip(object msgObject)
	{
		if (msgObject is ServerInventoryEquip serverInventoryEquip)
		{
			MilMo_InventoryEntry milMo_InventoryEntry = _items.Get(serverInventoryEquip.getItemId());
			if (milMo_InventoryEntry?.Item == null)
			{
				Debug.LogWarning($"Got equip message for item not in inventory ({serverInventoryEquip.getItemId()})");
			}
			else
			{
				milMo_InventoryEntry.IsEquipped = true;
			}
		}
	}

	private void ItemUnEquip(object msgObject)
	{
		if (msgObject is ServerInventoryUnequip serverInventoryUnequip)
		{
			MilMo_InventoryEntry milMo_InventoryEntry = _items.Get(serverInventoryUnequip.getItemId());
			if (milMo_InventoryEntry?.Item == null)
			{
				Debug.LogWarning($"Got un-equip message for item not in inventory ({serverInventoryUnequip.getItemId()})");
			}
			else
			{
				milMo_InventoryEntry.IsEquipped = false;
			}
		}
	}

	private void ItemRemove(object msgObject)
	{
		ServerInventoryRemove serverInventoryRemove = (ServerInventoryRemove)msgObject;
		if (serverInventoryRemove == null)
		{
			return;
		}
		int id = serverInventoryRemove.getItemID();
		MilMo_InventoryEntry milMo_InventoryEntry = _items.Get(id);
		if (milMo_InventoryEntry == null)
		{
			Debug.LogWarning("Got remove message to inventory for unknown inventory item");
			return;
		}
		_items.Remove(milMo_InventoryEntry);
		if (milMo_InventoryEntry.Item.IsWearable())
		{
			_wearableItems.Remove(milMo_InventoryEntry);
			if (!milMo_InventoryEntry.Item.IsWieldable())
			{
				EquipAll();
			}
		}
		if (milMo_InventoryEntry.Item.IsWieldable())
		{
			if (milMo_InventoryEntry.Item is MilMo_Wieldable milMo_Wieldable && milMo_Wieldable.IsFood())
			{
				MilMo_Avatar avatar = _player.Avatar;
				if (avatar != null && avatar.EmoteManager != null)
				{
					milMo_Wieldable.ChangeModifier("UsesLeft", "0");
					float delay = 0f;
					if (milMo_Wieldable is MilMo_WieldableFood milMo_WieldableFood && !_player.Avatar.EmoteManager.IsPlaying && milMo_WieldableFood.TimeSinceLastEat >= 0.5f)
					{
						delay = 0.3f;
					}
					MilMo_EventSystem.At(delay, delegate
					{
						_player.Avatar.EmoteManager.RegisterEmoteDoneCallback(delegate
						{
							_player.EquipSlots?.RemoveFromSlot(id);
						});
					});
					goto IL_0142;
				}
			}
			_player.EquipSlots?.RemoveFromSlot(id);
		}
		goto IL_0142;
		IL_0142:
		MilMo_Item item = milMo_InventoryEntry.Item;
		if (item is MilMo_Consumable || item is MilMo_Ability || item is MilMo_SkillItem)
		{
			MilMo_EventSystem.Instance.PostEvent("consumable_or_ability_removed", milMo_InventoryEntry);
		}
		MilMo_EventSystem.Instance.PostEvent("inventory_changed", this);
		MilMo_EventSystem.Instance.PostEvent("inventory_bag_remove", milMo_InventoryEntry);
		GameEvent.InventoryItemRemovedEvent?.RaiseEvent(milMo_InventoryEntry);
	}

	public void Store()
	{
		_avatar.BodyPackManager.Store();
	}

	public void AddToStoreAndRemoveByCategory(MilMo_Item item)
	{
		if (!(item is MilMo_Wearable milMo_Wearable))
		{
			return;
		}
		foreach (MilMo_InventoryEntry item2 in _wearableItems.Where((MilMo_InventoryEntry entry) => entry.IsEquipped && entry.Item.Identifier != item.Identifier))
		{
			if (!(item2.Item is MilMo_Wearable milMo_Wearable2) || (milMo_Wearable2.BodyPack != null && milMo_Wearable2.BodyPack.IsCategory(milMo_Wearable.BodyPack)))
			{
				item2.IsEquipped = false;
				Singleton<GameNetwork>.Instance.SendUnequipUpdate(item2.Id);
			}
		}
		_avatar.BodyPackManager.AddToStoreAndRemoveByCategory(milMo_Wearable.BodyPack, milMo_Wearable.ColorIndices);
	}
}
