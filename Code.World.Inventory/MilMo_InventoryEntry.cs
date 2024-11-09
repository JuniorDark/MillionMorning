using System;
using System.Collections.Generic;
using Code.Core.Items;
using Code.Core.Items.Home;
using Code.Core.Network;
using Code.Core.Network.types;
using Code.Core.Template;
using Core;
using UI.Elements.Slot;
using UnityEngine;

namespace Code.World.Inventory;

[Serializable]
public class MilMo_InventoryEntry : ISlotItemEntry
{
	public enum InventoryCategory
	{
		Unknown,
		Items,
		Clothes,
		Weapons,
		Special,
		Boxes,
		Converter,
		Keys,
		Abilities,
		HomeEquipment,
		HomeSkins,
		Hidden
	}

	public enum InventorySection
	{
		Unknown,
		Pants,
		Chest,
		Accessory,
		Head,
		Hair,
		Hands,
		Feet,
		Offhand,
		Tool,
		Club,
		Sword,
		Axe,
		Gun,
		Bow,
		Wand,
		Net,
		Shovel,
		PvpSword,
		PvpAxe,
		PvpClub,
		PvpGun,
		PvpBow,
		GeneralAbilities,
		Floors,
		Beds,
		Chairs,
		Windows,
		Misc,
		Sound,
		Lamps,
		Rugs,
		Tables,
		Wallpapers,
		Rooms,
		Chests,
		Doors,
		Pets
	}

	public delegate void OnReadDoneCallback(bool success, MilMo_InventoryEntry item);

	protected Dictionary<string, string> Modifiers = new Dictionary<string, string>();

	private int _amount;

	public Action<bool> OnFavoriteUpdated;

	public Action<int> OnAmountUpdated;

	public static readonly Dictionary<InventorySection, string> InventorySectionLocales = new Dictionary<InventorySection, string>
	{
		{
			InventorySection.Club,
			"WeaponType_Club"
		},
		{
			InventorySection.Sword,
			"WeaponType_Sword"
		},
		{
			InventorySection.Axe,
			"WeaponType_Axe"
		},
		{
			InventorySection.Bow,
			"WeaponType_Bow"
		},
		{
			InventorySection.Gun,
			"WeaponType_Gun"
		},
		{
			InventorySection.Wand,
			"WeaponType_Wand"
		},
		{
			InventorySection.Tool,
			"WeaponType_Tool"
		},
		{
			InventorySection.Offhand,
			"WeaponType_Offhand"
		},
		{
			InventorySection.GeneralAbilities,
			"ProfileWindow_10244"
		},
		{
			InventorySection.Chest,
			"ClothType_Chest"
		},
		{
			InventorySection.Pants,
			"ClothType_Pants"
		},
		{
			InventorySection.Feet,
			"ClothType_Feet"
		},
		{
			InventorySection.Hands,
			"ClothType_Hands"
		},
		{
			InventorySection.Head,
			"ClothType_Head"
		},
		{
			InventorySection.Hair,
			"ClothType_Hair"
		},
		{
			InventorySection.Accessory,
			"ClothType_Accessory"
		},
		{
			InventorySection.Floors,
			"CharacterShop_6950"
		},
		{
			InventorySection.Beds,
			"CharacterShop_6954"
		},
		{
			InventorySection.Chairs,
			"CharacterShop_6953"
		},
		{
			InventorySection.Windows,
			"CharacterShop_6952"
		},
		{
			InventorySection.Misc,
			"CharacterShop_122"
		},
		{
			InventorySection.Sound,
			"CharacterShop_6957"
		},
		{
			InventorySection.Lamps,
			"CharacterShop_6955"
		},
		{
			InventorySection.Rugs,
			"CharacterShop_6956"
		},
		{
			InventorySection.Tables,
			"CharacterShop_6951"
		},
		{
			InventorySection.Wallpapers,
			"CharacterShop_6949"
		},
		{
			InventorySection.Rooms,
			"CharacterShop_13320"
		},
		{
			InventorySection.Chests,
			"Chests"
		},
		{
			InventorySection.Doors,
			"CharacterShop_13318"
		},
		{
			InventorySection.Pets,
			"CharacterShop_101"
		}
	};

	public InventoryCategory Category;

	public InventorySection Section;

	protected OnReadDoneCallback OnReadDone;

	public int Id { get; set; }

	public int Amount
	{
		get
		{
			return _amount;
		}
		set
		{
			_amount = value;
			OnAmountUpdated?.Invoke(value);
		}
	}

	public MilMo_Item Item { get; set; }

	public bool IsEquipped { get; set; }

	public bool IsFavorite { get; private set; }

	public bool IsFullyAdded { get; set; }

	public int GetId()
	{
		return Id;
	}

	public int GetAmount()
	{
		return _amount;
	}

	public void RegisterOnAmountUpdated(Action<int> setAmount)
	{
		OnAmountUpdated = (Action<int>)Delegate.Combine(OnAmountUpdated, setAmount);
	}

	public void UnregisterOnAmountUpdated(Action<int> setAmount)
	{
		OnAmountUpdated = (Action<int>)Delegate.Remove(OnAmountUpdated, setAmount);
	}

