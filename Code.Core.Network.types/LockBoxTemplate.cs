using System.Collections.Generic;

namespace Code.Core.Network.types;

public class LockBoxTemplate : ItemTemplate
{
	public new class Factory : Template.Factory
	{
		public override Template Create(MessageReader reader)
		{
			return new LockBoxTemplate(reader);
		}
	}

	private readonly TemplateReference _keyTemplate;

	private const int TYPE_ID = 29;

	public override int GetTypeId()
	{
		return 29;
	}

	public LockBoxTemplate(MessageReader reader)
		: base(reader)
	{
		_keyTemplate = new TemplateReference(reader);
	}

	public LockBoxTemplate(TemplateReference keyTemplate, string visualrep, float pickupRadius, sbyte isUnique, sbyte isHappy, sbyte isAutoPickup, string happyPickupType, string pickupMessageSingle, string pickupMessageSeveral, IList<string> pickupSounds, string description, string shopDescription, string name, string pocketCategory, sbyte feed, string feedEventIngame, string feedEventExternal, string feedDescriptionIngame, string feedDescriptionExternal, string type, TemplateReference reference)
		: base(visualrep, pickupRadius, isUnique, isHappy, isAutoPickup, happyPickupType, pickupMessageSingle, pickupMessageSeveral, pickupSounds, description, shopDescription, name, pocketCategory, feed, feedEventIngame, feedEventExternal, feedDescriptionIngame, feedDescriptionExternal, type, reference)
	{
		_keyTemplate = keyTemplate;
	}

	public TemplateReference GetKeyTemplate()
	{
		return _keyTemplate;
	}

	public override int Size()
	{
		return base.Size() + _keyTemplate.Size();
	}

	public override void Write(MessageWriter writer)
	{
		base.Write(writer);
		_keyTemplate.Write(writer);
	}
}
