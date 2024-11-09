using System.Collections.Generic;

namespace Code.Core.Network.types;

public class CoinTemplate : ItemTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new CoinTemplate(reader);
		}
	}

	private readonly int _value;

	private const int TYPE_ID = 16;

	public override int GetTypeId()
	{
		return 16;
	}

	public CoinTemplate(MessageReader reader)
		: base(reader)
	{
		_value = reader.ReadInt32();
	}

	public CoinTemplate(int value, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_value = value;
	}

	public int GetValue()
	{
		return _value;
	}

	public override int Size()
	{
		return 4 + base.Size();
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteInt32(_value);
	}
}