	public Enum GetSection()
	{
		return Section;
	}

	public Enum GetCategory()
	{
		return Category;
	}

	public IEntryItem GetItem()
	{
		return Item;
	}

	public MilMo_InventoryEntry()
	{
		IsEquipped = false;
		IsFullyAdded = false;
	}

	public void Init()
	{
		SetCategoryAndSection();
	}

	public virtual void Read(InventoryEntry entry, OnReadDoneCallback callback)
	{
		Id = entry.GetId();
		Amount = entry.GetAmount();
		IsEquipped = entry.GetEquipped() != 0;
		IsFavorite = entry.GetFavorite() != 0;
		OnReadDone = callback;
		Modifiers = MilMo_Item.ReadModifiers(entry.GetItem().GetModifiers());
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(entry.GetItem().GetTemplate(), ReadFinished);
	}

	protected void ReadFinished(MilMo_Template template, bool timeOut)
	{
		if (!timeOut && template is MilMo_ItemTemplate milMo_ItemTemplate)
		{
			Item = milMo_ItemTemplate.Instantiate(Modifiers);
			OnReadDone?.Invoke(Item != null, this);
		}
	}

	public void SetFavorite(bool favorite)
	{
		IsFavorite = favorite;
		OnFavoriteUpdated?.Invoke(favorite);
		if (Singleton<GameNetwork>.Instance != null)
		{
			Singleton<GameNetwork>.Instance.SendClientSetItemFavorite(Id, (sbyte)(favorite ? 1 : 0));
		}
	}

	private void SetCategoryAndSection()
	{
		if (Item?.Template?.BagCategory == null)
		{
			Debug.LogWarning("Unable to find bag category");
			return;
		}
		string bagCategory = Item.Template.BagCategory;
		if (!(bagCategory == "Weapons"))
		{
			if (bagCategory == "Clothes")
			{
				Category = InventoryCategory.Clothes;
				Section = GetClothesSection();
			}
			else
			{
				Category = GetCategoryByName();
				Section = GetSectionByName();
			}
		}
		else
		{
			Category = InventoryCategory.Weapons;
			Section = GetWeaponSection();
		}
	}

	private InventoryCategory GetCategoryByName()
	{
		if (Item is MilMo_HomeEquipment milMo_HomeEquipment)
		{
			if (!milMo_HomeEquipment.Template.IsSkin)
			{
				return InventoryCategory.HomeEquipment;
			}
			return InventoryCategory.HomeSkins;
		}
		if (Item is MilMo_Consumable)
		{
			return InventoryCategory.Special;
		}
		if (Item is MilMo_Ability || Item is MilMo_SkillItem)
		{
			return InventoryCategory.Abilities;
		}
		string text = Item?.Template?.BagCategory;
		if (string.IsNullOrEmpty(text))
		{
			return InventoryCategory.Unknown;
		}
		if (!Enum.TryParse<InventoryCategory>(text, out var result))
		{
			Debug.LogWarning("Got unknown category " + text + " for item " + Item?.Template?.Identifier);
		}
		return result;
	}

	private InventorySection GetSectionByName()
	{
		if (Item is MilMo_HomeEquipment milMo_HomeEquipment)
		{
			Enum.TryParse<InventorySection>(milMo_HomeEquipment.StorageCategory, out var result);
			return result;
		}
		MilMo_Item item = Item;
		if (item is MilMo_Ability || item is MilMo_SkillItem)
		{
			return InventorySection.GeneralAbilities;
		}
		return InventorySection.Unknown;
	}

	private InventorySection GetWeaponSection()
	{
		if (Item is MilMo_Offhand)
		{
			return InventorySection.Offhand;
		}
		if (!(Item is MilMo_Weapon milMo_Weapon) || milMo_Weapon.Template.WeaponType == "Net")
		{
			return InventorySection.Tool;
		}
		return GetWeaponTypeByName(milMo_Weapon);
	}

	private InventorySection GetWeaponTypeByName(MilMo_Weapon weapon)
	{
		Enum.TryParse<InventorySection>(weapon.Template.WeaponType, out var result);
		return result;
	}

	private InventorySection GetClothesSection()
	{
		if (!(Item is MilMo_Wearable { BodyPack: not null } milMo_Wearable))
		{
			return InventorySection.Unknown;
		}
		switch (milMo_Wearable.BodyPack.Categories[0])
		{
		case "LEGS":
		case "NEATHLEGS":
			return InventorySection.Pants;
		case "TORSO":
		case "CORSET":
		case "VEST":
		case "JACKET":
		case "ARMOR":
		case "corset":
			return InventorySection.Chest;
		case "NECK":
		case "TAIL":
		case "EYEWEAR":
		case "EARWEAR":
		case "WINGS":
		case "BACKPACK":
		case "BELT":
			return InventorySection.Accessory;
		case "HEAD":
		case "HEADPARTICLE":
			return InventorySection.Head;
		case "HAIR":
		case "FACIALHAIR":
			return InventorySection.Hair;
		case "HANDS":
		case "ARMS":
		case "WRIST":
			return InventorySection.Hands;
		case "FEET":
			return InventorySection.Feet;
		default:
			return InventorySection.Unknown;
		}
	}
}
