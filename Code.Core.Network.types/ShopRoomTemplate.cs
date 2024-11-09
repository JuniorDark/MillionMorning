using System.Collections.Generic;

namespace Code.Core.Network.types;

public class ShopRoomTemplate : ItemTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new ShopRoomTemplate(reader);
		}
	}

	private readonly TemplateReference _roomPreset;

	private const int TYPE_ID = 33;

	public override int GetTypeId()
	{
		return 33;
	}

	public ShopRoomTemplate(MessageReader reader)
		: base(reader)
	{
		_roomPreset = new TemplateReference(reader);
	}

	public ShopRoomTemplate(TemplateReference roomPreset, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_roomPreset = roomPreset;
	}

	public TemplateReference GetRoomPreset()
	{
		return _roomPreset;
	}

	public override int Size()
	{
		return base.Size() + _roomPreset.Size();
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		_roomPreset.Write(writer);
	}
}
