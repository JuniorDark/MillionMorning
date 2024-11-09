using System.Collections.Generic;

namespace Code.Core.Network.types;

public class ShopItem : Template
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new ShopItem(reader);
		}
	}

	private readonly int _id;

	private readonly float _sortIndex;

	private readonly TemplateReference _item;

	private readonly short _amount;

	private readonly int _price;

	private readonly int _coinPrice;

	private readonly sbyte _target;

	private readonly string _overrideIconPath;

	private readonly Timespan _newItemSpan;

	private readonly long _availableFrom;

	private readonly IList<DiscountData> _coinDiscount;

	private readonly IList<DiscountData> _cashDiscount;

	private readonly IList<DiscountData> _giftDiscount;

	private const int TYPE_ID = 7;

	public override int GetTypeId()
	{
		return 7;
	}

	public ShopItem(MessageReader reader)
		: base(reader)
	{
		_id = reader.ReadInt32();
		_sortIndex = reader.ReadFloat();
		_item = new TemplateReference(reader);
		_amount = reader.ReadInt16();
		_price = reader.ReadInt32();
		_coinPrice = reader.ReadInt32();
		_target = reader.ReadInt8();
		_overrideIconPath = reader.ReadString();
		_newItemSpan = new Timespan(reader);
		_availableFrom = reader.ReadInt64();
		_coinDiscount = new List<DiscountData>();
		short num = reader.ReadInt16();
		for (short num2 = 0; num2 < num; num2++)
		{
			_coinDiscount.Add(new DiscountData(reader));
		}
		_cashDiscount = new List<DiscountData>();
		short num3 = reader.ReadInt16();
		for (short num4 = 0; num4 < num3; num4++)
		{
			_cashDiscount.Add(new DiscountData(reader));
		}
		_giftDiscount = new List<DiscountData>();
		short num5 = reader.ReadInt16();
		for (short num6 = 0; num6 < num5; num6++)
		{
			_giftDiscount.Add(new DiscountData(reader));
		}
	}

	public ShopItem(int id, float sortIndex, TemplateReference item, short amount, int price, int coinPrice, sbyte target, string overrideIconPath, Timespan newItemSpan, long availableFrom, IList<DiscountData> coinDiscount, IList<DiscountData> cashDiscount, IList<DiscountData> giftDiscount, string type, TemplateReference reference)
		: base(type, reference)
	{
		_id = id;
		_sortIndex = sortIndex;
		_item = item;
		_amount = amount;
		_price = price;
		_coinPrice = coinPrice;
		_target = target;
		_overrideIconPath = overrideIconPath;
		_newItemSpan = newItemSpan;
		_availableFrom = availableFrom;
		_coinDiscount = coinDiscount;
		_cashDiscount = cashDiscount;
		_giftDiscount = giftDiscount;
	}

	public int GetId()
	{
		return _id;
	}

	public float GetSortIndex()
	{
		return _sortIndex;
	}

	public TemplateReference GetItem()
	{
		return _item;
	}

	public short GetAmount()
	{
		return _amount;
	}

	public int GetPrice()
	{
		return _price;
	}

	public int GetCoinPrice()
	{
		return _coinPrice;
	}

	public sbyte GetTarget()
	{
		return _target;
	}

	public string GetOverrideIconPath()
	{
		return _overrideIconPath;
	}

	public Timespan GetNewItemSpan()
	{
		return _newItemSpan;
	}

	public long GetAvailableFrom()
	{
		return _availableFrom;
	}

	public IList<DiscountData> GetCoinDiscount()
	{
		return _coinDiscount;
	}

	public IList<DiscountData> GetCashDiscount()
	{
		return _cashDiscount;
	}

	public IList<DiscountData> GetGiftDiscount()
	{
		return _giftDiscount;
	}

	public override int Size()
	{
		return 51 + base.Size() + _item.Size() + MessageWriter.GetSize(_overrideIconPath) + (short)(_coinDiscount.Count * 17) + (short)(_cashDiscount.Count * 17) + (short)(_giftDiscount.Count * 17);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt32(_id);
		writer.WriteFloat(_sortIndex);
		_item.Write(writer);
		writer.WriteInt16(_amount);
		writer.WriteInt32(_price);
		writer.WriteInt32(_coinPrice);
		writer.WriteInt8(_target);
		writer.WriteString(_overrideIconPath);
		_newItemSpan.Write(writer);
		writer.WriteInt64(_availableFrom);
		writer.WriteInt16((short)_coinDiscount.Count);
		foreach (DiscountData item in _coinDiscount)
		{
			item.Write(writer);
		}
		writer.WriteInt16((short)_cashDiscount.Count);
		foreach (DiscountData item2 in _cashDiscount)
		{
			item2.Write(writer);
		}
		writer.WriteInt16((short)_giftDiscount.Count);
		foreach (DiscountData item3 in _giftDiscount)
		{
			item3.Write(writer);
		}
	}
}
