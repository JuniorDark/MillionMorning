using System;
using System.Collections.Generic;
using System.Linq;
using Code.Core.Items;
using Code.Core.Monetization;
using Code.Core.Network.types;
using Code.Core.ResourceSystem;
using Code.Core.Template;
using Code.Core.Utility;
using Core;
using UnityEngine;

namespace Code.World.CharacterShop.RemoteShop;

public sealed class MilMo_ShopItem
{
	public class DiscountData
	{
		public enum EDiscountType
		{
			Cash,
			Coin,
			Gift,
			NrOfTypes
		}

		private readonly DateTime _start;

		private readonly DateTime _end;

		public sbyte Discount { get; }

		public float DiscountPercent => 1f - (float)Discount / 100f;

		internal DiscountData()
		{
			_start = DateTime.MinValue;
			_end = DateTime.MinValue;
			Discount = 0;
		}

		internal DiscountData(sbyte discount, long start, long end)
		{
			_start = MilMo_Utility.GetDateTimeFromMilliseconds(start);
			_end = MilMo_Utility.GetDateTimeFromMilliseconds(end);
			Discount = discount;
		}

		public bool IsActive()
		{
			if (Discount == 0)
			{
				return false;
			}
			if (_start <= MilMo_ServerTime.Instance.GetServerTimeAsDateTime())
			{
				return _end >= MilMo_ServerTime.Instance.GetServerTimeAsDateTime();
			}
			return false;
		}
	}

	public delegate void OnReadDone(bool success, MilMo_ShopItem item);

	private const int CANT_BUY_WITH_COINS_PRICE = -1;

	private short _amount;

	private string _overrideIconPath;

	private int _basePrice;

	private int _coinsPrice;

	private Texture2D _overrideIcon;

	private MilMo_ShopItemTarget _target;

	private MilMo_Price _price;

	private readonly DiscountData _emptyDiscount = new DiscountData();

	private readonly List<DiscountData>[] _discountLists = new List<DiscountData>[3];

	private DateTime _availableFrom;

	private DateTime _newItemStartTime;

	private DateTime _newItemEndTime;

	public bool IsNew
	{
		get
		{
			if (_newItemEndTime > MilMo_ServerTime.Instance.GetServerTimeAsDateTime())
			{
				return _newItemStartTime < MilMo_ServerTime.Instance.GetServerTimeAsDateTime();
			}
			return false;
		}
	}

	public bool IsAvailable => _availableFrom < MilMo_ServerTime.Instance.GetServerTimeAsDateTime();

	public MilMo_ObjectMover Mover { get; set; }

	public int Id { get; private set; }

	public float SortIndex { get; private set; }

	public MilMo_Item Item { get; private set; }

	public bool CanBuyWithCoins => _coinsPrice != -1;

	public DiscountData CoinDiscount => GetActiveDiscount(DiscountData.EDiscountType.Coin);

	public DiscountData CashDiscount => GetActiveDiscount(DiscountData.EDiscountType.Cash);

	public DiscountData GiftDiscount => GetActiveDiscount(DiscountData.EDiscountType.Gift);

	public MilMo_LocString DisplayName
	{
		get
		{
			if (Item == null)
			{
				return MilMo_LocString.Empty;
			}
			if (_amount == 1)
			{
				return Item.Template.DisplayName;
			}
			MilMo_LocString copy = MilMo_Localization.GetLocString("World_3723").GetCopy();
			copy.SetFormatArgs(_amount, Item.Template.DisplayName);
			return copy;
		}
	}

	public void AsyncGetIcon(MilMo_Item.IconDone callback)
	{
		AsyncGetIcon(MilMo_ResourceManager.Priority.High, callback);
	}

	public void AsyncGetIcon(MilMo_ResourceManager.Priority priority, MilMo_Item.IconDone callback)
	{
		if (string.IsNullOrEmpty(_overrideIconPath))
		{
			if (Item != null)
			{
				Item.AsyncGetIcon(priority, callback);
			}
			else
			{
				callback(null);
			}
		}
		else if ((bool)_overrideIcon)
		{
			callback(_overrideIcon);
		}
		else
		{
			LoadAndSetIconAsync(priority, callback);
		}
	}

	private async void LoadAndSetIconAsync(MilMo_ResourceManager.Priority priority, MilMo_Item.IconDone callback)
	{
		callback(_overrideIcon = await MilMo_ResourceManager.Instance.LoadTextureAsync(_overrideIconPath, "Generic", priority));
	}

