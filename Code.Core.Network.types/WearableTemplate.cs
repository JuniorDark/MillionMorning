using System.Collections.Generic;

namespace Code.Core.Network.types;

public class WearableTemplate : ItemTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new WearableTemplate(reader);
		}
	}

	private readonly string _bodypack;

	private const int TYPE_ID = 26;

	public override int GetTypeId()
	{
		return 26;
	}

	public WearableTemplate(MessageReader reader)
		: base(reader)
	{
		_bodypack = reader.ReadString();
	}

	public WearableTemplate(string bodypack, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_bodypack = bodypack;
	}

	public string GetBodypack()
	{
		return _bodypack;
	}

	public override int Size()
	{
		return 2 + base.Size() + MessageWriter.GetSize(_bodypack);
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		writer.WriteString(_bodypack);
	}
}
