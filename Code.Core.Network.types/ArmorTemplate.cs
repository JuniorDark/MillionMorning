using System.Collections.Generic;

namespace Code.Core.Network.types;

public class ArmorTemplate : WearableTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new ArmorTemplate(reader);
		}
	}

	private readonly string _armorClass;

	private const int TYPE_ID = 82;

	public override int GetTypeId()
	{
		return 82;
	}

	public ArmorTemplate(MessageReader reader)
		: base(reader)
	{
		_armorClass = reader.ReadString();
	}

	public ArmorTemplate(string armorClass, string bodypack, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(bodypack, visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_armorClass = armorClass;
	}

	public string GetArmorClass()
	{
		return _armorClass;
	}

	public override int Size()
	{
		return 2 + base.Size() + MessageWriter.GetSize(_armorClass);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_armorClass);
	}
}