	public short GetAmount()
	{
		return _amount;
	}

	public int GetCoinPrice()
	{
		float num = _coinsPrice;
		DiscountData activeDiscount = GetActiveDiscount(DiscountData.EDiscountType.Coin);
		return Math.Max(1, (int)(num * activeDiscount.DiscountPercent));
	}

	public int GetPrice(bool memberPrice, bool isGift)
	{
		if (_price == null)
		{
			_price = MilMo_Monetization.Instance.GetPrice(_basePrice);
		}
		_price.IsMemberPrice = memberPrice;
		return GetDiscountedPrice(_price.Price, isGift);
	}

	private int GetDiscountedPrice(int price, bool isGift)
	{
		float num = price;
		DiscountData activeDiscount = GetActiveDiscount(DiscountData.EDiscountType.Cash);
		float discountPercent = activeDiscount.DiscountPercent;
		if (!isGift)
		{
			return Math.Max(1, (int)Math.Round(num * discountPercent));
		}
		DiscountData activeDiscount2 = GetActiveDiscount(DiscountData.EDiscountType.Gift);
		if (activeDiscount.Discount < activeDiscount2.Discount)
		{
			discountPercent = activeDiscount2.DiscountPercent;
		}
		return Math.Max(1, (int)(num * discountPercent));
	}

	public bool IsUseableByGender(bool gender)
	{
		if (_target != 0 && (!gender || _target != MilMo_ShopItemTarget.Boy))
		{
			if (!gender)
			{
				return _target == MilMo_ShopItemTarget.Girl;
			}
			return false;
		}
		return true;
	}

	private DiscountData GetActiveDiscount(DiscountData.EDiscountType type)
	{
		using (IEnumerator<DiscountData> enumerator = _discountLists[(int)type].Where((DiscountData data) => data.IsActive()).GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return _emptyDiscount;
	}

	public void Read(ShopItem item, OnReadDone callback)
	{
		Id = item.GetId();
		SortIndex = item.GetSortIndex();
		_amount = item.GetAmount();
		_overrideIconPath = item.GetOverrideIconPath();
		_basePrice = item.GetPrice();
		_coinsPrice = item.GetCoinPrice();
		for (int i = 0; i < 3; i++)
		{
			_discountLists[i] = new List<DiscountData>();
		}
		foreach (Code.Core.Network.types.DiscountData item2 in item.GetCoinDiscount())
		{
			_discountLists[1].Add(new DiscountData(item2.GetDiscountPercent(), item2.GetDiscountStart(), item2.GetDiscountEnd()));
		}
		foreach (Code.Core.Network.types.DiscountData item3 in item.GetGiftDiscount())
		{
			_discountLists[2].Add(new DiscountData(item3.GetDiscountPercent(), item3.GetDiscountStart(), item3.GetDiscountEnd()));
		}
		foreach (Code.Core.Network.types.DiscountData item4 in item.GetCashDiscount())
		{
			_discountLists[0].Add(new DiscountData(item4.GetDiscountPercent(), item4.GetDiscountStart(), item4.GetDiscountEnd()));
		}
		_newItemStartTime = MilMo_Utility.GetDateTimeFromMilliseconds(item.GetNewItemSpan().GetStartTime());
		_newItemEndTime = MilMo_Utility.GetDateTimeFromMilliseconds(item.GetNewItemSpan().GetEndTime());
		_availableFrom = MilMo_Utility.GetDateTimeFromMilliseconds(item.GetAvailableFrom());
		if (item.GetTarget() == 0)
		{
			_target = MilMo_ShopItemTarget.Generic;
		}
		else if (item.GetTarget() == 1)
		{
			_target = MilMo_ShopItemTarget.Boy;
		}
		else if (item.GetTarget() == 2)
		{
			_target = MilMo_ShopItemTarget.Girl;
		}
		Singleton<MilMo_TemplateContainer>.Instance.GetTemplate(item.GetItem(), delegate(MilMo_Template template, bool timeOut)
		{
			MilMo_ItemTemplate milMo_ItemTemplate = template as MilMo_ItemTemplate;
			if (timeOut || milMo_ItemTemplate == null)
			{
				callback?.Invoke(success: false, null);
			}
			else
			{
				Item = milMo_ItemTemplate.Instantiate(new Dictionary<string, string>());
				callback?.Invoke(success: true, this);
			}
		});
	}
}
