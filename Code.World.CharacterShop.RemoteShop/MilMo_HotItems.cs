using System;
using System.Collections.Generic;
using System.Text;
using Code.Core.Config;
using Code.Core.Network;
using Code.Core.ResourceSystem;
using Code.World.Player;
using Core;
using UnityEngine;

namespace Code.World.CharacterShop.RemoteShop;

public static class MilMo_HotItems
{
	public delegate void HotItemsDone(bool success);

	private static readonly List<MilMo_ShopItem> HotItems = new List<MilMo_ShopItem>();

	private static readonly List<string> HotItemIdentifiers = new List<string>();

	private static bool _hotItemsFileLoaded;

	private static int _hotItemsHashCode;

	private static IList<int> _theSeenHotItemsHashCodes = new List<int>();

	public static bool HotItemsAreNew
	{
		get
		{
			if (!_hotItemsFileLoaded)
			{
				throw new InvalidOperationException("Trying to check if hot items are new before hot items are loaded!");
			}
			return !_theSeenHotItemsHashCodes.Contains(_hotItemsHashCode);
		}
	}

	public static List<string> ItemIdentifiers => HotItemIdentifiers;

	public static List<MilMo_ShopItem> Items => HotItems;

	public static void ReadSeenHotItems(IList<int> seenHotItems)
	{
		_theSeenHotItemsHashCodes = seenHotItems;
	}

	public static void AsyncFetchHotItems(MilMo_ShopCategory root, HotItemsDone callback)
	{
		HotItems.Clear();
		AsyncLoadHotItemsFile(delegate(bool success)
		{
			if (!success)
			{
				callback(success: false);
			}
			else
			{
				foreach (string hotItemIdentifier in HotItemIdentifiers)
				{
					MilMo_ShopItem itemWithIdentifier = root.GetItemWithIdentifier(hotItemIdentifier);
					if (itemWithIdentifier == null)
					{
						Debug.LogWarning("Got hot shop item '" + hotItemIdentifier + "' that does not exist in shop");
					}
					else if (!itemWithIdentifier.IsUseableByGender(MilMo_Player.Instance.Avatar.Gender == 0))
					{
						Debug.Log("Hot shop item '" + hotItemIdentifier + "' is not useable by current gender");
					}
					else
					{
						Debug.Log("Adding item '" + hotItemIdentifier + "' to list of hot shop items");
						HotItems.Add(itemWithIdentifier);
					}
				}
				callback(success: true);
			}
		});
	}

	public static void AsyncLoadHotItemsFile(HotItemsDone callback)
	{
		if (_hotItemsFileLoaded)
		{
			callback(success: true);
			return;
		}
		MilMo_SimpleFormat.AsyncLoad(MilMo_Config.Instance.GetValue("HotItems.Path", "Apps/CharacterShop/Content/HotItems"), delegate(MilMo_SFFile file)
		{
			if (file == null)
			{
				callback(success: false);
			}
			else
			{
				StringBuilder stringBuilder = new StringBuilder();
				while (file.NextRow())
				{
					string @string = file.GetString();
					bool flag = true;
					if (file.HasMoreTokens() && file.PeekIsNext("AvailableFrom"))
					{
						file.NextToken();
						string[] array = file.GetString().Split("-".ToCharArray());
						DateTime dateTime = new DateTime(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]), 10, 0, 0, DateTimeKind.Utc);
						DateTime utcNow = DateTime.UtcNow;
						flag = dateTime < utcNow;
					}
					if (flag)
					{
						flag = file.CheckEventTags();
					}
					if (flag)
					{
						HotItemIdentifiers.Add(@string);
						stringBuilder.Append(@string);
					}
				}
				_hotItemsHashCode = stringBuilder.ToString().GetHashCode();
				_hotItemsFileLoaded = true;
				callback(success: true);
			}
		});
	}

	public static void SetHotItemsSeen()
	{
		if (!_hotItemsFileLoaded)
		{
			throw new InvalidOperationException("Trying to set hot items as seen before hot items are loaded!");
		}
		if (HotItemsAreNew && _hotItemsHashCode >= 1)
		{
			_theSeenHotItemsHashCodes.Add(_hotItemsHashCode);
			Singleton<GameNetwork>.Instance.SendSaveSeenHotItems(_hotItemsHashCode);
		}
	}
}
