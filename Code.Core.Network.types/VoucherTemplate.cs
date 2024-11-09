using System.Collections.Generic;

namespace Code.Core.Network.types;

public class VoucherTemplate : ItemTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new VoucherTemplate(reader);
		}
	}

	private readonly int _value;

	private readonly short _voucherPointsRequired;

	private readonly TemplateReference _nextLevel;

	private const int TYPE_ID = 27;

	public override int GetTypeId()
	{
		return 27;
	}

	public VoucherTemplate(MessageReader reader)
		: base(reader)
	{
		_value = reader.ReadInt32();
		_voucherPointsRequired = reader.ReadInt16();
		if (reader.ReadInt8() == 1)
		{
			_nextLevel = new TemplateReference(reader);
		}
	}

	public VoucherTemplate(int value, short voucherPointsRequired, TemplateReference nextLevel, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_value = value;
		_voucherPointsRequired = voucherPointsRequired;
		_nextLevel = nextLevel;
	}

	public int GetValue()
	{
		return _value;
	}

	public short GetVoucherPointsRequired()
	{
		return _voucherPointsRequired;
	}

	public TemplateReference GetNextLevel()
	{
		return _nextLevel;
	}

	public override int Size()
	{
		int num = 7 + base.Size();
		if (_nextLevel != null)
		{
			num += _nextLevel.Size();
		}
		return num;
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt32(_value);
		writer.WriteInt16(_voucherPointsRequired);
		if (_nextLevel == null)
		{
			writer.WriteInt8(0);
			return;
		}
		writer.WriteInt8(1);
		_nextLevel.Write(writer);
	}
}
