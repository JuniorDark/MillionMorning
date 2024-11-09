using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Avatar;
using Code.Core.BodyPack;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;

namespace Code.World.CharacterShop.RemoteShop;

public sealed class MilMo_ShopCategory
{
	public delegate void OnReadDone(bool success, MilMo_ShopCategory category);

	private int _id;

	private MilMo_BodyPackGender _gender;

	public MilMo_LocString DisplayName { get; private set; }

	public string IdentifierName { get; private set; }

	public string IconPath => "Content/GUI/Batch01/Textures/Shop/Icon" + IdentifierName;

	public List<MilMo_ShopItem> Items { get; }

	public List<MilMo_ShopCategory> SubCategories { get; }

	public MilMo_ShopCategory()
	{
		Items = new List<MilMo_ShopItem>();
		SubCategories = new List<MilMo_ShopCategory>();
		DisplayName = MilMo_LocString.Empty;
	}

	public bool IsGender(MilMo_Avatar avatar)
	{
		if (_gender != 0 && (_gender != MilMo_BodyPackGender.Boy || !avatar.IsBoy))
		{
			if (_gender == MilMo_BodyPackGender.Girl)
			{
				return !avatar.IsBoy;
			}
			return false;
		}
		return true;
	}

	public MilMo_ShopCategory GetCategoryWithId(int id)
	{
		if (_id != id)
		{
			return SubCategories.Select((MilMo_ShopCategory category) => category.GetCategoryWithId(id)).FirstOrDefault((MilMo_ShopCategory foundCategory) => foundCategory != null);
		}
		return this;
	}

	public MilMo_ShopItem GetItemWithId(int id)
	{
		using (IEnumerator<MilMo_ShopItem> enumerator = Items.Where((MilMo_ShopItem item) => item.Id == id).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return SubCategories.Select((MilMo_ShopCategory category) => category.GetItemWithId(id)).FirstOrDefault((MilMo_ShopItem item) => item != null);
	}

	public MilMo_ShopItem GetItemWithIdentifier(string identifier)
	{
		using (IEnumerator<MilMo_ShopItem> enumerator = Items.Where((MilMo_ShopItem item) => item.Item.Template.Identifier.Equals(identifier, StringComparison.InvariantCultureIgnoreCase)).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return SubCategories.Select((MilMo_ShopCategory category) => category.GetItemWithIdentifier(identifier)).FirstOrDefault((MilMo_ShopItem item) => item != null);
	}

	public void ReadItems(ShopItems items, OnReadDone callback)
	{
		int numberOfItems = items.GetItems().Count;
		int readItems = 0;
		foreach (ShopItem item in items.GetItems())
		{
			MilMo_ShopItem newItem = new MilMo_ShopItem();
			newItem.Read(item, delegate(bool success, MilMo_ShopItem createdItem)
			{
				if (success)
				{
					Items.Add(newItem);
				}
				readItems++;
				if (readItems == numberOfItems && callback != null)
				{
					Items.Sort((MilMo_ShopItem a, MilMo_ShopItem b) => a.SortIndex.CompareTo(b.SortIndex));
					callback(success: true, this);
				}
			});
		}
	}

	public void Read(ShopCategory category)
	{
		_id = category.GetId();
		string name = category.GetName();
		int num = name.LastIndexOf('#');
		if (num != -1)
		{
			string text = name.Substring(num + 1, name.Length - (num + 1));
			IdentifierName = name.Substring(0, num);
			DisplayName = MilMo_Localization.GetLocString(GetLocalizationStringFromIdentifierName(IdentifierName));
			if (text.Equals("male", StringComparison.InvariantCultureIgnoreCase))
			{
				_gender = MilMo_BodyPackGender.Boy;
			}
			else if (text.Equals("female", StringComparison.InvariantCultureIgnoreCase))
			{
				_gender = MilMo_BodyPackGender.Girl;
			}
		}
		else
		{
			IdentifierName = name;
			DisplayName = MilMo_Localization.GetLocString(GetLocalizationStringFromIdentifierName(IdentifierName));
		}
		foreach (ShopCategory subCategory in category.GetSubCategories())
		{
			MilMo_ShopCategory milMo_ShopCategory = new MilMo_ShopCategory();
			milMo_ShopCategory.Read(subCategory);
			SubCategories.Add(milMo_ShopCategory);
		}
	}

	private static string GetLocalizationStringFromIdentifierName(string identifierName)
	{
		return identifierName switch
		{
			"Items" => "CharacterShop_95", 
			"Upper_Body" => "CharacterShop_96", 
			"Lower_Body" => "CharacterShop_97", 
			"Shoes" => "CharacterShop_98", 
			"Accessories" => "CharacterShop_99", 
			"Hairstyles" => "CharacterShop_100", 
			"Pets" => "CharacterShop_101", 
			"Telepods" => "CharacterShop_103", 
			"Weapons" => "CharacterShop_102", 
			"Useables" => "CharacterShop_104", 
			"Clothes" => "CharacterShop_241", 
			"Subscriptions" => "CharacterShop_105", 
			"T-Shirts" => "CharacterShop_106", 
			"Shirts" => "CharacterShop_107", 
			"Sweaters" => "CharacterShop_108", 
			"Dresses" => "CharacterShop_109", 
			"Jackets" => "CharacterShop_110", 
			"Pants" => "CharacterShop_111", 
			"Skirts" => "CharacterShop_112", 
			"Modern" => "CharacterShop_113", 
			"Caps" => "CharacterShop_114", 
			"Hats" => "CharacterShop_115", 
			"Scarves" => "CharacterShop_116", 
			"Neckties" => "CharacterShop_117", 
			"Belts" => "CharacterShop_118", 
			"Eyewear" => "CharacterShop_119", 
			"Headwear" => "CharacterShop_120", 
			"Jewelry" => "CharacterShop_121", 
			"Misc" => "CharacterShop_122", 
			"Homes" => "CharacterShop_6959", 
			"Chairs" => "CharacterShop_6953", 
			"Beds" => "CharacterShop_6954", 
			"Tables" => "CharacterShop_6951", 
			"Wallpaper" => "CharacterShop_6949", 
			"Floors" => "CharacterShop_6950", 
			"Windows" => "CharacterShop_6952", 
			"Lamps" => "CharacterShop_6955", 
			"Rugs" => "CharacterShop_6956", 
			"Sound" => "CharacterShop_6957", 
			"Wings" => "CharacterShop_11358", 
			"Converters" => "CharacterShop_11359", 
			"Offhands" => "CharacterShop_11387", 
			"Neckwear" => "CharacterShop_12566", 
			"Capes" => "CharacterShop_12602", 
			"Rooms" => "CharacterShop_13314", 
			"Empty" => "CharacterShop_13320", 
			"Doors" => "CharacterShop_13318", 
			"Decorated" => "CharacterShop_13319", 
			"Stairs" => "CharacterShop_13415", 
			"Football" => "CharacterShop_13416", 
			"Halloween" => "CharacterShop_13417", 
			"Pumpkins" => "CharacterShop_13423", 
			"WinterFest" => "CharacterShop_13420", 
			"Carnaval" => "CharacterShop_13418", 
			"SpringFling" => "CharacterShop_13419", 
			"SeaFestival" => "CharacterShop_13421", 
			"ToothlessJack" => "CharacterShop_13422", 
			_ => identifierName, 
		};
	}
}
